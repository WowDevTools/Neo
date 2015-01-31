using SharpDX;
using WoWEditor6.Utils;

namespace WoWEditor6.Scene.Models.M2
{
    class M2RenderInstance
    {
        private readonly Matrix mInstanceMatrix;

        public BoundingBox BoundingBox;
        public BoundingSphere BoundingSphere;
        public bool IsUpdated { get; set; }

        public int Uuid { get; }
        public Matrix InstanceMatrix => mInstanceMatrix;
        public Matrix InverseMatrix;
        public int NumReferences { get; set; } = 1;

        public M2RenderInstance(int uuid, Vector3 position, Vector3 rotation, Vector3 scale, M2BatchRenderer renderer)
        {
            BoundingSphere = new BoundingSphere(renderer.BoundingSphere.Center + position,
                renderer.BoundingSphere.Radius * scale.X);
            Uuid = uuid;
            BoundingBox = renderer.BoundingBox;
            mInstanceMatrix = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(rotation.Y),
                MathUtil.DegreesToRadians(rotation.X), MathUtil.DegreesToRadians(rotation.Z)) * Matrix.Scaling(scale) * Matrix.Translation(position);
            BoundingBox = BoundingBox.Transform(ref mInstanceMatrix);
            Matrix.Invert(ref mInstanceMatrix, out InverseMatrix);
        }
    }
}
