using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// This class is used to control the camera and calculate the corresponding view and projection matrices.
/// </summary>
class Camera
{
    // Camera properties.
    private Vector3 up;
    private Vector3 eye;
    private Vector3 focus;

    // Calculated matrices.
    private Matrix viewMatrix;
    private Matrix projectionMatrix;

    // Variables related to camera movement.
    float deltaAngleH, deltaAngleV;
    float angleH = (float)(-MathHelper.PiOver2), angleV = -0.3f;
    float turnSpeed = 2, moveSpeed = 15;


    public Camera(Vector3 camEye, Vector3 camFocus, Vector3 camUp, float aspectRatio = 4.0f / 3.0f)
    {
        this.up = camUp;
        this.eye = camEye;
        this.focus = camFocus;

        // Create matrices.
        this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1f, 300.0f);
        this.updateViewMatrix();
        this.UpdateFocus();
    }

    #region Movement
    public void Update(GameTime gT)
    {
        // The time since the last update.
        float timeStep = (float)gT.ElapsedGameTime.TotalSeconds;
        deltaAngleH = 0;
        deltaAngleV = 0;

        // Checking for keyboard input and applying its interaction.
        KeyboardState kbState = Keyboard.GetState();

        // Check to see if the Left Arrow Key is pressed.
        if (kbState.IsKeyDown(Keys.Left))
            deltaAngleH += -turnSpeed * timeStep;
        // Check to see if the Right Arrow Key is pressed.
        if (kbState.IsKeyDown(Keys.Right))
            deltaAngleH += turnSpeed * timeStep;
        // Check to see of the Up Arrow Key is pressed.
        if (kbState.IsKeyDown(Keys.Up))
            deltaAngleV += -turnSpeed * timeStep;
        // Check to see if the Right Arrow Key is pressed.
        if (kbState.IsKeyDown(Keys.Down))
            deltaAngleV += turnSpeed * timeStep;

        // Check to see if the matrix needs to be adjusted with a new angle.
        if (deltaAngleH != 0 || deltaAngleV != 0)
        {
            angleH += deltaAngleH;
            angleV = MathHelper.Clamp(angleV - deltaAngleV, -MathHelper.PiOver2 + 0.01F, MathHelper.PiOver2 - 0.01F);

            UpdateFocus();
        }

        // Movements in the X,Z-plane.
        if (kbState.IsKeyDown(Keys.W))
            moveCamera(timeStep, new Vector3((float)Math.Cos(angleH), 0, (float)Math.Sin(angleH)));
        if (kbState.IsKeyDown(Keys.A))
            moveCamera(timeStep, new Vector3((float)Math.Sin(angleH), 0, -(float)Math.Cos(angleH)));
        if (kbState.IsKeyDown(Keys.S))
            moveCamera(timeStep, new Vector3(-(float)Math.Cos(angleH), 0, -(float)Math.Sin(angleH)));
        if (kbState.IsKeyDown(Keys.D))
            moveCamera(timeStep, new Vector3(-(float)Math.Sin(angleH), 0, (float)Math.Cos(angleH)));

        // Movement along the Y-axis.
        if (kbState.IsKeyDown(Keys.Space))
            moveCamera(timeStep, new Vector3(0, 1, 0));
        if (kbState.IsKeyDown(Keys.LeftShift))
            moveCamera(timeStep, new Vector3(0, -1, 0));
    }

    private void moveCamera(float TimeStep, Vector3 Direction)
    {
        this.Eye = this.Eye + Direction * moveSpeed * TimeStep;
        this.Focus = this.Focus + Direction * moveSpeed * TimeStep;
    }

    /// <summary>
    /// Updates the focus of the camera, using both the horizontal and the vertical angle to place the focus at the correct position in front of the camera.
    /// </summary>
    private void UpdateFocus()
    {
        Vector3 relativeFocus = new Vector3((float)Math.Cos(angleH), 0, (float)Math.Sin(angleH));
        relativeFocus = new Vector3(relativeFocus.X * (float)Math.Cos(angleV), (float)Math.Sin(angleV), relativeFocus.Z * (float)Math.Cos(angleV));

        Focus = Eye + relativeFocus;
    }
    #endregion

    /// <summary>
    /// Recalculates the view matrix from the up, eye and focus vectors.
    /// </summary>
    private void updateViewMatrix()
    {
        this.viewMatrix = Matrix.CreateLookAt(eye, focus, up);
    }

    /// <summary>
    /// Current position of the camera.
    /// </summary>
    public Vector3 Eye
    {
        get { return this.eye; }
        set { this.eye = value; this.updateViewMatrix(); }
    }

    /// <summary>
    /// The point the camera is looking at.
    /// </summary>
    public Vector3 Focus
    {
        get { return this.focus; }
        set { this.focus = value; this.updateViewMatrix(); }
    }

    /// <summary>
    /// The calculated view matrix.
    /// </summary>
    public Matrix ViewMatrix
    {
        get { return this.viewMatrix; }
    }

    /// <summary>
    /// The calculated projection matrix.
    /// </summary>
    public Matrix ProjectionMatrix
    {
        get { return this.projectionMatrix; }
    }

    /// <summary>
    /// Sets the view and projection matrices in the effect and also the cameraposition if the CameraPosition global is found.
    /// </summary>
    /// <param name="effect">The effect to set the parameters of.</param>
    public void SetEffectParameters(Effect effect)
    {
        // Set the right matrices in the effect.
        effect.Parameters["View"].SetValue(this.ViewMatrix);
        effect.Parameters["Projection"].SetValue(this.ProjectionMatrix);

        // If the shader has a global called "CameraPosition", we set it to the right Eye position of the camera.
        EffectParameter cameraPosition = effect.Parameters["CameraPosition"];
        if (cameraPosition != null)
            effect.Parameters["CameraPosition"].SetValue(this.Eye);
    }
}
