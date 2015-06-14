using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/// <summary>
/// This is the main type for your game
/// </summary>
public partial class Game1 : Microsoft.Xna.Framework.Game
{
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;
    FrameRateCounter frameRateCounter;
    Camera camera;
    SpriteFont spriteFont;

    // The scene.
    Model bunny;
    Model head;

    // The quad.
    private VertexPositionNormalTexture[] quadVertices;
    private short[] quadIndices;
    private Matrix quadTransform;

    // Lighting
    Effect lighting;
    MovingLight[] lights;

    // Post-Processing.
    Effect postProcessing;
    RenderTarget2D postRenderTarget;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        // Create and add a frame rate counter
        this.frameRateCounter = new FrameRateCounter(this);
        this.Components.Add(this.frameRateCounter);
    }

    protected override void Initialize()
    {
        // Copy over the device's rasterizer state to change the current cullMode.
        this.GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };

        // Set up the window.
        this.graphics.PreferredBackBufferWidth = 800;
        this.graphics.PreferredBackBufferHeight = 600;
        this.graphics.IsFullScreen = false;
        // Let the renderer draw and update as often as possible.
        this.graphics.SynchronizeWithVerticalRetrace = false;
        this.IsFixedTimeStep = false;
        // Flush the changes to the device parameters to the graphics card.
        this.graphics.ApplyChanges();

        // Initialize the camera, located at (-100, 0, 0), and looking at the origin.
        this.camera = new Camera(new Vector3(-20, 10, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

        // Make the cursor visible on screen.
        this.IsMouseVisible = true;

        // The rendertarget for the postprocessing.
        postRenderTarget = new RenderTarget2D(
            GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Create a new SpriteBatch, which can be used to draw textures.
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load the spritefont, to draw text onto the screen.
        spriteFont = Content.Load<SpriteFont>("spriteFont");

        // Load the lighting effect, and fill its parameters.
        lighting = Content.Load<Effect>("Effects/Lighting");

        // Fill the parameters.
        World = Matrix.CreateScale(0.5f);

        // Creating the moving lights.
        lights = new MovingLight[5];
        lights[0] = new MovingLight(new Vector3(020, 20, 000), 10.0f, Color.WhiteSmoke.ToVector4());
        lights[1] = new MovingLight(new Vector3(000, 20, 000), 10.0f, Color.Tomato.ToVector4());
        lights[2] = new MovingLight(new Vector3(000, 20, 020), 10.0f, Color.GreenYellow.ToVector4());
        lights[3] = new MovingLight(new Vector3(-20, 20, 000), 10.0f, Color.DarkBlue.ToVector4());
        lights[4] = new MovingLight(new Vector3(000, 20, -20), 10.0f, Color.HotPink.ToVector4());

        // The positions of the multiple lights.
        LightPositions = new Vector3[5];

        // The colors of the multiple lights.
        LightColors = new Vector4[5];

        cellShading = true;
        FillLightingParameters(lighting);

        // Load the models.
        bunny = this.Content.Load<Model>("Models/bunny");
        bunny.Meshes[0].MeshParts[0].Effect = lighting;

        head = this.Content.Load<Model>("Models/femalehead");
        head.Meshes[0].MeshParts[0].Effect = lighting;

        setupQuad();

        // Load the PostProcesssing effect, and fill its parameters.
        postProcessing = this.Content.Load<Effect>("Effects/PostProcessing");

        gamma = 1.0f;
        greyScale = false;
        FillPostParameters(postProcessing);
    }

    protected override void Update(GameTime gameTime)
    {
        // Allows the game to exit.
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            this.Exit();

        // Update the position of the camera.
        this.camera.Update(gameTime);

        // Update the positions and colors of the lights.
        for (int i = 1; i < lights.Length; i++)
        {
            lights[i].Update(gameTime);

            LightPositions[0] = lights[0].Position;
            LightPositions[1] = lights[1].Position;
            LightPositions[2] = lights[2].Position;
            LightPositions[3] = lights[3].Position;
            LightPositions[4] = lights[4].Position;

            LightColors[0] = lights[0].Color;
            LightColors[1] = lights[1].Color;
            LightColors[2] = lights[2].Color;
            LightColors[3] = lights[3].Color;
            LightColors[4] = lights[4].Color;
        }

        // Update the window title.
        this.Window.Title = "XNA Renderer | FPS: " + this.frameRateCounter.FrameRate;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Creating the Transformation Matrix for the VertexShader, so we can use shader model 3.
        Matrix projection = Matrix.CreateOrthographicOffCenter(
            0,
            GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height,
            0, 0, 1
            );
        Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

        postProcessing.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

        // Create the texture for post-processing.
        DrawToTexture(postRenderTarget, World);

        // Clear the screen
        GraphicsDevice.Clear(Color.Black);

        // Draw the texture with the post-processing effect applied.
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
            SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullNone, postProcessing);

        spriteBatch.Draw(postRenderTarget, new Rectangle(0, 0, 800, 600), Color.White);
        spriteBatch.DrawString(spriteFont, camera.Eye + "\n" + camera.Focus, new Vector2(10, 11), Color.LightGreen);

        spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Sets up a 2 by 2 quad around the origin.
    /// </summary>
    private void setupQuad()
    {
        float scale = 5.0f;

        // Normal points up
        Vector3 quadNormal = new Vector3(0, 1, 0);

        this.quadVertices = new VertexPositionNormalTexture[4];
        // Top left
        this.quadVertices[0].Position = new Vector3(-100, 0, -100);
        this.quadVertices[0].Normal = quadNormal;
        this.quadVertices[0].TextureCoordinate = new Vector2(0, 0);
        // Top right
        this.quadVertices[1].Position = new Vector3(100, 0, -100);
        this.quadVertices[1].Normal = quadNormal;
        this.quadVertices[1].TextureCoordinate = new Vector2(3, 0);
        // Bottom left
        this.quadVertices[2].Position = new Vector3(-100, 0, 100);
        this.quadVertices[2].Normal = quadNormal;
        this.quadVertices[2].TextureCoordinate = new Vector2(0, 3);
        // Bottom right
        this.quadVertices[3].Position = new Vector3(100, 0, 100);
        this.quadVertices[3].Normal = quadNormal;
        this.quadVertices[3].TextureCoordinate = new Vector2(3, 3);

        this.quadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
        this.quadTransform = Matrix.CreateScale(scale);
    }

    protected void DrawScene(Matrix World)
    {
        // Get a clear background.
        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);

        // Prepare The Quad.
        lighting.CurrentTechnique = lighting.Techniques["Technique1"];
        World = quadTransform * Matrix.CreateTranslation(Vector3.Zero);
        FillLightingParameters(lighting);
        camera.SetEffectParameters(lighting);

        foreach (EffectPass pass in lighting.CurrentTechnique.Passes)
            pass.Apply();

        // Draw the Quad.
        this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, quadVertices,
            0, quadVertices.Length, quadIndices, 0, this.quadIndices.Length / 3);

        // Draw the bunny.
        DrawModel(bunny, 10.0f, new Vector3(2, 10, 3));

        // Draw the head.
        DrawModel(head, 0.1f, new Vector3(0, 10, 0));
    }

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
