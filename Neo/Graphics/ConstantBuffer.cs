
namespace WoWEditor6.Graphics
{
    class ConstantBuffer : Buffer
    {
        public ConstantBuffer(GxContext context)
            : base(context, SharpDX.Direct3D11.BindFlags.ConstantBuffer)
        {

        }
    }
}
