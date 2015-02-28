using System;
using SharpDX;
using WoWEditor6.Utils;
using WoWEditor6.IO.Files.Models;

namespace WoWEditor6.Scene.Models.M2
{
    class M2RenderInstance
    {
        private Matrix mInstanceMatrix;
        private Matrix mInverseMatrix;
        private Matrix mInverseRotation;

        private Vector3 mPosition;
        private Vector3 mRotation;
        private Color4 mHighlightColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        private readonly Vector3 mScale;

        private bool mIsHighlighted;
        private bool mHighlightFinished;
        private TimeSpan mHighlightStartTime;

        private M2File mModel;
        private M2Renderer mRenderer;
        private BoundingBox mBoundingBox;

        public M2File Model { get { return mModel; } }

        public M2Renderer Renderer { get { return mRenderer; } }

        public BoundingBox BoundingBox { get { return mBoundingBox; } }

        public bool IsUpdated { get; set; }

        public int Uuid { get; private set; }

        public Matrix InstanceMatrix { get { return mInstanceMatrix; } }
        public Matrix InverseMatrix { get { return mInverseMatrix; } }
        public Matrix InverseRotation { get { return mInverseRotation; } }

        public Color4 HighlightColor { get { return mHighlightColor; } }

        public float Depth { get; private set; }

        public int NumReferences { get; set; }

        public M2RenderInstance(int uuid, Vector3 position, Vector3 rotation, Vector3 scale, M2Renderer renderer)
        {
            mScale = scale;
            mPosition = position;
            mRotation = rotation;
            NumReferences = 1;
            Uuid = uuid;

            mRenderer = renderer;
            mModel = mRenderer.Model;
            mBoundingBox = mModel.BoundingBox;

            var rotationMatrix = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(rotation.Y),
                MathUtil.DegreesToRadians(rotation.X), MathUtil.DegreesToRadians(rotation.Z));

            Matrix.Invert(ref rotationMatrix, out mInverseRotation);
            mInstanceMatrix = rotationMatrix * Matrix.Scaling(scale) * Matrix.Translation(position);
            mBoundingBox = BoundingBox.Transform(ref mInstanceMatrix);
            Matrix.Invert(ref mInstanceMatrix, out mInverseMatrix);
        }

        public void UpdatePosition(Vector3 position)
        {
            mPosition = position;

            var rotationMatrix = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(mRotation.Y),
                MathUtil.DegreesToRadians(mRotation.X), MathUtil.DegreesToRadians(mRotation.Z));
            Matrix.Invert(ref rotationMatrix, out mInverseRotation);

            mInstanceMatrix = rotationMatrix * Matrix.Scaling(mScale) * Matrix.Translation(mPosition);
            Matrix.Invert(ref mInstanceMatrix, out mInverseMatrix);
            mBoundingBox = mModel.BoundingBox.Transform(ref mInstanceMatrix);
        }

        private void UpdateHighlightColor(Color4 highlightColor)
        {
            mHighlightColor = highlightColor;
        }

        public bool IsVisible(Camera camera)
        {
            return camera.Contains(ref mBoundingBox);
        }

        public void UpdateBrushHighlighting(Vector3 brushPosition, float radius)
        {
            var targetVec = mPosition - brushPosition;
            var distance = targetVec.LengthSquared();
            var radiusSquared = radius * radius;

            var time = TimeManager.Instance.GetTime();
            var timeDelta = time - mHighlightStartTime;
            var timeMs = timeDelta.TotalMilliseconds;

            var src = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
            var dst = new Color4(1.5f, 1.5f, 1.5f, 1.0f);

            var fac = (float)(timeMs / 500.0);
            if (fac > 1.0f)
                fac = 1.0f;

            if (distance < radiusSquared)
            {
                if (!mIsHighlighted)
                {
                    mHighlightStartTime = time;
                    mIsHighlighted = true;
                    mHighlightFinished = false;
                    return;
                }
            }
            else
            {
                if (mIsHighlighted)
                {
                    mHighlightStartTime = time;
                    mIsHighlighted = false;
                    mHighlightFinished = false;
                    return;
                }

                fac = 1.0f - fac;
            }

            if (!mHighlightFinished)
                UpdateHighlightColor(dst * fac + src * (1.0f - fac));

            mHighlightFinished = (fac >= 1.0f);
        }

        public void UpdateDepth()
        {
            var camera = WorldFrame.Instance.ActiveCamera;
            Depth = (camera.Position - mPosition).LengthSquared();
        }
    }
}
