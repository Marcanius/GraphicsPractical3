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
public class Game1 : Microsoft.Xna.Framework.Game
{
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;
    FrameRateCounter frameRateCounter;
    Camera camera;

    // Post-Processing
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

        // Initialize the camera, located at (0,50,100), and looking at the origin.
        this.camera = new Camera(new Vector3(0, 50, 100), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

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

        // Load the PostProcesssing effect, and fill its parameter.
        postProcessing = this.Content.Load<Effect>("Effects/PostProcessing");
        postProcessing.Parameters["gamma"].SetValue(1.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        // Allows the game to exit
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            this.Exit();

        // Update the window title
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

        // Creating the world Matrix for the effect.
        Matrix World = Matrix.CreateScale(0.5f);

        // Create the texture for post-processing.
        DrawToTexture(postRenderTarget, World);

        // Clear the screen
        GraphicsDevice.Clear(Color.Black);

        // Draw the texture with the post-processing effect applied.
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
            SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullNone, postProcessing);

        spriteBatch.Draw(postRenderTarget, new Rectangle(0, 0, 800, 600), Color.White);

        spriteBatch.End();

        base.Draw(gameTime);
    }

    protected void DrawToTexture(RenderTarget2D renderTarget, Matrix world)
    {
        // Set the render target.
        GraphicsDevice.SetRenderTarget(renderTarget);

        // Draw the scene
        GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DeepSkyBlue, 1.0f, 0);

        // TODO: Draw Stuff and Things.

        // Drop the render target
        GraphicsDevice.SetRenderTarget(null);
    }
}
