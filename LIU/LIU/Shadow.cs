using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LIU
{
    class Shadow
    {
        // Stores the light that this shadow is attached to
        private Light light;

        //Stores the platform this shadow is attached to
        private Platform platform;

        //Stores the verticies of the triangles that is used to draw the shadow
        private VertexPositionColor[] tri, tri2, tri3, tri4;

        //Stores each of the verticies of the shadow
        private Vector2 p1, p2, p1f, p2f;

        //Stores the length of the shadow
        private float length = 250;


        int[] ind;

        //Property to get the light of the shadow
        public Light Light
        {
            get { return light; }
        }

        //Property to get the platform of the shadow
        public Platform Platform
        {
            get { return platform; }
        }


        /// <summary>
        /// Constructs the shadow by storing the light and platform and Finds the points of the shadow
        /// </summary>
        /// <param name="light"></param>
        /// <param name="platform"></param>
        public Shadow(Light light, Platform platform, float length)
        {
            tri = new VertexPositionColor[3];
            tri2 = new VertexPositionColor[3];
            tri3 = new VertexPositionColor[3];
            tri4 = new VertexPositionColor[3];

            this.light = light;
            this.platform = platform;
            this.length = length;

            FindShadow();
        }

        /// <summary>
        /// Finds the points of the shadow relative the light and stores the colors and verticies in each of the triangles
        /// </summary>
        public void FindShadow()
        {
            FindPoints(platform, light);

            Vector2 p1s = Screen.Instance.ConvertToScreenPos(p1);
            Vector2 p2s = Screen.Instance.ConvertToScreenPos(p2);
            Vector2 p1fs = Screen.Instance.ConvertToScreenPos(p1f);
            Vector2 p2fs = Screen.Instance.ConvertToScreenPos(p2f);

            tri[0] = new VertexPositionColor(new Vector3(p1s.X, p1s.Y, 0), Color.Black);
            tri[1] = new VertexPositionColor(new Vector3(p1fs.X, p1fs.Y, 0), Color.Black);
            tri[2] = new VertexPositionColor(new Vector3(p2fs.X, p2fs.Y, 0), Color.Black);

            tri2[0] = new VertexPositionColor(new Vector3(p2fs.X, p2fs.Y, 0), Color.Black);
            tri2[1] = new VertexPositionColor(new Vector3(p1s.X, p1s.Y, 0), Color.Black);
            tri2[2] = new VertexPositionColor(new Vector3(p2s.X, p2s.Y, 0), Color.Black);
        }

        /// <summary>
        /// Draws each of the three triangles of the shadow
        /// </summary>
        /// <param name="basicEffect"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="vertexBuffer"></param>
        public void Draw(BasicEffect basicEffect, GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer)
        {
            bool inLight = false;

            foreach (Light light in Light.SceneLights)
            {
                if (light != this.light && (light.DoesContainPoint(p1f) && light.DoesContainPoint(p2f)))
                {
                    inLight = true;
                }
            }

            if (!inLight)
            {
                vertexBuffer.SetData<VertexPositionColor>(tri);
                graphicsDevice.SetVertexBuffer(vertexBuffer);
                foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
                }

                vertexBuffer.SetData<VertexPositionColor>(tri2);
                foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
                }
            }
        }

        /// <summary>
        /// Finds all four vertices of the shadow based on the light and platform
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="light"></param>
        private void FindPoints(Platform platform, Light light)
        {
            if (light != null)
            {
                float offset = platform.Width / 2;

                if ((platform.GMidPoint.X + offset < light.GlobalPosition.X || platform.GMidPoint.X - offset > light.GlobalPosition.X) && (platform.GMidPoint.Y + offset < light.GlobalPosition.Y || platform.GMidPoint.Y - offset > light.GlobalPosition.Y))
                {
                    p1 = FindFurthestPoint(platform.Points, light, platform);

                    if (p1 == platform.GPointNE)
                    {
                        p2 = platform.GPointSW;
                    }
                    else if (p1 == platform.GPointNW)
                    {
                        p2 = platform.GPointSE;
                    }
                    else if (p1 == platform.GPointSW)
                    {
                        p2 = platform.GPointNE;
                    }
                    else if (p1 == platform.GPointSE)
                    {
                        p2 = platform.GPointNW;
                    }
                }
                else
                {
                    if (platform.GMidPoint.X + offset >= light.GlobalPosition.X && platform.GMidPoint.X - offset <= light.GlobalPosition.X)
                    {
                        if (light.GlobalPosition.Y > platform.GMidPoint.Y)
                        {
                            p1 = platform.GPointSW;
                            p2 = platform.GPointSE;
                        }
                        else
                        {
                            p1 = platform.GPointNW;
                            p2 = platform.GPointNE;
                        }
                    }
                    else
                    {
                        if (light.GlobalPosition.X > platform.GMidPoint.X)
                        {
                            p1 = platform.GPointNE;
                            p2 = platform.GPointSE;
                        }
                        else
                        {
                            p1 = platform.GPointNW;
                            p2 = platform.GPointSW;
                        }
                    }
                }

                if (p1.Y < p2.Y)
                {
                    Vector2 temp = p1;
                    p1 = p2;
                    p2 = temp;
                }

                float phiSlope = ((p1.Y - light.GlobalPosition.Y) / (p1.X - light.GlobalPosition.X));
                Vector2 phiPoint = p1;
                float alphaSlope = ((p2.Y - light.GlobalPosition.Y) / (p2.X - light.GlobalPosition.X));
                Vector2 alphaPoint = p2;
                float orthoSlope = 1 / ((platform.GMidPoint.Y - light.GlobalPosition.Y) / (platform.GMidPoint.X - light.GlobalPosition.X));
                Vector2 orthoPoint;

                if (platform.GMidPoint.X != light.GlobalPosition.X && platform.GMidPoint.Y != light.GlobalPosition.Y)
                {
                    if (platform.GMidPoint.X > light.GlobalPosition.X)
                    {
                        orthoPoint = new Vector2((float)(platform.GMidPoint.X + length * Math.Cos(Math.Atan(1 / orthoSlope))), (float)(platform.GMidPoint.Y + length * Math.Sin(Math.Atan(1 / orthoSlope))));
                    }
                    else
                    {
                        orthoPoint = new Vector2((float)(platform.GMidPoint.X - length * Math.Cos(Math.Atan(1 / orthoSlope))), (float)(platform.GMidPoint.Y - length * Math.Sin(Math.Atan(1 / orthoSlope))));
                    }

                    if (p1.X != light.GlobalPosition.X && p2.X != light.GlobalPosition.X)
                    {
                        p1f = new Vector2((orthoSlope * orthoPoint.X + phiSlope * phiPoint.X + orthoPoint.Y - phiPoint.Y) / (orthoSlope + phiSlope), phiSlope * (((orthoSlope * orthoPoint.X + phiSlope * phiPoint.X + orthoPoint.Y - phiPoint.Y) / (orthoSlope + phiSlope)) - phiPoint.X) + phiPoint.Y);
                        p2f = new Vector2((orthoSlope * orthoPoint.X + alphaSlope * alphaPoint.X + orthoPoint.Y - alphaPoint.Y) / (orthoSlope + alphaSlope), alphaSlope * (((orthoSlope * orthoPoint.X + alphaSlope * alphaPoint.X + orthoPoint.Y - alphaPoint.Y) / (orthoSlope + alphaSlope)) - alphaPoint.X) + alphaPoint.Y);
                    }
                    else
                    {
                        if (p1.X == light.GlobalPosition.X)
                        {
                            if (platform.GMidPoint.Y > light.GlobalPosition.Y)
                            {
                                p1f = new Vector2(p1.X, -orthoSlope * (p1.X - orthoPoint.X) + orthoPoint.Y);
                                p2f = new Vector2((orthoSlope * orthoPoint.X + alphaSlope * alphaPoint.X + orthoPoint.Y - alphaPoint.Y) / (orthoSlope + alphaSlope), alphaSlope * (((orthoSlope * orthoPoint.X + alphaSlope * alphaPoint.X + orthoPoint.Y - alphaPoint.Y) / (orthoSlope + alphaSlope)) - alphaPoint.X) + alphaPoint.Y);
                            }
                        }
                        else
                        {
                            p1f = new Vector2((orthoSlope * orthoPoint.X + phiSlope * phiPoint.X + orthoPoint.Y - phiPoint.Y) / (orthoSlope + phiSlope), phiSlope * (((orthoSlope * orthoPoint.X + phiSlope * phiPoint.X + orthoPoint.Y - phiPoint.Y) / (orthoSlope + phiSlope)) - phiPoint.X) + phiPoint.Y);
                            p2f = new Vector2(p2.X, -orthoSlope * (p2.X - orthoPoint.X) + orthoPoint.Y);
                        }
                    }
                }
                else
                {
                    if (platform.GMidPoint.X == light.GlobalPosition.X)
                    {
                        if (platform.GMidPoint.Y > light.GlobalPosition.Y)
                        {
                            phiSlope = ((p1.Y - light.GlobalPosition.Y) / (p1.X - light.GlobalPosition.X));
                            alphaSlope = ((p2.Y - light.GlobalPosition.Y) / (p2.X - light.GlobalPosition.X));
                            p1f = new Vector2((1 / phiSlope) * (p1.Y + length - light.GlobalPosition.Y) + light.GlobalPosition.X, p1.Y + length);
                            p2f = new Vector2((1 / alphaSlope) * (p2.Y + length - light.GlobalPosition.Y) + light.GlobalPosition.X, p2.Y + length);
                        }
                        else
                        {
                            phiSlope = ((p1.Y - light.GlobalPosition.Y) / (p1.X - light.GlobalPosition.X));
                            alphaSlope = ((p2.Y - light.GlobalPosition.Y) / (p2.X - light.GlobalPosition.X));
                            p1f = new Vector2((1 / phiSlope) * (p1.Y - length - light.GlobalPosition.Y) + light.GlobalPosition.X, p1.Y - length);
                            p2f = new Vector2((1 / alphaSlope) * (p2.Y - length - light.GlobalPosition.Y) + light.GlobalPosition.X, p2.Y - length);
                        }
                    }
                    else
                    {
                        if (platform.GMidPoint.X > light.GlobalPosition.X)
                        {
                            phiSlope = ((p1.Y - light.GlobalPosition.Y) / (p1.X - light.GlobalPosition.X));
                            alphaSlope = ((p2.Y - light.GlobalPosition.Y) / (p2.X - light.GlobalPosition.X));
                            p1f = new Vector2(p1.X + length, phiSlope * (p1.X + length - light.GlobalPosition.X) + light.GlobalPosition.Y);
                            p2f = new Vector2(p2.X + length, alphaSlope * (p2.X + length - light.GlobalPosition.X) + light.GlobalPosition.Y);
                        }
                        else
                        {
                            phiSlope = ((p1.Y - light.GlobalPosition.Y) / (p1.X - light.GlobalPosition.X));
                            alphaSlope = ((p2.Y - light.GlobalPosition.Y) / (p2.X - light.GlobalPosition.X));
                            p1f = new Vector2(p1.X - length, phiSlope * (p1.X - length - light.GlobalPosition.X) + light.GlobalPosition.Y);
                            p2f = new Vector2(p2.X - length, alphaSlope * (p2.X - length - light.GlobalPosition.X) + light.GlobalPosition.Y);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the furthest point from the line that runs through the platform
        /// </summary>
        /// <param name="points"></param>
        /// <param name="light"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        private Vector2 FindFurthestPoint(List<Vector2> points, Light light, Platform platform)
        {
            float maxValue = 0;
            Vector2 maxPoint = new Vector2();

            foreach (Vector2 point in points)
            {
                float theta = AngleFromPointToPoint(light.GlobalPosition, platform.GMidPoint);
                float alpha = AngleFromPointToPoint(light.GlobalPosition, point);
                float phi = Math.Abs(theta - alpha);
                float d = (float)Math.Sqrt(Math.Pow(point.Y - light.GlobalPosition.Y, 2) + Math.Pow(point.X - light.GlobalPosition.X, 2));

                if (maxValue < d * Math.Sin(phi))
                {
                    maxValue = (float)(d * Math.Sin(phi));
                    maxPoint = point;
                }
            }
            return maxPoint;
        }

        /// <summary>
        /// Finds the angle of the line from point 1 to point 2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float AngleFromPointToPoint(Vector2 p1, Vector2 p2)
        {
            if (p1.X < p2.X)
            {
                return (float)(Math.Atan((p1.Y - p2.Y) / (p1.X - p2.X)));
            }
            return (float)(Math.Atan((p1.Y - p2.Y) / (p1.X - p2.X)) + Math.PI);
        }

        /// <summary>
        /// Returns true if the point is within the shadow
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool WithinShadow(Vector2 point)
        {
            float q1 = ((p1.Y - p2.Y) / (p1.X - p2.X)) * (point.X - p1.X) + p1.Y;
            float q2 = ((p1f.Y - p2f.Y) / (p1f.X - p2f.X)) * (point.X - p1f.X) + p1f.Y;
            float q3 = ((p1.Y - p1f.Y) / (p1.X - p1f.X)) * (point.X - p1.X) + p1.Y;
            float q4 = ((p2.Y - p2f.Y) / (p2.X - p2f.X)) * (point.X - p2.X) + p2.Y;

            if (light.GlobalPosition.Y > platform.GMidPoint.Y)
            {
                if (light.GlobalPosition.X > platform.GMidPoint.X && light.GlobalPosition.Y < platform.GlobalRectangle.Bottom)
                {
                    if (point.Y >= q1 && point.Y >= q2 && point.Y <= q3 && point.Y >= q4)
                    {
                        return true;
                    }
                }
                else if (p1.Y != p2.Y)
                {
                    if (point.Y <= q1 && point.Y >= q2 && point.Y <= q3 && point.Y >= q4)
                    {
                        return true;
                    }
                }
                else
                {
                    if (point.Y <= q1 && point.Y >= q2 && point.Y <= q3 && point.Y <= q4)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (light.GlobalPosition.X < platform.GMidPoint.X && light.GlobalPosition.Y > platform.GlobalRectangle.Top)
                {
                    if (point.Y >= q1 && point.Y >= q2 && point.Y <= q3 && point.Y >= q4)
                    {
                        return true;
                    }
                }
                else if (p1.Y != p2.Y)
                {
                    if (point.Y >= q1 && point.Y <= q2 && point.Y <= q3 && point.Y >= q4)
                    {
                        return true;
                    }
                }
                else
                {
                    if (point.Y >= q1 && point.Y <= q2 && point.Y >= q3 && point.Y >= q4)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if one of the four vertices of the rectangle is within the shadow
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public bool WithinShadow(Rectangle rectangle)
        {
            Vector2[] points = { new Vector2(rectangle.Location.X, rectangle.Location.Y), new Vector2(rectangle.Location.X + rectangle.Width, rectangle.Location.Y), new Vector2(rectangle.Location.X, rectangle.Location.Y + rectangle.Height), new Vector2(rectangle.Location.X + rectangle.Width, rectangle.Location.Y + rectangle.Height) };

            foreach (Vector2 point in points)
            {
                if (WithinShadow(point))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Draws all the shadows within the global shadow list
        /// </summary>
        /// <param name="basicEffect"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="vertexBuffer"></param>
        public static void DrawShadows(BasicEffect basicEffect, GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer)
        {
            foreach (Shadow shadow in Platform.GlobalShadows)
            {
                shadow.Draw(basicEffect, graphicsDevice, vertexBuffer);
            }
        }
    }
}
