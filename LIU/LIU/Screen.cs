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
    class Screen
    {
        //fields 
        Vector2 globalPosition;
        private int width;
        private int height;
        private Varjo player;

        public float X
        {
            get { return globalPosition.X; }
            set { globalPosition.X = value; }
        }
        public float Y
        {
            get { return globalPosition.Y; }
            set { globalPosition.Y = value; }
        }
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        //a property to use the information from the fields
        // as a rectangle where necessary
        private Rectangle ScreenRect
        {
            get
            {
                return new Rectangle(
                     (int)globalPosition.X,
                     (int)globalPosition.Y,
                     width,
                     height);
            }
        }

        //A property to ensure a single instance
        private static Screen instance;
        public static Screen Instance
        {
            get
            {
                if (instance == null)
                { instance = new Screen(); }
                return instance;
            }
        }

        //constructor
        private Screen()
        {
            globalPosition = new Vector2(0, 0);
        }

        //Methods
        //this method is called to initialize some variables since 
        //they cannot be naturally created via a constructor 
        //or acessed via GraphicsDevice.Viewport.(Dimension);
        public void GetWidthAndHeight(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void GetCharacter(Varjo player)
        {
            this.player = player;
        }

        public void Update()
        {
            Vector2 newPos = new Vector2(player.GlobalPos.X - (Width / 2 - player.Width / 2), 0);
            if (newPos.X > 0)
            {
                globalPosition = newPos;
            }
            else if (newPos.X < 0)
            {
                globalPosition = new Vector2(0, 0);
            }
        }

        //2 ways to convert an object's global coordinates to local coordinates
        public Vector2 ConvertToScreenPos(Vector2 objPos)
        { return objPos - globalPosition; }
        public Rectangle ConvertToScreenpos(Rectangle objBox)
        {
            return new Rectangle(
                (objBox.X - (int)globalPosition.X),
                (objBox.Y - (int)globalPosition.Y),
                objBox.Width,
                objBox.Height);
        }

        //2 methods to check if an object is on-screen
        //one for rectangles and one for vectors
        public bool WithinScreen(Rectangle rectangle)
        {
            if (rectangle.Intersects(ScreenRect))
            { return true; }
            else { return false; }
        }
        public bool WithinScreen(Vector2 vector)
        {
            Point check = new Point((int)vector.X, (int)vector.Y);
            if (
                ScreenRect.Contains(check))
            { return true; }
            else { return false; }
        }
    }
}
