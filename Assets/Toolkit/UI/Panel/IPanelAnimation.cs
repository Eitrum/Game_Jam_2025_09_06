
namespace Toolkit.UI.PanelSystem {
    public interface IPanelAnimation {
        bool IsPlaying { get; }
        bool IsComplete { get; }
        void Show();
        void Hide();
        void Cancel();
    }

    public interface IPanelAnimationObject {
        bool IsComplete { get; }
        bool IsEnabled { get; }
        void Show(float duration);
        void Hide(float duration);
        void Update(float dt);
        void Cancel();
    }
}
