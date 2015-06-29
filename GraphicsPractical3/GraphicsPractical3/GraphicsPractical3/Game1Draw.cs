using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

partial class Game1
{
    // PostProcessing Pipeline:
    // Ambient Occlusion
    // Screen Space reflections.
    // Tone Mapping.
    // HDR BLoom / Glare.
    // Depth of Field.
    // Film Grain / vignetting / chromatic abberration.
    // Color grading
    // Gamma correction.

    protected override void Draw(GameTime gameTime)
    {
        // Create the texture for post-processing.
        GraphicsDevice.SetRenderTarget(sceneRender);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw the scene
        DrawScene(World);

        #region Post Processing

        // Clear the screen
        GraphicsDevice.SetRenderTarget(null);
        Texture2D intermediateResult = (Texture2D)sceneRender;
        GraphicsDevice.Clear(Color.Black);

        // Create the Transformation Matrix for the VertexShader, so we can use shader model 3.
        Matrix projection = Matrix.CreateOrthographicOffCenter(
            0,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            0, 0, 1
            );
        Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
        Matrix transform = halfPixelOffset * projection;

        postProcessing.Parameters["MatrixTransform"].SetValue(transform);

        // Draw the different post processing effects.
        if (bloom)
        {
            // Draw the scene, like a bad camera.
            intermediateResult = DrawBloom(intermediateResult);
        }

        if (greyScale)
        {
            // Draw the scene, like a dog would.
            intermediateResult = DrawGreyScale(intermediateResult);
        }

        if (gaussian)
        {
            // Draw the scene, with bad glasses.
            intermediateResult = DrawGaussianBlur(intermediateResult);
        }

        DrawNoEffect(intermediateResult);

        #endregion

        // Draw the string of totally useful information the player needs to have.
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
            SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullNone);
        spriteBatch.DrawString(spriteFont, hudMessage, new Vector2(10, 11), Color.White);
        spriteBatch.End();

