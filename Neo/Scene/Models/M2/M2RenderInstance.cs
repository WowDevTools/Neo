using System;
using Neo.Utils;
using Neo.IO.Files.Models;
using OpenTK;
using OpenTK.Graphics;
using SlimTK;

namespace Neo.Scene.Models.M2
{
    public class M2RenderInstance : IModelInstance
    {
        private Matrix4 mInstanceMatrix;
        private Matrix4 mInverseMatrix;
        private Matrix4 mInverseRotation;

        private Vector3 mPosition;
        private Vector3 mRotation;
        private Color4 mHighlightColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
        private Vector3 mScale;

        private bool mIsHighlighted;
        private bool mHighlightFinished;
        private TimeSpan mHighlightStartTime;

        private M2File mModel;
        private M2Renderer mRenderer;
        private BoundingBox mBoundingBox;

        private WorldText mWorldModelName;

        public Vector3 Position { get { return this.mPosition; } }
        public float Scale { get { return this.mScale.X; } }
        public Vector3 Rotation { get { return this.mRotation; } }

        public M2File Model { get { return this.mModel; } }

        public M2Renderer Renderer { get { return this.mRenderer; } }

        public BoundingBox BoundingBox { get { return this.mBoundingBox; } }
        public BoundingBox InstanceBoundingBox { get { return this.BoundingBox; } }

        public Vector3[] InstanceCorners { get; private set; }

        public bool IsUpdated { get; set; }

        public bool IsSpecial { get { return this.Uuid == Editing.ModelSpawnManager.M2InstanceUuid; } }

        public int Uuid { get; private set; }

        public Matrix4 InstanceMatrix { get { return this.mInstanceMatrix; } }
        public Matrix4 InverseMatrix { get { return this.mInverseMatrix; } }
        public Matrix4 InverseRotation { get { return this.mInverseRotation; } }

        public Color4 HighlightColor { get { return this.mHighlightColor; } }

        public float Depth { get; private set; }

        public int NumReferences { get; set; }

