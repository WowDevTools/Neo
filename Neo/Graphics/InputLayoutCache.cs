using System.Collections.Generic;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

namespace WoWEditor6.Graphics
{
    static class InputLayoutCache
    {
        private static readonly Dictionary<Mesh, InputLayout> Layouts =
            new Dictionary<Mesh,InputLayout>();

        public static InputLayout GetLayout(GxContext context, InputElement[] elements, Mesh mesh, ShaderProgram program)
        {
            InputLayout layout = new InputLayout(context.Device, program.VertexShaderCode.Data, elements);
            return layout;
        }
    }
}
