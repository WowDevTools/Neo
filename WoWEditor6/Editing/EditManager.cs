using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Editing
{
    class EditManager
    {
        public static EditManager Instance { get; } = new EditManager();

        private DateTime mLastChange = DateTime.Now;

        public EditMode CurrentMode { get; private set; }

        public void UpdateChanges()
        {
            var diff = DateTime.Now - mLastChange;
            if (diff.TotalMilliseconds < 20)
                return;

            mLastChange = DateTime.Now;
            if ((CurrentMode & EditMode.Sculpting) != 0)
                TerrainChangeManager.Instance.OnChange(diff);
        }

        public void EnableSculpting()
        {
            CurrentMode |= EditMode.Sculpting;
            CurrentMode &= ~EditMode.Texturing;
        }

        public void DisableSculpting()
        {
            CurrentMode &= ~EditMode.Sculpting;
        }

        public void EnableTexturing()
        {
            CurrentMode |= EditMode.Texturing;
            CurrentMode &= ~EditMode.Sculpting;
        }

        public void DisableTexturing()
        {
            CurrentMode &= ~EditMode.Texturing;
        }
    }
}
