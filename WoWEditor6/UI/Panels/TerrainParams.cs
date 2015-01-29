using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.UI.Components;
using WoWEditor6.UI.Dialogs;

namespace WoWEditor6.UI.Panels
{
    class TerrainParams : IComponent
    {
        private readonly TerrainSettings mDialog;

        public bool Visible { get { return mDialog.Visible; } set { mDialog.Visible = value; } }

        public TerrainParams()
        {
            mDialog = new TerrainSettings();
            mDialog.FormClosing += (sender, args) =>
            {
                if (args.CloseReason != System.Windows.Forms.CloseReason.UserClosing)
                    return;

                mDialog.Visible = false;
                args.Cancel = true;
            };

            mDialog.Owner = InterfaceManager.Instance.Window;
            mDialog.Visible = false;
        }

        public void OnResize(Vector2 newSize)
        {

        }

        public void OnRender(RenderTarget target)
        {

        }

        public void OnMessage(Message message)
        {

        }
    }
}
