using SharpDX;

namespace WoWEditor6.Scene
{
    class PerspectiveCamera : Camera
    {
        private float mAspect = 1.0f;
        private float mFov = 45.0f;
        private float mNear = 0.2f;
        private float mFar = 5000.0f;

        public PerspectiveCamera()
        {
            UpdateProjection();
        }

        private void UpdateProjection()
        {
            mMatProjection = Matrix.PerspectiveFovRH(MathUtil.DegreesToRadians(mFov), mAspect, mNear, mFar);
            OnProjectionChanged();
        }

        public void SetFarClip(float clip)
        {
            mFar = clip;
            UpdateProjection();
        }

        public void SetNearClip(float clip)
        {
            mNear = clip;
            UpdateProjection();
        }

        public void SetClip(float near, float far)
        {
            mNear = near;
            mFar = far;
            UpdateProjection();
        }

        public void SetAspect(float aspect)
        {
            mAspect = aspect;
            UpdateProjection();
        }

        public void SetFieldOfView(float fov)
        {
            mFov = fov;
            UpdateProjection();
        }
    }
}
