using System;
using System.Collections.Generic;
using System.Drawing;
using SharpDX;
using SharpDX.Direct2D1;
using WoWEditor6.Resources;
using WoWEditor6.Scene;
using WoWEditor6.Settings;
using WoWEditor6.UI.Components;
using WoWEditor6.UI.Panels;

namespace WoWEditor6.UI.Views
{
    class WorldView : IView
    {
        public static WorldView Instance { get; private set; }

        private readonly PerformanceControl mPerfControl = new PerformanceControl();

        private readonly Toolbar mTopToolbar = new Toolbar();
        private readonly Toolbar mLeftToolbar = new Toolbar();
        private readonly StatusBar mStatusBar = new StatusBar();
        private bool mToolbarInitialized;
        private readonly TerrainParams mTerrainParamsPanel = new TerrainParams();
        private readonly KeySettings mKeySettingsPanel = new KeySettings();
        private readonly Label mTooltipLabel = new Label();

        private readonly Dictionary<ToolbarFunction, Action<ImageButton>> mButtonHandlers =
            new Dictionary<ToolbarFunction, Action<ImageButton>>();

        private readonly Dictionary<ToolbarFunction, Image> mButtonImages;

        public KeySettings KeySettingsDialog { get { return mKeySettingsPanel; } }

        public WorldView()
        {
            Instance = this;

            mButtonHandlers.Add(ToolbarFunction.Terrain, OnTerrainButton);
            mButtonHandlers.Add(ToolbarFunction.KeyBinding, OnKeySettingsButton);
	        mButtonHandlers.Add(ToolbarFunction.Save, OnSave);

	        mButtonImages = new Dictionary<ToolbarFunction, Image>
	        {
		        {ToolbarFunction.Terrain, Images.wheelbarrow_48},
		        {ToolbarFunction.KeyBinding, Images.joystick_48},
		        {ToolbarFunction.Save, Images.save_48}
	        };
        }

        public void OnRender(RenderTarget target)
        {
#if DEBUG
            mPerfControl.OnRender(target);
#endif
            mTerrainParamsPanel.OnRender(target);
            mKeySettingsPanel.OnRender(target);

            mTopToolbar.OnRender(target);
            mLeftToolbar.OnRender(target);
            mStatusBar.OnRender(target);
        }

        public void OnMessage(Message message)
        {
            mTopToolbar.OnMessage(message);
            mLeftToolbar.OnMessage(message);
            mTerrainParamsPanel.OnMessage(message);
            mKeySettingsPanel.OnMessage(message);
        }

        public void OnResize(Vector2 newSize)
        {
            mPerfControl.Position = new Vector2(newSize.X - 350, 30.0f);
            mPerfControl.Size = new Vector2(300, 280);
            mTopToolbar.Position = new Vector2(0, 0);
            mTopToolbar.Size = new Vector2(newSize.X, 56);
            mLeftToolbar.Position = new Vector2(0, 56.0f);
            mLeftToolbar.Size = new Vector2(66.0f, newSize.Y - 86.0f);
            mTerrainParamsPanel.OnResize(newSize);
            mKeySettingsPanel.OnResize(newSize);
            mStatusBar.Size = new Vector2(newSize.X, 30.0f);
            mStatusBar.Position = new Vector2(0.0f, newSize.Y - 30.0f);
        }

        public void OnShow()
        {
            if (mToolbarInitialized == false)
                InitToolbars();

            mPerfControl.Reset();
        }

        private void InitToolbars()
        {
            ToolbarSettings.Load();

            mToolbarInitialized = false;
            foreach (var button in ToolbarSettings.Settings.Top.Buttons)
            {
                Action<ImageButton> callback;
                mButtonHandlers.TryGetValue(button.Function, out callback);
                var ibutton = new ImageButton(callback)
                {
                    Image = mButtonImages[button.Function]
                };

                var tooltip = button.Tooltip ?? "";
                ibutton.MouseEnter += ib => mTooltipLabel.Text = tooltip;
                ibutton.MouseLeave += ib => mTooltipLabel.Text = "";

                mTopToolbar.Buttons.Add(ibutton);
            }

            foreach(var button in ToolbarSettings.Settings.Left.Buttons)
            {
                Action<ImageButton> callback;
                mButtonHandlers.TryGetValue(button.Function, out callback);
                var ibutton = new ImageButton(callback)
                {
                    Image = mButtonImages[button.Function]
                };

                var tooltip = button.Tooltip ?? "";
                ibutton.MouseEnter += ib => mTooltipLabel.Text = tooltip;
                ibutton.MouseLeave += ib => mTooltipLabel.Text = "";

                mLeftToolbar.Buttons.Add(ibutton);
            }

            mTopToolbar.BorderOffsets = new Vector2(66.0f, 0.0f);
            mLeftToolbar.Orientation = ToolbarOrientation.Vertical;

            mStatusBar.BorderOffsets = new Vector2(66.0f, 0.0f);
            mStatusBar.Items.Add(mTooltipLabel);

            mTooltipLabel.Position = new Vector2(5, 5);
            mTooltipLabel.Size = new Vector2(float.MaxValue, 20.0f);
            mTooltipLabel.FontSize = 13.0f;
        }

        private void OnTerrainButton(ImageButton button)
        {
            mTerrainParamsPanel.Visible = true;
            Editing.EditManager.Instance.EnableSculpting();
        }

        private void OnKeySettingsButton(ImageButton button)
        {
            mKeySettingsPanel.Visible = true;
        }

		private void OnSave(ImageButton button)
		{
			WorldFrame.Instance.MapManager.OnSaveAllFiles();
		}
    }
}
