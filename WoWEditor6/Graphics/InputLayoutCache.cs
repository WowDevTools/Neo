using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    static class InputLayoutCache
    {
        private static readonly Dictionary<Mesh, Dictionary<ShaderProgram, InputLayout>> gLayouts =
            new Dictionary<Mesh, Dictionary<ShaderProgram, InputLayout>>();

        public static InputLayout GetLayout(GxContext context, InputElement[] elements, Mesh mesh, ShaderProgram program)
        {
            Dictionary<ShaderProgram, InputLayout> meshEntry;
            InputLayout layout;

            if (gLayouts.TryGetValue(mesh, out meshEntry))
            {
                if (meshEntry.TryGetValue(program, out layout))
                    return layout;

                layout = new InputLayout(context.Device, program.VertexShaderCode.Data, elements);
                meshEntry.Add(program, layout);
                return layout;
            }

            layout = new InputLayout(context.Device, program.VertexShaderCode.Data, elements);
            meshEntry = new Dictionary<ShaderProgram, InputLayout>()
            {
                {program, layout}
            };

            gLayouts.Add(mesh, meshEntry);
            return layout;
        }
    }
}
