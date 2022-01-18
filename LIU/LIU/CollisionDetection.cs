using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LIU
{
    class CollisionDetection
    {
        private static Dictionary<Rectangle, Node> parents, leafs;
        private static int worldWidth, worldHeight;

        public static Dictionary<Rectangle, Node> TreeParents
        {
            get { return parents; }
        }

        public static Dictionary<Rectangle, Node> TreeLeafs
        {
            get { return leafs; }
        }

        public static int WorldWidth
        {
            get { return worldWidth; }
        }

        public static int WorldHeight
        {
            get { return worldHeight; }
        }

        public CollisionDetection(int worldWidth, int worldHeight, List<Entity> entities)
        {
            parents = new Dictionary<Rectangle, Node>();
            leafs = new Dictionary<Rectangle, Node>();
            CollisionDetection.worldWidth = worldWidth * 2;
            CollisionDetection.worldHeight = worldHeight * 2;

            new Node(new Rectangle(-worldWidth, -worldHeight, worldWidth * 2, worldHeight * 2), entities, null);
        }

        public void Update()
        {
            int count = 0;
            List<Node> nodes = TreeLeafs.Values.ToList<Node>();
            foreach (Node node in nodes)
            {
                node.Update();
                foreach (Entity entity in node.Entities)
                {
                    if (!(entity is Varjo))
                    {
                        entity.Update();
                    }
                    else
                    {
                        if (count < 1)
                        {
                            count++;
                            entity.Update();
                        }
                        ((Varjo)entity).CollisionDetection(node.Entities);

                    }
                }
            }
            Console.WriteLine("Player Count: {0}", count);

            Screen.Instance.Update();
        }

        public void Draw(SpriteBatch sb)
        {
            int count = 0;
            List<Node> nodes = TreeLeafs.Values.ToList<Node>();
            foreach (Node node in nodes)
            {
                if (Screen.Instance.WithinScreen(node.Area))
                {
                    foreach (Entity entity in node.Entities)
                    {
                        if (!(entity is Light) && !(entity is Varjo))
                        {
                            entity.Draw(sb);
                        }
                        else if (entity is Varjo && count < 1)
                        {
                            count++;
                            entity.Draw(sb);
                        }
                    }
                }
            }
        }
    }
}
