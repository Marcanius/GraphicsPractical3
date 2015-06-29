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
Texture2D screenGrab,
	bloomMelt1,
	bloomMelt2,
	bloomMelt3,
	bloomMelt4;

sampler TextureSampler = sampler_state
{
	Texture = < screenGrab >;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Mirror;
	AddressV = Mirror;
};

sampler BloomSampler = sampler_state
{
	Texture = < bloomMelt1 >;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;/*
	AddressU = Mirror;
	AddressV = Mirror;*/
};

sampler HalfSampler = sampler_state
{
	Texture = < bloomMelt2 >;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;/*
	AddressU = Mirror;
	AddressV = Mirror;*/
};

sampler QuarterSampler = sampler_state
{
	Texture = < bloomMelt3 >;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;/*
	AddressU = Mirror;
	AddressV = Mirror;*/
};

sampler OctoSampler = sampler_state
{
	Texture = < bloomMelt4 >;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;/*
	AddressU = Mirror;
	AddressV = Mirror;*/
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

float4 BloomMelt(float2 TexCoord : TEXCOORD0) : COLOR0
{
	// Sampling the input texture for the image without any bloom.
	float4 result = tex2D(TextureSampler, TexCoord);

	// Sampling the first scaled down, blurred image.
	float4 firstBlur = tex2D(BloomSampler, TexCoord);
	result.r = max(result.r, firstBlur.r);
	result.g = max(result.g, firstBlur.g);
	result.b = max(result.b, firstBlur.b);

	float4 secondBlur = tex2D(HalfSampler, TexCoord);
	result.r = max(result.r, secondBlur.r);
	result.g = max(result.g, secondBlur.g);
	result.b = max(result.b, secondBlur.b);

	float4 thirdBlur = tex2D(QuarterSampler, TexCoord);
	result.r = max(result.r, thirdBlur.r);
	result.g = max(result.g, thirdBlur.g);
	result.b = max(result.b, thirdBlur.b);

	float4 fourthBlur = tex2D(OctoSampler, TexCoord);
	result.r = max(result.r, fourthBlur.r);
	result.g = max(result.g, fourthBlur.g);
	result.b = max(result.b, fourthBlur.b);

	return result;
}

technique BrightFilter
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 BrightnessFilter();
	}
}

technique FinalBloom
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 BloomMelt();
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
		PixelShader = compile ps_2_0 GaussianBlurHorizontal();
	}
}

technique GaussianV
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 GaussianBlurVertical();
	}
}

