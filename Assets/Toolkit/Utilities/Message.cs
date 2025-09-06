using System;
using System.Collections.Generic;

namespace Toolkit {
    public static class Message {
        public delegate void SimpleMessageDelegate(string payload);

        #region Variables

        private const string TAG = "[Toolkit.Message] - ";
        internal static List<Type> messageTypesActive = new List<Type>();
        internal static Dictionary<string, TLinkedList<SimpleMessageDelegate>> simpleMessage = new Dictionary<string, TLinkedList<SimpleMessageDelegate>>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Publish

        public static void Publish<T>(T message) where T : class {
            Message<T>.Publish(message);
        }

        public static void Publish(string eventId)
            => Publish(eventId, string.Empty);

        public static void Publish(string eventId, string payload) {
            if(simpleMessage.TryGetValue(eventId, out var callbacks)) {
                foreach(var c in callbacks) {
                    try {
                        c.Invoke(payload);
                    }
                    catch(Exception e) {
                        UnityEngine.Debug.LogError(TAG + $"SimpleMessage event has null failing callbacks: '{eventId}'");
                        UnityEngine.Debug.LogException(e);
                    }
                }
            }
        }

        #endregion

        #region Subscribe

        public static TLinkedListNode<MessageTarget<T>> Subscribe<T>(INullable nullable, Action<T> method) {
            return Message<T>.Subscribe(nullable, method);
        }

        public static TLinkedListNode<MessageTarget<T>> Subscribe<T>(Action<T> method, INullable nullable) {
            return Message<T>.Subscribe(nullable, method);
        }

        public static void Subscribe(string eventId, SimpleMessageDelegate callback) {
            if(!simpleMessage.TryGetValue(eventId, out var list)) {
                list = new TLinkedList<SimpleMessageDelegate>();
                simpleMessage.Add(eventId, list);
            }
            list.Add(callback);
        }

        #endregion

        #region Unsubscribe

        public static bool Unsubscribe<T>(TLinkedListNode<MessageTarget<T>> node) {
            return Message<T>.Unsubscribe(node);
        }

        public static bool Unsubscribe<T>(INullable nullable, Action<T> method) {
            return Message<T>.Unsubscribe(nullable, method);
        }

        public static void Unsubscribe(string eventId, SimpleMessageDelegate callback) {
            if(simpleMessage.TryGetValue(eventId, out var list)) {
                var iterator = list.GetEnumerator();
                while(iterator.MoveNext(out TLinkedListNode<SimpleMessageDelegate> node)) {
                    if(node.Value == callback)
                        iterator.DestroyCurrent();
                }
            }
        }

        public static void Clear(string eventId) {
            if(simpleMessage.TryGetValue(eventId, out var list))
                list.Clear();
        }

        public static void ClearAll() {
            simpleMessage.Clear();
        }

        #endregion
    }

    public sealed class MessageTarget<T> : IMessageTargetData {

        #region Variables

        private Action<T> method;
        private INullable nullable;

        private int calls = 0;

        #endregion

        #region Properties

        public bool IsNull => nullable == null || nullable.IsNull;
        public int Calls => calls;

        INullable IMessageTargetData.Nullable => nullable;
        System.Reflection.MethodInfo IMessageTargetData.MethodInfo => method.Method;

        #endregion

        #region Constructor

        internal MessageTarget(INullable nullable, Action<T> method) {
            this.method = method;
            this.nullable = nullable;
        }

        #endregion

        #region Sending

        public void Send(T message) {
            calls++;
            method(message);
        }

        #endregion

        #region Utility

        public bool Is(INullable nullable, Action<T> method) {
            return this.nullable == nullable && this.method == method;
        }

        #endregion
    }

    internal interface IMessageTargetData {
        int Calls { get; }
        System.Reflection.MethodInfo MethodInfo { get; }
        INullable Nullable { get; }
    }

    internal static class Message<T> {

        #region Variables

        private static TLinkedList<MessageTarget<T>> subscribers = new TLinkedList<MessageTarget<T>>();
        private static int publishedMessages = 0;

        #endregion

        #region Properties

        public static int SubscriberCount => subscribers.Count;

        public static IMessageTargetData[] Targets {
            get {
                var targets = new IMessageTargetData[SubscriberCount];
                int index = 0;
                foreach(var t in subscribers) {
                    targets[index++] = t;
                }
                return targets;
            }
        }

        #endregion

        #region Constructor

        static Message() {
            Message.messageTypesActive.Add(typeof(Message<T>));
        }

        #endregion

        #region Publish

        public static void Publish(T message) {
            publishedMessages++;
            var enumerator = subscribers.GetEnumerator();
            TLinkedListNode<MessageTarget<T>> node;
            while(enumerator.MoveNext(out node)) {
                if(node.Value.IsNull)
                    enumerator.DestroyCurrent();
                else
                    node.Value.Send(message);
            }
        }

        #endregion

        #region Subscribe

        public static TLinkedListNode<MessageTarget<T>> Subscribe(INullable nullable, Action<T> method) {
            return subscribers.AddLast(new MessageTarget<T>(nullable, method));
        }

        #endregion

        #region Unsubscribe

        public static bool Unsubscribe(TLinkedListNode<MessageTarget<T>> node) {
            return subscribers.Remove(node);
        }

        public static bool Unsubscribe(INullable nullable, Action<T> method) {
            var node = subscribers.Find(x => x.Is(nullable, method));
            return subscribers.Remove(node);
        }

        #endregion
    }
}
