#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace ProspectorPeril
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        enum PlayerState
        {
            Idle,
            Ascending,
            Descending,
            Attacking,
            Dead
        };

        enum GameState
        {
            Splash,
            Menu,
            Launch,
            Running,
            GameOver
        };

        #region MonoGame/XNA variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        #endregion

        #region Player variables
        Sprite player;
        PlayerState playerState;
        int playerLives = 3;
        float playerVerticalVelocity = 0.0f;
        #endregion

        #region Menu Variables
        Sprite tintedLayer;
        Sprite splashScreen;
        Sprite playButton;
        float splashTime = 3000;
        #endregion

        #region Game variables
        GameState gameState; 
        Sprite background;
        Sprite ground;
        Sprite fire;
        Sprite hudContainer;
        
        float scrollRate = 0.25f;
        float scrollY;

        int speed = 0;
        int speedDecay = 1;
        float speedTimer = 2000;
               
        // Enemy variables
        //List<Sprite> enemies = new List<Sprite>();
        //int enemyCount = 5;
        
        MouseState lastMouseState;
        #endregion

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Resize window to be 480x360
            graphics.PreferredBackBufferHeight = 360;
            graphics.PreferredBackBufferWidth = 480;
            graphics.ApplyChanges();

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

            #region Load textures
            var playerTex = Content.Load<Texture2D>("MainCharacter.png");
            var splashTex = Content.Load<Texture2D>("GGISplashScreen.png");
            var playButtonTex = Content.Load<Texture2D>("PlayButton.png");
            var tintedLayerTex = Content.Load<Texture2D>("TintedLayer.png");
            var backgroundTex = Content.Load<Texture2D>("BG_rocks.png");
            var hudTex = Content.Load<Texture2D>("HUDContainer.png");
            var groundTex = Content.Load<Texture2D>("Ground.png");
            var fireTex = Content.Load<Texture2D>("Ground.png");
            #endregion

            #region Create menu sprites
            tintedLayer = new Sprite(tintedLayerTex);
            splashScreen = new Sprite(splashTex);
            playButton = new Sprite(playButtonTex);
            #endregion

            #region Center playButton
            var viewportCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            var playTextureCenter = new Vector2(playButtonTex.Width / 2f, playButtonTex.Height / 2f);
            playButton.Position = viewportCenter - playTextureCenter;
            #endregion

            #region Create the game sprites
            background = new Sprite(backgroundTex);

            hudContainer = new Sprite(hudTex);
            hudContainer.Position.Y = graphics.GraphicsDevice.Viewport.Height - hudTex.Height;

            ground = new Sprite(groundTex);
            ground.Position.Y = hudContainer.Position.Y - groundTex.Height;

            fire = new Sprite(fireTex);
            fire.Position.Y = hudContainer.Position.Y;

            player = new Sprite(playerTex);
            var playerTextureCenter = new Vector2(playerTex.Width / 2f, playerTex.Height / 2f);
            player.Position.X = viewportCenter.X - playerTextureCenter.X;
            player.Position.Y = ground.Position.Y - playerTex.Height;
            #endregion
        }

        void UpdateSplash(GameTime gameTime)
        {
            splashTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (splashTime <= 0)
                gameState++;
        }

        void UpdateInput(KeyboardState currentKeyState, MouseState currentMouseState)
        {
            #region Mouse Handling
            if (currentMouseState.LeftButton == ButtonState.Pressed)
                lastMouseState = currentMouseState;

            if (currentMouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
            {
                lastMouseState = currentMouseState;

                switch (gameState)
                {
                    case GameState.Splash:
                        Console.WriteLine("Leaving splash");
                        gameState++;
                        break;
                    case GameState.Menu:
                        Console.WriteLine("Leaving menu");
                        gameState++;
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Keyboard Handling

            // Close the game application
            if (currentKeyState.IsKeyDown(Keys.Escape))
                Exit();
            
            // Up key pressed: Launch the player if they are waiting to be launched
            if (gameState == GameState.Launch && currentKeyState.IsKeyDown(Keys.Up))
            {
                playerState = PlayerState.Ascending;
                speed = 45;
                ground.Position.Y = graphics.GraphicsDevice.Viewport.Height;
                gameState++;
            }

            // If the player has not been launched or the game is over, exit UpdateInput here
            if (gameState != GameState.Running)
                return;

            // Move player left
            if (currentKeyState.IsKeyDown(Keys.Left))
                player.Position.X -= 7;

            // Move player right
            if (currentKeyState.IsKeyDown(Keys.Right))
                player.Position.X += 7;

            // Player attack!
            if (currentKeyState.IsKeyDown(Keys.Down))
                playerState = PlayerState.Attacking;

            #endregion
        }

        void UpdateScrolling(float deltaSeconds)
        {
            // Update the vertical scrolling for the background            
            scrollY += scrollRate * deltaSeconds;
        }

        void UpdatePlayer(GameTime gameTime)
        {
            // Update the player based on his state
            switch (playerState)
            {
                case PlayerState.Idle:
                    break;

                case PlayerState.Ascending:
                    playerVerticalVelocity = -4;

                    if (player.Position.Y <= 25)
                        playerState = PlayerState.Descending;

                    break;

                case PlayerState.Descending:
                    playerVerticalVelocity = 2;
                    speedTimer -= gameTime.ElapsedGameTime.Milliseconds;

                    if (speedTimer <= 0)
                    {
                        speedTimer = 2000;
                        speed -= speedDecay;
                    }

                    break;

                case PlayerState.Attacking:
                    playerVerticalVelocity = 9;
                    break;

                case PlayerState.Dead:
                    break;
            }

            player.Position.Y += playerVerticalVelocity;
        }

        void CheckForLoss()
        {
            if (player.Position.Y >= fire.Position.Y && fire.Visible)
            {
                playerState = PlayerState.Dead;
                gameState = GameState.GameOver;
            }

            if (player.Position.Y >= 424 && playerState != PlayerState.Ascending)
            {
                speed -= 10;
                playerState = PlayerState.Ascending;                
            }            
        }

        void UpdateFire(float deltaSeconds)
        {
            if (speed < 10)
            {
                fire.Position.Y -= deltaSeconds * 0.15f;
                fire.Visible = true;
            }
            else
            {                
                if (fire.Position.Y < hudContainer.Position.Y)
                    fire.Position.Y += deltaSeconds * 0.15f;
                else
                    fire.Visible = false;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.Splash)
                UpdateSplash(gameTime);

            // Get current states of mouse and keyboard
            UpdateInput(Keyboard.GetState(), Mouse.GetState());

            TimeSpan deltaTime = gameTime.ElapsedGameTime;
            float deltaSeconds = (float)deltaTime.Milliseconds;

            // If the game is running, update the main systems
            if (gameState == GameState.Running)
            {
                UpdateScrolling(deltaSeconds);

                UpdatePlayer(gameTime);

                UpdateFire(deltaSeconds);

                CheckForLoss();
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Begin drawing 
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);

            switch(gameState)
            {
                case GameState.Splash:
                    splashScreen.Draw(spriteBatch);
                    break;
                case GameState.Menu:
                    background.Draw(spriteBatch);
                    tintedLayer.Draw(spriteBatch);
                    playButton.Draw(spriteBatch);
                    break;
                case GameState.Launch:
                    background.Draw(spriteBatch);
                    ground.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    hudContainer.Draw(spriteBatch);
                    break;
                case GameState.Running:                    
                    spriteBatch.Draw(background.Textures[0], background.Position, new Rectangle(0, (int)-scrollY, background.Textures[0].Bounds.Width, background.Textures[0].Bounds.Height), Color.White);
                    ground.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    fire.Draw(spriteBatch);

                    //foreach(var enemy in enemies)
                    //    enemy.Draw(spriteBatch);
                    hudContainer.Draw(spriteBatch);
                    break;
                case GameState.GameOver:
                    Console.WriteLine("Game is over, do nothing");
                    break;
            }
                        
            // End drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
