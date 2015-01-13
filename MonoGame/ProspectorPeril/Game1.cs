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

/*
 * All projectiles are one frame per 0.07 secs
Launcher is one frame per 0.1 secs
First frame of barrel exploding anim is same as first frame of normal (so I just don't use the second). 2nd frame of exploding is red state. 3rd frame and on is exploding animation.
Prospector waits 0.1 secs between frames in both launch and attack anims
 */

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
        Viewport GraphicsViewport;
        Vector2 ViewportCenter;
        MouseState lastMouseState;
        #endregion

        #region Player variables
        Player player;
        PlayerState playerState;        
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
        Sprite launcher;
        Sprite fire;
        Sprite hudContainer;
        Sprite[] hearts = new Sprite[3];

        float scrollRate = 0.25f;
        float scrollY;

        int speed = 0;
        int speedDecay = 1;
        float speedTimer = 2000;
        int lives = 3;

        // Enemy variables
        List<Sprite> enemies = new List<Sprite>();
        
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

            GraphicsViewport = graphics.GraphicsDevice.Viewport;
            ViewportCenter = new Vector2(GraphicsViewport.Width / 2, GraphicsViewport.Height / 2);

            base.Initialize();
        }

        void CreatePlayer()
        {   
            string[] playerTextures = {   "character_prospector/PPeril_character_Static.png", 
                                          "character_prospector/PPeril_character_launch01.png", 
                                          "character_prospector/PPeril_character_launch02.png", 
                                          "character_prospector/PPeril_character_float.png", 
                                          "character_prospector/PPeril_character_attack01.png", 
                                          "character_prospector/PPeril_character_attack02.png"                                           
                                      };

            var textures = new List<Texture2D>();

            foreach(var textureString in playerTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            player = new Player(textures);
            player.AddAnimation("Idle", new int[] {0}, 0);
            player.AddAnimation("Launch", new int[] { 1, 2 }, 100);
            player.AddAnimation("Float", new int[] {3}, 0);
            player.AddAnimation("Attack", new int[] { 4, 5 }, 100);

            player.Scale = new Vector2(0.6f, 0.6f);
            player.Position.X = ViewportCenter.X - player.SpriteCenter.X;
            player.Position.Y = 116;
        }

        void CreateLauncher()
        {
            string[] launcherTextures = {   "launcher/launcher01.png", 
                                          "launcher/launcher02.png", 
                                          "launcher/launcher03.png", 
                                          "launcher/launcher04.png", 
                                          "launcher/launcher05.png", 
                                          "launcher/launcher06.png",
                                          "launcher/launcher07.png" 
                                      };

            var textures = new List<Texture2D>();

            foreach (var textureString in launcherTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            launcher = new Sprite(textures);
            launcher.Scale = new Vector2(0.75f, 0.75f);
            launcher.Position = new Vector2(36.5f, 106.5f);
            launcher.AddAnimation("Idle", new int[] { 0}, 0);
            launcher.AddAnimation("Launch", new int[] { 0, 1, 2, 3, 4, 5, 6 }, 100);            
        }

        Sprite CreateBarrel()
        {
            string[] barrelTextures = {   "enemy_barrel/enemyBarrel_01.png", 
                                          "enemy_barrel/enemyBarrel_02.png", 
                                          "enemy_barrel/enemyBarrel_03.png", 
                                          "enemy_barrel/enemyBarrel_04.png", 
                                          "enemy_barrel/enemyBarrel_05.png",
                                          "enemy_barrel_exploding/enemyBarellExplode_02.png",
                                          "enemy_barrel_exploding/enemyBarellExplode_03.png",
                                          "enemy_barrel_exploding/enemyBarellExplode_04.png",
                                          "enemy_barrel_exploding/enemyBarellExplode_05.png",
                                          "enemy_barrel_exploding/enemyBarellExplode_06.png",
                                          "enemy_barrel_exploding/enemyBarellExplode_07.png",
                                          "enemy_barrel_exploding/enemyBarellExplode_08.png",
                                      };

            var textures = new List<Texture2D>();

            foreach (var textureString in barrelTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            var barrel = new Barrel(textures);
            barrel.AddAnimation("Idle", new int[] { 0 }, 0);
            barrel.AddAnimation("Break", new int[] { 0, 1, 2, 3, 4 }, 70);
            barrel.AddAnimation("Prime", new int[] { 0, 5, 0, 5, 0, 5 }, 70);
            barrel.AddAnimation("Explode", new int[] { 5, 6, 7, 8, 9, 10, 11}, 70);
            return barrel;
        }

        Sprite CreateRock()
        {
            string[] rockTextures = {   "enemy_rock/enemy_rock01.png", 
                                          "enemy_rock/enemy_rock02.png", 
                                          "enemy_rock/enemy_rock03.png", 
                                          "enemy_rock/enemy_rock04.png", 
                                          "enemy_rock/enemy_rock05.png" 
                                      };

            var textures = new List<Texture2D>();

            foreach (var textureString in rockTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            var rock = new Rock(textures);
            rock.AddAnimation("Idle", new int[] { 0 }, 0);
            rock.AddAnimation("Break", new int[] { 0, 1, 2, 3, 4 }, 70);

            return rock;
        }

        Sprite CreateCart()
        {
            string[] cartTextures = {   "enemy_cart/enemy_cart01.png", 
                                          "enemy_cart/enemy_cart02.png", 
                                          "enemy_cart/enemy_cart03.png", 
                                          "enemy_cart/enemy_cart04.png", 
                                          "enemy_cart/enemy_cart05.png" 
                                      };

            var textures = new List<Texture2D>();

            foreach (var textureString in cartTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            var cart = new Cart(textures);
            cart.AddAnimation("Idle", new int[] { 0 }, 0);
            cart.AddAnimation("Break", new int[] { 0, 1, 2, 3, 4 }, 70);

            return cart;
        }

        void CreateEnemies()
        {
            enemies.Add(CreateRock());
            enemies.Add(CreateBarrel());
            enemies.Add(CreateCart());
        }

        void CreateInterface()
        {
            var splashTex = Content.Load<Texture2D>("Placeholders/GGISplashScreen.png");
            splashScreen = new Sprite(splashTex); 
            
            var playButtonTex = Content.Load<Texture2D>("Placeholders/PlayButton.png");
            playButton = new Sprite(playButtonTex);
            
            var playTextureCenter = new Vector2(playButtonTex.Width / 2f, playButtonTex.Height / 2f);
            playButton.Position = ViewportCenter - playTextureCenter;

            var tintedLayerTex = Content.Load<Texture2D>("Placeholders/TintedLayer.png");
            tintedLayer = new Sprite(tintedLayerTex); 
            
            var hudTex = Content.Load<Texture2D>("Placeholders/HUDContainer.png");
            hudContainer = new Sprite(hudTex);
            hudContainer.Position.Y = GraphicsViewport.Height - hudTex.Height;

            //for(int i = 0; i < 3; i++)
            //{
            //    hearts[i] = new Sprite()
            //}
        }

        public void CreateEnvironment()
        {
            var backgroundTex = Content.Load<Texture2D>("BG/Background.png");
            background = new Sprite(backgroundTex); 
            
            var fireTex = Content.Load<Texture2D>("Placeholders/Ground.png");
            fire = new Sprite(fireTex);
            fire.Position.Y = hudContainer.Position.Y;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
                       
            CreateInterface();
            CreateEnvironment();
            CreatePlayer();
            CreateLauncher();
            //CreateEnemies();            
        }

        void UpdateSplash(GameTime gameTime)
        {
            splashTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (splashTime <= 0)
                gameState++;
        }

        void UpdateInput(KeyboardState currentKeyState, MouseState currentMouseState)
        {
            //if (currentKeyState.IsKeyDown(Keys.W))
            //    player.Position.Y -= 0.5f;
            
            //if (currentKeyState.IsKeyDown(Keys.S))
            //    player.Position.Y += 0.5f;
            
            //if (currentKeyState.IsKeyDown(Keys.A))
            //    player.Position.X -= 0.5f;
            
            //if (currentKeyState.IsKeyDown(Keys.D))
            //    player.Position.X += 0.5f;
            
            #region Mouse Handling
            if (currentMouseState.LeftButton == ButtonState.Pressed)
                lastMouseState = currentMouseState;

            if (currentMouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
            {
                lastMouseState = currentMouseState;

                switch (gameState)
                {
                    case GameState.Splash:                        
                        gameState++;
                        break;
                    case GameState.Menu:                        
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
                launcher.PlayAnimation("Launch");
                player.PlayAnimation("Launch");
                
                playerState = PlayerState.Ascending;
                speed = 45;
                //launcher.Position.Y = GraphicsViewport.Height;
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
                    {
                        playerState = PlayerState.Descending;
                        player.PlayAnimation("Float");
                    }

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
                    launcher.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    hudContainer.Draw(spriteBatch);
                    break;
                case GameState.Running:
                    spriteBatch.Draw(background.Textures[0], background.Position, new Rectangle(0, (int)-scrollY, background.Textures[0].Bounds.Width, background.Textures[0].Bounds.Height), Color.White);
                    launcher.Update(gameTime);
                    launcher.Draw(spriteBatch);

                    player.Update(gameTime);
                    player.Draw(spriteBatch);

                    fire.Update(gameTime);
                    fire.Draw(spriteBatch);

                    foreach (var enemy in enemies)
                    {
                        enemy.Update(gameTime);
                        enemy.Draw(spriteBatch);
                    }

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
