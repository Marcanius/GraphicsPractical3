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
	float LdotNN = dot(normalize(LightPosition - Position), Normal);

	return LightColor * (DiffuseIntensity * max(0.0f, LdotNN));
}

// --------------------------------------- The Vertex Shader --------------------------------------- \\

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);

	output.Position3D = mul(input.Position, World);
	output.Normal = input.Normal;

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

	if (cellShading)
	{
		output.r = float((int)(output.r * 3)) / 3;
		output.g = float((int)(output.g * 3)) / 3;
		output.b = float((int)(output.b * 3)) / 3;
	}

	return output;
	return (1, 0, 1, 1);
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
