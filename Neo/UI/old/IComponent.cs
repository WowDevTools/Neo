using SharpDX.Direct2D1;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI
{
    interface IComponent
    {
        void OnRender(RenderTarget target);
        void OnMessage(Message message);
    }
}
