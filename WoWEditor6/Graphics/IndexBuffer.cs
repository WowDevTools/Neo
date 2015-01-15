using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Graphics
{
    class IndexBuffer : Buffer
    {
        public IndexBuffer(GxContext context) :
            base(context, SharpDX.Direct3D11.BindFlags.IndexBuffer)
        {

        }
    }
}
