using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LIU
{
    static class LevelLoader
    {
        public static Texture2D platformSprite, lightMask;


        public static List<Entity> LoadLevelFromFile(String filename, GraphicsDevice graphicsDevice)
        {
            String[][] world;
            StreamReader reader;
            int rows;


            if (filename.Length <= 4 || filename.Substring(filename.Length - 4, 3) != ".txt")
            {
                rows = TotalLines("../../../../" + filename + ".txt");
                reader = new StreamReader("../../../../" + filename + ".txt");
            }
            else
            {
                rows = TotalLines("../../../../" + filename);
                reader = new StreamReader("../../../../" + filename);
            }

            float sideOfGridPart = Game1.WorldScalar;


            world = new String[rows][];
            for (int i = 0; i < world.Length; i++)
            {
                world[i] = reader.ReadLine().Split(',');
            }

            List<Entity> entities = new List<Entity>();

            for (int y = 0; y < world.Length; y++)
            {
                for (int x = 0; x < world[y].Length; x++)
                {
                    switch (world[y][x].Substring(0, 1))
                    {
                        case "p":
                            entities.Add(new Platform(platformSprite, new Vector2(x * sideOfGridPart, y * sideOfGridPart), (int)sideOfGridPart, (int)sideOfGridPart));
                            break;

                        case "l":
                            int length = int.Parse(world[y][x].Substring(1, world[y][x].Length - 1));
                            entities.Add(new PointLight(new Vector2((x * sideOfGridPart) + (sideOfGridPart / 2), (y * sideOfGridPart) + (sideOfGridPart / 2)), length * sideOfGridPart, lightMask));
                            break;

                        default:
                            break;
                    }
                }
            }

            return entities;
        }

        private static int TotalLines(String filename)
        {
            StreamReader reader = new StreamReader(filename);
            int count = 0;
            while (!reader.EndOfStream)
            {
                reader.ReadLine();
                count++;
            }
            reader.Close();
            return count;
        }
    }
}
