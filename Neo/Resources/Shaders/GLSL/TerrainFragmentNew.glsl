#version 330 core

uniform GlobalParameters
{
	mat4 ModelViewProjection;
	vec4 viewport;

	vec4 ambientLight;
	vec4 diffuseLight;

	vec4 fogColor;
	// x -> fogStart
	// y -> fogEnd
	// z -> farClip
	vec4 fogParams;

	vec4 mousePosition;
	vec4 eyePosition;

	// x -> innerRadius
	// y -> outerRadius
	// z -> brushTime
	vec4 brushParams;
};

in vec3 Normal;
in vec2 textureCoordinate0;
in vec2 textureCoordinate1;
in vec2 textureCoordinate2;
in vec2 textureCoordinate3;
in vec2 textureCoordinateAlpha;
in vec4 vertexShadingColour;
in vec4 vertexShadingAddColour;
in float depth;
in vec3 worldPosition;

uniform TerrainTextures
{
	sampler2D alphaTexture;
	sampler2D holeTexture;

	sampler2D texture0;
	sampler2D texture1;
	sampler2D texture2;
	sampler2D texture3;

	sampler2D texture0_s;
    sampler2D texture1_s;
    sampler2D texture2_s;
    sampler2D texture3_s;
};

vec3 sunDirection = vec3(1, 1, -1);

struct LightConstantData
{
	vec3 DiffuseLight;
	vec3 AmbientLight;
	float SpecularLight;
};

uniform TextureParameters
{
	vec4 textureScales;
	vec4 specularFactors;
};

out vec4 colour;

vec4 sinusInterpolate(vec4 src, vec4 dst, float alpha)
{
	float sinusValue = sin(alpha * 3.1415926 / 2.0f);
	return sinusValue * dst + (1 - sinusValue) * src;
}

vec4 applyBrush(vec4 inColour, vec3 worldPos)
{
	vec3 directionVector = worldPos - mousePosition.xyz;
	float dotSquared = dot(directionVector.xy, directionVector.xy);
	float directionZSquared = directionVector.z * directionVector.z;

	float innerRadius = brushParams.x * brushParams.x;
	float outerRadius = brushParams.y * brushParams.y;

	float factor = 1.0f;
	float brushRotation = 0.0f;
	float radius = outerRadius;

	// Is the point part of the inner circle?
	if (dotSquared < innerRadius && innerRadius * 0.95 < dotSquared)
	{
		brushRotation = 1.0f;
		radius = innerRadius;
	}
	// Is the point part of the outer circle?
	else if (dotSquared < outerRadius && outerRadius * 0.95 < dotSquared)
	{
		brushRotation = -1.0f;
		radius = outerRadius;
	}
	// Not part of anything
	else
	{
		fac = 0.0f;
	}

	// Antialiasing for the circle borders
	float antiAliasSize = radius * 0.01;
	factor *= clamp((dotSquared - radius * 0.95) / antiAliasSize, 0, 1);
	factor *= clamp((radius - dotSquared) / antiAliasSize, 0, 1);

	float angle = atan2(directionVector.y, directionVector.x) + 3.1415926 * brushRotation;
	float brushTime = brushParams.z * brushRotation * 10;
	angle = fmod(abs(degrees(angle) + brushTime), 36.0f);

	// Antialiasing between the circle segments
	factor *= clamp((18.0 - angle) / 0.4, 0, 1);
	factor *= clamp((angle - 0.4)  / 0.4, 0, 1);

	factor *= clamp(1 - directionZSquared / 2000, 0, 1);

	vec4 brushColour = vec4(1, 1, 1, 1);
	brushColour.xyz -= inColour.rgb;
	return sinusInterpolate(colour, brushColour, factor);
}

