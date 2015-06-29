// --------------------------------------- Defines --------------------------------------- \\

#define initialSize 15

// --------------------------------------- Top Level Variables --------------------------------------- \\

// The transformation Matrix for the vertex shader.
float4x4 MatrixTransform;

// Booleans for the different post processes.
bool grayScale, godRays, gaussian;

// Array info needed for GaussianBlur
float2 offsetHor[initialSize];
float2 offsetVer[initialSize];
float weight[initialSize];

// Bloom
float brightnessThreshold;

// Texture deets
Texture2D screenGrab;

sampler TextureSampler = sampler_state
{
	Texture = < screenGrab >;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Mirror;
	AddressV = Mirror;
};

//--------------------------------------- The Vertex Shader---------------------------------------\\

void SpriteVertexShader(
	inout float4 color    : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : SV_Position)
{
	position = mul(position, MatrixTransform);
}

// -------------------------------------- The Pixel Shader -------------------------------------- \\

float4 GaussianBlurHorizontal(float2 TexCoord : TEXCOORD0) : COLOR0
{
	float4 result = float4(0.0f, 0.0f, 0.0f, 0.0f);

	//if (!gaussian)
	//	return tex2D(TextureSampler, TexCoord);

	for (int i = 0; i < initialSize; i++)
		result += tex2D(TextureSampler, offsetHor[i] + TexCoord) * weight[i];

	return result;
}

float4 GaussianBlurVertical(float2 TexCoord : TEXCOORD0) : COLOR0
{
	float4 result = float4(0.0f, 0.0f, 0.0f, 0.0f);

	//if (!gaussian)
	//	return tex2D(TextureSampler, TexCoord);

	for (int i = 0; i < initialSize; i++)
		result += tex2D(TextureSampler, offsetVer[i] + TexCoord) * weight[i];

	return result;
}

float4 ColorFilter(float2 TexCoord : TEXCOORD0) : COLOR0
{ 
	// Sample each pixel from the completed screen render.
	float4 input = tex2D(TextureSampler, TexCoord);

	float4 result;

	result.r = 0.30 * input.r + 0.59 * input.g + 0.11 * input.b;
	result.g = 0.30 * input.r + 0.59 * input.g + 0.11 * input.b;
	result.b = 0.30 * input.r + 0.59 * input.g + 0.11 * input.b;
	result.a = 1.0f;

	return result;
}

float4 BrightnessFilter(float2 TexCoord : TEXCOORD0) : COLOR0
{
	// Sample the texture.
	float4 result = tex2D(TextureSampler, TexCoord);

	// Subtract the threshold from each color channel.
	result.r -= brightnessThreshold;
	result.g -= brightnessThreshold;
	result.b -= brightnessThreshold;

	// Return the new color.
	return result;
}

float4 Bloom(float2 TexCoord : TEXCOORD0) : COLOR0
{

}

technique BrightFilter
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 BrightnessFilter();
	}
}

technique GreyScale
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 ColorFilter();
	}
}

technique GaussianH
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 GaussianBlurVertical();
	}
}

technique GaussianV
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 GaussianBlurHorizontal();
	}
}

