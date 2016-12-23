using System;
using System.Collections.Generic;
using System.Drawing;
using Neo.IO.Files.Terrain;
using Neo.Scene;
using Neo.Scene.Terrain;
using Neo.Utils;
using OpenTK;
using OpenTK.Input;

namespace Neo.Editing
{
    public enum ChunkEditMode
    {
        AreaPaint,
        AreaSelect,
        Hole,
        Flags
    }

    class ChunkEditManager
    {
        public static ChunkEditManager Instance { get; private set; }

        public event Action<MapChunk, bool> ForceRenderUpdate;
        public event Action<ChunkRenderFlags> OnChunkRenderModeChange;
        public event Action<int> SelectedAreaIdChange;
        public event Action<int> HoveredAreaChange;

        public ChunkRenderFlags ChunkRenderMode;
        public ChunkEditMode ChunkEditMode { get; set; }
        public Dictionary<int, Vector4> AreaColours;

        public bool SmallHole { get; set; } = true;
        public bool AddHole { get; set; } = true;

        public int SelectedAreaId { get; private set; }

        private MapChunk mHoveredChunk;
        private Color[] mBlockedColours = new[] //Colours prevented from being used in area painting
        {
            Color.White,
            Color.Black
        };

        static ChunkEditManager()
        {
            Instance = new ChunkEditManager();
        }

        private ChunkEditManager()
        {
            WorldFrame.Instance.OnWorldClicked += OnChunkClicked;
        }

        public void Initialize()
        {
            AreaColours = new Dictionary<int, Vector4>();
            ChunkEditMode = ChunkEditMode.AreaPaint;
        }

        public void OnFrame()
        {
            var chunk = WorldFrame.Instance.LastMouseIntersection.ChunkHit;
            if (chunk != null && chunk != mHoveredChunk)
            {
	            if (chunk.AreaId != mHoveredChunk?.AreaId)
	            {
		            HoveredAreaChange?.Invoke(chunk.AreaId);
	            }
                mHoveredChunk = chunk;

	            if (WorldFrame.Instance.RenderWindowContainsMouse())
	            {
		            OnChunkClicked(WorldFrame.Instance.LastMouseIntersection);
	            }
            }
            else if (chunk != null && SmallHole && ChunkEditMode == ChunkEditMode.Hole) //Small hole mode allow holding mouse down
            {
	            if (WorldFrame.Instance.RenderWindowContainsMouse())
	            {
		            OnChunkClicked(WorldFrame.Instance.LastMouseIntersection);
	            }
            }
        }

        public void SetChunkRenderMode(ChunkRenderFlags flags)
        {
            ChunkRenderMode = flags;
            OnChunkRenderModeChange?.Invoke(flags);
        }

        /// <summary>
        /// Returns area colour, creates random colour if not existent
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector4 GetAreaColour(int id, bool impass)
        {
            if (!AreaColours.ContainsKey(id))
            {
                Color colour = new Random().NextColor();

	            while (Array.IndexOf(mBlockedColours, colour) >= 0) //Blocked colour check
	            {
		            colour = new Random().NextColor();
	            }

                AreaColours.Add(id, new Vector4(colour.R / 255f, colour.G / 255f, colour.B / 255f, 0f));
            }

            if (impass)
            {
	            return new Vector4(1, 1, 1, 0);
            }

	        return AreaColours[id];
        }

        public void SetSelectedAreaId(int id)
        {
            SelectedAreaId = id;
        }

        public void OnChange(TimeSpan diff)
        {

        }

        private void OnChunkClicked(IntersectionParams intersection)
        {
            var chunk = mHoveredChunk;
	        MapArea parent;
            switch (ChunkEditMode)
            {
                case ChunkEditMode.AreaPaint:
				{
					if (SelectedAreaId == 0 || !InputHelper.IsButtonDown(MouseButton.Left))
					{
						return;
					}

                    if (chunk.Parent.TryGetTarget(out parent))
                    {
                        chunk.AreaId = SelectedAreaId;
                        parent.SetChanged();
                        ForceRenderUpdate?.Invoke(chunk, false);
                    }
                    break;
	            }
	            case ChunkEditMode.AreaSelect:
				{
					if (!InputHelper.IsButtonDown(MouseButton.Left))
					{
						return;
					}

                    SelectedAreaId = chunk.AreaId;
                    SelectedAreaIdChange?.Invoke(SelectedAreaId);
                    break;
	            }
                case ChunkEditMode.Hole:
	            {
		            if (!InputHelper.IsButtonDown(MouseButton.Left))
		            {
			            return;
		            }

		            if (chunk.Parent.TryGetTarget(out parent))
		            {
			            if (SmallHole)
			            {
				            chunk.SetHole(intersection, this.AddHole);
			            }
			            else
			            {
				            chunk.SetHoleBig(this.AddHole);
			            }

			            parent.SetChanged();
			            ForceRenderUpdate?.Invoke(chunk, true);
		            }

		            break;
	            }
                case ChunkEditMode.Flags:
	            {
		            if (!InputHelper.IsButtonDown(MouseButton.Left))
		            {
			            return;
		            }

		            if (chunk.Parent.TryGetTarget(out parent))
		            {
			            if (chunk.HasImpassFlag)
			            {
				            chunk.Flags &= ~0x2u;
			            }
			            else
			            {
				            chunk.Flags |= 0x2;
			            }

			            parent.SetChanged();
			            ForceRenderUpdate?.Invoke(chunk, false);
		            }
		            break;
	            }
            }
        }
    }
}
