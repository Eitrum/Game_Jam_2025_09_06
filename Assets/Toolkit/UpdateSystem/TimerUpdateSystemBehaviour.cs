using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    internal class TimerUpdateSystemBehaviour : MonoSingleton<TimerUpdateSystemBehaviour>
    {
        protected override bool KeepAlive { get => true; set => base.KeepAlive = value; }

        private TLinkedList<Container> containers = new TLinkedList<Container>();

        public static void Subscribe<T>(T nullable, float delay, OnTimerUpdateCallback callback) where T : INullable {
            Instance.containers.Add(new Container(nullable, callback, delay));
        }

        public static void Unsubscribe<T>(T nullable, OnTimerUpdateCallback callback) where T : INullable {
            var n = nullable as INullable;
            var node = Instance.containers.Find(x => (x.nullable == n && x.callback == callback));
            if(node != null)
                Instance.containers.Remove(node);
        }

        private void Update() {
            float dt = Time.deltaTime;
            var enumerator = containers.GetEnumerator();
            TLinkedListNode<Container> updateNode;
            while(enumerator.MoveNext(out updateNode)) {
                if(updateNode.Value.nullable.IsNull)
                    enumerator.DestroyCurrent();
                else
                    updateNode.Value.Update(dt);
            }
        }

        private class Container
        {
            public INullable nullable;
            public OnTimerUpdateCallback callback;
            private float timer = 0f;
            private float delay = 0f;

            public void Update(float dt) {
                timer += dt;
                if(timer >= delay) {
                    timer -= delay;
                    callback?.Invoke(delay);
                }
            }

            public Container(INullable nullable, OnTimerUpdateCallback callback, float delay) {
                this.nullable = nullable;
                this.callback = callback;
                this.delay = delay;
            }
        }
    }

    public delegate void OnTimerUpdateCallback(float dt);
}
