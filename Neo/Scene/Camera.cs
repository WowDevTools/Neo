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

        public Matrix4 View { get { return mView; } }
        public Matrix4 Projection { get { return mProj; } }
        public Matrix4 ViewProjection { get { return mViewProj; } }

        public Matrix4 ViewInverse { get { return mViewInverse; } }
        public Matrix4 ProjectionInverse { get { return mProjInverse; } }

        public Vector3 Position { get; private set; }

        public Vector3 Up { get { return mUp; } }
        public Vector3 Right { get { return mRight; } }
        public Vector3 Forward { get { return mForward; } }

        protected Camera()
        {
            mUp = Vector3.UnitZ;
            Position = new Vector3();
            mTarget = Vector3.UnitX;
            mRight = -Vector3.UnitY;
            mForward = Vector3.UnitX;

            UpdateView();
        }

        public bool Contains(ref BoundingBox box)
        {
            return mFrustum.Contains(ref box) != ContainmentType.Disjoint;
        }

        public bool Contains(ref BoundingSphere sphere)
        {
            return mFrustum.Contains(ref sphere) != ContainmentType.Disjoint;
        }

        public virtual void Update()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            mForward = mTarget - Position;
            mForward.Normalize();

	        mView = Matrix4.LookAt(Position, mTarget, mUp);
            Matrix4.Invert(ref mView, out mViewInverse);

            mFrustum.Update(mView, mProj);
            mViewProj = mView * mProj;

	        if (ViewChanged != null)
	        {
		        ViewChanged(this, mView);
	        }
        }

        protected void OnProjectionChanged(ref Matrix4 matProj)
        {
            mProj = matProj;
            Matrix4.Invert(ref mProj, out mProjInverse);

            mFrustum.Update(mView, mProj);
            mViewProj = mView * mProj;

	        if (ProjectionChanged != null)
	        {
		        ProjectionChanged(this, mProj);
	        }
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
            UpdateView();
        }

        public void SetTarget(Vector3 target)
        {
            mTarget = target;
            UpdateView();
        }

        public void SetParameters(Vector3 eye, Vector3 target, Vector3 up, Vector3 right)
        {
            mTarget = target;
            Position = eye;
            mUp = up;
            mRight = right;

            UpdateView();
        }

        public void Move(Vector3 amount)
        {
            Position += amount;
            mTarget += amount;
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
            Move(mForward * amount);
        }

        public void MoveRight(float amount)
        {
            Move(mRight * amount * (LeftHanded ? -1 : 1));
        }

        public void MoveLeft(float amount)
        {
            Move(mRight * -amount * (LeftHanded ? -1 : 1));
        }

        public void Pitch(float angle)
        {
            var matRot = Matrix4.CreateFromAxisAngle(mRight, MathHelper.DegreesToRadians(angle));
            mUp = Vector3.TransformVector(mUp, matRot);
            mUp.Normalize();

	        if (mUp.Z < 0)
	        {
		        mUp.Z = 0;
	        }

            mForward = Vector3.Cross(mUp, mRight);
            mTarget = Position + mForward;

            UpdateView();
        }

        public void Yaw(float angle)
        {
            var matRot = Matrix4.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(angle));
            mForward = Vector3.TransformVector(mForward, matRot);
            mForward.Normalize();

            mTarget = Position + mForward;
            mUp = Vector3.TransformVector(mUp, matRot);
            mUp.Normalize();

            mRight = Vector3.TransformVector(mRight, matRot);
            mRight.Normalize();

            UpdateView();
        }

        public void Roll(float angle)
        {
            var matRot = Matrix4.CreateFromAxisAngle(mForward, MathHelper.DegreesToRadians(angle));
            mUp = Vector3.TransformVector(mUp, matRot);
            mUp.Normalize();

            mRight = Vector3.TransformVector(mRight, matRot);
            mRight.Normalize();

            UpdateView();
        }
    }
}
