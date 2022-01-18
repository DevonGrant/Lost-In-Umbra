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
    class UISliders : UIButton
    {
        ///////////////////////////////////////////////////
        ///Fields
        ///
        private double value;
        private double maxValue;
        private double minValue;

        ///////////////////////////////////////////////////
        ///Properties
        ///
        public double Value
        {
            get { return this.value; }
        }

        public double MaxValue
        {
            get { return this.maxValue; }
        }

        public double MinValue
        {
            get { return this.minValue; }
        }

        public bool WithinButton
        {
            get { return this.isHover; }
        }

        ///////////////////////////////////////////////////
        ///Constructor
        ///

        public UISliders(OnClickDelegate onClick, OnHoverDelegate onHover, string text,
            Rectangle drawingRectangle, Texture2D background, int elevation, SpriteFont font,
            Color color, Color hoverColor, int hoverOffset, TextAlign align, int xOffset, int yOffset, int xSize, int ySize)
                : base(onClick, onHover, text, drawingRectangle, background, elevation, font, color, hoverOffset,
                      align, xOffset, yOffset, xSize, ySize)
        {
            this.isHover = false;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
