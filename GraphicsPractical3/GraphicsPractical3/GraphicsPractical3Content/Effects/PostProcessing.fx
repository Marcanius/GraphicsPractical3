// --------------------------------------- Defines --------------------------------------- \\

#define NUM_SAMPLES 100

// --------------------------------------- Top Level Variables --------------------------------------- \\

// The value for the gamma correction.
float gamma;

// The transformation Matrix for the vertex shader.
float4x4 MatrixTransform;

// Booleans for the different post processes.
bool grayScale, godRays;

// God Rays
// The sun.
float4 SunPosition;

float2 SunScreenPos;

float Density, Weight, Exposure, Decay;

// Texture deets
Texture2D screenGrab;

sampler TextureSampler = sampler_state
{
	Texture = < screenGrab >;
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

float4 GammaCorrection(float2 TexCoord : TEXCOORD0) : COLOR0
{
	// Sample each pixel from the completed screen render.
	float4 input = tex2D(TextureSampler, TexCoord);

	float4 output = input;

	// Correct the gamma of each color component.
	output.r = pow(input.r, 1 / gamma);
	output.g = pow(input.g, 1 / gamma);
	output.b = pow(input.b, 1 / gamma);

	// Return the processed color.
	return output;
}

float4 GaussianBlurV(float2 TexCoord : TEXCOORD0) : COLOR0
{
	return float4(0.5f, 0.5f, 0.5f, 1);
}

float4 GaussianBlurH(float2 TexCoord : TEXCOORD0) : COLOR0
{
	return float4(0.5f, 0.5f, 0.5f, 1);
}

float4 GodRays(float2 TexCoord : TEXCOORD0) : COLOR0
{
	// Calculate the vector between the pixel and the sun.
	float2 deltaCoord = (TexCoord - SunScreenPos.xy);

	// Divide by the number of samples to take, and scale by a control factor.
	deltaCoord *= Density / NUM_SAMPLES;

	// Store the initial sample.
	float3 Result = tex2D(TextureSampler, TexCoord);

		//return  float4(Result,1);

	// Setup the illumination factor.
	float illuminationDecay = 1.0f;

	// Evaluate the summation of the samples.
	for (int i = 0; i < NUM_SAMPLES; i++)
	{
		// Go to the next sample location.
		TexCoord -= deltaCoord;

		// Sample the location.
		float3 raySample = tex2D(TextureSampler, TexCoord);

		// Apply the attunuation and decay factor.
		raySample *= illuminationDecay * Weight;

		// Accumulate the combined color.
		Result += raySample;

		// Update the exponential decay factor
		illuminationDecay *= Decay;
	}

	// Output the final color with a scale control factor.
	return float4(Result * Exposure, 1);
}

float4 ColorFilter(float2 TexCoord : TEXCOORD0) : COLOR0
{ 
	// Sample each pixel from the completed screen render.
	float4 input = tex2D(TextureSampler, TexCoord);

	if (!grayScale)
		return input;

	float4 output = input;

	output.r = 0.30 * input.r + 0.59 * input.g + 0.11 * input.b;
	output.g = 0.30 * input.r + 0.59 * input.g + 0.11 * input.b;
	output.b = 0.30 * input.r + 0.59 * input.g + 0.11 * input.b;

	return output;
}

technique GreyScale
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 ColorFilter();
	}
}

technique VolumetricLighting
{
	/*pass Pass1
	{
		VertexShader = compile vs_3_0 Occluder();
	}*/

	pass Pass2
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 GodRays();
		AlphaBlendEnable = false;
		BlendOp = Add;
		SrcBlend = One;
		DestBlend = One;
	}
}

technique Gaussian
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 GaussianBlurV();
	}

	pass Pass2
	{
		PixelShader = compile ps_3_0 GaussianBlurH();
	}
}

