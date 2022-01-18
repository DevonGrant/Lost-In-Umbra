using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LIU
{
    abstract class Entity
    {
        protected Rectangle prevGlobalRectangle;
        protected Vector2 globalPosition, screenPosition;
        protected int width, height;
        protected float scale;
        protected Texture2D texture;
        public Node parentNode;

        public Rectangle GlobalRectangle
        {
            get { return new Rectangle((int)globalPosition.X, (int)globalPosition.Y, width, height); }
        }
        public Rectangle ScreenRectangle
        {
            get { return new Rectangle((int)screenPosition.X, (int)screenPosition.Y, width, height); }
        }
        public Vector2 GlobalPos
        {
            get { return globalPosition; }
        }
        public Vector2 ScreenPos
        {
            get { return screenPosition; }
        }
        public Vector2 MidPoint
        {
            get { return new Vector2(GlobalPos.X + Width / 2, GlobalPos.Y + Height / 2); }
        }
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        protected Entity(Texture2D texture, Vector2 globalPosition, int width, int height)
        {
            this.texture = texture;
            this.globalPosition = globalPosition;
            this.screenPosition = Screen.Instance.ConvertToScreenPos(globalPosition);
            this.width = width;
            this.height = height;
            this.scale = 1;
            prevGlobalRectangle = GlobalRectangle;
        }

        protected Entity(Texture2D texture, Vector2 globalPosition, int width, int height, float scale)
        {
            this.texture = texture;
            this.globalPosition = globalPosition;
            this.screenPosition = Screen.Instance.ConvertToScreenPos(globalPosition);
            this.width = width;
            this.height = height;
            this.scale = scale;
            prevGlobalRectangle = GlobalRectangle;
        }

        public abstract void Update();

        public abstract void Draw(SpriteBatch sb);

        public abstract int IsColliding(Entity other);

        public abstract bool DoesContainPoint(Vector2 point);

        public void UpdatePrev()
        {
            prevGlobalRectangle = GlobalRectangle;
        }

        public bool HasMoved()
        {
            if (prevGlobalRectangle != GlobalRectangle)
            {
                return true;
            }
            return false;
        }
    }
}
