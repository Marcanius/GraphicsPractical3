using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class MovingLight
{
    public Vector3 Origin, Position;
    public float Radius;
    public Vector4 Color;
    double angle;

    public MovingLight(Vector3 Origin, float Radius, Vector4 Color)
    {
        this.Origin = Origin;
        this.Radius = Radius;
        this.Color = Color;
    }

    public void Update(GameTime gT)
    {
        angle = (float)gT.TotalGameTime.Milliseconds / 1000;
        this.Position = Origin + new Vector3((float)Math.Cos(angle * 2 * MathHelper.Pi), 0, (float)Math.Sin(angle * 2 * MathHelper.Pi)) * Radius;
    }
}
