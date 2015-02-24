using System.Collections.Generic;
using System.IO;
using SharpDX;

namespace WoWEditor6.IO.Files.Models
{
    class TextureInfo
    {
        public int TextureType { get; set; }
        public Graphics.Texture Texture { get; set; }
    }

    public enum M2HullShaderType
    {
        HS_T1,
        HS_T1_T2,
        HS_T1_T2_T3,
        HS_T1_T2_T3_T4,
    };

    public enum M2DomainShaderType
    {
        DS_T1,
        DS_T1_T2,
        DS_T1_T2_T3,
        DS_T1_T2_T3_T4,
    };

    public enum M2VertexShaderType
    {
        VS_Diffuse_T1,
        VS_Diffuse_Env,
        VS_Diffuse_T1_T2,
        VS_Diffuse_T1_Env,
        VS_Diffuse_Env_T1,
        VS_Diffuse_Env_Env,
        VS_Diffuse_T1_Env_T1,
        VS_Diffuse_T1_T1,
        VS_Diffuse_T1_T1_T1,
        VS_Diffuse_EdgeFade_T1,
        VS_Diffuse_T2,
        VS_Diffuse_T1_Env_T2,
        VS_Diffuse_EdgeFade_T1_T2,
        VS_Diffuse_T1_T1_T1_T2,
        VS_Diffuse_EdgeFade_Env,
        VS_Diffuse_T1_T2_T1,
    };

    public enum M2PixelShaderType
    {
        PS_Combiners_Opaque,
        PS_Combiners_Mod,
        PS_Combiners_Opaque_Mod,
        PS_Combiners_Opaque_Mod2x,
        PS_Combiners_Opaque_Mod2xNA,
        PS_Combiners_Opaque_Opaque,
        PS_Combiners_Mod_Mod,
        PS_Combiners_Mod_Mod2x,
        PS_Combiners_Mod_Add,
        PS_Combiners_Mod_Mod2xNA,
        PS_Combiners_Mod_AddNA,
        PS_Combiners_Mod_Opaque,
        PS_Combiners_Opaque_Mod2xNA_Alpha,
        PS_Combiners_Opaque_AddAlpha,
        PS_Combiners_Opaque_AddAlpha_Alpha,
        PS_Combiners_Opaque_Mod2xNA_Alpha_Add,
        PS_Combiners_Mod_AddAlpha,
        PS_Combiners_Mod_AddAlpha_Alpha,
        PS_Combiners_Opaque_Alpha_Alpha,
        PS_Combiners_Opaque_Mod2xNA_Alpha_3s,
        PS_Combiners_Opaque_AddAlpha_Wgt,
        PS_Combiners_Mod_Add_Alpha,
        PS_Combiners_Opaque_ModNA_Alpha,
        PS_Combiners_Mod_AddAlpha_Wgt,
        PS_Combiners_Opaque_Mod_Add_Wgt,
        PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,
        PS_Combiners_Mod_Dual_Crossfade,
        PS_Combiners_Opaque_Mod2xNA_Alpha_Alpha,
        PS_Combiners_Mod_Masked_Dual_Crossfade,
        PS_Combiners_Opaque_Alpha,
        PS_Guild,
        PS_Guild_NoBorder,
        PS_Guild_Opaque,
        PS_Combiners_Mod_Depth,
        PS_Illum,
    };

    abstract class M2File
    {
        public M2Vertex[] Vertices { get; protected set; }
        public List<M2RenderPass> Passes { get; private set; }
        public ushort[] Indices { get; protected set; }

        public BoundingBox BoundingBox { get; protected set; }
        public BoundingSphere BoundingSphere { get; protected set; }
        public bool HasBlendPass { get; protected set; }
        public bool HasOpaquePass { get; protected set; }

        public TextureInfo[] TextureInfos { get; protected set; }

        public string ModelRoot { get; private set; }

        public bool NeedsPerInstanceAnimation { get; protected set; }

        public float BoundingRadius { get; protected set; }

        public short[] AnimationLookup { get; protected set; }

        protected M2File(string path)
        {
            ModelRoot = Path.GetDirectoryName(path) ?? "";
            TextureInfos = new TextureInfo[0];
            Vertices = new M2Vertex[0];
            Passes = new List<M2RenderPass>();
            Indices = new ushort[0];
        }

        public abstract int GetNumberOfBones();

        public abstract bool Load();

        struct M2ShaderEffect
        {
	        public readonly M2PixelShaderType	pixel;
            public readonly M2VertexShaderType vertex;
            public readonly M2HullShaderType hull;
            public readonly M2DomainShaderType domain;
            public readonly uint ff_colorOp;
            public readonly uint ff_alphaOp;
	
	        public M2ShaderEffect(  M2PixelShaderType ps,
                                    M2VertexShaderType vs, 
                                    M2HullShaderType hs,
                                    M2DomainShaderType ds, 
                                    uint colorOp, 
                                    uint alphaOp
                                 )
                            
	        {
		        this.pixel = ps;
		        this.vertex = vs;
		        this.hull = hs;
		        this.domain = ds;
		        this.ff_colorOp = colorOp;
		        this.ff_alphaOp = alphaOp;
	        }
        };

