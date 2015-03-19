using System;
using SharpDX;
using WoWEditor6.IO.Files.Models;
using WoWEditor6.Utils;

namespace WoWEditor6.Scene.Models.WMO
{
    class WmoInstance : IModelInstance
    {
        private readonly Matrix mInstanceMatrix;
        private Matrix mInverseInstanceMatrix;
        private WeakReference<WmoRootRender> mRenderer; 

        public BoundingBox BoundingBox;

        private WorldText mWorldModelName;

        public int Uuid { get; private set; }
        public BoundingBox[] GroupBoxes { get; private set; }
        public Matrix InstanceMatrix { get { return mInstanceMatrix; } }

        public WmoRoot ModelRoot { get; private set; }

        public int ReferenceCount;

        public WmoInstance(int uuid, Vector3 position, Vector3 rotation, WmoRootRender model)
        {
            ReferenceCount = 1;
            Uuid = uuid;
            BoundingBox = model.BoundingBox;
            mInstanceMatrix = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(rotation.Y),
                MathUtil.DegreesToRadians(rotation.X), MathUtil.DegreesToRadians(rotation.Z)) * Matrix.Translation(position);

            mRenderer = new WeakReference<WmoRootRender>(model);

            BoundingBox = BoundingBox.Transform(ref mInstanceMatrix);
            GroupBoxes = new BoundingBox[model.Groups.Count];
            for(var i = 0; i < GroupBoxes.Length; ++i)
            {
                var group = model.Groups[i];
                GroupBoxes[i] = group.BoundingBox.Transform(ref mInstanceMatrix);
            }
            Matrix.Invert(ref mInstanceMatrix, out mInverseInstanceMatrix);

            mInstanceMatrix = Matrix.Transpose(mInstanceMatrix);
            ModelRoot = model.Data;
        }

        public bool Intersects(IntersectionParams parameters, ref Ray globalRay, out float distance)
        {
            distance = float.MaxValue;
            if (globalRay.Intersects(ref BoundingBox) == false)
                return false;

            WmoRootRender renderer;
            if (mRenderer.TryGetTarget(out renderer) == false)
                return false;

            var instRay = Picking.Build(ref parameters.ScreenPosition, ref parameters.InverseView,
                ref parameters.InverseProjection, ref mInverseInstanceMatrix);

            var hasHit = false;
            for (var i = 0; i < GroupBoxes.Length; ++i)
            {
                if (globalRay.Intersects(ref GroupBoxes[i]) == false)
                    continue;

                float dist;
                if (renderer.Groups[i].Intersects(parameters, ref instRay, out dist) && dist < distance)
                {
                    distance = dist;
                    hasHit = true;
                }
            }

            return hasHit;
        }

        ~WmoInstance()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            DestroyModelNameplate();

            ModelRoot = null;
            mRenderer = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void CreateModelNameplate()
        {
            if (mWorldModelName != null)
                return;

            mWorldModelName = new WorldText
            {
                Text = System.IO.Path.GetFileName(ModelRoot.FileName),
                Scaling = 1.0f,
                DrawMode = WorldText.TextDrawMode.TextDraw3D
            };

            UpdateModelNameplate();
            WorldFrame.Instance.WorldTextManager.AddText(mWorldModelName);
        }

        public void DestroyModelNameplate()
        {
            if (mWorldModelName == null)
                return;

            WorldFrame.Instance.WorldTextManager.RemoveText(mWorldModelName);
            mWorldModelName.Dispose();
            mWorldModelName = null;
        }

        private void UpdateModelNameplate()
        {
            if (mWorldModelName == null)
                return;

            var diff = BoundingBox.Maximum - BoundingBox.Minimum;
            mWorldModelName.Scaling = diff.Length() / 60.0f;
            if (mWorldModelName.Scaling < 0.3f)
                mWorldModelName.Scaling = 0.3f;

            var position = BoundingBox.Minimum + (diff * 0.5f);
            position.Z = 1.5f + BoundingBox.Minimum.Z + (diff.Z * 1.08f);
            mWorldModelName.Position = position;
        }
    }
}
