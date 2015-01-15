using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.Graphics
{
    class VertexBuffer : Buffer
    {
        public VertexBuffer(GxContext context) :
            base(context, SharpDX.Direct3D11.BindFlags.VertexBuffer)
        {

        }
    }
}
