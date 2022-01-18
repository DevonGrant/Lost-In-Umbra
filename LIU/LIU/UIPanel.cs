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
    class UIPanel : UIComponent
    {
        ///////////////////////////////////////////////////
        ///Fields
        ///
        private List<UIComponent> children;

        ///////////////////////////////////////////////////
        ///Constructor
        ///
        public UIPanel(Rectangle drawingRectangle, Texture2D background, int elevation, int xOffset, int yOffset, int xSize, int ySize)
            : base(drawingRectangle, background, elevation, xOffset, yOffset, xSize, ySize)
        {
            children = new List<UIComponent>();
        }

        //////////////////////////////////////////////////
        ///Methods
        ///

        /// <summary>
        /// Add a new component to the panel
        /// </summary>
        public void AddComponent(UIComponent component)
        {
            component.ChangePos(
                new Vector2(component.DrawingRectangle.X + DrawingRectangle.X,
                component.DrawingRectangle.Y + DrawingRectangle.Y));
            children.Add(component);
        }

        /// <summary>
        /// Update the draw for the components within the panel 
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            foreach (UIComponent component in children)
            {
                component.Draw(sb);
            }

        }

        /// <summary>
        /// Update any mechanics for the components within the panel
        /// </summary>
        public override void Update()
        {
            base.Update();
            foreach (UIComponent component in children)
            {
                component.Update();
            }
        }
    }
}
