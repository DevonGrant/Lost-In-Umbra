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
    class PointLight : Light
    {
        //Radius of the point light
        private float radius;

        //Property of the light's radius
        public float Radius
        {
            get { return radius; }
        }

        /// <summary>
        /// Constructs the point light given a global position, radius, and texture
        /// </summary>
        /// <param name="globalPos"></param>
        /// <param name="radius"></param>
        /// <param name="sprite"></param>
        public PointLight(Vector2 globalPos, float radius, Texture2D lightMask) : base(globalPos, lightMask)
        {
            this.radius = radius;
        }

        /// <summary>
        /// Update function for the light
        /// </summary>
        public override void Update()
        {
            screenPosition = Screen.Instance.ConvertToScreenPos(globalPosition);
        }

        /// <summary>
        /// Draw function for the light
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            float spriteRotation = 0;
            float spritelayer = 0;
            SpriteEffects spriteEffect = new SpriteEffects();
            Vector2 spriteOrigin = new Vector2(lightMask.Width / 2, lightMask.Height / 2);
            Vector2 spriteScale = new Vector2(2 * radius / lightMask.Width, 2 * radius / lightMask.Height);

            spriteBatch.Draw(texture: lightMask, position: screenPosition, sourceRectangle: null, color: Color.White, rotation: spriteRotation, origin: spriteOrigin, scale: spriteScale, effects: spriteEffect, layerDepth: spritelayer);
        }

        /// <summary>
        /// Checks to see if the point is within the light
        /// </summary>
        /// <param name="point"></param>
        public override bool IsWithinLight(Vector2 point)
        {
            float d = (float)Math.Sqrt(Math.Pow(point.Y - GlobalPosition.Y, 2) + Math.Pow(point.X - GlobalPosition.X, 2));
            return d <= radius;
        }

        /// <summary>
        /// Checks to see if the platform is within the light
        /// </summary>
        /// <param name="platform"></param>
        public override bool IsWithinLight(Platform platform)
        {
            foreach (Vector2 point in platform.Points)
            {
                if (IsWithinLight(point))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the rectangle is within the light
        /// </summary>
        /// <param name="rect"></param>
        public override bool IsWithinLight(Rectangle rect)
        {
            Vector2[] points = { new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y), new Vector2(rect.X, rect.Y + rect.Height), new Vector2(rect.X + rect.Width, rect.Y + rect.Height) };
            foreach (Vector2 point in points)
            {
                if (IsWithinLight(point))
                {
                    return true;
                }
            }
            return false;
        }

        public override float GetLength()
        {
            return radius;
        }

        public override bool DoesContainPoint(Vector2 point)
        {
            return IsWithinLight(point);
        }

        public override int IsColliding(Entity other)
        {
            if (GlobalRectangle.Intersects(other.GlobalRectangle))
            {
                if (GlobalRectangle.Left < other.MidPoint.X && GlobalRectangle.Top > other.GlobalRectangle.Bottom)
                {
                    return 4;
                }
                else if (GlobalRectangle.Right > other.MidPoint.X && GlobalRectangle.Top > other.GlobalRectangle.Bottom)
                {
                    return 2;
                }
                else if (GlobalRectangle.Top > other.MidPoint.Y && GlobalRectangle.Bottom > other.GlobalRectangle.Bottom)
                {
                    return 3;
                }
                else if (GlobalRectangle.Bottom < other.MidPoint.Y && GlobalRectangle.Top < other.GlobalRectangle.Top)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
