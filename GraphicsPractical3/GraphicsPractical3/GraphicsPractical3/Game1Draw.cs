using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

partial class Game1
{
    bool drawLights;

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Create the texture for post-processing.
        DrawToTexture(postRenderTarget, World);


        // Clear the screen
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

        currentTechnique = "GreyScale";

        Vector3 tempSunScreen = Vector3.Transform(SunPosition, transform);
        this.SunScreenPos = new Vector2(tempSunScreen.X, tempSunScreen.Y);

        FillPostParameters(postProcessing);

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
            SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullNone, postProcessing);

        // Draw the texture with the post-processing effect applied.
        spriteBatch.Draw(postRenderTarget, new Rectangle(0, 0, 800, 600), Color.White);

        // Draw the string of totally useful information the player needs to have.
        spriteBatch.DrawString(spriteFont, camera.Eye + "\n" + camera.Focus, new Vector2(10, 11), Color.LightGreen);

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
        //World = quadTransform * Matrix.CreateTranslation(Vector3.Zero);
        //FillLightingParameters(lighting);
        //camera.SetEffectParameters(lighting);

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

        //camera.SetEffectParameters(effect);
        effect.CurrentTechnique = effect.Techniques["Technique1"];

        foreach (EffectPass p in effect.CurrentTechnique.Passes)
            p.Apply();

        // Draw the model.
        mesh.Draw();
    }

    // Draws all objects in the scene to a rendertarget, so we can apply post-processing.
    protected void DrawToTexture(RenderTarget2D renderTarget, Matrix World)
    {
        // Set the render target.
        GraphicsDevice.SetRenderTarget(renderTarget);

        // Draw the scene
        DrawScene(World);

        // Drop the render target
        GraphicsDevice.SetRenderTarget(null);
    }
}
