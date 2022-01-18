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
    enum TextAlign
    {
        Center,
        Left,
        Right
    }

    class UIText : UIComponent
    {
        ///////////////////////////////////////////////////
        ///Fields
        ///
        protected SpriteFont font;
        protected String text;
        protected Vector2 pos, origin;
        protected Color color;
        protected TextAlign align;

        ///////////////////////////////////////////////////
        ///Properties
        /// NONE

        ///////////////////////////////////////////////////
        ///Constructor
        ///

        ///<summary>
        ///Initialize the text and any background you want for the text blurb
        /// </summary>
        public UIText(String text, Rectangle drawingRectangle, Texture2D backgroundTexture,
            int elevation, SpriteFont font, Color color, TextAlign align, int xOffset, int yOffset, int xSize, int ySize)
            : base(drawingRectangle, backgroundTexture, elevation, xOffset, yOffset, xSize, ySize)
        {
            this.font = font;
            this.text = text;
            this.color = color;
            this.align = align;
            switch (this.align)
            {
                case TextAlign.Center:
                    origin = new Vector2(font.MeasureString(text).X / 2, 0);
                    break;

                case TextAlign.Left:
                    origin = new Vector2(0, 0);
                    break;

                case TextAlign.Right:
                    origin = new Vector2(font.MeasureString(text).X, 0);
                    break;
            }
            pos = new Vector2(drawingRectangle.X, drawingRectangle.Y + font.MeasureString(text).Y / 2);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.DrawString(this.font, this.text, this.pos, this.color, 0, this.origin, 1, SpriteEffects.None, Elevation);
        }

        public override void Update()
        {
        }
    }
}
