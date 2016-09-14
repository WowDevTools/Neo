using SharpDX.Direct2D1;
using Neo.UI.Components;

namespace Neo.UI
{
    interface IComponent
    {
        void OnRender(RenderTarget target);
        void OnMessage(Message message);
    }
}
