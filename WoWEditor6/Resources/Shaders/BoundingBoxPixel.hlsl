struct PixelInput {
	float4 position : SV_Position;
	float3 texCoord : TEXCOORD0;
};

float4 main(PixelInput input) : SV_Target {
	float3 tc = input.texCoord;

	float4 ret = float4(1, 1, 1, 1);
	if ((tc.x < 0.02 || tc.x > 0.98) && (tc.y < 0.02 || tc.y > 0.98)) {
		ret = float4(1, 1, 1, 0.7);
	}
	else if ((tc.z < 0.02 || tc.z > 0.98) && (tc.x < 0.02 || tc.x > 0.98 || tc.y < 0.02 || tc.y > 0.98)) {
		ret = float4(1, 1, 1, 0.7);
	}
	else
		discard;

	return ret;
}