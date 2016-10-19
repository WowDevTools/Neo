#version 330 core

uniform uint OperationMode;

// Operation modes
const uint PS_Combiners_Opaque                      = 1;
const uint PS_Combiners_Mod                         = 2;
const uint PS_Combiners_Opaque_Alpha                = 3;
const uint PS_Combiners_Opaque_Mod                  = 4;
const uint PS_Combiners_Opaque_Mod2x                = 5;
const uint PS_Combiners_Opaque_Mod2xNA              = 6;
const uint PS_Combiners_Opaque_Opaque               = 7;
const uint PS_Combiners_Mod_Mod                     = 8;
const uint PS_Combiners_Mod_Mod2x                   = 9;
const uint PS_Combiners_Mod_Add                     = 10;
const uint PS_Combiners_Mod_Mod2xNA                 = 11;
const uint PS_Combiners_Mod_AddNA                   = 12;
const uint PS_Combiners_Mod_Opaque                  = 13;
const uint PS_Combiners_Opaque_AddAlpha             = 14;
const uint PS_Combiners_Opaque_Alpha_Alpha          = 15;
const uint PS_Combiners_Opaque_AddAlpha_Alpha       = 16;
const uint PS_Combiners_Mod_AddAlpha                = 17;
const uint PS_Combiners_Mod_Add_Alpha               = 18;
const uint PS_Combiners_Mod_AddAlpha_Alpha          = 19;
const uint PS_Combiners_Opaque_Mod2xNA_Alpha        = 20;
const uint PS_Combiners_Opaque_ModNA_Alpha          = 21;
const uint PS_Combiners_Opaque_AddAlpha_Wgt         = 22;
const uint PS_Combiners_Mod_Dual_Crossfade          = 23;
const uint PS_Combiners_Mod_Masked_Dual_Crossfade   = 24;

uniform Textures
{
	sampler2D texture1;
	sampler2D texture2;
	sampler2D texture3;
	sampler2D texture4;
};

uniform GlobalParameters
{
	mat4 ModelViewProjection;
	vec4 viewport;

	vec4 ambientLight;
	vec4 diffuseLight;

	vec4 fogColour;
	// x -> fogStart
	// y -> fogEnd
	// z -> farClip
	//vec4 fogParams;

	float fogStart;
	float fogEnd;
	float farClip;

	vec4 mousePosition;
	vec4 eyePosition;

	// x -> innerRadius
	// y -> outerRadius
	// z -> brushTime
	//vec4 brushParams;
	float innerBrushRadius;
	float outerBrushRadius;
	float brushTime;
};

uniform PerModelPass
{
	mat4 uvAnimation1;
	mat4 uvAnimation2;
	mat4 uvAnimation3;
	mat4 uvAnimation4;

	// x -> unlit
	// y -> unfogged
	// z -> alphaKey
	//vec3 modelPassParameters;
	bool modelIsUnlit;
	bool modelIsUnfogged;
	bool modelIsAlphaKeyed;

	vec4 animatedColour;
	vec4 transparency;
};

// Input data
in VertexData
{
	vec4 position;
	vec3 normal;
	vec2 UV1;
	vec2 UV2;
	vec2 UV3;
	vec2 UV4;
	float depth;
	vec3 worldPosition;
	vec4 inputColour;
	vec4 inputColourModifier;
};

// Output colour
out vec4 outputColour;

vec3 getDiffuseLight(vec3 normal)
{
	vec3 lightDirection = vec3(-1, 1, -1);
	float light = clamp(dot(normal, normalize(-lightDirection)));
	vec3 diffuse = diffuseLight.rgb * light;

	diffuse += ambientLight.rgb;
	diffuse += clamp(diffuse, 0.0f, 1.0f);

	return diffuse;
}

vec3 applyFog(vec3 finalColour)
{
	float fogDepth = depth - fogStart;
	fogDepth /= (fogEnd - fogStart);

	float fog = 0;
	if (!modelIsUnfogged)
	{
		fog =pow(clamp(fogDepth, 0.0f, 1.0f), 1.5f);
	}

	return fog * fogColour.rgb + (1.0f - fog) * finalColour.rgb;
}

vec4 commonFinalize(vec4 finalizeColour)
{
	if (!modelIsUnlit) // not unlit
	{
		finalizeColour.rgb *= getDiffuseLight(normal);
	}

	finalizeColour.rgb = applyFog(finalizeColour.rgb);
	return finalizeColour * inputColourModifier;
}