LightConstantData buildConstantLighting(vec3 normal, vec3 worldPos)
{
	vec3 lightDir = normalize(vec3(1, 1, -1));
	normal = normalize(normal);

	float light = dot(normal, -lightDir);
	if (light < 0.0)
	{
		light = 0.0;
	}

	if (light > 0.5)
	{
		light = 0.5 + (light - 0.5) * 0.65;
	}

	vec3 diffuseLight = diffuseLight.rgb * light;
	vec3 ambientLight = ambientLight.rgb;

	vec3 v = normalize(eyePosition.xyz - worldPos);
	vec3 h = normalize(-lightDir + v);
	float specularLight = max(0, dot(normal, h));

	return LightConstantData(diffuseLight, ambientLight, specularLight);
}

void main()
{
    vec4 alpha = alphaTexture.Sample(alphaSampler, textureCoordinateAlpha);
    float holeValue = holeTexture.Sample(alphaSampler, textureCoordinateAlpha).x;
    if (holeValue < 0.5)
    {
        discard;
    }

	vec4 c0 = texture(texture0, textureCoordinate0 * textureScales.x);
	vec4 c1 = texture(texture1, textureCoordinate1 * textureScales.y);
	vec4 c2 = texture(texture2, textureCoordinate2 * textureScales.z);
	vec4 c3 = texture(texture3, textureCoordinate3 * textureScales.w);

	vec4 c0_s = texture(texture0_s, textureCoordinate0 * textureScales.x);
	vec4 c1_s = texture(texture1_s, textureCoordinate0 * textureScales.y);
	vec4 c2_s = texture(texture2_s, textureCoordinate0 * textureScales.z);
	vec4 c3_s = texture(texture3_s, textureCoordinate0 * textureScales.w);

	LightConstantData lightData = buildConstantLighting(Normal, worldPosition);
	vec3 spc0 = c0_s.a * c0_s.rgb * pow(lightData.SpecularLight, 8) * specularFactors.x;
	vec3 spc1 = c1_s.a * c1_s.rgb * pow(lightData.SpecularLight, 8) * specularFactors.y;
	vec3 spc2 = c2_s.a * c2_s.rgb * pow(lightData.SpecularLight, 8) * specularFactors.z;
	vec3 spc3 = c3_s.a * c3_s.rgb * pow(lightData.SpecularLight, 8) * specularFactors.w;

	spc0 += (1 - specularFactors.x) * pow(lightData.SpecularLight, 8);
	spc1 += (1 - specularFactors.y) * pow(lightData.SpecularLight, 8);
	spc2 += (1 - specularFactors.z) * pow(lightData.SpecularLight, 8);
	spc3 += (1 - specularFactors.w) * pow(lightData.SpecularLight, 8);

	c0.rgb *= lightData.DiffuseLight + lightData.AmbientLight + spc0;
	c1.rgb *= lightData.DiffuseLight + lightData.AmbientLight + spc1;
	c2.rgb *= lightData.DiffuseLight + lightData.AmbientLight + spc2;
	c3.rgb *= lightData.DiffuseLight + lightData.AmbientLight + spc3;

	c0.rgb = saturate(c0.rgb);
	c1.rgb = saturate(c1.rgb);
	c2.rgb = saturate(c2.rgb);
	c3.rgb = saturate(c3.rgb);

    colour = (1.0 - (alpha.g + alpha.b + alpha.a)) * c0;
    colour = c1 * alpha.g;
    colour = c2 * alpha.b;
    colour = c3 * alpha.a;

    vec4 textureColor = colour;

    //color.rgb *= getDiffuseLight(input.normal, input.worldPosition);
    colour.rgb *= vertexShadingColour.bgr * 2;
    // PORT: Possible bug introduced (textureColor -> textureColor.bgr)
    colour.rgb += vertexShadingAddColour.bgr * textureColor.bgr;
    colour.rgb *= alpha.r;
    colour.rgb = saturate(colour.rgb);

    float fogDepth = depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = 1.0f - pow(saturate(fogDepth), 1.5);

    colour.rgb = (1.0 - fog) * fogColor.rgb + fog * colour.rgb;
    colour = applyBrush(colour, worldPosition);
    colour.a = holeValue;
}
