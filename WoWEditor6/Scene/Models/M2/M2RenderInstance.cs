using System;
using SharpDX;
using WoWEditor6.Utils;

namespace WoWEditor6.Scene.Models.M2
{
    class M2RenderInstance
    {
        private Matrix mInstanceMatrix;
        private Vector3 mPosition;
        private Vector3 mRotation;
        private Color4 mHighlightColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        private readonly Vector3 mScale;
        private BoundingBox mOrigBoundingBox;
        private bool mIsHighlighted;
        private bool mHighlightFinished;
        private TimeSpan mHighlightStartTime;

        public BoundingBox BoundingBox;

        public readonly M2Renderer Renderer;

        public bool IsUpdated { get; set; }

        public int Uuid { get; private set; }

        public Matrix InstanceMatrix { get { return mInstanceMatrix; } }
        public Matrix InverseMatrix;

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
            Renderer = renderer;
            BoundingBox = renderer.Model.BoundingBox;
            mOrigBoundingBox = BoundingBox;
            mInstanceMatrix = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(rotation.Y),
                MathUtil.DegreesToRadians(rotation.X), MathUtil.DegreesToRadians(rotation.Z)) * Matrix.Scaling(scale) * Matrix.Translation(position);
            BoundingBox = BoundingBox.Transform(ref mInstanceMatrix);
            Matrix.Invert(ref mInstanceMatrix, out InverseMatrix);
        }

        public void UpdatePosition(Vector3 position)
        {
            mPosition = position;
            mInstanceMatrix = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(mRotation.Y),
                MathUtil.DegreesToRadians(mRotation.X), MathUtil.DegreesToRadians(mRotation.Z)) * Matrix.Scaling(mScale) * Matrix.Translation(mPosition);

            Matrix.Invert(ref mInstanceMatrix, out InverseMatrix);
            BoundingBox = mOrigBoundingBox.Transform(ref mInstanceMatrix);
        }

        private void UpdateHighlightColor(Color4 highlightColor)
        {
            mHighlightColor = highlightColor;
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