        public M2RenderInstance(int uuid, Vector3 position, Vector3 rotation, Vector3 scale, M2Renderer renderer)
        {
	        this.mScale = scale;
	        this.mPosition = position;
	        this.mRotation = rotation;
	        this.NumReferences = 1;
	        this.Uuid = uuid;

	        this.mRenderer = renderer;
	        this.mModel = this.mRenderer.Model;
	        this.mBoundingBox = this.mModel.BoundingBox;

            var rotationMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(this.mRotation.X)) *
							     Matrix4.CreateRotationY(MathHelper.DegreesToRadians(this.mRotation.Y)) *
							     Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.mRotation.Z));

	        Matrix4.Invert(ref rotationMatrix, out this.mInverseRotation);
	        this.mInstanceMatrix = rotationMatrix * Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(position);
	        this.mBoundingBox = this.BoundingBox.Transform(ref this.mInstanceMatrix);
            Matrix4.Invert(ref this.mInstanceMatrix, out this.mInverseMatrix);

	        this.InstanceCorners = this.mModel.BoundingBox.GetCorners();

	        // TODO: Investigate correct function to use here
            Vector3.TransformVector(this.InstanceCorners, ref this.mInstanceMatrix, this.InstanceCorners);
        }

        ~M2RenderInstance()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            DestroyModelNameplate();

	        this.mModel = null;
	        this.mRenderer = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Intersects(IntersectionParams parameters, ref Ray globalRay, out float value)
        {
            value = float.MaxValue;

            if (globalRay.Intersects(ref this.mBoundingBox) == false)
            {
	            return false;
            }

	        var instRay = Picking.Build(ref parameters.ScreenPosition, ref parameters.InverseView,
                ref parameters.InverseProjection, ref this.mInverseMatrix);
            return this.mModel.Intersect(ref instRay, out value);
        }

        public void Rotate(float x, float y, float z)
        {
	        this.mRotation.X += x;
	        this.mRotation.Y += y;
	        this.mRotation.Z += z;

	        var rotationMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(this.mRotation.X)) *
	                             Matrix4.CreateRotationY(MathHelper.DegreesToRadians(this.mRotation.Y)) *
	                             Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.mRotation.Z));

	        Matrix4.Invert(ref rotationMatrix, out this.mInverseRotation);

	        this.mInstanceMatrix = rotationMatrix * Matrix4.CreateScale(this.mScale) * Matrix4.CreateTranslation(this.mPosition);
            Matrix4.Invert(ref this.mInstanceMatrix, out this.mInverseMatrix);

	        this.mBoundingBox = this.mModel.BoundingBox.Transform(ref this.mInstanceMatrix);

	        this.InstanceCorners = this.mModel.BoundingBox.GetCorners();

	        // TODO: Investigate correct function to use here
	        Vector3.TransformVector(this.InstanceCorners, ref this.mInstanceMatrix, this.InstanceCorners);
            UpdateModelNameplate();
        }

        public void SetPosition(Vector3 position)
        {
	        this.mPosition.X += position.X;
	        this.mPosition.Y += position.Y;
	        this.mPosition.Z += position.Z;

	        var rotationMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(this.mRotation.X)) *
	                             Matrix4.CreateRotationY(MathHelper.DegreesToRadians(this.mRotation.Y)) *
	                             Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.mRotation.Z));

	        Matrix4.Invert(ref rotationMatrix, out this.mInverseRotation);

	        this.mInstanceMatrix = rotationMatrix * Matrix4.CreateScale(this.mScale) * Matrix4.CreateTranslation(this.mPosition);
            Matrix4.Invert(ref this.mInstanceMatrix, out this.mInverseMatrix);

	        this.mBoundingBox = this.mModel.BoundingBox.Transform(ref this.mInstanceMatrix);

	        this.InstanceCorners = this.mModel.BoundingBox.GetCorners();

	        // TODO: Investigate correct function to use here
	        Vector3.TransformVector(this.InstanceCorners, ref this.mInstanceMatrix, this.InstanceCorners);
            UpdateModelNameplate();
        }

	    [Obsolete]
        public Vector3 GetRotation()
        {
            return this.mRotation;
        }

	    [Obsolete]
        public Vector3 GetPosition()
        {
            return this.mPosition;
        }

        public void UpdateScale(float scale)
        {
	        this.mScale.X += scale;
	        this.mScale.Y += scale;
	        this.mScale.Z += scale;

            if (this.mScale.X < 0.0f)
            {
	            this.mScale.X = 0.0f;
	            this.mScale.Y = 0.0f;
	            this.mScale.Z = 0.0f;
            }

            if (this.mScale.X > 63.9f)
            {
	            this.mScale.X = 63.9f;
	            this.mScale.Y = 63.9f;
	            this.mScale.Z = 63.9f;
            }

	        var rotationMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(this.mRotation.X)) *
	                             Matrix4.CreateRotationY(MathHelper.DegreesToRadians(this.mRotation.Y)) *
	                             Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.mRotation.Z));

	        Matrix4.Invert(ref rotationMatrix, out this.mInverseRotation);

	        this.mInstanceMatrix = rotationMatrix * Matrix4.CreateScale(this.mScale) * Matrix4.CreateTranslation(this.mPosition);
            Matrix4.Invert(ref this.mInstanceMatrix, out this.mInverseMatrix);

	        this.mBoundingBox = this.mModel.BoundingBox.Transform(ref this.mInstanceMatrix);

	        this.InstanceCorners = this.mModel.BoundingBox.GetCorners();

	        // TODO: Investigate correct function to use here
	        Vector3.TransformVector(this.InstanceCorners, ref this.mInstanceMatrix, this.InstanceCorners);
            UpdateModelNameplate();
        }

        public void CreateModelNameplate()
        {
            if (this.mWorldModelName != null)
            {
	            return;
            }

	        this.mWorldModelName = new WorldText
            {
                Text = this.mModel.ModelName,
                Scaling = 1.0f,
                DrawMode = WorldText.TextDrawMode.TextDraw3D
            };

            UpdateModelNameplate();
            WorldFrame.Instance.WorldTextManager.AddText(this.mWorldModelName);
        }

        public void DestroyModelNameplate()
        {
            if (this.mWorldModelName == null)
            {
	            return;
            }

	        WorldFrame.Instance.WorldTextManager.RemoveText(this.mWorldModelName);
	        this.mWorldModelName.Dispose();
	        this.mWorldModelName = null;
        }

        private void UpdateModelNameplate()
        {
            if (this.mWorldModelName == null)
            {
	            return;
            }

	        Vector3 diff = this.mBoundingBox.Minimum - this.mBoundingBox.Maximum;
	        this.mWorldModelName.Scaling = diff.Length / 60.0f;
            if (this.mWorldModelName.Scaling < 0.3f)
            {
	            this.mWorldModelName.Scaling = 0.3f;
            }

	        Vector3 position = this.mBoundingBox.Minimum + (diff * 0.5f);
            position.Z = 1.5f + this.mBoundingBox.Minimum.Z + (diff.Z * 1.08f);
	        this.mWorldModelName.Position = position;
        }

        private void UpdateHighlightColor(Color4 highlightColor)
        {
	        this.mHighlightColor = highlightColor;
        }

        public bool IsVisible(Camera camera)
        {
            return camera.Contains(ref this.mBoundingBox);
        }

        public void UpdateBrushHighlighting(Vector3 brushPosition, float radius)
        {
            var targetVec = this.mPosition - brushPosition;
            var distance = targetVec.LengthSquared;
            var radiusSquared = radius * radius;

            var time = TimeManager.Instance.GetTime();
            var timeDelta = time - this.mHighlightStartTime;
            var timeMs = timeDelta.TotalMilliseconds;

            var src = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
            var dst = new Color4(1.5f, 1.5f, 1.5f, 1.0f);

            var fac = (float)(timeMs / 500.0);
            if (fac > 1.0f)
            {
	            fac = 1.0f;
            }

	        if (distance < radiusSquared)
            {
                if (!this.mIsHighlighted)
                {
	                this.mHighlightStartTime = time;
	                this.mIsHighlighted = true;
	                this.mHighlightFinished = false;
                    return;
                }
            }
            else
            {
                if (this.mIsHighlighted)
                {
	                this.mHighlightStartTime = time;
	                this.mIsHighlighted = false;
	                this.mHighlightFinished = false;
                    return;
                }

                fac = 1.0f - fac;
            }

            if (!this.mHighlightFinished)
            {
	            UpdateHighlightColor(dst * fac + src * (1.0f - fac));
            }

	        this.mHighlightFinished = (fac >= 1.0f);
        }

        public void UpdateDepth()
        {
            var camera = WorldFrame.Instance.ActiveCamera;
	        this.Depth = (camera.Position - this.mPosition).LengthSquared;
        }

        public Vector3 GetNamePlatePosition()
        {
            if (this.mWorldModelName == null)
            {
	            return new Vector3(0.0f, 0.0f, 0.0f);
            }

	        return this.mWorldModelName.Position;
        }

        public void Remove()
        {
            WorldFrame.Instance.M2Manager.RemoveInstance(this.mModel.FileName, this.Uuid);
            WorldFrame.Instance.ClearSelection();
        }

        public string GetModelName()
        {
            return this.mModel.FileName;
        }
    }
}
