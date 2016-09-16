using OpenTK;

namespace Neo.IO.Files.Models
{
	public class BillboardParameters
    {
        public Vector3 Forward;
        public Vector3 Right;
        public Vector3 Up;
        public Matrix4 InverseRotation;
    }

	public interface IM2Animator
    {
        void Update(BillboardParameters billboard);

        bool SetAnimation(uint animation);
        bool SetAnimation(Storage.AnimationType animation);
        void SetAnimationByIndex(uint animation);
        void ResetAnimationTimes();
        Vector4 GetColorValue(int texAnim);
        float GetAlphaValue(int alphaAnim);
        bool GetUvAnimMatrix(int uvIndex, out Matrix4 matrix);
        Matrix4 GetBoneMatrix(int bone, BillboardParameters billboard);
        Matrix4 GetBoneMatrix(uint time, short bone, BillboardParameters billboard);
        bool GetBones(Matrix4[] bones);
    }
}
