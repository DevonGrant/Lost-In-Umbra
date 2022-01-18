using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace LIU
{
    enum VarjoState     //State for the controller
    {
        FaceLeft,
        FaceRight,
        WalkLeft,
        WalkRight,
        FadeLeft,
        FadeRight,
    }
    enum VerticalState
    {
        Jump,
        Falling,
        Swimming,
        None
    }
    enum ShadowState    //State for the enviornmental impact
    {
        InShadow,
        InLight,
        Neutral
    }


    class Varjo : Entity
    {
        public Dictionary<string, Keys> input;

        private VarjoState state;   //Player controller state
        public VerticalState vState;
        private ShadowState envState; //enviornment state
        private KeyboardState previousKBState;
        private float currSpeed, currJumpSpeed;
        private Vector2 initialPos;
        private bool canDash;

        public const float MaxSpeed = 3.5f,
            MaxJumpSpeed = 7,
            MaxFallSpeed = -5,
            Accel = .15f,
            Decel = .5f,
            JumpAccel = .5f,
            JumpDecel = .5f,
            FadeDistance = 75,
            SwimAccel = .5f,
            SwimDecel = .25f,
            ShadowBoost = 5;

        // Animation
        private int frame;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        private const int WalkFrameCount = 2;       // The number of frames in the animation
        private const int VarjoRectOffsetY = 576;   // How far down in the image are the frames?
        private const int VarjoRectHeight = 61;     // The height of a single frame
        private const int VarjoRectWidth = 66;      // The width of a single frame

        public float X
        {
            get { return globalPosition.X; }
            set { globalPosition.X = value; }
        }

        public Varjo(Texture2D spriteSheet, Vector2 varjoLoc, float scale, ShadowState intEnvoState, float groundY, float maxJump) : base(spriteSheet, varjoLoc, (int)(VarjoRectWidth * scale), (int)(VarjoRectHeight * scale), scale)
        {
            this.state = VarjoState.FaceRight;
            this.vState = VerticalState.Falling;
            this.envState = intEnvoState;
            this.canDash = true;
            currSpeed = 0;
            currJumpSpeed = 0;
            initialPos = varjoLoc;

            fps = 1.0;
            timePerFrame = 1.0 / fps;

            input = new Dictionary<string, Keys>();
            SetControls(Keys.Up, Keys.Up, Keys.Left, Keys.Right, Keys.Down, Keys.LeftShift);
        }

        //Public Methods

        public void UpdateAnimation(GameTime gameTime)
        {
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds; //Time passed

            if (timeCounter >= timePerFrame)
            {
                frame += 1;
                if (frame > WalkFrameCount)
                {
                    frame = 1;
                    timeCounter -= timePerFrame;
                }
            }
        }

        public override void Update()
        {
            switch (state)
            {
                //Face Left
                case VarjoState.FaceLeft:
                    if (Keyboard.GetState().IsKeyDown(input["left"]))
                    {
                        state = VarjoState.WalkLeft;
                        //remember the alternate parts
                    }
                    if (Keyboard.GetState().IsKeyDown(input["right"]))
                    {

                        state = VarjoState.FaceRight;

                    }

                    if (SingleKeyPress(input["fade"]) && canDash)
                    {
                        state = VarjoState.FadeLeft;
                    }
                    break;

                //Face Right
                case VarjoState.FaceRight:
                    if (Keyboard.GetState().IsKeyDown(input["right"]))
                    {
                        state = VarjoState.WalkRight;
                        //remember the alternate parts
                    }
                    if (Keyboard.GetState().IsKeyDown(input["left"]))
                    {

                        state = VarjoState.FaceLeft;

                    }

                    if (SingleKeyPress(input["fade"]) && canDash)
                    {
                        state = VarjoState.FadeRight;
                    }
                    break;

                //Walk Left
                case VarjoState.WalkLeft:
                    if (Keyboard.GetState().IsKeyDown(input["left"]) && Math.Abs(currSpeed) < Math.Abs(MaxSpeed) && vState != VerticalState.Swimming)
                    {
                        currSpeed -= Accel;
                    }
                    else if (currSpeed < 0 && Keyboard.GetState().IsKeyUp(input["left"]) && vState != VerticalState.Swimming)
                    {
                        currSpeed += Decel;
                    }
                    else if (currSpeed == 0)
                    {
                        state = VarjoState.FaceLeft;
                    }
                    else if (Keyboard.GetState().IsKeyUp(input["left"]))
                    {
                        currSpeed = 0;
                    }

                    if (SingleKeyPress(input["fade"]) && canDash)
                    {
                        state = VarjoState.FadeLeft;
                    }
                    break;

                case VarjoState.WalkRight:
                    if (Keyboard.GetState().IsKeyDown(input["right"]) && Math.Abs(currSpeed) < Math.Abs(MaxSpeed) && vState != VerticalState.Swimming)
                    {
                        currSpeed += Accel;
                    }
                    else if (currSpeed > 0 && Keyboard.GetState().IsKeyUp(input["right"]) && vState != VerticalState.Swimming)
                    {
                        currSpeed -= Decel;
                    }
                    else if (currSpeed == 0)
                    {
                        state = VarjoState.FaceRight;
                    }
                    else if (Keyboard.GetState().IsKeyUp(input["right"]))
                    {
                        currSpeed = 0;
                    }

                    if (SingleKeyPress(input["fade"]) && canDash)
                    {
                        state = VarjoState.FadeRight;
                    }
                    break;

                case VarjoState.FadeLeft:
                    canDash = false;
                    float theta;
                    if (Keyboard.GetState().IsKeyDown(input["up"]) && Keyboard.GetState().IsKeyUp(input["left"]))
                    {
                        theta = (float)Math.PI / 2;
                    }
                    else if (Keyboard.GetState().IsKeyDown(input["left"]) && Keyboard.GetState().IsKeyDown(input["up"]))
                    {
                        theta = (float)Math.PI / 4;
                    }
                    else
                    {
                        theta = 0;
                    }

                    globalPosition.X -= FadeDistance * (float)Math.Cos(theta);
                    globalPosition.Y -= FadeDistance * (float)Math.Sin(theta);

                    currSpeed -= (FadeDistance * (float)Math.Cos(theta)) / 10;
                    currJumpSpeed += (FadeDistance * (float)Math.Sin(theta)) / 10;

                    foreach (Shadow shadow in Platform.GlobalShadows)
                    {
                        if (shadow.WithinShadow(GlobalRectangle))
                        {
                            vState = VerticalState.Swimming;
                            break;
                        }
                    }

                    state = VarjoState.WalkLeft;

                    break;

                case VarjoState.FadeRight:
                    canDash = false;
                    float alpha;
                    if (Keyboard.GetState().IsKeyDown(input["up"]) && Keyboard.GetState().IsKeyUp(input["right"]))
                    {
                        alpha = (float)Math.PI / 2;
                    }
                    else if (Keyboard.GetState().IsKeyDown(input["right"]) && Keyboard.GetState().IsKeyDown(input["up"]))
                    {
                        alpha = (float)Math.PI / 4;
                    }
                    else
                    {
                        alpha = 0;
                    }

                    globalPosition.X += FadeDistance * (float)Math.Cos(alpha);
                    globalPosition.Y -= FadeDistance * (float)Math.Sin(alpha);

                    currSpeed += (FadeDistance * (float)Math.Cos(alpha)) / 10;
                    currJumpSpeed += (FadeDistance * (float)Math.Sin(alpha)) / 10;

                    foreach (Shadow shadow in Platform.GlobalShadows)
                    {
                        if (shadow.WithinShadow(GlobalRectangle))
                        {
                            vState = VerticalState.Swimming;
                            break;
                        }
                    }

                    state = VarjoState.WalkRight;
                    break;
            }

            if (SingleKeyPress(input["jump"]) && (currJumpSpeed == 0 || vState == VerticalState.None) && vState != VerticalState.Swimming)
            {
                vState = VerticalState.Jump;
                currJumpSpeed += 3;
            }
            if (currJumpSpeed == 0 || vState == VerticalState.None)
            {
                canDash = true;
            }

            switch (vState)
            {
                case VerticalState.Jump:
                    if (currJumpSpeed < MaxJumpSpeed && Keyboard.GetState().IsKeyDown(input["jump"]))
                    {
                        currJumpSpeed += JumpAccel;
                        globalPosition.Y -= currJumpSpeed;
                    }
                    else
                    {
                        vState = VerticalState.Falling;
                    }
                    break;

                case VerticalState.Falling:
                    if (currJumpSpeed > MaxFallSpeed)
                    {
                        currJumpSpeed -= JumpDecel;
                    }

                    if (Math.Abs(currSpeed) > 10 || Math.Abs(currJumpSpeed) > 10)
                    {
                        foreach (Shadow shadow in Platform.GlobalShadows)
                        {
                            if (shadow.WithinShadow(GlobalRectangle))
                            {
                                vState = VerticalState.Swimming;
                                break;
                            }
                        }
                    }
                    break;

                case VerticalState.Swimming:
                    if (Keyboard.GetState().IsKeyDown(input["up"]) && Math.Abs(currJumpSpeed) < MaxJumpSpeed)
                    {
                        currJumpSpeed += SwimAccel;
                    }
                    else if (Keyboard.GetState().IsKeyDown(input["down"]) && Math.Abs(currJumpSpeed) < MaxJumpSpeed)
                    {
                        currJumpSpeed -= SwimAccel;
                    }
                    else if (currJumpSpeed > 0)
                    {
                        currJumpSpeed -= SwimDecel;
                    }
                    else if (currJumpSpeed < 0)
                    {
                        currJumpSpeed += SwimDecel;
                    }

                    if (Keyboard.GetState().IsKeyDown(input["left"]) && Math.Abs(currSpeed) < MaxSpeed)
                    {
                        currSpeed -= SwimAccel;
                    }
                    else if (Keyboard.GetState().IsKeyDown(input["right"]) && Math.Abs(currSpeed) < MaxSpeed)
                    {
                        currSpeed += SwimAccel;
                    }
                    else if (currSpeed > 0)
                    {
                        currSpeed -= SwimDecel;
                    }
                    else if (currSpeed < 0)
                    {
                        currSpeed += SwimDecel;
                    }
                    break;

                default:
                    currJumpSpeed = 0;

                    break;
            }

            bool chk = false;
            foreach (Shadow shadow in Platform.GlobalShadows)
            {
                if (!chk && shadow.WithinShadow(GlobalRectangle))
                {
                    chk = true;
                }
            }
            if (!chk && vState == VerticalState.Swimming)
            {
                vState = VerticalState.Falling;
                float theta = (float)Math.Atan2(currJumpSpeed, currSpeed);
                currJumpSpeed += ShadowBoost * (float)Math.Sin(theta);
                currSpeed += ShadowBoost * (float)Math.Cos(theta);
            }

            bool check;
            Rectangle prevRectangle = GlobalRectangle;
            if (check = !CollisionDetection(parentNode.Entities) && GlobalRectangle.X == prevRectangle.X)
            {
                globalPosition.X += currSpeed;
            }
            if (check && GlobalRectangle.Y == prevRectangle.Y)
            {
                globalPosition.Y -= currJumpSpeed;
            }

            screenPosition = Screen.Instance.ConvertToScreenPos(globalPosition);
            previousKBState = Keyboard.GetState();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case VarjoState.FaceLeft:
                    DrawStanding(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;
                case VarjoState.FaceRight:
                    DrawStanding(SpriteEffects.None, spriteBatch);
                    break;

                case VarjoState.WalkLeft:
                    DrawWalking(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case VarjoState.WalkRight:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;

                case VarjoState.FadeLeft:
                    DrawFading(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;
                case VarjoState.FadeRight:
                    DrawFading(SpriteEffects.None, spriteBatch);
                    break;
            }

        }

        public bool OnScreen()
        {
            return Screen.Instance.ConvertToScreenpos(GlobalRectangle).Y < Screen.Instance.Height;
        }

        public void ResetPos()
        {
            globalPosition = initialPos;
        }

        public override int IsColliding(Entity other)
        {
            Rectangle newRect = new Rectangle((int)(GlobalRectangle.X + currSpeed), (int)(GlobalRectangle.Y - currJumpSpeed), GlobalRectangle.Width, GlobalRectangle.Height);
            if (newRect.Intersects(other.GlobalRectangle))
            {
                if (GlobalRectangle.Bottom < other.GlobalRectangle.Top + 5 && vState == VerticalState.Falling)
                {
                    return 1;
                }
                else if (GlobalRectangle.Right < other.GlobalRectangle.Left + 5)
                {
                    return 4;
                }
                else if (GlobalRectangle.Left > other.GlobalRectangle.Right - 5)
                {
                    return 2;
                }
                else if (GlobalRectangle.Top > other.GlobalRectangle.Bottom - 5)
                {
                    return 3;
                }
            }
            newRect = new Rectangle(GlobalRectangle.X, GlobalRectangle.Y + 10, GlobalRectangle.Width, GlobalRectangle.Height);
            if (!newRect.Intersects(other.GlobalRectangle))
            {
                return 0;
            }
            return -1;
        }

        public bool CollisionDetection(List<Entity> entities)
        {
            bool check = false, vCheck = false;
            foreach (Entity entity in entities)
            {
                if (!(entity is Varjo))
                {
                    switch (IsColliding(entity))
                    {
                        case 1:
                            check = true;
                            vCheck = true;
                            globalPosition = new Vector2(GlobalPos.X, entity.GlobalPos.Y - Height - 1);
                            currJumpSpeed = 0;
                            if (vState != VerticalState.Swimming)
                            {
                                vState = VerticalState.None;
                            }
                            break;

                        case 2:
                            check = true;
                            vCheck = true;
                            globalPosition = new Vector2(entity.GlobalPos.X + entity.Width + 1, GlobalPos.Y);
                            break;

                        case 3:
                            check = true;
                            vCheck = true;
                            globalPosition = new Vector2(GlobalPos.X, entity.GlobalPos.Y + entity.Height + 1);
                            if (vState != VerticalState.Swimming)
                            {
                                vState = VerticalState.Falling;
                            }
                            break;

                        case 4:
                            check = true;
                            vCheck = true;
                            globalPosition = new Vector2(entity.GlobalPos.X - Width - 1, GlobalPos.Y);
                            break;

                        case 0:

                            break;

                        default:
                            vCheck = true;
                            currJumpSpeed = 0;
                            break;
                    }
                }
            }
            if (GlobalRectangle.X < 0)
            {
                globalPosition.X = 0;
            }
            if (!vCheck && vState != VerticalState.Jump && vState != VerticalState.Swimming)
            {
                vState = VerticalState.Falling;
            }
            return check;
        }

        public override bool DoesContainPoint(Vector2 point)
        {
            if (GlobalRectangle.Contains(point))
            {
                return true;
            }
            return false;
        }

        //Private Methods

        private void DrawStanding(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,                    // - The texture to draw
                screenPosition,                       // - The location to draw on the screen
                new Rectangle(                  // - The "source" rectangle
                    0,                          //   - This rectangle specifies
                    VarjoRectOffsetY,           //	   where "inside" the texture
                    VarjoRectWidth,             //     to get pixels (We don't want to
                    VarjoRectHeight),           //     draw the whole thing)
                Color.White,                    // - The color
                0,                              // - Rotation (none currently)
                Vector2.Zero,                   // - Origin inside the image (top left)
                scale,                           // - Scale (100% - no change)
                flipSprite,                     // - Can be used to flip the image
                0);                             // - Layer depth (unused)
        }
        private void DrawWalking(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,                    // - The texture to draw
                screenPosition,                       // - The location to draw on the screen
                new Rectangle(                  // - The "source" rectangle
                    frame * VarjoRectWidth,     //   - This rectangle specifies
                    VarjoRectOffsetY,           //	   where "inside" the texture
                    VarjoRectWidth,             //     to get pixels (We don't want to
                    VarjoRectHeight),           //     draw the whole thing)
                Color.White,                    // - The color
                0,                              // - Rotation (none currently)
                Vector2.Zero,                   // - Origin inside the image (top left)
                scale,                           // - Scale (100% - no change)
                flipSprite,                     // - Can be used to flip the image
                0);                             // - Layer depth (unused)
        }
        private void DrawFading(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,                    // - The texture to draw
                screenPosition,                       // - The location to draw on the screen
                new Rectangle(                  // - The "source" rectangle
                    0,                          //   - This rectangle specifies
                    VarjoRectOffsetY,           //	   where "inside" the texture
                    VarjoRectWidth,             //     to get pixels (We don't want to
                    VarjoRectHeight),           //     draw the whole thing)
                Color.White,                    // - The color
                0,                              // - Rotation (none currently)
                Vector2.Zero,                   // - Origin inside the image (top left)
                scale,                           // - Scale (100% - no change)
                flipSprite,                     // - Can be used to flip the image
                0);                             // - Layer depth (unused)
        }
        private void DrawJump(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,                    // - The texture to draw
                screenPosition,                       // - The location to draw on the screen
                new Rectangle(                  // - The "source" rectangle
                    VarjoRectWidth,                          //   - This rectangle specifies
                    VarjoRectOffsetY,           //	   where "inside" the texture
                    VarjoRectWidth,             //     to get pixels (We don't want to
                    VarjoRectHeight),           //     draw the whole thing)
                Color.White,                    // - The color
                0,                              // - Rotation (none currently)
                Vector2.Zero,                   // - Origin inside the image (top left)
                scale,                           // - Scale (100% - no change)
                flipSprite,                     // - Can be used to flip the image
                0);                             // - Layer depth (unused)
        }
        private void DrawSwimming(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,                    // - The texture to draw
                screenPosition,                       // - The location to draw on the screen
                new Rectangle(                  // - The "source" rectangle
                    0,                          //   - This rectangle specifies
                    VarjoRectOffsetY,           //	   where "inside" the texture
                    VarjoRectWidth,             //     to get pixels (We don't want to
                    VarjoRectHeight),           //     draw the whole thing)
                Color.White,                    // - The color
                0,                              // - Rotation (none currently)
                Vector2.Zero,                   // - Origin inside the image (top left)
                scale,                           // - Scale (100% - no change)
                flipSprite,                     // - Can be used to flip the image
                0);                             // - Layer depth (unused)
        }
        private void SetControls(Keys jump, Keys up, Keys left, Keys right, Keys down, Keys fade)
        {
            input["jump"] = jump;
            input["up"] = up;
            input["left"] = left;
            input["right"] = right;
            input["down"] = down;
            input["fade"] = fade;
        }
        private bool SingleKeyPress(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && previousKBState.IsKeyUp(key);
        }
    }
}
