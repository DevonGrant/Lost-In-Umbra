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
    public delegate void OnClickDelegate(object sender);
    public delegate void OnHoverDelegate(object sender);

    class UIButton : UIText
    {
        ///////////////////////////////////////////////////
        ///Fields
        ///
        protected OnClickDelegate OnClick;
        protected OnHoverDelegate OnHover;
        protected Color idleColor;
        public bool isHover;
        protected int hoverOffset;
        protected int trueY;

        ///////////////////////////////////////////////////
        ///Properties
        ///None

        ///////////////////////////////////////////////////
        ///Constructor
        ///

        public UIButton(OnClickDelegate onClick, OnHoverDelegate onHover, string text,
            Rectangle drawingRectangle, Texture2D background, int elevation, SpriteFont font,
            Color color, int hoverOffset, TextAlign align, int xOffset, int yOffset, int xSize, int ySize)
            : base(text, drawingRectangle, background, elevation, font, color, align, xOffset, yOffset, xSize, ySize)
        {
            this.OnClick += onClick;
            this.OnHover += onHover;
            this.idleColor = color;
            this.hoverOffset = hoverOffset;
            this.trueY = yOffset; //saves the Y offset from when the hover occurs, since the yOffset int is passed on to the parent class for the draw method

            this.isHover = false;
        }

        ///////////////////////////////////////////////////
        ///Methods
        ///

        public override void Draw(SpriteBatch sb)
        {
            if (isHover)
            {
                yOffset += hoverOffset;
            }
            else
            {
                yOffset = trueY;
            }
            base.Draw(sb);
        }

        public override void Update()
        {
            if (DrawingRectangle.Contains(Mouse.GetState().Position))
            {
                OnHover(this);
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    OnClick(this);
                }
            }
            else
            {
                isHover = false;
            }
            base.Update();
        }
    }
}
