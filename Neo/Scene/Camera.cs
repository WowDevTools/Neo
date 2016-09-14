using System;
using SharpDX;
using Neo.Graphics;

namespace Neo.Scene
{
    class Camera
    {
        private Matrix mView;
        private Matrix mProj;

        private Matrix mViewProj;
        private Matrix mViewInverse;
        private Matrix mProjInverse;

        private Vector3 mTarget;
        private Vector3 mUp;
        private Vector3 mRight;
        private Vector3 mForward;
        private readonly ViewFrustum mFrustum = new ViewFrustum();

        public event Action<Camera, Matrix> ViewChanged , ProjectionChanged;

        public bool LeftHanded { get; set; }

        public Matrix View { get { return mView; } }
        public Matrix Projection { get { return mProj; } }
        public Matrix ViewProjection { get { return mViewProj; } }

        public Matrix ViewInverse { get { return mViewInverse; } }
        public Matrix ProjectionInverse { get { return mProjInverse; } }

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

            mView = (LeftHanded == false)
                ? Matrix.LookAtRH(Position, mTarget, mUp)
                : Matrix.LookAtLH(Position, mTarget, mUp);
            Matrix.Invert(ref mView, out mViewInverse);

            mFrustum.Update(mView, mProj);
            mViewProj = mView * mProj;

            if (ViewChanged != null)
                ViewChanged(this, mView);
        }

        protected void OnProjectionChanged(ref Matrix matProj)
        {
            mProj = matProj;
            Matrix.Invert(ref mProj, out mProjInverse);

            mFrustum.Update(mView, mProj);
            mViewProj = mView * mProj;

            if (ProjectionChanged != null)
                ProjectionChanged(this, mProj);
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
            var matRot = Matrix.RotationAxis(mRight, MathUtil.DegreesToRadians(angle));
            mUp = Vector3.TransformCoordinate(mUp, matRot);
            mUp.Normalize();

            if (mUp.Z < 0)
                mUp.Z = 0;

            mForward = Vector3.Cross(mUp, mRight);
            mTarget = Position + mForward;

            UpdateView();
        }

        public void Yaw(float angle)
        {
            var matRot = Matrix.RotationAxis(Vector3.UnitZ, MathUtil.DegreesToRadians(angle));
            mForward = Vector3.TransformCoordinate(mForward, matRot);
            mForward.Normalize();

            mTarget = Position + mForward;
            mUp = Vector3.TransformCoordinate(mUp, matRot);
            mUp.Normalize();

            mRight = Vector3.TransformCoordinate(mRight, matRot);
            mRight.Normalize();

            UpdateView();
        }

        public void Roll(float angle)
        {
            var matRot = Matrix.RotationAxis(mForward, MathUtil.DegreesToRadians(angle));
            mUp = Vector3.TransformCoordinate(mUp, matRot);
            mUp.Normalize();

            mRight = Vector3.TransformCoordinate(mRight, matRot);
            mRight.Normalize();

            UpdateView();
        }
    }
}
