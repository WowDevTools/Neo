using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace Neo.Scene.Models.M2
{
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

    public enum M2FragmentShaderType
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

    public class M2ShadersClass
    {
	    private readonly List<PixelShader> mPixelShaders = new List<PixelShader>();
	    private readonly List<VertexShader> mVertexShaders_Instanced = new List<VertexShader>();
	    private readonly List<VertexShader> mVertexShaders_Single = new List<VertexShader>();

	    private struct M2ShaderEffect
        {
            public readonly M2FragmentShaderType PixelShader;
            public readonly M2VertexShaderType VertexShader;
            public readonly M2HullShaderType HullShader;
            public readonly M2DomainShaderType DomainShader;
            public readonly uint ColorOperation;
            public readonly uint AlphaOperation;

            public M2ShaderEffect(M2FragmentShaderType ps, M2VertexShaderType vs,
                M2HullShaderType hs, M2DomainShaderType ds, uint colorOp, uint alphaOp)
            {
	            this.PixelShader = ps;
	            this.VertexShader = vs;
	            this.HullShader = hs;
	            this.DomainShader = ds;
	            this.ColorOperation = colorOp;
	            this.AlphaOperation = alphaOp;
            }
        };

	    private static readonly M2ShaderEffect[] M2ShaderEffects = {
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha,            M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_AddAlpha,                 M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_AddAlpha_Alpha,           M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_Add,        M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_AddAlpha,                    M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_AddAlpha,                 M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_AddAlpha,                    M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_AddAlpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_Alpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_3s,         M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_AddAlpha_Wgt,             M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_Add_Alpha,                   M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_ModNA_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_AddAlpha_Wgt,                M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_AddAlpha_Wgt,                M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_AddAlpha_Wgt,             M2VertexShaderType.VS_Diffuse_T1_T2,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_Mod_Add_Wgt,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,  M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_Dual_Crossfade,              M2VertexShaderType.VS_Diffuse_T1_T1_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_Depth,                       M2VertexShaderType.VS_Diffuse_EdgeFade_T1,      M2HullShaderType.HS_T1,             M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_AddAlpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env_T2,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_Mod,                         M2VertexShaderType.VS_Diffuse_EdgeFade_T1_T2,   M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_Masked_Dual_Crossfade,       M2VertexShaderType.VS_Diffuse_T1_T1_T1_T2,      M2HullShaderType.HS_T1_T2_T3_T4,    M2DomainShaderType.DS_T1_T2_T3_T4,  0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_Alpha,                    M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,  M2VertexShaderType.VS_Diffuse_T1_Env_T2,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2FragmentShaderType.PS_Combiners_Mod_Depth,                       M2VertexShaderType.VS_Diffuse_EdgeFade_Env,     M2HullShaderType.HS_T1,             M2DomainShaderType.DS_T1,           0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Guild,                                     M2VertexShaderType.VS_Diffuse_T1_T2_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Guild_NoBorder,                            M2VertexShaderType.VS_Diffuse_T1_T2,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2_T3,     0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Guild_Opaque,                              M2VertexShaderType.VS_Diffuse_T1_T2_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2FragmentShaderType.PS_Illum,                                     M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
        };

        public static M2VertexShaderType GetVertexShaderType(short shaderId, ushort opCount)
        {
            if ((shaderId & 0x8000) != 0)
            {
                var shaderID = (ushort)(shaderId & (~0x8000));
                return M2ShaderEffects[shaderID].VertexShader;
            }

            if (opCount == 1)
            {
                return ((shaderId & 0x80) != 0) ? M2VertexShaderType.VS_Diffuse_Env
                     : ((shaderId & 0x4000) != 0) ? M2VertexShaderType.VS_Diffuse_T2
                                                  : M2VertexShaderType.VS_Diffuse_T1;
            }

            if ((shaderId & 0x80) != 0)
            {
                return ((shaderId & 0x8) != 0) ? M2VertexShaderType.VS_Diffuse_Env_Env
                                               : M2VertexShaderType.VS_Diffuse_Env_T1;
            }

            return ((shaderId & 0x8) != 0) ? M2VertexShaderType.VS_Diffuse_T1_Env
                 : ((shaderId & 0x4000) != 0) ? M2VertexShaderType.VS_Diffuse_T1_T2
                                              : M2VertexShaderType.VS_Diffuse_T1_T1;
        }

        public static M2FragmentShaderType GetPixelShaderType(short shaderId, ushort opCount)
        {
            if ((shaderId & 0x8000) == 1)
            {
                var shaderID = (ushort)(shaderId & (~0x8000));
                return M2ShaderEffects[shaderID].PixelShader;
            }

            if (opCount == 1)
            {
                return ((shaderId & 0x70) != 0) ? M2FragmentShaderType.PS_Combiners_Mod
                                                : M2FragmentShaderType.PS_Combiners_Opaque;
            }

            var lower = (uint)(shaderId & 7);
            if ((shaderId & 0x70) != 0)
            {
                switch (lower)
                {
                    case 0:  return M2FragmentShaderType.PS_Combiners_Mod_Opaque;
                    case 3:  return M2FragmentShaderType.PS_Combiners_Mod_Add;
                    case 4:  return M2FragmentShaderType.PS_Combiners_Mod_Mod2x;
                    case 6:  return M2FragmentShaderType.PS_Combiners_Mod_Mod2xNA;
                    case 7:  return M2FragmentShaderType.PS_Combiners_Mod_AddNA;
                    default: return M2FragmentShaderType.PS_Combiners_Mod_Mod;
                }
            }

            switch (lower)
            {
                case 0:  return M2FragmentShaderType.PS_Combiners_Opaque_Opaque;
                case 3:  return M2FragmentShaderType.PS_Combiners_Opaque_AddAlpha;
                case 4:  return M2FragmentShaderType.PS_Combiners_Opaque_Mod2x;
                case 6:  return M2FragmentShaderType.PS_Combiners_Opaque_Mod2xNA;
                case 7:  return M2FragmentShaderType.PS_Combiners_Opaque_AddAlpha;
                default: return M2FragmentShaderType.PS_Combiners_Opaque_Mod;
            }
        }

        public static M2VertexShaderType GetVertexShaderTypeOld(short shaderId, ushort opCount)
        {
            switch (shaderId)
            {
                case 0:  return M2VertexShaderType.VS_Diffuse_T1;
                case 1:  return M2VertexShaderType.VS_Diffuse_T1;
                case 2:  return M2VertexShaderType.VS_Diffuse_T1_T2;
                default: return M2VertexShaderType.VS_Diffuse_T1;
            }
        }

        public static M2FragmentShaderType GetPixelShaderTypeOld(short shaderId, ushort opCount)
        {
            switch (shaderId)
            {
                case 0:  return M2FragmentShaderType.PS_Combiners_Opaque;
                case 1:  return M2FragmentShaderType.PS_Combiners_Mod;
                case 2:  return M2FragmentShaderType.PS_Combiners_Opaque_Mod;
                default: return M2FragmentShaderType.PS_Combiners_Opaque;
            }
        }

        public void Initialize(DeviceContext ctx)
        {
            InitializePixelShaders(ctx);
            InitializeVertexShaders_Instanced(ctx);
            InitializeVertexShaders_Single(ctx);
        }

        private void InitializePixelShaders(DeviceContext ctx)
        {
            // enum order
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Mod));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Mod2x));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Mod2xNA));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Opaque));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_Mod));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_Mod2x));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_Add));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_Mod2xNA));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_AddNA));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_Opaque));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Mod2xNA_Alpha));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_AddAlpha));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_AddAlpha_Alpha));
	        this.mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Mod2xNA_Alpha_Add));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_AddAlpha));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_AddAlpha_Alpha));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Alpha_Alpha));
	        this.mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Mod2xNA_Alpha_3s));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_AddAlpha_Wgt));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_Add_Alpha));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_ModNA_Alpha));
	        this.mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_AddAlpha_Wgt));
	        this.mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Mod_Add_Wgt));
	        this.mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_Dual_Crossfade));
	        this.mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Mod2xNA_Alpha_Alpha));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_Masked_Dual_Crossfade));
	        this.mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Opaque_Alpha));
	        this.mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Guild));
	        this.mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Guild_NoBorder));
	        this.mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Guild_Opaque));
	        this.mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Combiners_Mod_Depth));
	        this.mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Fragment_PS_Illum));
        }

        private void InitializeVertexShaders_Instanced(DeviceContext ctx)
        {
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_Env));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T2));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_Env));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_Env_T1));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_Env_Env));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_Env_T1));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T1));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T1_T1));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_EdgeFade_T1));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T2));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_Env_T2));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_EdgeFade_T1_T2));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T1_T1_T2));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_EdgeFade_Env));
	        this.mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T2_T1));
        }

        private void InitializeVertexShaders_Single(DeviceContext ctx)
        {
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_Env));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T2));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_Env));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_Env_T1));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_Env_Env));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_Env_T1));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T1));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T1_T1));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_EdgeFade_T1));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T2));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_Env_T2));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_EdgeFade_T1_T2));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T1_T1_T2));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_EdgeFade_Env));
	        this.mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T2_T1));
        }

        public VertexShader GetVertexShader_Instanced(M2VertexShaderType VertexShaderType)
        {
            var vertexShaderType = (int)VertexShaderType;
            if (vertexShaderType < this.mVertexShaders_Instanced.Count)
            {
                var vs = this.mVertexShaders_Instanced[vertexShaderType];
                if (vs != null)
                {
	                return vs;
                }
            }

            return this.mVertexShaders_Instanced[(int)M2VertexShaderType.VS_Diffuse_T1];
        }

        public VertexShader GetVertexShader_Single(M2VertexShaderType VertexShaderType)
        {
            var vertexShaderType = (int)VertexShaderType;
            if (vertexShaderType < this.mVertexShaders_Single.Count)
            {
                var vs = this.mVertexShaders_Single[vertexShaderType];
                if (vs != null)
                {
	                return vs;
                }
            }

            return this.mVertexShaders_Single[(int)M2VertexShaderType.VS_Diffuse_T1];
        }

        public PixelShader GetPixelShader(M2FragmentShaderType PixelShaderType)
        {
            var pixelShaderType = (int)PixelShaderType;
            if (pixelShaderType < this.mPixelShaders.Count)
            {
                var ps = this.mPixelShaders[pixelShaderType];
                if (ps != null)
                {
	                return ps;
                }
            }

            return this.mPixelShaders[(int)M2FragmentShaderType.PS_Combiners_Opaque];
        }
    }
}
