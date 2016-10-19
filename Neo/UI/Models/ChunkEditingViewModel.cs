using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.Editing;
using Neo.Scene.Terrain;
using Neo.UI.Widgets;

namespace Neo.UI.Models
{
    class ChunkEditingViewModel
    {
        private readonly ChunkEditingWidget mWidget;

        public ChunkEditingWidget Widget { get { return mWidget; } }

        public ChunkEditingViewModel(ChunkEditingWidget widget)
        {
            mWidget = widget;
        }

        public void SetChunkEditState(ChunkEditMode mode)
        {
            ChunkEditManager.Instance.ChunkEditMode = mode;
        }

        public void HandleChunkLinesChange(bool value)
        {
            ChunkRenderFlags flags = ChunkEditManager.Instance.ChunkRenderMode;
            if (value)
            {
                flags &= ~ChunkRenderFlags.HideLines;
                flags |= ChunkRenderFlags.ShowLines;
            }
            else
            {
                flags &= ~ChunkRenderFlags.ShowLines;
                flags |= ChunkRenderFlags.HideLines;
            }

            ChunkEditManager.Instance.SetChunkRenderMode(flags);
        }

        public void HandleAreaColourChange(bool value)
        {
            ChunkRenderFlags flags = ChunkEditManager.Instance.ChunkRenderMode;
            if (value)
            {
                flags &= ~ChunkRenderFlags.HideArea;
                flags |= ChunkRenderFlags.ShowArea;
            }
            else
            {
                flags &= ~ChunkRenderFlags.ShowArea;
                flags |= ChunkRenderFlags.HideArea;
            }

            ChunkEditManager.Instance.SetChunkRenderMode(flags);
        }

        public void HandleAreaSelectionChange(int areaid)
        {
            ChunkEditManager.Instance.SetSelectedAreaId(areaid);
        }

        public void HandleHoleParamsChange(bool smallhole, bool addhole)
        {
            ChunkEditManager.Instance.AddHole = addhole;
            ChunkEditManager.Instance.SmallHole = smallhole;
        }
    }
}
