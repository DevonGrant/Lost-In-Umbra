using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LIU
{
    class Node
    {
        private Rectangle area;
        private Node parent;
        private List<Node> children;
        private List<Entity> entities;

        public List<Entity> Entities
        {
            get { return entities; }
        }
        public Rectangle Area
        {
            get { return area; }
        }

        public Node(Rectangle area, List<Entity> entities, Node parent)
        {
            this.area = area;
            this.parent = parent;
            this.children = new List<Node>();
            this.entities = new List<Entity>();

            foreach (Entity entity in entities)
            {
                if (area.Intersects(entity.GlobalRectangle))
                {
                    this.entities.Add(entity);
                    entity.parentNode = this;
                }
            }

            if (entities.Count > 4 && (area.Width > 1 && area.Height > 1))
            {
                CollisionDetection.TreeParents.Add(area, this);
                children.Add(new Node(new Rectangle(area.X, area.Y, area.Width / 2, area.Height / 2), this.entities, this));
                children.Add(new Node(new Rectangle(area.X + (area.Width / 2), area.Y, area.Width / 2, area.Height / 2), this.entities, this));
                children.Add(new Node(new Rectangle(area.X, area.Y + (area.Height / 2), area.Width / 2, area.Height / 2), this.entities, this));
                children.Add(new Node(new Rectangle(area.X + (area.Width / 2), area.Y + (area.Height / 2), area.Width / 2, area.Height / 2), this.entities, this));
            }
            else
            {
                CollisionDetection.TreeLeafs.Add(area, this);
            }
        }

        public void Update()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities.ElementAt(i).HasMoved())
                {
                    Rectangle newArea = new Rectangle(0, 0, 0, 0);
                    Node p = CollisionDetection.TreeParents[new Rectangle(-CollisionDetection.WorldWidth / 2, -CollisionDetection.WorldHeight / 2, CollisionDetection.WorldWidth, CollisionDetection.WorldHeight)];
                    CollisionDetection.TreeParents[p.Area].AddEntity(entities.ElementAt(i));

                    if (!entities.ElementAt(i).GlobalRectangle.Intersects(area))
                    {
                        entities.RemoveAt(i);
                    }
                }
            }
        }

        public void AddEntity(Entity entity)
        {
            if (children.Count > 0)
            {
                foreach (Node child in children)
                {
                    if (child.area.Intersects(entity.GlobalRectangle))
                    {
                        child.AddEntity(entity);
                    }
                }
            }
            else if (!entities.Contains(entity))
            {
                entities.Add(entity);
                entity.parentNode = this;
            }

            if (!CollisionDetection.TreeParents.ContainsKey(area) && entities.Count > 4 && (area.Width > 1 && area.Height > 1))
            {
                if (CollisionDetection.TreeLeafs.ContainsKey(area))
                {
                    CollisionDetection.TreeLeafs.Remove(area);
                }
                CollisionDetection.TreeParents.Add(area, this);
                children.Add(new Node(new Rectangle(area.X, area.Y, area.Width / 2, area.Height / 2), this.entities, this));
                children.Add(new Node(new Rectangle(area.X + (area.Width / 2), area.Y, area.Width / 2, area.Height / 2), this.entities, this));
                children.Add(new Node(new Rectangle(area.X, area.Y + (area.Height / 2), area.Width / 2, area.Height / 2), this.entities, this));
                children.Add(new Node(new Rectangle(area.X + (area.Width / 2), area.Y + (area.Height / 2), area.Width / 2, area.Height / 2), this.entities, this));
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(LevelLoader.platformSprite, Screen.Instance.ConvertToScreenpos(area), new Color(Color.Blue, 50));
        }
    }
}
