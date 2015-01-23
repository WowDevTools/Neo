using SharpDX;
using WoWEditor6.Utils;

namespace WoWEditor6.Scene.Models.WMO
{
    class WmoInstance
    {
        private Vector3 mPosition;
        private Vector3 mRotation;
        private readonly Matrix mInstanceMatrix;

        public BoundingBox BoundingBox;

        public int Uuid { get; }
        public BoundingBox[] GroupBoxes { get; }
        public Matrix InstanceMatrix => mInstanceMatrix;

        public WmoInstance(int uuid, Vector3 position, Vector3 rotation, WmoRootRender model)
        {
            Uuid = uuid;
            mPosition = position;
            mRotation = rotation;
            BoundingBox = model.BoundingBox;
            mInstanceMatrix = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(rotation.Y),
                                  MathUtil.DegreesToRadians(rotation.X), MathUtil.DegreesToRadians(rotation.Z)) * Matrix.Translation(position);

            BoundingBox = BoundingBox.Transform(ref mInstanceMatrix);
            GroupBoxes = new BoundingBox[model.Groups.Count];
            for(var i = 0; i < GroupBoxes.Length; ++i)
            {
                var group = model.Groups[i];
                GroupBoxes[i] = group.BoundingBox.Transform(ref mInstanceMatrix);
            }

            mInstanceMatrix = Matrix.Transpose(mInstanceMatrix);
        }


    }
}