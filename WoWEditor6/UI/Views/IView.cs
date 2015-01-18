using SharpDX;

namespace WoWEditor6.UI.Views
{
    interface IView : IComponent
    {
        void OnResize(Vector2 newSize);
        void OnShow();
    }
}
