using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.UI.Components;

namespace WoWEditor6.UI.Panels
{
    class TerrainParams : IComponent
    {
        private readonly Frame mFrame;

        public bool Visible { get; set; }

        public TerrainParams()
        {
            mFrame = new Frame
            {
                HasCaption = false,
                Size = new Vector2(200, 150),
            };
        }

        public void OnResize(Vector2 newSize)
        {
            mFrame.Position = new Vector2(newSize.X - 330, 100.0f);
        }

        public void OnRender(RenderTarget target)
        {
            if (Visible == false)
                return;

            mFrame.OnRender(target);
        }

        public void OnMessage(Message message)
        {
            mFrame.OnMessage(message);
        }
    }
}
