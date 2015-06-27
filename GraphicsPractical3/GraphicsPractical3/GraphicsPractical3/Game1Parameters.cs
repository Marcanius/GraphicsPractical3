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
    string currentTechnique = "ColorFilter";
    bool greyScale, gaussian, bloom, godRays, HDR;
    float gamma = 1.0f;
    Vector3 SunPosition = new Vector3(0, 0, 0);
    Vector2 SunScreenPos;

    private void FillLightingParameters(Effect effect)
    {
        // View, Projection
        this.camera.SetEffectParameters(effect);
        // World
        effect.Parameters["World"].SetValue(World);
        // WorldIT
        effect.Parameters["WorldIT"].SetValue(Matrix.Transpose(Matrix.Invert(World)));
        // The arrays of lights.
        effect.Parameters["LightPositions"].SetValue(LightPositions);
        effect.Parameters["LightColors"].SetValue(LightColors);
        effect.Parameters["AmbientColor"].SetValue(Color.DarkBlue.ToVector4());
        effect.Parameters["AmbientIntensity"].SetValue(0.2f);
        effect.Parameters["DiffuseIntensity"].SetValue(1.0f);
        effect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4());
        effect.Parameters["SpecularIntensity"].SetValue(2.0f);
        effect.Parameters["SpecularPower"].SetValue(25.0f);

        // Whether we use cell shading.
        effect.Parameters["cellShading"].SetValue(cellShading);
    }

    private void FillPostParameters(Effect effect)
    {
        // Current Technique
        effect.CurrentTechnique = effect.Techniques[currentTechnique];

        // Gamma Correction.
        effect.Parameters["gamma"].SetValue(this.gamma);
        
        // GodRays
        effect.Parameters["SunScreenPos"].SetValue(this.SunScreenPos);
        effect.Parameters["Density"].SetValue(0.8f);
        effect.Parameters["Weight"].SetValue(5f);
        effect.Parameters["Exposure"].SetValue(.01f);
        effect.Parameters["Decay"].SetValue(0.9f);

        // Gaussian
        effect.Parameters["weight"].SetValue(weights);
        effect.Parameters["offsetHor"].SetValue(offsetHor);
        effect.Parameters["offsetVer"].SetValue(offsetVer);
    }
}
