using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LIU
{
    abstract class Light : Entity
    {

        public static List<Light> SceneLights = new List<Light>();

        //Stores the sprite of the light
        protected Texture2D lightMask;

        //Property of the light's global position
        public Vector2 GlobalPosition
        {
            get { return globalPosition; }
        }

        //Property of the light's screen position
        public Vector2 ScreenPosition
        {
            get { return screenPosition; }
        }

        /// <summary>
        /// Constructs the light given a initial global position and texture
        /// </summary>
        /// <param name="gPos"></param>
        protected Light(Vector2 gPos, Texture2D s) : base(s, gPos, 0, 0)
        {
            globalPosition = gPos;
            lightMask = s;
        }

        /// <summary>
        /// Checks to see if the point is within the light
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract bool IsWithinLight(Vector2 point);

        /// <summary>
        /// Checks to see if the platform is within the light
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public abstract bool IsWithinLight(Platform platform);

        /// <summary>
        /// Checks to see if the rectangle is within the light
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public abstract bool IsWithinLight(Rectangle rect);

        /// <summary>
        /// Get length of the light
        /// </summary>
        /// <returns></returns>
        public abstract float GetLength();
    }
}
