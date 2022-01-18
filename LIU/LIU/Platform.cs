using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace LIU
{
    class Platform : Entity
    {
        //Stores the verticies of the platform
        private List<Vector2> points;

        //List of all lights that the platform is within
        private List<Light> lights;

        //List of all shadows produced by the platform
        private List<Shadow> shadows;

        //Whether the platform is in light or not
        private bool isInLight;

        //A List of all the curent shadows in the game
        public static List<Shadow> GlobalShadows = new List<Shadow>();

        //Property to get the NW point of the platform
        public Vector2 PointNW
        {
            get { return screenPosition; }
        }

        //Property to get the NE point of the platform
        public Vector2 PointNE
        {
            get { return new Vector2(screenPosition.X + Width, screenPosition.Y); }
        }

        //Property to get the SW point of the platform
        public Vector2 PointSW
        {
            get { return new Vector2(screenPosition.X, screenPosition.Y + Height); }
        }

        //Property to get the SE point of the platform
        public Vector2 PointSE
        {
            get { return new Vector2(screenPosition.X + Width, screenPosition.Y + Height); }
        }

        //Property to get the Midpoint of the platform
        public Vector2 MidPoint
        {
            get { return new Vector2(screenPosition.X + Width / 2, screenPosition.Y + Height / 2); }
        }

        public Vector2 GPointNW
        {
            get { return new Vector2(GlobalRectangle.Location.X, GlobalRectangle.Location.Y); }
        }
        public Vector2 GPointNE
        {
            get { return new Vector2(GlobalRectangle.Location.X + Width, GlobalRectangle.Location.Y); }
        }
        public Vector2 GPointSW
        {
            get { return new Vector2(GlobalRectangle.Location.X, GlobalRectangle.Location.Y + Height); }
        }
        public Vector2 GPointSE
        {
            get { return new Vector2(GlobalRectangle.Location.X + Width, GlobalRectangle.Location.Y + Height); }
        }
        public Vector2 GMidPoint
        {
            get { return new Vector2(GlobalRectangle.Location.X + Width / 2, GlobalRectangle.Location.Y + Height / 2); }
        }

        //Property to get the list of verticies
        public List<Vector2> Points
        {
            get { return points; }
        }
        /// <summary>
        /// Constructs the platform given the rectangle and texture
        /// </summary>
        /// <param name="r"></param>
        /// <param name="t"></param>
        /// <param name="gd"></param>
        public Platform(Texture2D sprite, Vector2 globalPosition, int width, int height) : base(sprite, globalPosition, width, height)
        {
            points = new List<Vector2>();
            points.Add(GPointNW);
            points.Add(GPointNE);
            points.Add(GPointSW);
            points.Add(GPointSE);

            lights = new List<Light>();
            shadows = new List<Shadow>();
        }

        /// <summary>
        /// Platform's update function that updates the platform every frame
        /// </summary>
        /// <param name="sceneLights"></param>
        public override void Update()
        {
            bool hasChanged = false;
            foreach (Light light in Light.SceneLights)
            {
                if (!hasChanged)
                {
                    hasChanged = IsInLight(light.IsWithinLight(this), light);
                }
                else
                {
                    IsInLight(light.IsWithinLight(this), light);
                }
            }
            if (hasChanged)
            {
                foreach (Light light in lights)
                {
                    if (isInLight && !DoesShadowsContainLight(light))
                    {
                        GlobalShadows.Add(new Shadow(light, this, light.GetLength()));
                        shadows.Add(new Shadow(light, this, light.GetLength()));
                    }
                }
            }

            screenPosition = Screen.Instance.ConvertToScreenPos(globalPosition);
        }

        /// <summary>
        /// Platform's Draw method that is responsilbe for drawing the platform evrey frame
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, ScreenRectangle, Color.White);
        }

        /// <summary>
        /// Adds the light to the list of lights and removes the light if the platform is no longer within the light
        /// </summary>
        /// <param name="isInLight"></param>
        /// <param name="light"></param>
        /// <returns></returns>
        public bool IsInLight(bool isInLight, Light light)
        {
            if (isInLight)
            {
                this.isInLight = true;
                if (!lights.Contains(light))
                {
                    lights.Add(light);
                    return true;
                }
            }
            else
            {
                if (lights.Contains(light))
                {
                    lights.RemoveAt(lights.IndexOf(light));
                    GlobalShadows.RemoveAt(IndexOfGlobalShadows(light, this));
                    shadows.RemoveAt(IndexOfLightInShadows(light));
                    if (lights.Count <= 0)
                    {
                        this.isInLight = false;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the light is contained of the in the list of shadows
        /// </summary>
        /// <param name="light"></param>
        /// <returns></returns>
        private bool DoesShadowsContainLight(Light light)
        {
            foreach (Shadow shadow in shadows)
            {
                if (shadow.Light == light)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the index of the shadow in global shadows with the desired light and platform
        /// </summary>
        /// <param name="light"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        private static int IndexOfGlobalShadows(Light light, Platform platform)
        {
            if (GlobalShadows.Count > 0)
            {
                for (int i = 0; i < GlobalShadows.Count; i++)
                {
                    if (GlobalShadows.ElementAt(i).Light == light && GlobalShadows.ElementAt(i).Platform == platform)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the shadow with the desired light
        /// </summary>
        /// <param name="light"></param>
        /// <returns></returns>
        private int IndexOfLightInShadows(Light light)
        {
            if (shadows.Count > 0)
            {
                for (int i = 0; i < shadows.Count; i++)
                {
                    if (shadows.ElementAt(i).Light == light)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Updates all the shadows connected to this platform
        /// </summary>
        public static void UpdateShadows()
        {
            foreach (Shadow shadow in GlobalShadows)
            {
                shadow.FindShadow();
            }
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

        public override bool DoesContainPoint(Vector2 point)
        {
            if (GlobalRectangle.Contains(point))
            {
                return true;
            }
            return false;
        }
    }
}
