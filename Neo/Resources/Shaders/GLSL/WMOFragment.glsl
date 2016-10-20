#version 330 core

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
	vec4 fogParams;

	vec4 mousePosition;
	vec4 eyePosition;

	// x -> innerRadius
	// y -> outerRadius
	// z -> brushTime
	vec4 brushParams;
};

uniform Textures
{
	sampler2D batchTexture;
};

// Operation modes
uniform OperationModes
{
	uint OperationMode;
	bool ShouldBlend;
};

// Operation mode definitions
const uint Outdoor  = 0;
const uint Indoor   = 1;

in FragmentData
{
	vec4 fragmentPosition;
    vec3 fragmentNormal;
    vec2 fragmentUV;
    float fragmentDepth;
    vec4 fragmentColour;
    vec3 fragmentWorldPosition;
};

out vec4 finalColour;

vec3 getDiffuseLight(vec3 normal)
{
	float light = dot(normal, normalize(-vec3(-1, 1, -1)));
	if (light < 0.0)
	{
		light = 0.0;
	}

	if (light > 0.5)
	{
		light = 0.5 + (light - 0.5) * 0.65;
	}

	vec3 diffuse = diffuseLight.rgb * light;
	diffuse += ambientLight.rgb;
	diffuse = clamp(diffuse, 0.0f, 1.0f);

	return diffuse;
}

void main()
{
	vec4 textureColour = texture(batchTexture, fragmentUV);
	vec3 lightColour = getDiffuseColour(fragmentNormal);

	if (ShouldBlend)
	{
		if (textureColour.a < (5.0f / 255.0f))
        {
            discard;
        }
    }

	switch(OperationMode)
	{
		case Outdoor:
		{
			lightColour.rgb += fragmentColour.bgr * 2;
			lightColour.rgb = clamp(lightColour.rgb, 0.0f, 1.0f);
			textureColour.rgb *= lightColour;
			break;
		}
		case Indoor:
		{
			vec3 groupColour = fragmentColour.bgr;
			vec3 finalColour = fragmentColour.a * lightColour + (1 - fragmentColour.a) * groupColour;
			finalColour = clamp(finalColour, 0.0f, 1.0f);

			textureColour.rgb *= finalColour;
			break;
		}
		default:
		{
			discard;
		}
	}

	float fogDepth = fragmentDepth - fogStart;
    fogDepth /= (fogEnd - fogStart);

    float fog = 1.0f - pow(clamp(fogDepth, 0.0f, 1.0f), 1.5);

	textureColour.rgb = (1.0f - fog) * fogColour.rgb + fog * textureColour.rgb;
	finalColour = textureColour;
}