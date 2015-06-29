using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

public partial class Game1 : Game
{
    // Effect Parameters
    Matrix World;
    Vector3[] LightPositions;
    Vector4[] LightColors;
    bool cellShading;

    // PostProcessing Parameters.
    bool greyScale, gaussian, bloom;
    float brightnessThreshold = 0.3f;

    private void FillLightingParameters(Effect effect)
    {
        // View, Projection.
        this.camera.SetEffectParameters(effect);

        // World
        effect.Parameters["World"].SetValue(World);
        effect.Parameters["WorldIT"].SetValue(Matrix.Transpose(Matrix.Invert(World)));

        // The arrays of lights.
        effect.Parameters["LightPositions"].SetValue(LightPositions);
        effect.Parameters["LightColors"].SetValue(LightColors);

        // Ambient Lighting.
        effect.Parameters["AmbientColor"].SetValue(Color.DarkBlue.ToVector4());
        effect.Parameters["AmbientIntensity"].SetValue(0.2f);

        // Diffuse Lighting.
        effect.Parameters["DiffuseIntensity"].SetValue(1.0f);

        // Specular lighting.
        effect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4());
        effect.Parameters["SpecularIntensity"].SetValue(2.0f);
        effect.Parameters["SpecularPower"].SetValue(25.0f);

        // Whether we use cell shading.
        effect.Parameters["cellShading"].SetValue(cellShading);
    }
}
