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
        // Gets the keyboard states.
        prev = curr;
        curr = Keyboard.GetState();

        // Allows the game to exit.
        if (Hold(Keys.Escape))
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
            }

        // Update the window title.
        this.Window.Title = "XNA Renderer | FPS: " + this.frameRateCounter.FrameRate;

        // Change parameters based on input.
        // Whether lights move.
        if (Click(Keys.R))
            updateLights = !updateLights;
        
        // Cell shading.
        if (Click(Keys.T))
            cellShading = !cellShading;

        // GreyScaling.
        if (Click(Keys.U))
            greyScale = !greyScale;

        #region Gaussian Blurring

        // Whether we blur at all.
        if (Click(Keys.J))
        {
            gaussian = !gaussian;
            CalculateOffsetAndWeight(400, 300);
        }

        // The Radius.
        if (Hold(Keys.Y) && Click(Keys.NumPad8))
        {
            radius = (int)MathHelper.Clamp(radius + 2, 1, 11);
            CalculateOffsetAndWeight(400, 300);
        }
        if (Hold(Keys.Y) && Click(Keys.NumPad2))
        {
            radius = (int)MathHelper.Clamp(radius - 2, 1, 11);
            CalculateOffsetAndWeight(400, 300);
        }

        // The Amount.
        if (Hold(Keys.H) && Click(Keys.NumPad8))
        {
            amount = MathHelper.Clamp(amount + 0.2f, 0.2f, 5f);
            CalculateOffsetAndWeight(400, 300);
        }
        if (Hold(Keys.H) && Click(Keys.NumPad2))
        {
            amount = MathHelper.Clamp(amount - 0.2f, 0.2f, 5f);
            CalculateOffsetAndWeight(400, 300);
        }

        #endregion

        // Bloom.
        if (Click(Keys.I))
            bloom = !bloom;

        #region HUD

        hudMessage = "Hold [LCTRL] for Controls";
            //+ "\nCamera Position: " + camera.Eye
            //+ "\nCamera Focus: " + camera.Focus;

        // Displays our super intuitive control scheme.
        if (Hold(Keys.LeftControl))
        {
            hudMessage = "\nCamera Controls: [WASD] for movement along the XZ-Plane\n[LSHIFT + SPACE] for movement along the Y-axis";
            hudMessage += "\nPress [R] to toggle the movement of the lights";
            hudMessage += "\nPress [T] to toggle Cell Shading";
            hudMessage += "\nPress [U] to toggle Color Filtering";
            hudMessage += "\nPress [J] to toggle Gaussian Blurring";
            hudMessage += "\nPress [I] to toggle Bloom";
        }

        // Displays some helpful information in case of bloom, namely, that this is a case of bloom.
        if (bloom)
            hudMessage += "\nBloom: On";

        // Displays some helpful information in case of gaussian blurring
        if (gaussian)
            hudMessage += "\n-------------------------------------\nRadius: " + radius + "\n    Hold [Y] and press [NUM8] or [NUM2] to change the Radius"
                + "\nAmount: " + amount + "\n    Hold [H] and press [NUM8] or [NUM2] to change the Amount";

        #endregion

        base.Update(gameTime);
    }

    // Returns whether a key is pressed down this update, but not the previous update.
    private bool Click(Keys KeyToCheck)
    {
        if (curr.IsKeyDown(KeyToCheck) && prev.IsKeyUp(KeyToCheck))
            return true;
        return false;
    }

    // Returns whether a key is being held down this update.
    public bool Hold(Keys KeyToCheck)
    {
        if (curr.IsKeyDown(KeyToCheck))
            return true;
        return false;
    }
}
