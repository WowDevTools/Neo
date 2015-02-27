using System.Collections.Generic;
using SharpDX.Direct3D11;

// ReSharper disable InconsistentNaming

namespace WoWEditor6.Scene.Models.M2
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

    class M2ShadersClass
    {
        readonly List<PixelShader> mPixelShaders = new List<PixelShader>();
        readonly List<VertexShader> mVertexShaders_Instanced = new List<VertexShader>();
        readonly List<VertexShader> mVertexShaders_Single = new List<VertexShader>();

        struct M2ShaderEffect
        {
            public readonly M2PixelShaderType PixelShader;
            public readonly M2VertexShaderType VertexShader;
            public readonly M2HullShaderType HullShader;
            public readonly M2DomainShaderType DomainShader;
            public readonly uint ColorOperation;
            public readonly uint AlphaOperation;

            public M2ShaderEffect(M2PixelShaderType ps, M2VertexShaderType vs,
                M2HullShaderType hs, M2DomainShaderType ds, uint colorOp, uint alphaOp)
            {
                PixelShader = ps;
                VertexShader = vs;
                HullShader = hs;
                DomainShader = ds;
                ColorOperation = colorOp;
                AlphaOperation = alphaOp;
            }
        };

        static readonly M2ShaderEffect[] M2ShaderEffects = {
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha,            M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha,                 M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha_Alpha,           M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_Add,        M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha,                    M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha,                 M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha,                    M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Alpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_3s,         M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha_Wgt,             M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Add_Alpha,                   M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_ModNA_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Wgt,                M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Wgt,                M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha_Wgt,             M2VertexShaderType.VS_Diffuse_T1_T2,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod_Add_Wgt,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,  M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Dual_Crossfade,              M2VertexShaderType.VS_Diffuse_T1_T1_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Depth,                       M2VertexShaderType.VS_Diffuse_EdgeFade_T1,      M2HullShaderType.HS_T1,             M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env_T2,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Mod,                         M2VertexShaderType.VS_Diffuse_EdgeFade_T1_T2,   M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Masked_Dual_Crossfade,       M2VertexShaderType.VS_Diffuse_T1_T1_T1_T2,      M2HullShaderType.HS_T1_T2_T3_T4,    M2DomainShaderType.DS_T1_T2_T3_T4,  0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Alpha,                    M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,  M2VertexShaderType.VS_Diffuse_T1_Env_T2,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Depth,                       M2VertexShaderType.VS_Diffuse_EdgeFade_Env,     M2HullShaderType.HS_T1,             M2DomainShaderType.DS_T1,           0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Guild,                                     M2VertexShaderType.VS_Diffuse_T1_T2_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Guild_NoBorder,                            M2VertexShaderType.VS_Diffuse_T1_T2,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2_T3,     0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Guild_Opaque,                              M2VertexShaderType.VS_Diffuse_T1_T2_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Illum,                                     M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
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
                return ((shaderId & 0x80) != 0) ? M2VertexShaderType.VS_Diffuse_T1_T2
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

        public static M2PixelShaderType GetPixelShaderType(short shaderId, ushort opCount)
        {
            if ((shaderId & 0x8000) == 1)
            {
                var shaderID = (ushort)(shaderId & (~0x8000));
                return M2ShaderEffects[shaderID].PixelShader;
            }

            if (opCount == 1)
            {
                return ((shaderId & 0x70) != 0) ? M2PixelShaderType.PS_Combiners_Mod
                                                : M2PixelShaderType.PS_Combiners_Opaque;
            }

            var lower = (uint)(shaderId & 7);
            if ((shaderId & 0x70) != 0)
            {
                switch (lower)
                {
                    case 0:  return M2PixelShaderType.PS_Combiners_Mod_Opaque;
                    case 3:  return M2PixelShaderType.PS_Combiners_Mod_Add;
                    case 4:  return M2PixelShaderType.PS_Combiners_Mod_Mod2x;
                    case 6:  return M2PixelShaderType.PS_Combiners_Mod_Mod2xNA;
                    case 7:  return M2PixelShaderType.PS_Combiners_Mod_AddNA;
                    default: return M2PixelShaderType.PS_Combiners_Mod_Mod;
                }
            }

            switch (lower)
            {
                case 0:  return M2PixelShaderType.PS_Combiners_Opaque_Opaque;
                case 3:  return M2PixelShaderType.PS_Combiners_Opaque_AddAlpha;
                case 4:  return M2PixelShaderType.PS_Combiners_Opaque_Mod2x;
                case 6:  return M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA;
                case 7:  return M2PixelShaderType.PS_Combiners_Opaque_AddAlpha;
                default: return M2PixelShaderType.PS_Combiners_Opaque_Mod;
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

        public static M2PixelShaderType GetPixelShaderTypeOld(short shaderId, ushort opCount)
        {
            switch (shaderId)
            {
                case 0:  return M2PixelShaderType.PS_Combiners_Opaque;
                case 1:  return M2PixelShaderType.PS_Combiners_Mod;
                case 2:  return M2PixelShaderType.PS_Combiners_Opaque_Mod;
                default: return M2PixelShaderType.PS_Combiners_Opaque;
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
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2x));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Opaque));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Mod));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Mod2x));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Add));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Mod2xNA));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_AddNA));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Opaque));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_AddAlpha));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_AddAlpha_Alpha));
            mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha_Add));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_AddAlpha));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_AddAlpha_Alpha));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Alpha_Alpha));
            mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha_3s));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_AddAlpha_Wgt));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Add_Alpha));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_ModNA_Alpha));
            mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_AddAlpha_Wgt));
            mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod_Add_Wgt));
            mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Dual_Crossfade));
            mPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha_Alpha));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Masked_Dual_Crossfade));
            mPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Alpha));
            mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Guild));
            mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Guild_NoBorder));
            mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Guild_Opaque));
            mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Depth));
            mPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Illum));
        }

        private void InitializeVertexShaders_Instanced(DeviceContext ctx)
        {
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_Env));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T2));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_Env));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_Env_T1));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_Env_Env));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_Env_T1));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T1));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T1_T1));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_EdgeFade_T1));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T2));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_Env_T2));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_EdgeFade_T1_T2));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T1_T1_T2));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_EdgeFade_Env));
            mVertexShaders_Instanced.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T2_T1));
        }

        private void InitializeVertexShaders_Single(DeviceContext ctx)
        {
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_Env));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T2));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_Env));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_Env_T1));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_Env_Env));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_Env_T1));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T1));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T1_T1));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_EdgeFade_T1));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T2));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_Env_T2));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_EdgeFade_T1_T2));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T1_T1_T2));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_EdgeFade_Env));
            mVertexShaders_Single.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexSingle_VS_Diffuse_T1_T2_T1));
        }

        public VertexShader GetVertexShader_Instanced(M2VertexShaderType VertexShaderType)
        {
            var vertexShaderType = (int)VertexShaderType;
            if (vertexShaderType < mVertexShaders_Instanced.Count)
            {
                var vs = mVertexShaders_Instanced[vertexShaderType];
                if (vs != null) return vs;
            }

            return mVertexShaders_Instanced[(int)M2VertexShaderType.VS_Diffuse_T1];
        }

        public VertexShader GetVertexShader_Single(M2VertexShaderType VertexShaderType)
        {
            var vertexShaderType = (int)VertexShaderType;
            if (vertexShaderType < mVertexShaders_Single.Count)
            {
                var vs = mVertexShaders_Single[vertexShaderType];
                if (vs != null) return vs;
            }

            return mVertexShaders_Single[(int)M2VertexShaderType.VS_Diffuse_T1];
        }

        public PixelShader GetPixelShader(M2PixelShaderType PixelShaderType)
        {
            var pixelShaderType = (int)PixelShaderType;
            if (pixelShaderType < mPixelShaders.Count)
            {
                var ps = mPixelShaders[pixelShaderType];
                if (ps != null) return ps;
            }
            
            return mPixelShaders[(int)M2PixelShaderType.PS_Combiners_Opaque];
        }
    }
}
