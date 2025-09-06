using System;

namespace Toolkit {
    public interface IEarlyUpdate : INullable {
        void EarlyUpdate(float dt);
    }

    public interface IUpdate : INullable {
        void Update(float dt);
    }

    public interface ILateUpdate : INullable {
        void LateUpdate(float dt);
    }

    public interface IPostUpdate : INullable {
        void PostUpdate(float dt);
    }

    public interface IFixedUpdate : INullable {
        void FixedUpdate(float dt);
    }

    public interface IOnBeforeRender : INullable {
        void OnBeforeRender(float dt);
    }

    public interface IOnGUI : INullable
    {
        void OnGUI(float dt);
    }
}
