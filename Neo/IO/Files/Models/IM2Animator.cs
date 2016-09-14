using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    class BillboardParameters
    {
        public Vector3 Forward;
        public Vector3 Right;
        public Vector3 Up;
        public Matrix InverseRotation;
    }

    interface IM2Animator
    {
        void Update(BillboardParameters billboard);

        bool SetAnimation(uint animation);
        bool SetAnimation(Storage.AnimationType animation);
        void SetAnimationByIndex(uint animation);
        void ResetAnimationTimes();
        Vector4 GetColorValue(int texAnim);
        float GetAlphaValue(int alphaAnim);
        bool GetUvAnimMatrix(int uvIndex, out Matrix matrix);
        Matrix GetBoneMatrix(int bone, BillboardParameters billboard);
        Matrix GetBoneMatrix(uint time, short bone, BillboardParameters billboard);
        bool GetBones(Matrix[] bones);
    }
}
