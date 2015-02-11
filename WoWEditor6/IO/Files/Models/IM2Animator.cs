using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    interface IM2Animator
    {
        void Update();

        void SetAnimation(uint animation);
        void SetAnimationByIndex(uint animation);
        void ResetAnimationTimes();
        Vector4 GetColorValue(int texAnim);
        float GetAlphaValue(int alphaAnim);
        bool GetUvAnimMatrix(int uvIndex, out Matrix matrix);
        Matrix GetBoneMatrix(int bone);
        Matrix GetBoneMatrix(uint time, short bone);
        bool GetBones(Matrix[] bones);
    }
}
