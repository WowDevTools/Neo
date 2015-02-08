using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.Scene;
using WoWEditor6.UI.Components;
using WoWEditor6.UI.Dialogs;

namespace WoWEditor6.UI.Panels
{
    class KeySettings
    {
        private readonly InputSettings mSettingsDialog;

        public bool Visible { get { return mSettingsDialog.Visible; } set { mSettingsDialog.Visible = value; } }

        public KeySettings()
        {
            mSettingsDialog = new InputSettings();
            mSettingsDialog.FormClosing += (sender, args) =>
            {
                if (args.CloseReason != System.Windows.Forms.CloseReason.UserClosing)
                    return;

                mSettingsDialog.Visible = false;
                args.Cancel = true;
            };

            mSettingsDialog.Visible = false;
	        mSettingsDialog.InvertMouseBox.CheckedChanged +=
		        (sender, args) =>
		        {
			        var invert = mSettingsDialog.InvertMouseBox.Checked;
			        if (WorldFrame.Instance.LeftHandedCamera)
				        invert = !invert;

			        WorldFrame.Instance.CamControl.InvertX = invert;
			        WorldFrame.Instance.CamControl.InvertY = !invert;
		        };
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
