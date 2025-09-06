using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolkit.Threading;

namespace Toolkit.Network
{
    public static class HttpClientExtensions
    {
        #region Callback Get

        public static TaskContainer<HttpResponseMessage> Get(
            this HttpClient client,
            string requestUri,
            Action<HttpResponseMessage> responseCallback) 
        {
            return Get(client, new Uri(requestUri), HttpCompletionOption.ResponseContentRead, responseCallback);
        }

        public static TaskContainer<HttpResponseMessage> Get(
            this HttpClient client,
            Uri requestUri,
            HttpCompletionOption option,
            Action<HttpResponseMessage> responseCallback)
        {
            TaskContainer<HttpResponseMessage> taskContainer = new TaskContainer<HttpResponseMessage>((token) => client.GetAsync(requestUri, option, token));
            taskContainer.OnComplete += responseCallback;
            return taskContainer;
        }

        public static TaskContainer<HttpResponseMessage> Get(
            this HttpClient client, 
            Uri requestUri, 
            HttpCompletionOption option, 
            float timeout, 
            Action<HttpResponseMessage> responseCallback) 
        {
            TaskContainer<HttpResponseMessage> taskContainer = new TaskContainer<HttpResponseMessage>((token) => client.GetAsync(requestUri, option, token));
            taskContainer.OnComplete += responseCallback;
            taskContainer.Cancel(timeout);
            return taskContainer;
        }

        #endregion
    }
}
