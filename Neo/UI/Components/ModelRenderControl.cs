using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Neo.Graphics;
using Neo.IO;
using Neo.IO.Files;
using Neo.IO.Files.Models;
using Neo.Scene;
using Neo.Scene.Models.M2;
using Neo.Scene.Texture;
using Neo.Storage;
using Color = System.Drawing.Color;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Rectangle = System.Drawing.Rectangle;
<<<<<<< HEAD:Neo/UI/Components/ModelRenderControl.cs
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using SlimTK;
=======
>>>>>>> 68061948880901a73043ec22a51fa0c733353565:Neo/UI/Components/ModelRenderControl.cs

namespace Neo.UI.Components
{
    public partial class ModelRenderControl : UserControl
    {
	    private class AnimationIndexEntry
        {
            public int AnimationIndex { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return this.Name;
            }
        }

        private PerspectiveCamera mCamera;
        private CameraControl mCamControl;
        private UniformBuffer mMatrixBuffer;
        private RenderTarget mTarget;
        private Texture2D mResolveTexture;
        private Texture2D mMapTexture;
        private Bitmap mPaintBitmap;

        private M2Renderer mRenderer;

        public override bool Focused
        {
            get
            {
                return base.Focused || IsFocusedElementInTree(this.flowLayoutPanel1);
            }
        }


