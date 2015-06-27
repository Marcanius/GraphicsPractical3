using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

partial class Game1
{
    KeyboardState curr, prev;
    bool updateLights = true;

    protected override void Update(GameTime gameTime)
    {
        prev = curr;
        curr = Keyboard.GetState();

        // Allows the game to exit.
        if (curr.IsKeyDown(Keys.Escape))
            this.Exit();

        // Update the position of the camera.
        this.camera.Update(gameTime);

        // Update the positions and colors of the lights.
        if (updateLights)
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].Update(gameTime);

                LightPositions[i] = lights[i].Position;
                LightColors[i] = lights[i].Color;

                //LightPositions[0] = camera.Eye;
            }

        // Update the window title.
        this.Window.Title = "XNA Renderer | FPS: " + this.frameRateCounter.FrameRate;

        // Change parameters based on input
        // Lighting
        // Whether lights move.
        if (Click(Keys.R))
            updateLights = !updateLights;

        // Whether lights get drawn.
        if (Click(Keys.F))
            drawLights = !drawLights;

        // Cell shading.
        if (Click(Keys.T))
            cellShading = !cellShading;

        // Postprocessing
        // GreyScaling.
        if (Click(Keys.U))
            greyScale = !greyScale;

        // Gaussian Blurring.
        if (Click(Keys.J))
            gaussian = !gaussian;

        // Gaussian Radius.
        if (Hold(Keys.Y) && Click(Keys.Up))
        {
            radius = (int)MathHelper.Clamp(radius + 2, 1, 11);
            CalculateOffsetAndWeight(400, 300);
        }
        if (Hold(Keys.Y) && Click(Keys.Down))
        {
            radius = (int)MathHelper.Clamp(radius - 2, 1, 11);
            CalculateOffsetAndWeight(400, 300);
        }

        // Gaussian Amount.
        if (Hold(Keys.H) && Click(Keys.Up))
        {
            amount = MathHelper.Clamp(amount + 0.2f, 0.2f, 5f);
            CalculateOffsetAndWeight(400, 300);
        }
        if (Hold(Keys.H) && Click(Keys.Down))
        {
            amount = MathHelper.Clamp(amount - 0.2f, 0.2f, 5f);
            CalculateOffsetAndWeight(400, 300);
        }

        // Bloom.
        if (Click(Keys.I))
            bloom = !bloom;

        // God Rays.
        if (Click(Keys.K))
            godRays = !godRays;

        // HDR.
        if (Click(Keys.O))
            HDR = !HDR;

        hudMessage = "Camera Position: " + camera.Eye +
                     "\nCamera Focus: " + camera.Focus;

        if (gaussian)
            hudMessage += "\nRadius: " + radius +
                          "\nAmount: " + amount;

        base.Update(gameTime);
    }

    private bool Click(Keys KeyToCheck)
    {
        if (curr.IsKeyDown(KeyToCheck) && prev.IsKeyUp(KeyToCheck))
            return true;
        return false;
    }

    public bool Hold(Keys KeyToCheck)
    {
        if (curr.IsKeyDown(KeyToCheck))
            return true;
        return false;
    }
}
