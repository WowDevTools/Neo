using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Matrix GetUvAnimMatrix(int uvIndex);
        Matrix GetBoneMatrix(int bone);
        Matrix GetBoneMatrix(uint time, short bone);
        bool GetBones(Matrix[] bones);
    }
}
