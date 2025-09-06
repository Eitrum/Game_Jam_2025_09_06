using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Collections;
using System.Threading;
using UnityEngine;
using System.Net.Http.Headers;

namespace Toolkit.Network {
    public static class SimpleHttpClient {
        #region Variables

        private const string TAG = "<color=orange>[Simple Http Client]</color> - ";
        /// <summary>
        /// The default timeout in milliseconds.
        /// </summary>
        public const int DEFAULT_TIMEOUT = 30000;
        private static HttpClient client = new HttpClient();
        internal static List<Request> currentRequests = new List<Request>();

        #endregion

        #region Constructor

        static SimpleHttpClient() {
            // Do not 100% know why I need dis, but I do...
            client.DefaultRequestHeaders.Add("User-Agent", "Toolkit");
        }

        #endregion

        #region Get Content

        public static Request GetContent(string uriRequest) => new Request(client, uriRequest);
        public static Request GetContent(Uri uriRequest) => new Request(client, uriRequest);
        public static Request GetContent(string uriRequest, float timeout) => new Request(client, uriRequest, timeout);
        public static Request GetContent(Uri uriRequest, float timeout) => new Request(client, uriRequest, timeout);

        #endregion

        #region Post Content

        public static Request PostContent(string uriRequest, HttpContent content) => new Request(client, uriRequest, content);
        public static Request PostContent(string uriRequest, byte[] content) => new Request(client, uriRequest, new ByteArrayContent(content));
        public static Request PostContent(string uriRequest, string plainText) => new Request(client, uriRequest, new StringContent(plainText));

        public static Request PostContent(HttpClient client, string uriRequest, HttpContent content) => new Request(client, uriRequest, content);
        public static Request PostContent(HttpClient client, string uriRequest, byte[] content) => new Request(client, uriRequest, new ByteArrayContent(content));
        public static Request PostContent(HttpClient client, string uriRequest, string plainText) => new Request(client, uriRequest, new StringContent(plainText));

        #endregion

        #region Send

        public static Request Send(HttpRequestMessage request) => new Request(client, request);

        #endregion

        #region Get Redirected URL

        public static Threading.TaskContainer GetRedirectedUrl(string url, Action<string> onComplete)
            => GetRedirectedUrl(url, onComplete, null);

        public static Threading.TaskContainer GetRedirectedUrl(string url, Action<string> onComplete, Action<Exception> onError) {
            var task = Threading.TaskUtility.CreateContainer(Internal_GetRedirectedUrl(url));
            task.OnComplete += onComplete;
            if(onError != null)
                task.OnError += (t) => onError?.Invoke(t.Exception);
            return task;
        }

        private static async Task<string> Internal_GetRedirectedUrl(string url) {
            var handler = new HttpClientHandler() {
                AllowAutoRedirect = false
            };
            string redirectedUrl = null;

            using(HttpClient client = new HttpClient(handler)) {
                client.DefaultRequestHeaders.Add("User-Agent", "Toolkit");
                HttpResponseMessage response = await client.GetAsync(url);
                HttpContent content = response.Content;
                HttpResponseHeaders headers = response.Headers;
                if(headers != null && headers.Location != null) {
                    redirectedUrl = headers.Location.AbsoluteUri;
                }
            }
            return redirectedUrl;
        }

        #endregion

        public class Request : IEnumerator {
            public enum State {
                Canceled = -2,
                Error = -1,
                None = 0,
                Complete = 1,
            }

            #region Variables

            private string uri;
            private Task<HttpResponseMessage> task;
            private byte[] data;
            private CancellationTokenSource cancellationTokenSource;
            private State state = State.None;

            private Action<byte[]> onComplete;
            private Action<Request> onCompleteRequest;
            private Action<Request> onError;
            private Action<Request> onCanceled;

            #endregion

            #region Properties

            public bool IsDone => state != State.None;
            public bool IsFaulty => state == State.Error;
            public bool IsCanceled => state == State.Canceled;
            public bool IsCompleted => state == State.Complete;

            public string UriRequest => uri;
            public State RequestState {
                get => state;
                private set {
                    if(this.state != State.None)
                        return;
                    this.state = value;
                    UtcCompleteTime = TKDateTime.UtcNow;
                }
            }

            public System.Exception Exception { get; private set; } = null;
            public HttpResponseMessage Message { get; private set; } = null;
            public HttpContent Content { get; private set; } = null;
            public TKDateTime UtcTime { get; private set; } = TKDateTime.UtcNow;
            public TKDateTime UtcCompleteTime { get; private set; }
            public TKTimeSpan CompleteDuration => UtcCompleteTime - UtcTime;

            public byte[] Data => data;

            #endregion

            #region Constructor

            public Request(HttpClient client, string uri)
                : this(client, uri, true, -1f) { }

            public Request(HttpClient client, Uri uri)
                : this(client, uri.OriginalString, true, -1f) { }

