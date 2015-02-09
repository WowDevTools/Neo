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
        private readonly Vector3 mScale;
        private BoundingBox mOrigBoundingBox;
        private WeakReference<M2BatchRenderer> mRenderer;

        public BoundingBox BoundingBox;
        public BoundingSphere BoundingSphere;
        public bool IsUpdated { get; set; }

        public int Uuid { get; private set; }
        public Matrix InstanceMatrix { get { return mInstanceMatrix; } }
        public Matrix InverseMatrix;
        public int NumReferences { get; set; }

        public M2RenderInstance(int uuid, Vector3 position, Vector3 rotation, Vector3 scale, M2BatchRenderer renderer)
        {
            mRenderer = new WeakReference<M2BatchRenderer>(renderer);
            mScale = scale;
            mPosition = position;
            mRotation = rotation;
            NumReferences = 1;
            BoundingSphere = new BoundingSphere(renderer.BoundingSphere.Center + position,
                renderer.BoundingSphere.Radius * scale.X);
            Uuid = uuid;
            BoundingBox = renderer.BoundingBox;
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

            M2BatchRenderer renderer;
            if (mRenderer.TryGetTarget(out renderer) == false)
                return;

            renderer.ForceUpdate();
        }
    }
}