void main()
{
	vec4 combinedColour = vec4(0.0f, 0.0f, 0.0f, 0.0f);
	vec4 finalColour = vec4(0.0f, 0.0f, 0.0f, 0.0f);

	vec4 textureColour1 = texture(texture1, UV1);
	if (modelIsAlphaKeyed && textureColour1.a <= 0.5f)
	{
		discard;
	}

	switch (OperationMode)
	{
		case PS_Combiners_Opaque:
		{
			vec4 result0 = textureColour1;
			result0.rgb *= inputColour.rgb;
			result0.a *= 2.0f;

			combinedColour.rgb = result0.rgb;
			combinedColour.a = inputColour.a;
			break;
		}
		case PS_Combiners_Mod:
		{
			textureColour1.a *= transparency.x;

			vec4 result0 = textureColour1;
            result0.rgb *= inputColour.rgb;
            result0.a *= 2.0f;

            combinedColour.rgb = result0.rgb;
            combinedColour.a = inputColour.a;
            break;
		}
		case PS_Combiners_Opaque_Alpha:
		{
			vec4 textureColour2 = texture(texture2, UV2);

			textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result1.rgb = -result0.rgb + result1.rgb;
            result0.rgb = result1.a * result1.rgb + result0.rgb;
            result0.rgb *= inputColor.rgb;
            result0.rgb *= 2.0f;

            combinedColour.rgb = r0.rgb;
            break;
		}
		case PS_Combiners_Opaque_Mod:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= result1.rgb;
            result0.rgb *= inputColour.rgb;
            result0.rgb *= 2.0f;

            combinedColour.rgb = r0.rgb;
            combinedColour.a = result1.a * inputColour.a;
            break;
		}
		case PS_Combiners_Opaque_Mod2x:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= result1.rgb;
            result0.rgb *= inputColour.rgb;
            result0.rgb *= 4.0f;

            combinedColour.rgb = result0.rgb;
            combinedColour.a = dot(inputColour.aa, result1.aa);
            break;
		}
		case PS_Combiners_Opaque_Mod2xNA:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= result1.rgb;
            result0.rgb *= inputColour.rgb;
            result0.rgb *= 4.0f;

            combinedColour.rgb = result0.rgb;
            combinedColour.a = inputColour.a;
            break;
		}
		case PS_Combiners_Opaque_Opaque:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= result1.rgb;
            result0.rgb *= inputColour.rgb * 0.5f;
            result0.rgb *= 2.0f;

            combinedColour.rgb = result0.rgb;
            combinedColour.a = inputColour.a;
            break;
		}
		case PS_Combiners_Mod_Mod:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= result1.rgb;
            result0.rgb *= inputColour.rgb;
            result0.rgb *= 4.0f;
            result0.a *= inputColour.a;

            combinedColour.rgb = result0.rgb;
            combinedColour.a = result1.a * result0.a;
			break;
		}
		case PS_Combiners_Mod_Mod2x:
		{
			vec4 textureColour2 = texture(texture2, UV2);

	        textureColour1.a *= transparency.x;
	        textureColour2.a *= transparency.y;

	        vec4 result0 = textureColour1;
	        vec4 result1 = textureColour2;

	        result0.rgb *= result1.rgb;
	        result0.rgb *= inputColour.rgb;
	        result0.rgb *= 4.0f;
            result0.a *= inputColour.a;

	        combinedColour.rgb = result0.rgb;
	        combinedColour.a = dot(result0.aa, result1.aa);
	        break;
		}
		case PS_Combiners_Mod_Add:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= result1.rgb;
            result0.rgb *= inputColour.rgb;
            result0.rgb *= result0.rgb * 2.0f + result1.rgb;

            combinedColour.rgb = result0.rgb;
            combinedColour.a = inputColour.a * result0.a * result1.a;
            break;
		}
		case PS_Combiners_Mod_Mod2xNA:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result1.rgb *= result1.rgb;
            result1.rgb *= inputColour.rgb;
            result1.rgb *= 4.0f;

            combinedColour.rgb = result1.rgb;
            combinedColour.a = result0.a * inputColour.a;
            break;
		}
		case PS_Combiners_Mod_AddNA:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= inputColour.rgb;
            result1.rgb = result0.rgb * 2.0f + result1.rgb;

            combinedColour.rgb = result1.rgb;
            combinedColour.a = result0.a * inputColour.a;
            break;
		}
		case PS_Combiners_Mod_Opaque:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result1.rgb *= result0.rgb;
            result1.rgb *= inputColour.rgb;
            result1.rgb *= 2.0f;

            combinedColour.rgb = result1.rgb;
            combinedColour.a = result0.a * inputColour.a;
            break;
		}
		case PS_Combiners_Opaque_AddAlpha:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= inputColour.rgb;
            result0.rgb += result0.rgb;
            result0.rgb *= result1.rgb * result1.a + result0.rgb;

            combinedColour.rgb = result0.rgb;
            combinedColour.a = inputColour.a;
            break;
		}
		case PS_Combiners_Opaque_Alpha_Alpha:
		{
			vec4 textureColour2 = texture(texture2, UV2);

	        textureColour1.a *= transparency.x;
	        textureColour2.a *= transparency.y;

	        vec4 result0 = textureColour1;
	        vec4 result1 = textureColour2;

			result1.rgb = result1.rgb - result0.rgb;
			result1.rgb = result1.a * result1.rgb + result0.rgb;
			result0.rgb = -result1.rgb + result0.rgb;
			result1.rgb = result0.a * result0.rgb + result1.rgb;

			result1.rgb *= inputColour.rgb;
			result1.rgb *= 2.0f;

	        combinedColour.rgb = result1.rgb;
	        combinedColour.a = inputColour.a;
	        break;
		}
		case PS_Combiners_Opaque_AddAlpha_Alpha:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result1.rgb = result1.a * result1.rgb;
            result0.rgb *= inputColour.rgb;
            result1.a = -result0.a + 1;
            result0.rgb += result0.rgb;
            result1.rgb = result1.rgb * result1.a + result0.rgb;

            combinedColour.rgb = result1.rgb;
            combinedColour.a = inputColour.a;
            break;
		}
		case PS_Combiners_Mod_AddAlpha:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= inputColour.rgb;
            result0.rgb += result0.rgb;
            result1.rgb = result1.rgb * result1.a + result0.rgb;

            combinedColour.rgb = result1.rgb;
            combinedColour.a = result0.a * inputColour.a;
            break;

		}
		case PS_Combiners_Mod_Add_Alpha:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= inputColour.rgb;
            result0.rgb += result0.rgb;

            float modifier = -result0.a + 1;
            result0.rgb = result1.rgb * modifier + result0.rgb;

            combinedColour.rgb = result0.rgb;
            combinedColour.a = inputColour.a * result0.a * result1.a;
            break;
		}
		case PS_Combiners_Mod_AddAlpha_Alpha:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = vec4(0);
			vec4 result2 = textureColour2;

			result1.x = -result0.w + 1;

			result0 = result0.wxyz * inputColour.wxyz * 0,5f;
			result0.yzw = result0.yzw + result0.yzw;
			result1.yzw = result2.www * result2.xyz;
			result0.yzw = result1.yzw * result1.xxx + result0.yzw;

            combinedColour.rgb = result0.yzw;

            result0.y = dot(result2.xyz, vec4(0.3f, 0.59f, 0.11f, 0.0f).rgb); // uhhh indeed
            combinedColour.a = result2.w * result0.y + result0.x;
            break;
		}
		case PS_Combiners_Opaque_Mod2xNA_Alpha:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

			result1.rgb *= result0.rgb;
			result0.rgb = -result1.rgb * 2.0f + result0.rgb;
			result1.rgb += result1.rgb;
			result1.rgb = result0.a * result0.rgb + result1.rgb;
			result1.rgb *= inputColour.a * 0.5f;
			result1.rgb *= 2.0f;

            combinedColour.rgb = result1.rgb;
            combinedColour.a = inputColour.a;
            break;

		}
		case PS_Combiners_Opaque_ModNA_Alpha:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;
			vec3 result2 = result1.rgb * result0.rgb;

			result1.rgb = -result0.rgb * result1.rgb + result0.rgb;
			result1.rgb = result0.a * result1.rgb + result2.rgb;
			result1.rgb *= inputColour.a;
			result1.rgb *= 2.0f;

            combinedColour.rgb = result1.rgb;
            combinedColour.a = inputColour.a;
            break;
		}
		case PS_Combiners_Opaque_AddAlpha_Wgt:
		{
			vec4 textureColour2 = texture(texture2, UV2);

            textureColour1.a *= transparency.x;
            textureColour2.a *= transparency.y;

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;

            result0.rgb *= inputColour.rgb;
            result0.rgb *= result0.rgb;
            result1.rgb *= result1.a;
            result0.rgb = result1.rgb + result0.rgb;

            combinedColour.rgb = result0.rgb;
            combinedColour.a = inputColour.a;
            break;
		}
		case PS_Combiners_Mod_Dual_Crossfade:
		{
			vec4 textureColour2 = texture(texture2, UV2);
			vec4 textureColour3 = texture(texture2, UV3);

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;
            vec4 result2 = textureColour3;
			vec3 result3 = vec3(0);

			result1 -= result0;
			result3.xy = transparency.xy;
			result1 = result3.x * result1 + result0;
			result2 -= result1;
			result2 = result3.y * result2 + result1;
			result2.rgb *= inputColour.rgb;
			result2.rgb *= 2.0f;

            combinedColour.rgb = result3.rgb;
            combinedColour.a = result3.a * inputColour.a;
            break;
		}
		case PS_Combiners_Mod_Masked_Dual_Crossfade:
		{
			vec4 textureColour2 = texture(texture2, UV2);
            vec4 textureColour3 = texture(texture3, UV3);
            vec4 textureColour4 = texture(texture4, UV4);

            vec4 result0 = textureColour1;
            vec4 result1 = textureColour2;
            vec4 result2 = textureColour3;
            vec3 result3 = vec3(0);

            result1.wxyz -= result0.wxyz;
            result3.xy = transparency.xy;
            result1 = result3.x * result1 + result0.wxyz;
            result2 = result2.wxyz + -result1;
            result2 = result3.y * result2 + result1;
            result2 *= inputColour;
            result2 *= 2.0f;

            result1 = textureColour4;

            combinedColour.rgb = result2.rgb;
            combinedColour.a = result2.x * result1.w;
            break;
		}
		default:
		{
			discard;
		}
	}

	outputColour = commonFinalize(combinedColour);
}