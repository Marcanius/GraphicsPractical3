// --------------------------------------- Defines --------------------------------------- \\

#define Max_Lights 5

// --------------------------------------- Top Level Variables --------------------------------------- \\

// Matrices for 3D projection
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldIT;

// Light Sources.
float3 LightPositions[Max_Lights];
float4 LightColors[Max_Lights];

// Ambient Lighting.
float4 AmbientColor;
float AmbientIntensity;

// Diffuse Lighting.
float DiffuseIntensity;

// Specular Lighting.
float4 SpecularColor;
float SpecularIntensity, SpecularPower;

// Cell Shading.
bool cellShading;

// --------------------------------------- Shader Inputs --------------------------------------- \\

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Normal : TEXCOORD0;
	float3 Position3D : TEXCOORD1;
};

// --------------------------------------- Lighting Functions --------------------------------------- \\

float4 DiffuseShading(float3 Position, float3 Normal, float3 LightPosition, float4 LightColor)
{
	//float distance = length(LightPosition - Position);
	float LdotNN = dot(normalize(LightPosition - Position), Normal);

	return LightColor * (DiffuseIntensity * max(0.0f, LdotNN));// *(1 / pow(distance, 2));
}

// --------------------------------------- The Vertex Shader --------------------------------------- \\

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Position3D = worldPosition;
	output.Normal = normalize(mul(input.Normal, WorldIT));

	return output;
}

// --------------------------------------- The Pixel Shader -------------------------------------- \\

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// By default, a pixel gets the ambient coloring.
	float4 output = AmbientColor * AmbientIntensity;

	// Determine for each of the color channels what the highest values from all lights are.
	for (int i = 0; i < Max_Lights; i++)
	{
		float4 comparison = DiffuseShading(input.Position3D, input.Normal, LightPositions[i], LightColors[i]);

		output.r = max(output.r, comparison.r);
		output.g = max(output.g, comparison.g);
		output.b = max(output.b, comparison.b);
	}

	// We lose precision in the color values by first mutiplying by 3 and losing the decimal and then divide by 3.
	if (cellShading)
	{
		output.r = float((int)(output.r * 5)) / 5;
		output.g = float((int)(output.g * 5)) / 5;
		output.b = float((int)(output.b * 5)) / 5;
	}

	// We divide by the diffuse intensity, because this is the highest value our colors can get.
	output /= DiffuseIntensity;

	return output;
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