        public ModelRenderControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.Selectable, true);
            InitializeComponent();
        }

        public void SetModel(string model, int variation = 0)
        {
            var file = ModelFactory.Instance.CreateM2(model);
            file.DisplayOptions.TextureVariation = variation;
            if (file.Load() == false)
            {
	            return;
            }

	        this.mRenderer = new M2Renderer(file);
            SetModelCameraParameters(file);
        }

        public void SetCreatureDisplayEntry(int entry)
        {
            var displayInfo = DbcStorage.CreatureDisplayInfo.GetRowById(entry);
            if (displayInfo == null)
            {
	            return;
            }

	        var modelId = displayInfo.GetInt32(1);
            var modelData = DbcStorage.CreatureModelData.GetRowById(modelId);
            if (modelData == null)
            {
	            return;
            }

	        var modelPath = string.Empty;
            if (FileManager.Instance.Version <= FileDataVersion.Warlords)
            {
                var fileDataId = modelData.GetInt32(2);
                var modelPathEntry = DbcStorage.FileData.GetRowById(fileDataId);
                if (modelPathEntry != null)
                {
	                modelPath = Path.Combine(modelPathEntry.GetString(2), modelPathEntry.GetString(1));
                }
            }
            else
            {
	            modelPath = modelData.GetString(2);
            }

	        if (string.IsNullOrEmpty(modelPath))
	        {
		        return;
	        }

	        if (modelPath.ToUpperInvariant().EndsWith(".MDX"))
	        {
		        modelPath = Path.ChangeExtension(modelPath, ".m2");
	        }

	        if (FileManager.Instance.Provider.Exists(modelPath) == false)
	        {
		        return;
	        }

	        var file = ModelFactory.Instance.CreateM2(modelPath);
            try
            {
                if (file.Load() == false)
                {
	                return;
                }
            }
            catch (Exception)
            {
                return;
            }

	        this.mRenderer = new M2Renderer(file);
            for (var i = 0; i < this.mRenderer.PortraitRenderer.Textures.Length; ++i)
            {
                var tex = this.mRenderer.PortraitRenderer.Textures[i];
                var root = this.mRenderer.Model.ModelRoot;

                switch (tex.TextureType)
                {
                    case TextureType.MonsterSkin3:
                    case TextureType.MonsterSkin2:
                    case TextureType.MonsterSkin1:
                        tex.Texture = TextureManager.Instance.GetTexture(GetSkinName(root, displayInfo, (int)tex.TextureType - 11));
                        break;
                }
            }

            SetModelCameraParameters(file);
        }

        private void SetModelCameraParameters(M2File file)
        {
            var bboxMin = file.BoundingBox.Minimum.Z;
            var bboxMax = file.BoundingBox.Maximum.Z;
            WorldFrame.Instance.Dispatcher.BeginInvoke(() =>
            {
	            this.mCamera.SetParameters(new Vector3(file.BoundingRadius * 1.5f, 0, bboxMin + (bboxMax - bboxMin) / 2),
                    new Vector3(0, 0, bboxMin + (bboxMax - bboxMin) / 2), Vector3.UnitZ, Vector3.UnitY);

	            this.comboBox1.Items.Clear();

                var values = Enum.GetValues(typeof(AnimationType));

                if (file.AnimationLookup.Length > 0) //Animation Lookup
                {
                    foreach (int anim in values)
                    {
                        if (anim >= file.AnimationLookup.Length)
                        {
	                        continue;
                        }

	                    if (file.AnimationLookup[anim] < 0)
	                    {
		                    continue;
	                    }

	                    this.comboBox1.Items.Add(new AnimationIndexEntry
                        {
                            AnimationIndex = anim,
                            Name = Enum.GetName(typeof(AnimationType), anim)
                        });
                    }
                }
                else if (file.AnimationIds.Length > 0) //Raw Animation Check
                {
                    foreach (int anim in values)
                    {
                        if (Array.IndexOf(file.AnimationIds, (ushort)anim) == -1)
                        {
	                        continue;
                        }

	                    this.comboBox1.Items.Add(new AnimationIndexEntry
                        {
                            AnimationIndex = anim,
                            Name = Enum.GetName(typeof(AnimationType), anim)
                        });
                    }
                }

                if (this.comboBox1.Items.Count > 0) //Preset combobox and animator state
                {
	                this.comboBox1.SelectedIndex = 0;
	                this.mRenderer.PortraitRenderer.Animator.SetAnimation((AnimationType)((AnimationIndexEntry) this.comboBox1.Items[0]).AnimationIndex);
                }
            });


            if (file.DisplayOptions.TextureVariationFiles.Count > 1)
            {
	            this.nudVariation.ReadOnly = false;
	            this.nudVariation.Maximum = file.DisplayOptions.TextureVariationFiles.Count;
	            this.nudVariation.Value = file.DisplayOptions.TextureVariation + 1;
	            this.nudVariation.Increment = 1;
            }
            else
            {
	            this.nudVariation.Increment = 0;
	            this.nudVariation.ReadOnly = true;
            }
        }

        private string GetSkinName(string root, IDataStorageRecord displayInfo, int index)
        {
            if (displayInfo == null)
            {
	            throw new ArgumentNullException("displayInfo");
            }
	        var skinString = displayInfo.GetString(6 + index);
            return string.IsNullOrEmpty(skinString)
                ? "default_texture"
                : string.Format("{0}\\{1}.blp", root, skinString);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            if (this.mPaintBitmap != null)
            {
                e.Graphics.DrawImage(this.mPaintBitmap, new PointF(0, 0));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Site != null && Site.DesignMode)
            {
	            return;
            }

	        if (WorldFrame.Instance == null || WorldFrame.Instance.GraphicsContext == null)
	        {
		        return;
	        }

	        this.mTarget = new RenderTarget(WorldFrame.Instance.GraphicsContext);
	        this.mMatrixBuffer = new UniformBuffer(WorldFrame.Instance.GraphicsContext);

	        this.mCamera = new PerspectiveCamera();
	        this.mCamera.ViewChanged += ViewChanged;
	        this.mCamera.ProjectionChanged += ProjChanged;
	        this.mCamera.SetClip(0.2f, 1000.0f);
	        this.mCamera.SetParameters(new Vector3(10, 0, 0), Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
	        this.mCamControl = new CameraControl(this)
            {
                TurnFactor = 0.1f,
                SpeedFactor = 20.0f
            };

            MouseDown += OnClick;
            Resize += OnResize;
	        this.renderTimer.Tick += OnRenderTimerTick;

            OnResize(this, null);

	        this.renderTimer.Start();
        }

	    private void OnResize(object sender, EventArgs args)
        {
            var texDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = ClientSize.Height,
                Width = ClientSize.Width,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0),
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1
            };

            if (this.mResolveTexture != null)
            {
	            this.mResolveTexture.Dispose();
            }

	        this.mResolveTexture = new Texture2D(WorldFrame.Instance.GraphicsContext.Device, texDesc);

            if (this.mMapTexture != null)
            {
	            this.mMapTexture.Dispose();
            }

	        texDesc.CpuAccessFlags = CpuAccessFlags.Read;
            texDesc.Usage = ResourceUsage.Staging;
	        this.mMapTexture = new Texture2D(WorldFrame.Instance.GraphicsContext.Device, texDesc);

	        this.mTarget.Resize(ClientSize.Width, ClientSize.Height, true);
	        this.mCamera.SetAspect((float)ClientSize.Width / ClientSize.Height);
        }

	    private void OnClick(object sender, MouseEventArgs args)
        {
            Focus();
        }

	    private void OnRenderTimerTick(object sender, EventArgs args)
        {
	        this.mCamControl.Update(this.mCamera, false);
            if (WorldFrame.Instance.Dispatcher.InvokeRequired)
            {
	            WorldFrame.Instance.Dispatcher.BeginInvoke(OnRenderModel);
            }
            else
            {
	            OnRenderModel();
            }
        }

	    private void ViewChanged(Camera cam, Matrix4 matView)
        {
	        this.mMatrixBuffer.BufferData(cam.ViewProjection);
        }

	    private void ProjChanged(Camera cam, Matrix4 matProj)
        {
	        this.mMatrixBuffer.BufferData(cam.ViewProjection);
        }

	    private unsafe void OnRenderModel()
        {
            if (this.mRenderer == null)
            {
	            return;
            }

	        this.mTarget.Clear();
	        this.mTarget.Apply();

            var ctx = WorldFrame.Instance.GraphicsContext;
            var vp = ctx.Viewport;
            ctx.Context.Rasterizer.SetViewport(new Viewport(0, 0, ClientSize.Width, ClientSize.Height, 0.0f, 1.0f));

            ctx.Context.VertexShader.SetConstantBuffer(0, this.mMatrixBuffer.BufferID);
	        this.mRenderer.RenderPortrait();

	        this.mTarget.Remove();
            ctx.Context.Rasterizer.SetViewport(vp);

            ctx.Context.ResolveSubresource(this.mTarget.Texture, 0, this.mResolveTexture, 0, Format.B8G8R8A8_UNorm);
            ctx.Context.CopyResource(this.mResolveTexture, this.mMapTexture);

            var box = ctx.Context.MapSubresource(this.mMapTexture, 0, MapMode.Read, MapFlags.None);
            var bmp = new Bitmap(ClientSize.Width, ClientSize.Height, PixelFormat.Format32bppArgb);
            var bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            byte* ptrDst = (byte*)bmpd.Scan0.ToPointer();
            byte* ptrSrc = (byte*)box.DataPointer.ToPointer();

            for (var i = 0; i < bmp.Height; ++i)
            {
                UnsafeNativeMethods.CopyMemory(ptrDst + i * bmp.Width * 4, ptrSrc + i * box.RowPitch, bmp.Width * 4);
            }

            bmp.UnlockBits(bmpd);
            if (this.mPaintBitmap != null)
            {
	            this.mPaintBitmap.Dispose();
            }

	        this.mPaintBitmap = bmp;
            ctx.Context.UnmapSubresource(this.mMapTexture, 0);
            Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
	        this.mCamControl.InvertX = this.checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
	        this.mCamControl.InvertY = this.checkBox2.Checked;
        }

        private bool IsFocusedElementInTree(Control control)
        {
            return control.Focused || control.Controls.OfType<Control>().Any(IsFocusedElementInTree);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.mRenderer == null)
            {
	            return;
            }

	        var item = this.comboBox1.SelectedItem as AnimationIndexEntry;
            if (item == null)
            {
	            return;
            }

	        this.mRenderer.PortraitRenderer.Animator.SetAnimation((AnimationType)item.AnimationIndex);
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
	            return;
            }

	        e.Handled = true;
        }

        private void nudVariation_ValueChanged(object sender, EventArgs e)
        {
	        this.nudVariation.ReadOnly = true;
	        this.nudVariation.Increment = 0;
            SetModel(this.mRenderer.Model.FileName, (int)(this.nudVariation.Value - 1));
        }
    }
}