            public Request(HttpClient client, string uri, float timeout)
                : this(client, uri, false, timeout) { }

            public Request(HttpClient client, Uri uri, float timeout)
                : this(client, uri.OriginalString, false, timeout) { }

            public Request(HttpClient client, string uri, bool useDefaultTimeout)
                : this(client, uri, useDefaultTimeout, -1f) { }

            public Request(HttpClient client, Uri uri, bool useDefaultTimeout)
                : this(client, uri.OriginalString, useDefaultTimeout, -1f) { }

            private Request(HttpClient client, string uri, bool useDefaultTimeout, float timeout) {
                this.uri = uri;
                this.cancellationTokenSource = new CancellationTokenSource();
                if(useDefaultTimeout)
                    this.cancellationTokenSource.CancelAfter(DEFAULT_TIMEOUT);
                if(timeout > Mathf.Epsilon)
                    Cancel(timeout);

                this.task = client.GetAsync(uri, HttpCompletionOption.ResponseContentRead, cancellationTokenSource.Token);
                Load();
            }

            /// POST CONTENT

            public Request(HttpClient client, string uri, HttpContent content)
                : this(client, uri, content, true, -1f) { }

            public Request(HttpClient client, Uri uri, HttpContent content)
                : this(client, uri.OriginalString, content, true, -1f) { }

            public Request(HttpClient client, string uri, HttpContent content, float timeout)
                : this(client, uri, content, false, timeout) { }

            public Request(HttpClient client, Uri uri, HttpContent content, float timeout)
                : this(client, uri.OriginalString, content, false, timeout) { }

            public Request(HttpClient client, string uri, HttpContent content, bool useDefaultTimeout)
                : this(client, uri, content, useDefaultTimeout, -1f) { }

            public Request(HttpClient client, Uri uri, HttpContent content, bool useDefaultTimeout)
                : this(client, uri.OriginalString, content, useDefaultTimeout, -1f) { }

            private Request(HttpClient client, string uri, HttpContent content, bool useDefaultTimeout, float timeout) {
                this.uri = uri;
                this.cancellationTokenSource = new CancellationTokenSource();
                if(useDefaultTimeout)
                    this.cancellationTokenSource.CancelAfter(DEFAULT_TIMEOUT);
                if(timeout > Mathf.Epsilon)
                    Cancel(timeout);

                this.task = client.PostAsync(uri, content, cancellationTokenSource.Token);
                Load();
            }



            /// POST CONTENT

            public Request(HttpClient client, HttpRequestMessage request)
                : this(client, request, true, -1f) { }

            public Request(HttpClient client, HttpRequestMessage request, float timeout)
                : this(client, request, false, timeout) { }

            public Request(HttpClient client, HttpRequestMessage request, bool useDefaultTimeout)
                : this(client, request, useDefaultTimeout, -1f) { }

            private Request(HttpClient client, HttpRequestMessage request, bool useDefaultTimeout, float timeout) {
                this.uri = request.RequestUri.AbsoluteUri;
                this.cancellationTokenSource = new CancellationTokenSource();
                if(useDefaultTimeout)
                    this.cancellationTokenSource.CancelAfter(DEFAULT_TIMEOUT);
                if(timeout > Mathf.Epsilon)
                    Cancel(timeout);

                this.task = client.SendAsync(request, cancellationTokenSource.Token);
                Load();
            }

            #endregion

            #region Loading

            private async void Load() {
                try {
                    await task;
                    switch(task.Status) {
                        case TaskStatus.Canceled:
                            RequestState = State.Canceled;
                            onCanceled?.Invoke(this);
                            return;
                        case TaskStatus.Faulted:
                            RequestState = State.Error;
                            Exception = task.Exception;
                            onError?.Invoke(this);
                            return;
                        case TaskStatus.RanToCompletion:

                            break;
                        default:
                            UnityEngine.Debug.LogWarning(TAG + $"Non-supported status of task found '{task.Status}' at http request '{uri}'");
                            break;
                    }
                }
                catch(System.Exception e) {
                    Exception = e;
                    RequestState = State.Error;
                    onError?.Invoke(this);
                    return;
                }

                if(cancellationTokenSource.IsCancellationRequested) {
                    RequestState = State.Canceled;
                    onCanceled?.Invoke(this);
                    return;
                }

                if(state != State.None) // Safety check
                    return;

                Message = task.Result;

                if(Message.StatusCode != HttpStatusCode.OK) {
                    RequestState = State.Error;
                    onError?.Invoke(this);
                    return;
                }
                Content = Message.Content;
                try {
                    var readContentTask = Content.ReadAsByteArrayAsync();
                    await readContentTask;

                    switch(readContentTask.Status) {
                        case TaskStatus.RanToCompletion:
                            data = readContentTask.Result;
                            RequestState = State.Complete;
                            break;
                        case TaskStatus.Canceled:
                            RequestState = State.Canceled;
                            onCanceled?.Invoke(this);
                            return;
                        default:
                            RequestState = State.Error;
                            Exception = readContentTask.Exception;
                            onError?.Invoke(this);
                            return;
                    }
                }
                catch(System.Exception e) {
                    Exception = e;
                    RequestState = State.Error;
                    onError?.Invoke(this);
                    return;
                }

                if(data != null && state == State.Complete) {
                    onComplete?.Invoke(data);
                    onCompleteRequest?.Invoke(this);
                }
                else {
                    RequestState = State.Error;
                    onError?.Invoke(this);
                }
            }

