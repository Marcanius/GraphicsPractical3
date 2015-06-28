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
    List<VertexPositionNormalTexture[]> listVertices;
    List<short[]> listIndices;

    #endregion

    public MJPLogo()
    {
        listVertices = new List<VertexPositionNormalTexture[]>();
        listIndices = new List<short[]>();

        // Vertices.
        // The bottom.
        listVertices.Add(readTxt("Content/Models/MJPTrianglesBot.txt"));

        // The bottom diagonals.
        // The sides.

        // The top diagonals.

        // The top.
        listVertices.Add(readTxt("Content/Models/MJPTrianglesTop.txt"));


        // Bottom indices.
        listIndices.Add(setupIndicesFlat());
        // 
    }

    private VertexPositionNormalTexture[] readTxt(string inputPath)
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

        // Read the file again for the bottom half.

        // Read the input from the text file, creating vertices from the lines.
        sR = new StreamReader(inputPath);

        // Read the first line of from the file.
        input = sR.ReadLine();

        // Keep reading until we have reached the end of the file.
        while (input != null)
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

    private VertexPositionNormalTexture[] setupoOne(int Y)
    {
        VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[53];

        // Position
        vertices[00].Position = new Vector3(01, Y, 01);
        vertices[01].Position = new Vector3(02, Y, 01);
        vertices[02].Position = new Vector3(03, Y, 02);
        vertices[03].Position = new Vector3(03, Y, 04);
        vertices[04].Position = new Vector3(04, Y, 05);
        vertices[05].Position = new Vector3(07, Y, 05);
        vertices[06].Position = new Vector3(08, Y, 06);
        vertices[07].Position = new Vector3(13, Y, 06);
        vertices[08].Position = new Vector3(14, Y, 05);
        vertices[09].Position = new Vector3(16, Y, 05);

        vertices[10].Position = new Vector3(17, Y, 04);
        vertices[11].Position = new Vector3(17, Y, 03);
        vertices[12].Position = new Vector3(19, Y, 01);
        vertices[13].Position = new Vector3(26, Y, 01);
        vertices[14].Position = new Vector3(27, Y, 02);
        vertices[15].Position = new Vector3(27, Y, 03);
        vertices[16].Position = new Vector3(20, Y, 03);
        vertices[17].Position = new Vector3(19, Y, 04);
        vertices[18].Position = new Vector3(19, Y, 11);
        vertices[19].Position = new Vector3(21, Y, 13);

        vertices[20].Position = new Vector3(27, Y, 13);
        vertices[21].Position = new Vector3(27, Y, 14);
        vertices[22].Position = new Vector3(26, Y, 15);
        vertices[23].Position = new Vector3(25, Y, 15);
        vertices[24].Position = new Vector3(24, Y, 14);
        vertices[25].Position = new Vector3(19, Y, 14);
        vertices[26].Position = new Vector3(18, Y, 15);
        vertices[27].Position = new Vector3(18, Y, 11);
        vertices[28].Position = new Vector3(17, Y, 10);
        vertices[29].Position = new Vector3(17, Y, 08);

        vertices[30].Position = new Vector3(16, Y, 07);
        vertices[31].Position = new Vector3(12, Y, 07);
        vertices[32].Position = new Vector3(11, Y, 08);
        vertices[33].Position = new Vector3(11, Y, 10);
        vertices[34].Position = new Vector3(10, Y, 11);
        vertices[35].Position = new Vector3(09, Y, 10);
        vertices[36].Position = new Vector3(09, Y, 08);
        vertices[37].Position = new Vector3(08, Y, 07);
        vertices[38].Position = new Vector3(04, Y, 07);
        vertices[39].Position = new Vector3(03, Y, 08);

        vertices[40].Position = new Vector3(03, Y, 14);
        vertices[41].Position = new Vector3(02, Y, 15);
        vertices[42].Position = new Vector3(02, Y, 21);
        vertices[43].Position = new Vector3(03, Y, 22);
        vertices[44].Position = new Vector3(02, Y, 23);
        vertices[45].Position = new Vector3(01, Y, 23);
        vertices[46].Position = new Vector3(00, Y, 22);
        vertices[47].Position = new Vector3(00, Y, 17);
        vertices[48].Position = new Vector3(01, Y, 16);
        vertices[49].Position = new Vector3(01, Y, 09);

        vertices[50].Position = new Vector3(02, Y, 08);
        vertices[51].Position = new Vector3(02, Y, 04);
        vertices[52].Position = new Vector3(01, Y, 03);

        // Normals.
        for (short i = 0; i < 53; i++)
            vertices[i].Normal = new Vector3(0, Math.Sign(Y), 0);

        return vertices;
    }

    private VertexPositionNormalTexture[] setupTwo(int Y)
    {
        VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[166];

        // Positions.
        vertices[00].Position = new Vector3(00, Y, 00);
        vertices[01].Position = new Vector3(03, Y, 00);
        vertices[02].Position = new Vector3(04, Y, 01);
        vertices[03].Position = new Vector3(04, Y, 03);
        vertices[04].Position = new Vector3(05, Y, 04);
        vertices[05].Position = new Vector3(08, Y, 04);
        vertices[06].Position = new Vector3(09, Y, 05);
        vertices[07].Position = new Vector3(12, Y, 05);
        vertices[08].Position = new Vector3(13, Y, 04);
        vertices[09].Position = new Vector3(15, Y, 04);

        vertices[10].Position = new Vector3(16, Y, 03);
        vertices[11].Position = new Vector3(16, Y, 02);
        vertices[12].Position = new Vector3(18, Y, 00);
        vertices[13].Position = new Vector3(27, Y, 00);
        vertices[14].Position = new Vector3(28, Y, 01);
        vertices[15].Position = new Vector3(28, Y, 02);
        vertices[16].Position = new Vector3(30, Y, 04);
        vertices[17].Position = new Vector3(31, Y, 04);
        vertices[18].Position = new Vector3(32, Y, 05);
        vertices[19].Position = new Vector3(32, Y, 11);

        vertices[20].Position = new Vector3(31, Y, 12);
        vertices[21].Position = new Vector3(30, Y, 12);
        vertices[22].Position = new Vector3(28, Y, 14);
        vertices[23].Position = new Vector3(28, Y, 15);
        vertices[24].Position = new Vector3(27, Y, 16);
        vertices[25].Position = new Vector3(24, Y, 16);
        vertices[26].Position = new Vector3(23, Y, 15);
        vertices[27].Position = new Vector3(20, Y, 15);
        vertices[28].Position = new Vector3(19, Y, 16);
        vertices[29].Position = new Vector3(19, Y, 19);

        vertices[30].Position = new Vector3(20, Y, 20);
        vertices[31].Position = new Vector3(20, Y, 23);
        vertices[32].Position = new Vector3(18, Y, 25);
        vertices[33].Position = new Vector3(17, Y, 25);
        vertices[34].Position = new Vector3(16, Y, 26);
        vertices[35].Position = new Vector3(16, Y, 27);
        vertices[36].Position = new Vector3(15, Y, 28);
        vertices[37].Position = new Vector3(08, Y, 28);
        vertices[38].Position = new Vector3(07, Y, 27);
        vertices[39].Position = new Vector3(07, Y, 25);

        vertices[40].Position = new Vector3(09, Y, 23);
        vertices[41].Position = new Vector3(11, Y, 23);
        vertices[42].Position = new Vector3(12, Y, 24);
        vertices[43].Position = new Vector3(14, Y, 24);
        vertices[44].Position = new Vector3(16, Y, 22);
        vertices[45].Position = new Vector3(16, Y, 16);
        vertices[46].Position = new Vector3(17, Y, 15);
        vertices[47].Position = new Vector3(17, Y, 12);
        vertices[48].Position = new Vector3(16, Y, 11);
        vertices[49].Position = new Vector3(16, Y, 09);

        vertices[50].Position = new Vector3(15, Y, 08);
        vertices[51].Position = new Vector3(13, Y, 08);
        vertices[52].Position = new Vector3(12, Y, 09);
        vertices[53].Position = new Vector3(12, Y, 11);
        vertices[54].Position = new Vector3(11, Y, 12);
        vertices[55].Position = new Vector3(09, Y, 12);
        vertices[56].Position = new Vector3(08, Y, 11);
        vertices[57].Position = new Vector3(08, Y, 09);
        vertices[58].Position = new Vector3(07, Y, 08);
        vertices[59].Position = new Vector3(05, Y, 08);

        vertices[60].Position = new Vector3(04, Y, 09);
        vertices[61].Position = new Vector3(04, Y, 15);
        vertices[62].Position = new Vector3(03, Y, 16);
        vertices[63].Position = new Vector3(03, Y, 20);
        vertices[64].Position = new Vector3(04, Y, 21);
        vertices[65].Position = new Vector3(04, Y, 23);
        vertices[66].Position = new Vector3(03, Y, 24);
        vertices[67].Position = new Vector3(00, Y, 24);
        vertices[68].Position = new Vector3(-1, Y, 23);
        vertices[69].Position = new Vector3(-1, Y, 16);

        vertices[70].Position = new Vector3(00, Y, 15);
        vertices[71].Position = new Vector3(00, Y, 08);
        vertices[72].Position = new Vector3(01, Y, 07);
        vertices[73].Position = new Vector3(01, Y, 05);
        vertices[74].Position = new Vector3(00, Y, 04);

        vertices[75].Position = new Vector3(21, Y, 04);
        vertices[76].Position = new Vector3(26, Y, 04);
        vertices[77].Position = new Vector3(28, Y, 06);
        vertices[78].Position = new Vector3(28, Y, 10);
        vertices[79].Position = new Vector3(26, Y, 12);
        vertices[80].Position = new Vector3(22, Y, 12);
        vertices[81].Position = new Vector3(20, Y, 10);
        vertices[82].Position = new Vector3(20, Y, 05);

        // ----

        vertices[83].Position = new Vector3(00, -Y, 00);
        vertices[84].Position = new Vector3(03, -Y, 00);
        vertices[85].Position = new Vector3(04, -Y, 01);
        vertices[86].Position = new Vector3(04, -Y, 03);
        vertices[87].Position = new Vector3(05, -Y, 04);
        vertices[88].Position = new Vector3(08, -Y, 04);
        vertices[89].Position = new Vector3(09, -Y, 05);

        vertices[90].Position = new Vector3(12, -Y, 05);
        vertices[91].Position = new Vector3(13, -Y, 04);
        vertices[92].Position = new Vector3(15, -Y, 04);
        vertices[93].Position = new Vector3(16, -Y, 03);
        vertices[94].Position = new Vector3(16, -Y, 02);
        vertices[95].Position = new Vector3(18, -Y, 00);
        vertices[96].Position = new Vector3(27, -Y, 00);
        vertices[97].Position = new Vector3(28, -Y, 01);
        vertices[98].Position = new Vector3(28, -Y, 02);
        vertices[99].Position = new Vector3(30, -Y, 04);

        vertices[100].Position = new Vector3(31, -Y, 04);
        vertices[101].Position = new Vector3(32, -Y, 05);
        vertices[102].Position = new Vector3(32, -Y, 11);
        vertices[103].Position = new Vector3(31, -Y, 12);
        vertices[104].Position = new Vector3(30, -Y, 12);
        vertices[105].Position = new Vector3(28, -Y, 14);
        vertices[106].Position = new Vector3(28, -Y, 15);
        vertices[107].Position = new Vector3(27, -Y, 16);
        vertices[108].Position = new Vector3(24, -Y, 16);
        vertices[109].Position = new Vector3(23, -Y, 15);

        vertices[110].Position = new Vector3(20, -Y, 15);
        vertices[111].Position = new Vector3(19, -Y, 16);
        vertices[112].Position = new Vector3(19, -Y, 19);
        vertices[113].Position = new Vector3(20, -Y, 20);
        vertices[114].Position = new Vector3(20, -Y, 23);
        vertices[115].Position = new Vector3(18, -Y, 25);
        vertices[116].Position = new Vector3(17, -Y, 25);
        vertices[117].Position = new Vector3(16, -Y, 26);
        vertices[118].Position = new Vector3(16, -Y, 27);
        vertices[119].Position = new Vector3(15, -Y, 28);

        vertices[120].Position = new Vector3(08, -Y, 28);
        vertices[121].Position = new Vector3(07, -Y, 27);
        vertices[122].Position = new Vector3(07, -Y, 25);
        vertices[123].Position = new Vector3(09, -Y, 23);
        vertices[124].Position = new Vector3(11, -Y, 23);
        vertices[125].Position = new Vector3(12, -Y, 24);
        vertices[126].Position = new Vector3(14, -Y, 24);
        vertices[127].Position = new Vector3(16, -Y, 22);
        vertices[128].Position = new Vector3(16, -Y, 16);
        vertices[129].Position = new Vector3(17, -Y, 15);

        vertices[130].Position = new Vector3(17, -Y, 12);
        vertices[131].Position = new Vector3(16, -Y, 11);
        vertices[132].Position = new Vector3(16, -Y, 09);
        vertices[133].Position = new Vector3(15, -Y, 08);
        vertices[134].Position = new Vector3(13, -Y, 08);
        vertices[135].Position = new Vector3(12, -Y, 09);
        vertices[136].Position = new Vector3(12, -Y, 11);
        vertices[137].Position = new Vector3(11, -Y, 12);
        vertices[138].Position = new Vector3(09, -Y, 12);
        vertices[139].Position = new Vector3(08, -Y, 11);

        vertices[140].Position = new Vector3(08, -Y, 09);
        vertices[141].Position = new Vector3(07, -Y, 08);
        vertices[142].Position = new Vector3(05, -Y, 08);
        vertices[143].Position = new Vector3(04, -Y, 09);
        vertices[144].Position = new Vector3(04, -Y, 15);
        vertices[145].Position = new Vector3(03, -Y, 16);
        vertices[146].Position = new Vector3(03, -Y, 20);
        vertices[147].Position = new Vector3(04, -Y, 21);
        vertices[148].Position = new Vector3(04, -Y, 23);
        vertices[149].Position = new Vector3(03, -Y, 24);

        vertices[150].Position = new Vector3(00, -Y, 24);
        vertices[151].Position = new Vector3(-1, -Y, 23);
        vertices[152].Position = new Vector3(-1, -Y, 16);
        vertices[153].Position = new Vector3(00, -Y, 15);
        vertices[154].Position = new Vector3(00, -Y, 08);
        vertices[155].Position = new Vector3(01, -Y, 07);
        vertices[156].Position = new Vector3(01, -Y, 05);
        vertices[157].Position = new Vector3(00, -Y, 04);

        vertices[158].Position = new Vector3(21, Y, 04);
        vertices[159].Position = new Vector3(26, Y, 04);

        vertices[160].Position = new Vector3(28, Y, 06);
        vertices[161].Position = new Vector3(28, Y, 10);
        vertices[162].Position = new Vector3(26, Y, 12);
        vertices[163].Position = new Vector3(22, Y, 12);
        vertices[164].Position = new Vector3(20, Y, 10);
        vertices[165].Position = new Vector3(20, Y, 05);

        return vertices;
    }


    private short[] setupIndicesFlat()
    {
        return new short[] 
        {
            // Area 00
            00, 01, 02,     00, 02, 03,     00, 03, 51,     00, 51, 52,
            // Area 01
            51, 03, 04,     51, 04, 38,     51, 38, 39,     51, 39, 50,
            // Area 02
            04, 05, 06,     04, 06, 37,     04, 37, 38,
            // Area 03
            06, 07, 31,     06, 31, 32,     06, 32, 36,     06, 36, 37,
            // Area 04
            07, 08, 09,     07, 09, 30,     07, 30, 31,
            // Area 05
            09, 10, 17,     09, 17, 29,     09, 29, 30,
            // Area 06
            11, 12, 16,     11, 16, 17,     11, 17, 10,
            // Area 07
            12, 13, 14,     12, 14, 15,     12, 15, 16,
            // Area 08
            29, 17, 18,     29, 18, 27,     29, 27, 28,
            // Area 09
            27, 18, 25,     27, 25, 26,
            // Area 10
            18, 19, 25,
            // Area 11
            25, 19, 24,
            // Area 12
            19, 20, 24,
            // Area 13
            24, 20, 21,     24, 21, 22,     24, 22, 23,
            // Area 14
            36, 32, 33,     36, 33, 34,     36, 34, 35,
            // Area 15
            49, 50, 39,     49, 39, 40,     49, 40, 41,     49, 41, 48,
            // Area 16
            48, 41, 42,     48, 42, 46,     48, 46, 47,
            // Area 17
            46, 42, 43,     46, 43, 44,     46, 44, 45,
            // Area 18
            53, 54, 55,     53, 55, 56,     53, 56, 57,     53, 57, 58,
            // Area 19
            65, 66, 59,     65, 59, 60,     65, 60, 61,     65, 61, 62,     65, 62, 63,     65, 63, 64,
            // Area 20
            73, 67, 68,     73, 68, 69,     73, 69, 70,     73, 70, 71,     73, 71, 72
        };
    }

    public void Draw(GraphicsDevice GraphicsDevice)
    {

        // Draw the bottom.
        GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, listVertices[0],
            0, listVertices[0].Length, listIndices[0], 0, this.listIndices[0].Length / 3);

        // Draw bottom diagonals.

        // Draw the sides.

        // Draw the top diagonals.

        // Draw the top.
        GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, listVertices[1],
            0, listVertices[1].Length, listIndices[0], 0, this.listIndices[0].Length / 3);
    }
}
