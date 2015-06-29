using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class MovingLight
{
    public Vector3 Origin, Position;
    public float Radius, Offset, Speed;
    public Vector4 Color;
    private double angle;

    public MovingLight(Vector3 Origin, float Radius, float Offset, float Speed, Vector4 Color)
    {
        this.Origin = Origin;
        this.Radius = Radius;
        this.Color = Color;
        this.Position = Origin + new Vector3(Radius, 0, 0);
        this.Offset = Offset;
        this.Speed = Speed;
    }

    public void Update(GameTime gT)
    {
        angle += Speed * (float)gT.ElapsedGameTime.TotalMilliseconds / 1000;
        this.Position = Origin + new Vector3(
            (float)Math.Cos((angle + Offset) * 2 * MathHelper.Pi), 
            (float)Math.Cos(angle * 5 + Offset), 
            (float)Math.Sin((angle + Offset) * 2 * MathHelper.Pi)) * Radius;
    }
}
