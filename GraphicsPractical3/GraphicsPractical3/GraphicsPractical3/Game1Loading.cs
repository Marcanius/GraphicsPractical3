using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public partial class Game1 : Microsoft.Xna.Framework.Game
{
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;
    FrameRateCounter frameRateCounter;
    Camera camera;
    SpriteFont spriteFont;
    string hudMessage;

    // The scene.
    Model bunny;
    Model head;
    MJPLogo mjp;

    // The quad.
    private VertexPositionNormalTexture[] quadVertices;
    private short[] quadIndices;
    private Matrix quadTransform;

    // The box
    private List<VertexPositionNormalTexture[]> boxes;
    private short[] boxIndices;
    //private Matrix boxTransform;

    // Lighting
    Effect lighting;
    MovingLight[] lights;

    // Post-Processing.
    Effect postProcessing;
    RenderTarget2D sceneRender, 
        gaussianRender, gaussianRender2, 
        greyRender, 
        bloomRenderBright, 
        bloomRenderBlurH, bloomRenderBlurHV,
        bloomRender2BlurH, bloomRender2BlurHV,
        bloomRender4BlurH, bloomRender4BlurHV,
        bloomRender8BlurH, bloomRender8BlurHV,
        bloomRenderFinish;
    Vector2[] offsetHor, offsetVer;
    float[] weights;
    float amount = 2.0f;
    int radius = 7;

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
        this.camera = new Camera(new Vector3(0, 15, 15), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

        // Make the cursor visible on screen.
        this.IsMouseVisible = true;

        #region RenderTargets

        // The rendertarget for the postprocessing.
        sceneRender = new RenderTarget2D(
            GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        gaussianRender = new RenderTarget2D(
            GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        // The render targets for gaussian blur.
        gaussianRender2 = new RenderTarget2D(GraphicsDevice, 
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight, 
            false, 
            GraphicsDevice.PresentationParameters.BackBufferFormat, 
            DepthFormat.Depth24);

        greyRender = new RenderTarget2D(GraphicsDevice, 
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight, 
            false, 
            GraphicsDevice.PresentationParameters.BackBufferFormat, 
            DepthFormat.Depth24);

        // The render target for the brightness pass of bloom.
        bloomRenderBright = new RenderTarget2D(GraphicsDevice, 
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight, 
            false, 
            GraphicsDevice.PresentationParameters.BackBufferFormat, 
            DepthFormat.Depth24);

        bloomRenderBlurH = new RenderTarget2D(GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        bloomRenderBlurHV = new RenderTarget2D(GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        bloomRender2BlurH = new RenderTarget2D(GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth / 2,
            GraphicsDevice.PresentationParameters.BackBufferHeight / 2,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        bloomRender2BlurHV = new RenderTarget2D(GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth / 2,
            GraphicsDevice.PresentationParameters.BackBufferHeight / 2,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        bloomRender4BlurH = new RenderTarget2D(GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth / 4,
            GraphicsDevice.PresentationParameters.BackBufferHeight / 4,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        bloomRender4BlurHV = new RenderTarget2D(GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth / 4,
            GraphicsDevice.PresentationParameters.BackBufferHeight / 4,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        bloomRender8BlurHV = new RenderTarget2D(GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth / 8,
            GraphicsDevice.PresentationParameters.BackBufferHeight / 8,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        bloomRender8BlurHV = new RenderTarget2D(GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth / 8,
            GraphicsDevice.PresentationParameters.BackBufferHeight / 8,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);

        bloomRenderFinish = new RenderTarget2D(GraphicsDevice,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight,
            false,
            GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
        #endregion

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
        lights[0] = new MovingLight(new Vector3(000, 13, 000), 2, 0, 1.0f, Color.IndianRed.ToVector4());
        lights[1] = new MovingLight(new Vector3(000, 13, 000), -4, .25f, 1.1f, Color.LimeGreen.ToVector4());
        lights[2] = new MovingLight(new Vector3(000, 13, 000), 6, .5f, 0.8f, Color.SkyBlue.ToVector4());
        lights[3] = new MovingLight(new Vector3(-00, 13, 000), -8, .75f, 1.2f, Color.Fuchsia.ToVector4());
        lights[4] = new MovingLight(new Vector3(000, 13, -00), 10, 1f, 1.25f, Color.Cyan.ToVector4());

        // Create the sun for the god rays.

        // The positions of the multiple lights.
        LightPositions = new Vector3[5];
        LightPositions[0] = lights[0].Position;
        LightPositions[1] = lights[1].Position;
        LightPositions[2] = lights[2].Position;
        LightPositions[3] = lights[3].Position;
        LightPositions[4] = lights[4].Position;

        // The colors of the multiple lights.
        LightColors = new Vector4[5];
        LightColors[0] = lights[0].Color;
        LightColors[1] = lights[1].Color;
        LightColors[2] = lights[2].Color;
        LightColors[3] = lights[3].Color;
        LightColors[4] = lights[4].Color;

        cellShading = false;

        FillLightingParameters(lighting);

        // Load the models.
        bunny = this.Content.Load<Model>("Models/bunny");
        bunny.Meshes[0].MeshParts[0].Effect = lighting;

        head = this.Content.Load<Model>("Models/femalehead");
        head.Meshes[0].MeshParts[0].Effect = lighting;

        setupQuad();

        boxes = new List<VertexPositionNormalTexture[]>();
        for (int i = 0; i < 5; i++)
        {
            boxes.Add(setUpBoxes(new Vector3(100 + 23 * i, 5 - 2.5f * i, -200 - 7 * i)));
        }

        // Setup the MJP Logo
        mjp = new MJPLogo();

        // Load the PostProcesssing effect, and fill its parameters.
        CalculateOffsetAndWeight(400, 300);

        postProcessing = this.Content.Load<Effect>("Effects/PostProcessing");

        currentTechnique = "GreyScale";
        FillPostParameters(postProcessing);
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
        this.quadVertices[0].Position = new Vector3(-10000, 0, -10000);
        this.quadVertices[0].Normal = quadNormal;
        this.quadVertices[0].TextureCoordinate = new Vector2(0, 0);
        // Top right
        this.quadVertices[1].Position = new Vector3(10000, 0, -10000);
        this.quadVertices[1].Normal = quadNormal;
        this.quadVertices[1].TextureCoordinate = new Vector2(3, 0);
        // Bottom left
        this.quadVertices[2].Position = new Vector3(-10000, 0, 10000);
        this.quadVertices[2].Normal = quadNormal;
        this.quadVertices[2].TextureCoordinate = new Vector2(0, 3);
        // Bottom right
        this.quadVertices[3].Position = new Vector3(10000, 0, 10000);
        this.quadVertices[3].Normal = quadNormal;
        this.quadVertices[3].TextureCoordinate = new Vector2(3, 3);


        this.quadIndices = new short[] { 0, 1, 2, 1, 2, 3 };
        this.quadTransform = Matrix.CreateScale(scale);
    }

    private VertexPositionNormalTexture[] setUpBoxes(Vector3 Translation)
    {
        float root2 = (float)Math.Sqrt(2);

        VertexPositionNormalTexture[] boxVertices = new VertexPositionNormalTexture[8];

        // Ceiling
        // TopLeft
        boxVertices[0].Position = new Vector3(-10, 100, -10) + Translation;
        boxVertices[0].Normal = new Vector3(-root2, root2, -root2);
        // TopRight
        boxVertices[1].Position = new Vector3(10, 100, -10) + Translation;
        boxVertices[1].Normal = new Vector3(root2, root2, -root2);
        // BottomLeft
        boxVertices[2].Position = new Vector3(-10, 100, 10) + Translation;
        boxVertices[2].Normal = new Vector3(-root2, root2, root2);
        // BottomRight
        boxVertices[3].Position = new Vector3(10, 100, 10) + Translation;
        boxVertices[3].Normal = new Vector3(root2, root2, root2);

        // Floor
        // TopLeft
        boxVertices[4].Position = new Vector3(-10, -10, -10) + Translation;
        boxVertices[4].Normal = new Vector3(-root2, -root2, -root2);
        // TopRight
        boxVertices[5].Position = new Vector3(10, -10, -10) + Translation;
        boxVertices[5].Normal = new Vector3(root2, -root2, -root2);
        // BottomLeft
        boxVertices[6].Position = new Vector3(-10, -10, 10) + Translation;
        boxVertices[6].Normal = new Vector3(-root2, -root2, root2);
        // BottomRight
        boxVertices[7].Position = new Vector3(10, -10, 10) + Translation;
        boxVertices[7].Normal = new Vector3(root2, -root2, root2);


        this.boxIndices = new short[] 
        { 
            0, 1, 2, 1, 2, 3,   // Top
            4, 5, 6, 5, 6, 7,   // Bottom
            2, 3, 6, 3, 6, 7,   // Front
            0, 1, 4, 1, 4, 5,   // Back
            0, 2, 4, 2, 4, 6,   // Left
            1, 3, 5, 3, 5, 7,   // Right
        };

        // Return the finished box.
        return boxVertices;
    }

    protected void CalculateOffsetAndWeight(int width, int height)
    {
        // Offsets
        offsetHor = new Vector2[1 + (2 * radius)];
        offsetVer = new Vector2[1 + (2 * radius)];

        float horOffset = 1.0f / width;
        float verOffset = 1.0f / height;

        int position = 0;

        for (int i = radius; i >= -radius; i--)
        {
            position = radius + i;
            offsetHor[position] = new Vector2(horOffset * i, 0.0f);
            offsetVer[position] = new Vector2(0.0f, verOffset * i);
        }

        // Weight
        weights = new float[1 + (radius * 2)];
        float sigma = radius / amount;

        float SquareSigmaTwo = sigma * sigma * 2.0f;
        float RootSigma = (float)Math.Sqrt(Math.PI * SquareSigmaTwo);
        float result = 0.0f;
        float distance;
        int index;

        for (int i = radius; i >= -radius; i--)
        {
            index = i + radius;
            distance = i * i;
            weights[index] = (float)Math.Exp(-distance / SquareSigmaTwo) / RootSigma;
            result += weights[index];
        }

        for (int i = 0; i < weights.Length; i++)
            weights[i] = weights[i] / result;
    }
}