        static readonly List<M2ShaderEffect> M2ShaderEffects = new List<M2ShaderEffect>
            ( new [] {
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha,            M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_AddAlpha,                 M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_AddAlpha_Alpha,           M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_Add,        M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_AddAlpha,                    M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_AddAlpha,                 M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_AddAlpha,                    M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_Alpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_3s,         M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_AddAlpha_Wgt,             M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_Add_Alpha,                   M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_ModNA_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Wgt,                M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Wgt,                M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_AddAlpha_Wgt,             M2VertexShaderType.VS_Diffuse_T1_T2,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_Mod_Add_Wgt,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,  M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_Dual_Crossfade,              M2VertexShaderType.VS_Diffuse_T1_T1_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_Depth,                       M2VertexShaderType.VS_Diffuse_EdgeFade_T1,      M2HullShaderType.HS_T1,             M2DomainShaderType.DS_T1_T2,        0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env_T2,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_Mod,                         M2VertexShaderType.VS_Diffuse_EdgeFade_T1_T2,   M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_Masked_Dual_Crossfade,       M2VertexShaderType.VS_Diffuse_T1_T1_T1_T2,      M2HullShaderType.HS_T1_T2_T3_T4,    M2DomainShaderType.DS_T1_T2_T3_T4,  0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_Alpha,                    M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,  M2VertexShaderType.VS_Diffuse_T1_Env_T2,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Combiners_Mod_Depth,                       M2VertexShaderType.VS_Diffuse_EdgeFade_Env,     M2HullShaderType.HS_T1,             M2DomainShaderType.DS_T1,           0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Guild,                                     M2VertexShaderType.VS_Diffuse_T1_T2_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2,        0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Guild_NoBorder,                            M2VertexShaderType.VS_Diffuse_T1_T2,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2_T3,     0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Guild_Opaque,                              M2VertexShaderType.VS_Diffuse_T1_T2_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2,        0, 0 ),
                    new M2ShaderEffect( M2PixelShaderType.PS_Illum,                                     M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0 ),
            });

        protected M2VertexShaderType GetVertexShaderType( short shader_id, ushort op_count )
        {
            if( ( shader_id & 0x8000 ) != 0 )
            {
                ushort shaderID = (ushort)( shader_id & (~0x8000) );
                return M2ShaderEffects[shaderID].vertex;
            }
            else
            {
                if( op_count == 1 )
                {
                    return      ( ( shader_id & 0x80 ) != 0 )   ? M2VertexShaderType.VS_Diffuse_T1_T2
                            :   ( ( shader_id & 0x4000 ) != 0 ) ? M2VertexShaderType.VS_Diffuse_T2
                                                                : M2VertexShaderType.VS_Diffuse_T1;
                }
                else
                {
                    if( ( shader_id & 0x80 ) != 0 )
                    {
                        return ( ( shader_id & 0x8 ) != 0 ) ? M2VertexShaderType.VS_Diffuse_Env_Env
                                                            : M2VertexShaderType.VS_Diffuse_Env_T1;
                    }
                    else
                    {
                        return      ( ( shader_id & 0x8 )    != 0 ) ? M2VertexShaderType.VS_Diffuse_T1_Env
                               :    ( ( shader_id & 0x4000 ) != 0 ) ? M2VertexShaderType.VS_Diffuse_T1_T2
                                                                    : M2VertexShaderType.VS_Diffuse_T1_T1;
                    }
                }
            }
        }

        protected M2PixelShaderType GetPixelShaderType( short shader_id, ushort op_count )
        {
            if( ( shader_id & 0x8000 ) == 1 )
            {
                ushort shaderID = (ushort)( shader_id & ( ~0x8000 ) );
                return M2ShaderEffects[shaderID].pixel;
            }
            else
            {
                if( op_count == 1 )
                {
                    return ( ( shader_id & 0x70 ) != 0 ) ? M2PixelShaderType.PS_Combiners_Mod : M2PixelShaderType.PS_Combiners_Opaque;
                }
                else
                {
                    uint lower = (uint)(shader_id & 7);
                    if( ( shader_id & 0x70 ) != 0 )
                    {
                        return      lower == 0 ? M2PixelShaderType.PS_Combiners_Mod_Opaque
                                :   lower == 3 ? M2PixelShaderType.PS_Combiners_Mod_Add
                                :   lower == 4 ? M2PixelShaderType.PS_Combiners_Mod_Mod2x
                                :   lower == 6 ? M2PixelShaderType.PS_Combiners_Mod_Mod2xNA
                                :   lower == 7 ? M2PixelShaderType.PS_Combiners_Mod_AddNA
                                :                M2PixelShaderType.PS_Combiners_Mod_Mod;
                    }
                    else
                    {
                        return      lower == 0 ? M2PixelShaderType.PS_Combiners_Opaque_Opaque
                                :   lower == 3 ? M2PixelShaderType.PS_Combiners_Opaque_AddAlpha
                                :   lower == 4 ? M2PixelShaderType.PS_Combiners_Opaque_Mod2x
                                :   lower == 6 ? M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA
                                :   lower == 7 ? M2PixelShaderType.PS_Combiners_Opaque_AddAlpha
                                :                M2PixelShaderType.PS_Combiners_Opaque_Mod;
                    }
                }
            }
        }
    }
}
