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

    // Lighting
    Effect lighting;

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
        this.camera = new Camera(new Vector3(-100, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

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

        // The positions of the multiple lights.
        LightPositions = new Vector3[5];
        LightPositions[0] = new Vector3(100, 0, 0);
        LightPositions[1] = new Vector3(0, 100, 0);
        LightPositions[2] = new Vector3(0, 0, 100);
        LightPositions[3] = new Vector3(-100, 0, 0);
        LightPositions[4] = new Vector3(0, 0, -100);

        // The colors of the multiple lights.
        LightColors = new Vector4[5];
        LightColors[0] = Color.WhiteSmoke.ToVector4();
        LightColors[1] = Color.Tomato.ToVector4();
        LightColors[2] = Color.GreenYellow.ToVector4();
        LightColors[3] = Color.DarkBlue.ToVector4();
        LightColors[4] = Color.HotPink.ToVector4();

        FillLightingParameters(lighting);

        // Load the models.
        bunny = this.Content.Load<Model>("Models/bunny");
        bunny.Meshes[0].MeshParts[0].Effect = lighting;

        head = this.Content.Load<Model>("Models/femalehead");
        head.Meshes[0].MeshParts[0].Effect = lighting;

        // Load the PostProcesssing effect, and fill its parameter.
        postProcessing = this.Content.Load<Effect>("Effects/PostProcessing");
        postProcessing.Parameters["gamma"].SetValue(1.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        // Allows the game to exit.
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            this.Exit();

        this.camera.Update(gameTime);

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

    protected void DrawScene(Matrix World)
    {
        // Get a clear background.
        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);

        // Draw the quad.


        // Draw the bunny.
        DrawModel(bunny, World);

        // Draw the head.
        DrawModel(head, World);
    }

    public void DrawModel(Model Model, Matrix World)
    {
        // Get the mesh. 
        ModelMesh mesh = Model.Meshes[0];
        // Get the effect.
        Effect effect = mesh.Effects[0];

        // Set the effect parameters.
        camera.SetEffectParameters(effect);
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
