using System;
using Neo.IO.Files.Models;
using Neo.Utils;
using OpenTK;
using SlimTK;

namespace Neo.Scene.Models.WMO
{
	public class WmoInstance : IModelInstance
    {
        private Matrix4 mInstanceMatrix;
        private Matrix4 mInverseInstanceMatrix;

        private WeakReference<WmoRootRender> mRenderer;

        public BoundingBox BoundingBox;

        private WorldText mWorldModelName;

        public BoundingBox InstanceBoundingBox { get { return this.BoundingBox; } }

        public int Uuid { get; private set; }
        public BoundingBox[] GroupBoxes { get; private set; }
        public Matrix4 InstanceMatrix { get { return this.mInstanceMatrix; } }
        public Vector3[] InstanceCorners { get; private set; }

        public bool IsSpecial { get { return false; } }

        public WmoRoot ModelRoot { get; private set; }

        public int ReferenceCount;

        private Vector3 mPosition;
        private Vector3 mRotation;

        private WmoRootRender mModel;


        public WmoInstance(int uuid, Vector3 position, Vector3 rotation, WmoRootRender model)
        {
	        this.ReferenceCount = 1;
	        this.Uuid = uuid;
	        this.BoundingBox = model.BoundingBox;

	        this.mPosition = position;
	        this.mRotation = rotation;
	        this.mModel = model;

	        this.mInstanceMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(this.mRotation.X)) *
	                          Matrix4.CreateRotationY(MathHelper.DegreesToRadians(this.mRotation.Y)) *
	                          Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.mRotation.Z));

	        this.mRenderer = new WeakReference<WmoRootRender>(model);

	        this.InstanceCorners = model.BoundingBox.GetCorners();
	        // TODO: Find correct function to use here
            Vector3.TransformVector(this.InstanceCorners, ref this.mInstanceMatrix, this.InstanceCorners);

	        this.BoundingBox = this.BoundingBox.Transform(ref this.mInstanceMatrix);
	        this.GroupBoxes = new BoundingBox[model.Groups.Count];
            for(var i = 0; i < this.GroupBoxes.Length; ++i)
            {
                var group = model.Groups[i];
	            this.GroupBoxes[i] = group.BoundingBox.Transform(ref this.mInstanceMatrix);
            }
            Matrix4.Invert(ref this.mInstanceMatrix, out this.mInverseInstanceMatrix);

	        this.mInstanceMatrix = Matrix4.Transpose(this.mInstanceMatrix);
	        this.ModelRoot = model.Data;
        }

        public bool Intersects(IntersectionParams parameters, ref Ray globalRay, out float distance)
        {
            distance = float.MaxValue;
            if (globalRay.Intersects(ref this.BoundingBox) == false)
            {
	            return false;
            }

	        WmoRootRender renderer;
            if (this.mRenderer.TryGetTarget(out renderer) == false)
            {
	            return false;
            }

	        var instRay = Picking.Build(ref parameters.ScreenPosition, ref parameters.InverseView,
                ref parameters.InverseProjection, ref this.mInverseInstanceMatrix);

            var hasHit = false;
            for (var i = 0; i < this.GroupBoxes.Length; ++i)
            {
                if (globalRay.Intersects(ref this.GroupBoxes[i]) == false)
                {
	                continue;
                }

	            float dist;
                if (renderer.Groups[i].Intersects(parameters, ref instRay, out dist) && dist < distance)
                {
                    distance = dist;
                    hasHit = true;
                }
            }

            return hasHit;
        }

        ~WmoInstance()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            DestroyModelNameplate();

	        this.ModelRoot = null;
	        this.mRenderer = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void CreateModelNameplate()
        {
            if (this.mWorldModelName != null)
            {
	            return;
            }

	        this.mWorldModelName = new WorldText
            {
                Text = System.IO.Path.GetFileName(this.ModelRoot.FileName),
                Scaling = 1.0f,
                DrawMode = WorldText.TextDrawMode.TextDraw3D
            };

            UpdateModelNameplate();
            WorldFrame.Instance.WorldTextManager.AddText(this.mWorldModelName);
        }

        public void Rotate(float x, float y, float z)
        {
	        this.mRotation.X += x;
	        this.mRotation.Y += y;
	        this.mRotation.Z += z;

	        this.mInstanceMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(this.mRotation.X)) *
	                          Matrix4.CreateRotationY(MathHelper.DegreesToRadians(this.mRotation.Y)) *
	                          Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.mRotation.Z));

	        this.mInstanceMatrix *= Matrix4.CreateTranslation(this.mPosition);

            //mRenderer = new WeakReference<WmoRootRender>(mModel);


            Matrix4.Invert(ref this.mInstanceMatrix, out this.mInverseInstanceMatrix);

	        this.BoundingBox = this.mModel.BoundingBox.Transform(ref this.mInstanceMatrix);
	        this.GroupBoxes = new BoundingBox[this.mModel.Groups.Count];
            for (var i = 0; i < this.GroupBoxes.Length; ++i)
            {
                var group = this.mModel.Groups[i];
	            this.GroupBoxes[i] = group.BoundingBox.Transform(ref this.mInstanceMatrix);
            }

	        this.InstanceCorners = this.mModel.BoundingBox.GetCorners();
	        // TODO: Find correct function to use here
	        Vector3.TransformVector(this.InstanceCorners, ref this.mInstanceMatrix, this.InstanceCorners);
	        this.mInstanceMatrix = Matrix4.Transpose(this.mInstanceMatrix);
	        this.ModelRoot = this.mModel.Data;
            UpdateModelNameplate();
        }

        public void UpdateScale(float s)
        {
            // Unsupported by WMO.
        }

        public void SetPosition(Vector3 position)
        {
	        this.mPosition += position;

	        this.mInstanceMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(this.mRotation.X)) *
	                          Matrix4.CreateRotationY(MathHelper.DegreesToRadians(this.mRotation.Y)) *
	                          Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(this.mRotation.Z));
            //mRenderer = new WeakReference<WmoRootRender>(mModel);

	        this.mInstanceMatrix *= Matrix4.CreateTranslation(this.mPosition);

            Matrix4.Invert(ref this.mInstanceMatrix, out this.mInverseInstanceMatrix);

	        this.BoundingBox = this.mModel.BoundingBox.Transform(ref this.mInstanceMatrix); //here is the problem, after this line the bBox is fucked up

	        this.GroupBoxes = new BoundingBox[this.mModel.Groups.Count];
            for (var i = 0; i < this.GroupBoxes.Length; ++i)
            {
                var group = this.mModel.Groups[i];
	            this.GroupBoxes[i] = group.BoundingBox.Transform(ref this.mInstanceMatrix);
            }

	        this.InstanceCorners = this.mModel.BoundingBox.GetCorners();
	        // TODO: Find correct function to use here
	        Vector3.TransformVector(this.InstanceCorners, ref this.mInstanceMatrix, this.InstanceCorners);
	        this.mInstanceMatrix = Matrix4.Transpose(this.mInstanceMatrix);
	        this.ModelRoot = this.mModel.Data;
            UpdateModelNameplate();
        }

	    [Obsolete]
        public Vector3 GetPosition()
        {
            return this.mPosition;
        }

	    [Obsolete]
        public Vector3 GetRotation()
        {
            return this.mRotation;
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

	        Vector3 diff = this.BoundingBox.Maximum - this.BoundingBox.Minimum;
	        this.mWorldModelName.Scaling = diff.Length / 60.0f;
            if (this.mWorldModelName.Scaling < 0.3f)
            {
	            this.mWorldModelName.Scaling = 0.3f;
            }

	        var position = this.BoundingBox.Minimum + (diff * 0.5f);
            position.Z = 1.5f + this.BoundingBox.Minimum.Z + (diff.Z * 1.08f);
	        this.mWorldModelName.Position = position;
        }

        public Vector3 GetNamePlatePosition()
        {
            if (this.mWorldModelName == null)
            {
	            return new Vector3(0.0f,0.0f,0.0f);
            }

	        return this.mWorldModelName.Position;
        }

        public void Remove()
        {
            WorldFrame.Instance.WmoManager.RemoveInstance(this.ModelRoot.FileName, this.Uuid,true);
            WorldFrame.Instance.ClearSelection();
        }

        public string GetModelName()
        {
            return this.ModelRoot.FileName;
        }
    }
}
