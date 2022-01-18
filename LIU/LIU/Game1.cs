using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LIU
{

    enum GameState
    {
        MainMenu,
        Play,
        Pause,
        NextLevel,
        GameOver,
    }

    public class Game1 : Game
    {
        public static float WorldScalar;

        GameState gameState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Varjo varjo;
        List<Entity> entities;
        CollisionDetection collisionDetection;
        KeyboardState prevKB;

        SpriteFont font;
        UIPanel mainMenu, pauseMenu;
        Texture2D menuSheet;

        BasicEffect basicEffect;
        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection;
        Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
        VertexBuffer vertexBuffer;
        public static Effect effect1;

        RenderTarget2D lightsTarget, mainTarget;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            entities = new List<Entity>();
            gameState = GameState.MainMenu;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            WorldScalar = GraphicsDevice.Viewport.Height / 20;
            Screen.Instance.GetWidthAndHeight(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            basicEffect = new BasicEffect(GraphicsDevice);
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Texture setup
            LevelLoader.platformSprite = Content.Load<Texture2D>("platform");
            LevelLoader.lightMask = Content.Load<Texture2D>("lightMask");
            effect1 = Content.Load<Effect>("lighteffect");
            menuSheet = Content.Load<Texture2D>("MenuSpriteSheet");

            //Rendering setup
            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(
            GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(
            GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);

            //Entity Setup
            Vector2 varjoLoc = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2); //location
            Texture2D spriteSheet = Content.Load<Texture2D>("VictimsAndVillagers");
            varjo = new Varjo(spriteSheet, varjoLoc, WorldScalar / 40, ShadowState.Neutral, GraphicsDevice.Viewport.Height - 50, 200);
            Screen.Instance.GetCharacter(varjo);
            entities = LevelLoader.LoadLevelFromFile("test", GraphicsDevice);
            entities.Add(varjo);

            foreach (Entity entity in entities)
            {
                if (entity is Light)
                {
                    Light.SceneLights.Add((Light)entity);
                }
            }

            //Collision Setup
            collisionDetection = new CollisionDetection(20000, 20000, entities);

            //UI Setup
            font = Content.Load<SpriteFont>("MyFont");

            ////Initialize the main Menu ////
            { //Use to collapse the pause area while you don't need to use it
                mainMenu = new UIPanel(
                    new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                    menuSheet,
                    0,
                    0, 0,   //the picture starts at the origin of the sheet
                    902, 783);  //the image size
                    //0 0 0 902 793

                //adds the play button to the main menu
                mainMenu.AddComponent(new UIButton(
                    PlayButton_Click,
                    Button_Hover,
                    "",
                    new Rectangle(       //places the button on the screen
                        (int)(GraphicsDevice.Viewport.Width / 2 - (245.0 / 2)), (int)(GraphicsDevice.Viewport.Height * (4.0 / 8)),
                        GraphicsDevice.Viewport.Width / 8, GraphicsDevice.Viewport.Height / 10),
                    menuSheet,
                    0,
                    font,
                    Color.White,
                    265,      //hover offset if the y difference between the base button and the highlighted version.  
                    TextAlign.Center,
                    903, 0,     //gets the picture of the button UPPER LEFT PIXEL
                    245, 84));  //from the texture WIDTH AND HEIGHT
                    //265 903 0 245 84

                //adds the options button to the main menu
                mainMenu.AddComponent(new UIButton(
                    OptionsButton_Click,
                    Button_Hover,
                    "",
                    new Rectangle(       //places the button on the screen
                        (int)(GraphicsDevice.Viewport.Width / 2 - (245.0 / 2)), (int)(GraphicsDevice.Viewport.Height * (5.0 / 8)),
                        GraphicsDevice.Viewport.Width / 8, GraphicsDevice.Viewport.Height / 10),
                    menuSheet,
                    0,
                    font,
                    Color.White,
                    265,      //hover offset if the y difference between the base button and the highlighted version.
                    TextAlign.Center,
                    903, 86,     //gets the picture of the button 
                    245, 86));  //from the texture
                    // 265 903 85 245 84

                //creates the click button in the main menu
                mainMenu.AddComponent(new UIButton(
                    QuitButton_Click,
                    Button_Hover,
                    "",
                    new Rectangle(       //places the button on the screen
                        (int)(GraphicsDevice.Viewport.Width / 2 - (245.0 / 2)), (int)(GraphicsDevice.Viewport.Height * (6.0 / 8)),
                        GraphicsDevice.Viewport.Width / 8, GraphicsDevice.Viewport.Height / 10),
                    menuSheet,
                    0,
                    font,
                    Color.White,
                    265,      //hover offset if the y difference between the base button and the highlighted version.
                    TextAlign.Center,
                    903, 174,     //gets the picture of the button 
                    245, 71));  //from the texture
                    //265 903 170 245 75

                //adds the title to the main menu
                mainMenu.AddComponent(new UIButton(
                    TitleButton_Click,
                    Button_Hover,
                    "",
                    new Rectangle(       //places the button on the screen
                        (int)(GraphicsDevice.Viewport.Width / 2 - (245.0 / 2)), (int)(GraphicsDevice.Viewport.Height * (2.0 / 8)),
                        GraphicsDevice.Viewport.Width / 3, GraphicsDevice.Viewport.Height / 10),
                    menuSheet,
                    0,
                    font,
                    Color.White,
                    0,      //hover offset if the y difference between the base button and the highlighted version.
                    TextAlign.Center,
                    0, 799,     //gets the picture of the button 
                    703, 155));  //from the texture
                    // 0, 0, menuSheet.Height - 155, 705 155 
            }
            /////Initialize the pause menu /////
            { //Use to collapse the pause area while you don't need to use it
                pauseMenu = new UIPanel(
                    new Rectangle(0, 0,
                        GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                    null,
                    0,
                    0, 0,   //the picture starts at the origin of the sheet
                    0, 0);  //the image size
                    // 0 0 0 0 0 

                //adds the resume button to the pause menu
                pauseMenu.AddComponent(new UIButton(
                    PlayButton_Click,
                    Button_Hover,
                    "",
                    new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (245.0 / 2)), (int)(GraphicsDevice.Viewport.Height * (4.0 / 8)),
                        GraphicsDevice.Viewport.Width / 8, GraphicsDevice.Viewport.Height / 10),
                    menuSheet,
                    0,
                    font,
                    Color.White,
                    89,      //hover offset if the y difference between the base button and the highlighted version.
                    TextAlign.Center,
                    903, 515,
                    245, 84));
                    // 89 903 515 245 84

                //adds the options button to the pause menu
                pauseMenu.AddComponent(new UIButton(
                    OptionsButton_Click,
                    Button_Hover,
                    "",
                    new Rectangle(       //places the button on the screen
                        (int)(GraphicsDevice.Viewport.Width / 2 - (245.0 / 2)), (int)(GraphicsDevice.Viewport.Height * (5.0 / 8)),
                        GraphicsDevice.Viewport.Width / 8, GraphicsDevice.Viewport.Height / 10),
                    menuSheet,
                    0,
                    font,
                    Color.White,
                    265,      //hover offset if the y difference between the base button and the highlighted version.
                    TextAlign.Center,
                    903, 86,     //gets the picture of the button 
                    245, 86));  //from the texture
                    // 265 903 85 245 84

                //creates the click button in the main menu
                pauseMenu.AddComponent(new UIButton(
                    QuitButton_Click,
                    Button_Hover,
                    "",
                    new Rectangle(       //places the button on the screen
                        (int)(GraphicsDevice.Viewport.Width / 2 - (245.0 / 2)), (int)(GraphicsDevice.Viewport.Height * (6.0 / 8)),
                        GraphicsDevice.Viewport.Width / 8, GraphicsDevice.Viewport.Height / 10),
                    menuSheet,
                    0,
                    font,
                    Color.White,
                    265,      //hover offset if the y difference between the base button and the highlighted version.
                    TextAlign.Center,
                    903, 174,     //gets the picture of the button 
                    245, 71));  //from the texture
                    // 265 903 170 245 84
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Play:
                    varjo.UpdateAnimation(gameTime);
                    varjo.Update();
                    if (!varjo.OnScreen())
                    {
                        gameState = GameState.MainMenu;
                        varjo.ResetPos();
                    }

                    Platform.UpdateShadows();

                    foreach (Light light in Light.SceneLights)
                    {
                        light.Update();
                    }

                    collisionDetection.Update();

                    varjo.UpdatePrev();

                    if (prevKB.IsKeyUp(Keys.Escape) && Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        gameState = GameState.Pause;
                    }
                    break;

                case GameState.Pause:
                    pauseMenu.Update();
                    if (prevKB.IsKeyUp(Keys.Escape) && Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        Exit();
                    }
                    break;

                case GameState.MainMenu:
                    mainMenu.Update();
                    if (prevKB.IsKeyUp(Keys.Escape) && Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        Exit();
                    }
                    break;

                default:
                    break;
            }

            prevKB = Keyboard.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Play:
                    GraphicsDevice.SetRenderTarget(lightsTarget);
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                    foreach (Light light in Light.SceneLights)
                    {
                        light.Draw(spriteBatch);
                    }
                    spriteBatch.End();


                    GraphicsDevice.SetRenderTarget(mainTarget);
                    GraphicsDevice.Clear(Color.White);
                    spriteBatch.Begin();

                    spriteBatch.End();

                    GraphicsDevice.SetRenderTarget(null);
                    GraphicsDevice.Clear(Color.White);

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    effect1.Parameters["lightMask"].SetValue(lightsTarget);
                    effect1.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
                    spriteBatch.End();

                    basicEffect.World = Matrix.Identity;
                    basicEffect.View = Matrix.Identity;
                    basicEffect.Projection = projection;

                    basicEffect.VertexColorEnabled = true;

                    RasterizerState rasterizerState = new RasterizerState();
                    rasterizerState.CullMode = CullMode.None;
                    GraphicsDevice.RasterizerState = rasterizerState;

                    Shadow.DrawShadows(basicEffect, GraphicsDevice, vertexBuffer);

                    //Draw in this spriteBatch
                    spriteBatch.Begin();

                    collisionDetection.Draw(spriteBatch);

                    spriteBatch.End();
                    break;

                case GameState.Pause:
                    GraphicsDevice.SetRenderTarget(lightsTarget);
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                    foreach (Light light in Light.SceneLights)
                    {
                        light.Draw(spriteBatch);
                    }
                    spriteBatch.End();


                    GraphicsDevice.SetRenderTarget(mainTarget);
                    GraphicsDevice.Clear(Color.White);

                    GraphicsDevice.SetRenderTarget(null);
                    GraphicsDevice.Clear(Color.White);

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    effect1.Parameters["lightMask"].SetValue(lightsTarget);
                    effect1.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
                    spriteBatch.End();

                    basicEffect.World = Matrix.Identity;
                    basicEffect.View = Matrix.Identity;
                    basicEffect.Projection = projection;

                    basicEffect.VertexColorEnabled = true;

                    rasterizerState = new RasterizerState();
                    rasterizerState.CullMode = CullMode.None;
                    GraphicsDevice.RasterizerState = rasterizerState;

                    Shadow.DrawShadows(basicEffect, GraphicsDevice, vertexBuffer);

                    //Draw in this spriteBatch
                    spriteBatch.Begin();

                    collisionDetection.Draw(spriteBatch);

                    spriteBatch.End();

                    spriteBatch.Begin();
                    pauseMenu.Draw(spriteBatch);
                    spriteBatch.End();
                    break;

                case GameState.MainMenu:
                    GraphicsDevice.Clear(Color.DimGray);
                    spriteBatch.Begin();
                    mainMenu.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);

        }

        public void PlayButton_Click(object sender)
        {
            if (sender is UIButton)
            {
                gameState = GameState.Play;
            }
        }

        public void Button_Hover(object sender)
        {
            if (sender is UIButton)
            {
                UIButton button = (UIButton)sender;
                button.isHover = true;
            }
        }

        //what to do on the click of the quit button
        public void QuitButton_Click(object sender)
        {
            if (sender is UIButton)
            {
                Exit();
            }
        }

        // creates the title as a button. potential easter egg
        public void TitleButton_Click(object sender)
        {

        }

        //what to do when the options button is hit.
        public void OptionsButton_Click(object sender)
        {

        }
    }
}