        base.Draw(gameTime);
    }

    // Draws all the models and quads and boxes.
    protected void DrawScene(Matrix World)
    {
        // Get a clear background.
        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);

        // Prepare The Quad.
        //lighting.CurrentTechnique = lighting.Techniques["Technique1"];

        foreach (EffectPass pass in lighting.CurrentTechnique.Passes)
            pass.Apply();

        // Draw the Quad.
        this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, quadVertices,
            0, quadVertices.Length, quadIndices, 0, this.quadIndices.Length / 3);

        // Draw the Box.
        for (int i = 0; i < boxes.Count; i++)
        {
            this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, boxes[i],
                0, boxes[i].Length, boxIndices, 0, this.boxIndices.Length / 3);
        }

        // Draw the bunny.
        DrawModel(bunny, 10.0f, new Vector3(2, 10, 3));

        // Draw the head.
        DrawModel(head, 0.1f, new Vector3(0, 10, 0));

        // Draw the logo.
        mjp.Draw(GraphicsDevice);

        World = quadTransform * Matrix.CreateTranslation(new Vector3(0, 10, 0));
        FillLightingParameters(lighting);
        camera.SetEffectParameters(lighting);

        foreach (EffectPass pass in lighting.CurrentTechnique.Passes)
            pass.Apply();
    }

    // Applies the correct parameters to each effect and draws a model.
    public void DrawModel(Model Model, float Scale, Vector3 Translation)
    {
        // Get the mesh. 
        ModelMesh mesh = Model.Meshes[0];
        // Get the effect.
        Effect effect = mesh.Effects[0];

        // Set the effect parameters.
        World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Translation);
        FillLightingParameters(effect);

        effect.CurrentTechnique = effect.Techniques["Technique1"];

        foreach (EffectPass p in effect.CurrentTechnique.Passes)
            p.Apply();

        // Draw the model.
        mesh.Draw();
    }

    protected Texture2D DrawGaussianBlur(Texture2D inputTexture)
    {
        // Set the post effect parameters.
        postProcessing.CurrentTechnique = postProcessing.Techniques["GaussianH"];
        postProcessing.Parameters["screenGrab"].SetValue(inputTexture);
        postProcessing.Parameters["weight"].SetValue(weights);
        postProcessing.Parameters["offsetHor"].SetValue(offsetHor);
        postProcessing.Parameters["offsetVer"].SetValue(offsetVer);

        // Create the first render, with the horizontal pass applied.
        GraphicsDevice.SetRenderTarget(gaussianRender);
        GraphicsDevice.Clear(Color.Black);

        // Draw the render, with the horizontal blur pass applied.
        spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postProcessing);
        spriteBatch.Draw(inputTexture, new Rectangle(0, 0, 800, 600), Color.White);
        spriteBatch.End();

        // Get the intermediate result, and render to the result Render Target.
        GraphicsDevice.SetRenderTarget(gaussianRender2);
        Texture2D intermediate = (Texture2D)gaussianRender;
        GraphicsDevice.Clear(Color.Black);

        // Set the post effect parameters.
        postProcessing.CurrentTechnique = postProcessing.Techniques["GaussianV"];
        postProcessing.Parameters["screenGrab"].SetValue(intermediate);

        // Draw the render, with the vertical blur pass applied.
        spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postProcessing);
        spriteBatch.Draw(intermediate, new Rectangle(0, 0, 800, 600), Color.White);
        spriteBatch.End();

        // Return the end result.
        GraphicsDevice.SetRenderTarget(null);
        return (Texture2D)gaussianRender2;
    }

    // Draw the rendered scene, with bloom applied.
    protected Texture2D DrawBloom(Texture2D inputTexture)
    {
        // Create the first render target for the brightness filter pass.
        GraphicsDevice.SetRenderTarget(bloomRenderBright);
        GraphicsDevice.Clear(Color.Black);

        // Create a texture without any dark colors.
        postProcessing.CurrentTechnique = postProcessing.Techniques["BrightFilter"];
        postProcessing.Parameters["screenGrab"].SetValue(inputTexture); 
        postProcessing.Parameters["brightnessThreshold"].SetValue(brightnessThreshold);

        spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postProcessing);
        spriteBatch.Draw(inputTexture, new Rectangle(0, 0, bloomRenderBright.Width, bloomRenderBright.Height), Color.White);
        spriteBatch.End();

        // Scale the texture to 1/2, 1/4 and 1/8 size.
        // Render the texture again, to a new target, and with a smaller scale.
        radius = 5;
        amount = 5;


        helpDraw(bloomRenderBlurH, bloomRenderBlurHV, bloomRenderBright);
        helpDraw(bloomRender2BlurH, bloomRender2BlurHV, bloomRenderBright);
        helpDraw(bloomRender4BlurH, bloomRender4BlurHV, bloomRenderBright);
        helpDraw(bloomRender8BlurH, bloomRender8BlurHV, bloomRenderBright);

        // Resize the downscaled texture to the original size.

        // Paste all the textures together.

        // Return the end result.
        GraphicsDevice.SetRenderTarget(null);
        return bloomRenderFinish;
    }

    private void helpDraw(RenderTarget2D rH, RenderTarget2D rV, RenderTarget2D r1)
    {
        GraphicsDevice.SetRenderTarget(rH);
        GraphicsDevice.Clear(Color.Black);

        postProcessing.CurrentTechnique = postProcessing.Techniques["GaussianH"];
        postProcessing.Parameters["screenGrab"].SetValue(r1);
        CalculateOffsetAndWeight(rH.Width / 2, rV.Height / 2);
        postProcessing.Parameters["weight"].SetValue(weights);
        postProcessing.Parameters["offsetHor"].SetValue(offsetHor);
        postProcessing.Parameters["offsetVer"].SetValue(offsetVer);
        postProcessing.CurrentTechnique.Passes["Pass1"].Apply();

        spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postProcessing);
        spriteBatch.Draw(bloomRenderBright, new Rectangle(0, 0, rH.Width, rH.Height), Color.White);
        spriteBatch.End();

        GraphicsDevice.SetRenderTarget(rV);
        GraphicsDevice.Clear(Color.Black);

        postProcessing.CurrentTechnique = postProcessing.Techniques["GaussianV"];
        postProcessing.Parameters["screenGrab"].SetValue(rH);
        postProcessing.CurrentTechnique.Passes["Pass1"].Apply();

        spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postProcessing);
        spriteBatch.Draw(rH, new Rectangle(0, 0, rV.Width, rV.Height), Color.White);
        spriteBatch.End();
    }

    // Draw the rendered scene, with greyScaling applied.
    protected Texture2D DrawGreyScale(Texture2D inputTexture)
    {
        // Render to a single render target.
        GraphicsDevice.SetRenderTarget(greyRender);
        GraphicsDevice.Clear(Color.Black);

        // Set the parameters for the effect
        postProcessing.CurrentTechnique = postProcessing.Techniques["GreyScale"];
        postProcessing.Parameters["screenGrab"].SetValue(inputTexture);

        spriteBatch.Begin(0, BlendState.Opaque, null, null, null, postProcessing);
        spriteBatch.Draw(inputTexture, new Rectangle(0, 0, greyRender.Width, greyRender.Height), Color.White);
        spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);

        return (Texture2D)greyRender;
    }

    // Draw the rendered scene, with no postprocessing applied.
    protected void DrawNoEffect(Texture2D inputTexture)
    {
        GraphicsDevice.SetRenderTarget(null);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
            SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullNone);
        spriteBatch.Draw(inputTexture, new Rectangle(0, 0, 800, 600), Color.White);
        spriteBatch.End();
    }
}
