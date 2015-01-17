using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WoWEditor6.UI.Views
{
    interface IView : IComponent
    {
        void OnResize(Vector2 newSize);
    }
}
