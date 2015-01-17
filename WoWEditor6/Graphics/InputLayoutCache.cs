using System.Collections.Generic;
using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    static class InputLayoutCache
    {
        private static readonly Dictionary<Mesh, Dictionary<ShaderProgram, InputLayout>> Layouts =
            new Dictionary<Mesh, Dictionary<ShaderProgram, InputLayout>>();

        public static InputLayout GetLayout(GxContext context, InputElement[] elements, Mesh mesh, ShaderProgram program)
        {
            Dictionary<ShaderProgram, InputLayout> meshEntry;
            InputLayout layout;

            if (Layouts.TryGetValue(mesh, out meshEntry))
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

            Layouts.Add(mesh, meshEntry);
            return layout;
        }
    }
}