            public async System.Threading.Tasks.Task<State> Wait(CancellationToken cancellation = default) {
                while(state == State.None) {
                    await System.Threading.Tasks.Task.Yield();
                    if(cancellation.IsCancellationRequested) {
                        return State.Canceled;
                    }
                }
                return state;
            }

            #endregion

            #region Cancel

            public void Cancel() {
                if(state == State.None)
                    cancellationTokenSource.Cancel();
            }

            public void Cancel(float delay) {
                if(state != State.None)
                    return;
                int delayMs = Mathf.CeilToInt(delay * 1000f);
                if(delayMs < 0)
                    delayMs = 0;
                cancellationTokenSource.CancelAfter(delayMs);
            }

            #endregion

            #region Callbacks

            public Request OnComplete(Action<byte[]> onComplete) {
                this.onComplete += onComplete;
                if(state == State.Complete)
                    onComplete?.Invoke(data);

                return this;
            }

            public Request OnComplete(Action<Request> onComplete) {
                this.onCompleteRequest += onComplete;
                if(state == State.Complete)
                    onComplete?.Invoke(this);

                return this;
            }

            public Request OnError(Action<Request> onError) {
                this.onError += onError;
                if(state == State.Error)
                    onError?.Invoke(this);

                return this;
            }

            public Request OnErrorLog() {
                this.onError += InternalLogCallback;
                if(state == State.Error)
                    InternalLogCallback(this);
                return this;
            }

            public Request OnCanceled(Action<Request> onCanceled) {
                this.onCanceled += onCanceled;
                if(state == State.Canceled)
                    onCanceled?.Invoke(this);

                return this;
            }

            private static void InternalLogCallback(Request r) {
                r.LogError();
            }

            #endregion

            #region Logging

            public string GetError() {
                switch(state) {
                    case State.Error: {
                            if(Exception != null)
                                return $"Request '{uri}' raised an exception:\n\tMessage: {Exception.Message}\n\tSource: {Exception.Source}\n\n\tStack Trace: {Exception.StackTrace}";
                            else
                                return $"Request ended with an error '{uri}'\n\tStatusCode: {(Message?.StatusCode ?? ((HttpStatusCode)0))}";
                        }
                    case State.Canceled:
                        return $"Request was canceled '{uri}'";
                }
                return "unknown";
            }

            public void LogError() {
                switch(state) {
                    case State.Error: {
                            if(Exception != null)
                                Debug.LogError(TAG + $"Request '{uri}' raised an exception:\n\tMessage: {Exception.Message}\n\tSource: {Exception.Source}\n\n\tStack Trace: {Exception.StackTrace}");
                            else
                                Debug.LogError(TAG + $"Request ended with an error '{uri}'\n\tStatusCode: {(Message?.StatusCode ?? ((HttpStatusCode)0))}");
                        }
                        break;
                    case State.Canceled:
                        Debug.LogWarning(TAG + $"Request was canceled '{uri}'");
                        break;
                    case State.Complete:
                        Debug.Log(TAG + $"Request was completed successfully '{uri}'");
                        break;
                    case State.None:
                        Debug.Log(TAG + $"No errors, request is still on-going '{uri}'");
                        break;
                }
            }



            public void LogError(string customTag) {
                switch(state) {
                    case State.Error: {
                            if(Exception != null)
                                Debug.LogError(customTag + $"Request '{uri}' raised an exception:\n\tMessage: {Exception.Message}\n\tSource: {Exception.Source}\n\n\tStack Trace: {Exception.StackTrace}");
                            else
                                Debug.LogError(customTag + $"Request ended with an error '{uri}'\n\tStatusCode: {(Message?.StatusCode ?? ((HttpStatusCode)0))}");
                        }
                        break;
                    case State.Canceled:
                        Debug.LogWarning(customTag + $"Request was canceled '{uri}'");
                        break;
                    case State.Complete:
                        Debug.Log(customTag + $"Request was completed successfully '{uri}'");
                        break;
                    case State.None:
                        Debug.Log(customTag + $"No errors, request is still on-going '{uri}'");
                        break;
                }
            }

            #endregion

            #region IEnumerator Impl

            object IEnumerator.Current => data;
            bool IEnumerator.MoveNext() => IsDone;
            void IEnumerator.Reset() { }

            #endregion
        }
    }
}
