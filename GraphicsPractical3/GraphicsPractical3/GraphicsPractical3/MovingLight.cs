using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class MovingLight
{
    public Vector3 Origin, Position;
    public float Radius, Offset;
    public Vector4 Color;
    double angle;

    public MovingLight(Vector3 Origin, float Radius, float Offset, Vector4 Color)
    {
        this.Origin = Origin;
        this.Radius = Radius;
        this.Color = Color;
        this.Position = Origin + new Vector3(Radius, 0, 0);
        this.Offset = Offset;
    }

    public void Update(GameTime gT)
    {
        angle = (float)gT.TotalGameTime.Milliseconds / 1000;
        this.Position = Origin + new Vector3((float)Math.Cos((angle + Offset) * 2 * MathHelper.Pi), 0, (float)Math.Sin((angle + Offset) * 2 * MathHelper.Pi)) * Radius;
    }
}
