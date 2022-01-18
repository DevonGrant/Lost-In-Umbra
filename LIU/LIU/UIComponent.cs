using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LIU
{
    public class UIComponent
    {
        ///////////////////////////////////////////////////
        ///Fields
        ///


        protected Rectangle screenPosition;
        //screenPosition (Rectangle)

        protected Texture2D background;
        //background (picture)

        //layerDepth (int)
        protected int layerDepth;

        //sourceRectangle
        protected int xOffset;
        protected int yOffset;
        protected int xSize;
        protected int ySize;

        ///////////////////////////////////////////////////
        ///Properties
        ///

        public Rectangle DrawingRectangle
        {
            get { return this.screenPosition; }
        }

        public Texture2D Background
        {
            get { return this.background; }
        }

        public int Elevation
        {
            get { return this.layerDepth; }
        }

        ///////////////////////////////////////////////////
        ///Constructor
        ///

        //parameterize with picture, position, and depth
        public UIComponent(Rectangle screenPosition, Texture2D background, int layerDepth, int xOffset, int yOffset,
            int xSize, int ySize)
        {
            this.screenPosition = screenPosition;
            this.background = background;
            this.layerDepth = layerDepth;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.xSize = xSize;
            this.ySize = ySize;
        }

        ///////////////////////////////////////////////////
        ///Methods
        ///

        public virtual void Update() { }

        public virtual void Draw(SpriteBatch sb)
        {
            if (this.background != null)
            {
                sb.Draw(texture: background, destinationRectangle: screenPosition, sourceRectangle: new Rectangle(xOffset, yOffset, xSize, ySize), color: Color.White);
            }
        }

        public virtual void ChangePos(Vector2 newPos)
        {
            screenPosition = new Rectangle(
                (int)newPos.X,
                (int)newPos.Y,
                screenPosition.Width,
                screenPosition.Height);
        }
    }
}
