using System;
using System.Collections.Generic;
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

        public event Action<MapChunk> ForceRenderUpdate;
        public event Action<ChunkRenderFlags> OnChunkRenderModeChange;
        public event Action<int> SelectedAreaIdChange;
        public event Action<int> HoveredAreaChange;

        public ChunkRenderFlags ChunkRenderMode;
        public ChunkEditMode ChunkEditMode { get; set; }
        public Dictionary<int, Vector4> AreaColours;

        public int SelectedAreaId { get; private set; }

        private MapChunk mHoveredChunk;


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

                OnChunkClicked(WorldFrame.Instance.LastMouseIntersection);
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
        public Vector4 GetAreaColour(int id)
        {
            if (!AreaColours.ContainsKey(id))
            {
                var colour = new Random().NextColor();
                AreaColours.Add(id, new Vector4(colour.R / 255f, colour.G / 255f, colour.B / 255f, 0f));
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
            switch (ChunkEditMode)
            {
                case ChunkEditMode.AreaPaint:
				{
					if (SelectedAreaId == 0 || !InputHelper.IsButtonDown(MouseButton.Left))
					{
						return;
					}

                    MapArea parent;
                    if (chunk.Parent.TryGetTarget(out parent))
                    {
                        chunk.AreaId = SelectedAreaId;
                        parent.SetChanged();
                        ForceRenderUpdate?.Invoke(chunk);
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
		            break;
	            }
                case ChunkEditMode.Flags:
	            {
		            break;
	            }
            }
        }
    }
}
