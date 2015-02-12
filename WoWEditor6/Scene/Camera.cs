using System;
using SharpDX;
using WoWEditor6.Graphics;

namespace WoWEditor6.Scene
{
    class Camera
    {
        private Matrix mMatView;
        // ReSharper disable once InconsistentNaming
        protected Matrix mMatProjection;
        private Matrix mViewNoTranspose;
        private Matrix mProjNoTranspose;
        private Matrix mViewInverted;
        private Matrix mProjInverted;

        private Vector3 mTarget;
        private Vector3 mUp;
        private Vector3 mRight;
        private Vector3 mForward;
        private readonly ViewFrustum mFrustum = new ViewFrustum();

        public event Action<Camera, Matrix> ViewChanged , ProjectionChanged;

        public bool LeftHanded { get; set; }

        public Matrix View { get { return mMatView; } }
        public Matrix Projection { get { return mMatProjection; } }
        public Matrix ViewInverse { get { return mViewInverted; } }
        public Matrix ProjectionInverse { get { return mProjInverted; } }

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
            mViewNoTranspose = LeftHanded == false ? Matrix.LookAtRH(Position, mTarget, mUp) : Matrix.LookAtLH(Position, mTarget, mUp);
            Matrix.Invert(ref mViewNoTranspose, out mViewInverted);
            Matrix.Transpose(ref mViewNoTranspose, out mMatView);
            if (ViewChanged != null)
                ViewChanged(this, mMatView);

            mFrustum.Update(mViewNoTranspose, mProjNoTranspose);
        }

        protected void OnProjectionChanged()
        {
            mProjNoTranspose = mMatProjection;
            Matrix.Invert(ref mMatProjection, out mProjInverted);
            Matrix.Transpose(ref mMatProjection, out mMatProjection);
            if (ProjectionChanged != null)
                ProjectionChanged(this, mMatProjection);

            mFrustum.Update(mViewNoTranspose, mProjNoTranspose);
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
