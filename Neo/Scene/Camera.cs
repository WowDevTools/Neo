using System;
using Neo.Graphics;
using OpenTK;
using SlimTK;

namespace Neo.Scene
{
    public class Camera
    {
        private Matrix4 mView;
        private Matrix4 mProj;

        private Matrix4 mViewProj;
        private Matrix4 mViewInverse;
        private Matrix4 mProjInverse;

        private Vector3 mTarget;
        private Vector3 mUp;
        private Vector3 mRight;
        private Vector3 mForward;
        private readonly ViewFrustum mFrustum = new ViewFrustum();

        public event Action<Camera, Matrix4> ViewChanged , ProjectionChanged;

        public bool LeftHanded { get; set; }

        public Matrix4 View { get { return this.mView; } }
        public Matrix4 Projection { get { return this.mProj; } }
        public Matrix4 ViewProjection { get { return this.mViewProj; } }

        public Matrix4 ViewInverse { get { return this.mViewInverse; } }
        public Matrix4 ProjectionInverse { get { return this.mProjInverse; } }

        public Vector3 Position { get; private set; }

        public Vector3 Up { get { return this.mUp; } }
        public Vector3 Right { get { return this.mRight; } }
        public Vector3 Forward { get { return this.mForward; } }

        protected Camera()
        {
	        this.mUp = Vector3.UnitZ;
	        this.Position = new Vector3();
	        this.mTarget = Vector3.UnitX;
	        this.mRight = -Vector3.UnitY;
	        this.mForward = Vector3.UnitX;

            UpdateView();
        }

        public bool Contains(ref BoundingBox box)
        {
            return this.mFrustum.Contains(ref box) != ContainmentType.Disjoint;
        }

        public bool Contains(ref BoundingSphere sphere)
        {
            return this.mFrustum.Contains(ref sphere) != ContainmentType.Disjoint;
        }

        public virtual void Update()
        {
            UpdateView();
        }

        private void UpdateView()
        {
	        this.mForward = this.mTarget - this.Position;
	        this.mForward.Normalize();

	        this.mView = Matrix4.LookAt(this.Position, this.mTarget, this.mUp);
            Matrix4.Invert(ref this.mView, out this.mViewInverse);

	        this.mFrustum.Update(this.mView, this.mProj);
	        this.mViewProj = this.mView * this.mProj;

	        if (ViewChanged != null)
	        {
		        ViewChanged(this, this.mView);
	        }
        }

        protected void OnProjectionChanged(ref Matrix4 matProj)
        {
	        this.mProj = matProj;
            Matrix4.Invert(ref this.mProj, out this.mProjInverse);

	        this.mFrustum.Update(this.mView, this.mProj);
	        this.mViewProj = this.mView * this.mProj;

	        if (ProjectionChanged != null)
	        {
		        ProjectionChanged(this, this.mProj);
	        }
        }

        public void SetPosition(Vector3 position)
        {
	        this.Position = position;
            UpdateView();
        }

        public void SetTarget(Vector3 target)
        {
	        this.mTarget = target;
            UpdateView();
        }

        public void SetParameters(Vector3 eye, Vector3 target, Vector3 up, Vector3 right)
        {
	        this.mTarget = target;
	        this.Position = eye;
	        this.mUp = up;
	        this.mRight = right;

            UpdateView();
        }

        public void Move(Vector3 amount)
        {
	        this.Position += amount;
	        this.mTarget += amount;
            UpdateView();
        }

        public void MoveUp(float amount)
        {
            Move(Vector3.UnitZ * amount);
        }

        public void MoveDown(float amount)
        {
            Move(Vector3.UnitZ * -amount);
        }

        public void MoveForward(float amount)
        {
            Move(this.mForward * amount);
        }

        public void MoveRight(float amount)
        {
            Move(this.mRight * amount * (this.LeftHanded ? -1 : 1));
        }

        public void MoveLeft(float amount)
        {
            Move(this.mRight * -amount * (this.LeftHanded ? -1 : 1));
        }

        public void Pitch(float angle)
        {
            var matRot = Matrix4.CreateFromAxisAngle(this.mRight, MathHelper.DegreesToRadians(angle));
	        this.mUp = Vector3.TransformVector(this.mUp, matRot);
	        this.mUp.Normalize();

	        if (this.mUp.Z < 0)
	        {
		        this.mUp.Z = 0;
	        }

	        this.mForward = Vector3.Cross(this.mUp, this.mRight);
	        this.mTarget = this.Position + this.mForward;

            UpdateView();
        }

        public void Yaw(float angle)
        {
            var matRot = Matrix4.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(angle));
	        this.mForward = Vector3.TransformVector(this.mForward, matRot);
	        this.mForward.Normalize();

	        this.mTarget = this.Position + this.mForward;
	        this.mUp = Vector3.TransformVector(this.mUp, matRot);
	        this.mUp.Normalize();

	        this.mRight = Vector3.TransformVector(this.mRight, matRot);
	        this.mRight.Normalize();

            UpdateView();
        }

        public void Roll(float angle)
        {
            var matRot = Matrix4.CreateFromAxisAngle(this.mForward, MathHelper.DegreesToRadians(angle));
	        this.mUp = Vector3.TransformVector(this.mUp, matRot);
	        this.mUp.Normalize();

	        this.mRight = Vector3.TransformVector(this.mRight, matRot);
	        this.mRight.Normalize();

            UpdateView();
        }
    }
}
