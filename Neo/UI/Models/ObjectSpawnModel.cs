using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.UI.Models
{
    class ObjectSpawnModel
    {
        public bool DeselectModelOnClick { get; set; }

        public ObjectSpawnModel()
        {
            EditorWindowController.Instance.SpawnModel = this;
        }
    }
}
