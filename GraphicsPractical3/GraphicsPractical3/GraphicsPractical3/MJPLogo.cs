using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class MJPLogo
{

    #region Variables
    VertexPositionNormalTexture[] listVertices;
    short[] listIndices;

    #endregion

    public MJPLogo()
    {
        // Add all the vertices.
        listVertices = setupVertices("Content/Models/MJPTrianglesTop.txt");

        // Add all the indices.
        listIndices = setupIndicesFlat("Content/Models/MJPIndices.txt");
    }

    // Creates all the vertices from the lines in the given txt file.
    private VertexPositionNormalTexture[] setupVertices(string inputPath)
    {
        // Create a list of vertices, we need a dynamic list, because we do not know how many vertices we need the create.
        List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();

        // Read the input from the text file, creating vertices from the lines.
        StreamReader sR = new StreamReader(inputPath);

        // Read the first line of from the file.
        string input = sR.ReadLine();

        // Keep reading until we have reached the end of the file.
        while (input != null)
        {
            // Split the input into relevant parts.
            string[] split = input.Split(' ');

            // Create the vertex from the input.
            VertexPositionNormalTexture vertex = new VertexPositionNormalTexture();
            vertex.Position = new Vector3(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
            vertex.Normal = Vector3.Normalize(new Vector3(int.Parse(split[3]), int.Parse(split[4]), int.Parse(split[5])));

            // Add the vertex to the list.
            vertices.Add(vertex);

            // Read the next line.
            input = sR.ReadLine();
        }

        // Read the file again for the bottom half of the structure.

        // Read the input from the text file, creating vertices from the lines.
        sR = new StreamReader(inputPath);

        // Read the first line of from the file.
        input = sR.ReadLine();

        // Keep reading until we have reached the end of the file.
        while ( !string.IsNullOrEmpty(input) )
        {
            // Split the input into relevant parts.
            string[] split = input.Split(' ');

            // Create the vertex from the input.
            VertexPositionNormalTexture vertex = new VertexPositionNormalTexture();
            vertex.Position = new Vector3(int.Parse(split[0]), -int.Parse(split[1]), int.Parse(split[2]));
            vertex.Normal = Vector3.Normalize(new Vector3(int.Parse(split[3]), -int.Parse(split[4]), int.Parse(split[5])));

            // Add the vertex to the list.
            vertices.Add(vertex);

            // Read the next line.
            input = sR.ReadLine();
        }

        // Return the finished list as an array.
        return vertices.ToArray();
    }

    private short[] setupIndicesFlat(string inputPath)
    {
        // Prep.
        List<short> result = new List<short>();

        StreamReader sR = new StreamReader(inputPath);

        // Read the single line of input.
        string input = sR.ReadLine();

        // Split the single line of input into usable shorts for the indices.
        string[] inputSplit = input.Split('.');

        // Add all the indices to the list.
        for (int i = 0; i < inputSplit.Length; i++)
        {
            result.Add(short.Parse(inputSplit[i]));
        }

        // Return the final list in an array.
        return result.ToArray();
    }

    public void Draw(GraphicsDevice GraphicsDevice)
    {
        // Draw the bottom.
        GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, listVertices,
            0, listVertices.Length, listIndices, 0, this.listIndices.Length / 3);
    }
}
