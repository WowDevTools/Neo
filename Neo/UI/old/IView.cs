using SharpDX;

namespace Neo.UI.Views
{
    interface IView : IComponent
    {
        void OnResize(Vector2 newSize);
        void OnShow();
    }
}
