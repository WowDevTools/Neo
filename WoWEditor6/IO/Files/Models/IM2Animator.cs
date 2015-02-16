using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    interface IM2Animator
    {
        void Update(Matrix invRot, Matrix view);

        void SetAnimation(uint animation);
        void SetAnimationByIndex(uint animation);
        void ResetAnimationTimes();
        Vector4 GetColorValue(int texAnim);
        float GetAlphaValue(int alphaAnim);
        bool GetUvAnimMatrix(int uvIndex, out Matrix matrix);
        Matrix GetBoneMatrix(int bone, ref Matrix invRot, ref Matrix view);
        Matrix GetBoneMatrix(uint time, short bone, ref Matrix invRot, ref Matrix view);
        bool GetBones(Matrix[] bones);
    }
}
