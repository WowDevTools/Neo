using OpenTK;

namespace Neo.Scene
{
	internal class PerspectiveCamera : Camera
    {
        private float mAspect = 1.0f;
        private float mFov = 55.0f;

        public float NearClip { get; private set; }

        public float FarClip { get; private set; }

        public PerspectiveCamera()
        {
	        this.NearClip = 0.2f;
	        this.FarClip = 2000.0f;
            UpdateProjection();
        }

        public override void Update()
        {
            base.Update();
            UpdateProjection();
        }

        private void UpdateProjection()
        {
	        var matProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(this.mFov), this.mAspect, this.NearClip, this.FarClip);
            OnProjectionChanged(ref matProjection);
        }

        public void SetFarClip(float clip)
        {
	        this.FarClip = clip;
            UpdateProjection();
        }

        public void SetNearClip(float clip)
        {
	        this.NearClip = clip;
            UpdateProjection();
        }

        public void SetClip(float near, float far)
        {
	        this.NearClip = near;
	        this.FarClip = far;
            UpdateProjection();
        }

        public void SetAspect(float aspect)
        {
	        this.mAspect = aspect;
            UpdateProjection();
        }

        public void SetFieldOfView(float fov)
        {
	        this.mFov = fov;
            UpdateProjection();
        }
    }
}
