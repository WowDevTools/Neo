using SlimTK;

namespace Neo.IO.Files.Models
{
    public class M2SubMeshInfo
    {
        public int StartIndex { get; set; }
        public int NumIndices { get; set; }
        public BoundingSphere BoundingSphere;
    }
}
