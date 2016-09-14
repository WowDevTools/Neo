using SharpDX;

namespace WoWEditor6.Scene
{
    class PerspectiveCamera : Camera
    {
        private float mAspect = 1.0f;
        private float mFov = 55.0f;

        public float NearClip { get; private set; }

        public float FarClip { get; private set; }

        public PerspectiveCamera()
        {
            NearClip = 0.2f;
            FarClip = 2000.0f;
            UpdateProjection();
        }

        public override void Update()
        {
            base.Update();
            UpdateProjection();
        }

        private void UpdateProjection()
        {
            var matProjection = (LeftHanded == false)
                ? Matrix.PerspectiveFovRH(MathUtil.DegreesToRadians(mFov), mAspect, NearClip, FarClip)
                : Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(mFov), mAspect, NearClip, FarClip);

            OnProjectionChanged(ref matProjection);
        }

        public void SetFarClip(float clip)
        {
            FarClip = clip;
            UpdateProjection();
        }

        public void SetNearClip(float clip)
        {
            NearClip = clip;
            UpdateProjection();
        }

        public void SetClip(float near, float far)
        {
            NearClip = near;
            FarClip = far;
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
