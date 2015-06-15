﻿using System;
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

        // Bloom.
        if (Click(Keys.I))
            bloom = !bloom;

        // God Rays.
        if (Click(Keys.K))
            godRays = !godRays;

        // HDR.
        if (Click(Keys.O))
            HDR = !HDR;

        base.Update(gameTime);
    }

    private bool Click(Keys KeyToCheck)
    {
        if (curr.IsKeyDown(KeyToCheck) && prev.IsKeyUp(KeyToCheck))
            return true;
        return false;
    }
}
