
// The value for the gamma correction.
float gamma;
// The transformation Matrix for the vertex shader.
float4x4 MatrixTransform;

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

float4 GaussianBlur() : COLOR0
{

}

float4 ColorFilter() : COLOR0
{
	
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 GammaCorrection();
	}
}

