using System;
using SharpDX;

namespace WoWEditor6.Scene
{
    class Camera
    {
        private Matrix mMatView;
        // ReSharper disable once InconsistentNaming
        protected Matrix mMatProjection;
        private Matrix mViewInverted;
        private Matrix mProjInverted;

        private Vector3 mPosition;
        private Vector3 mTarget;
        private Vector3 mUp;
        private Vector3 mRight;
        private Vector3 mForward;

        public event Action<Camera, Matrix> ViewChanged , ProjectionChanged;

        protected Camera()
        {
            mUp = Vector3.UnitZ;
            mPosition = new Vector3();
            mTarget = Vector3.UnitX;
            mRight = Vector3.UnitY;
            mForward = Vector3.UnitX;

            UpdateView();
        }

        private void UpdateView()
        {
            mForward = mTarget - mPosition;
            mForward.Normalize();
            mMatView = Matrix.LookAtLH(mPosition, mTarget, mUp);
            Matrix.Invert(ref mMatView, out mViewInverted);
            ViewChanged?.Invoke(this, mMatView);
        }

        protected void OnProjectionChanged()
        {
            Matrix.Invert(ref mMatProjection, out mProjInverted);
            ProjectionChanged?.Invoke(this, mMatProjection);
        }

        public void SetPosition(Vector3 position)
        {
            mPosition = position;
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
            mPosition = eye;
            mUp = up;
            mRight = right;

            UpdateView();
        }

        public void Move(Vector3 amount)
        {
            mPosition += amount;
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
            Move(mRight * amount);
        }

        public void MoveLeft(float amount)
        {
            Move(mRight * -amount);
        }

        public void Pitch(float angle)
        {
            var matRot = Matrix.RotationAxis(mRight, MathUtil.DegreesToRadians(angle));
            mForward = Vector3.TransformCoordinate(mForward, matRot);
            mForward.Normalize();

            mTarget = mPosition + mForward;
            mUp = Vector3.TransformCoordinate(mUp, matRot);
            mUp.Normalize();

            UpdateView();
        }

        public void Yaw(float angle)
        {
            var matRot = Matrix.RotationAxis(Vector3.UnitZ, MathUtil.DegreesToRadians(angle));
            mForward = Vector3.TransformCoordinate(mForward, matRot);
            mForward.Normalize();

            mTarget = mPosition + mForward;
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
