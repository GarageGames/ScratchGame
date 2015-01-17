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
        Random random = new Random();
        #endregion

        #region Player variables
        Player player;        
        #endregion

        #region Menu Variables
        Sprite endScreen;
        Sprite startScreen;
        Sprite splashScreen;
        Sprite playButton;
        float splashTime = 3000;
        #endregion

        #region Game variables
        GameState gameState;
        Sprite background;
        Launcher launcher;
        Explosion fire;
        Sprite hudContainer;
        Sprite[] hearts = new Sprite[3];

        float scrollRate = 0.25f;
        float scrollY;
        
        Sprite[] SpeedDigits = new Sprite[2];
        Sprite Arrow;

        // Enemy variables
        List<Enemy> enemies = new List<Enemy>();
        float spawnTimer = 1000;
        int enemyIndex = 0;
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
            player.AddAnimation("Attack", new int[] { 4, 5 , 3}, 100);

            player.Scale = new Vector2(0.6f, 0.6f);
            player.Position.X = ViewportCenter.X - player.SpriteCenter.X;
            player.Position.Y = 116;
            
            player.EnableCollision();
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

            launcher = new Launcher(textures);
            launcher.Scale = new Vector2(0.75f, 0.75f);
            launcher.Position = new Vector2(36.5f, 106.5f);
            launcher.AddAnimation("Idle", new int[] { 0}, 0);
            launcher.AddAnimation("Launch", new int[] { 0, 1, 2, 3, 4, 5, 6 }, 100);            
        }

        Enemy CreateBarrel()
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

        Enemy CreateRock()
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

        Enemy CreateCart()
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
            cart.Position = cart.SpawnPosition = new Vector2(random.Next(0, 480), 360);
            cart.Velocity = new Vector2(random.Next(1, 2), -2);
            return cart;
        }

        void CreateEnemies()
        {
            var rock = CreateRock();
            rock.SpawnPosition = new Vector2(-200, random.Next(-15, -5));
            rock.SpawnVelocity = new Vector2(random.Next(2, 3), random.Next(2, 3));
            enemies.Add(rock);
            
            enemies.Add(CreateCart());

            rock = CreateRock();
            rock.SpawnPosition = new Vector2(500, random.Next(-15, -5));
            rock.SpawnVelocity = new Vector2(random.Next(-3, -2), random.Next(2, 3));
            enemies.Add(rock);
            
            enemies.Add(CreateCart());

            rock = CreateRock();
            rock.SpawnPosition = new Vector2(-200, random.Next(-15, -5));
            rock.SpawnVelocity = new Vector2(random.Next(2, 3), random.Next(2, 3));
            enemies.Add(rock);

            enemies.Add(CreateCart());            
        }

        void CreateInterface()
        {
            var splashTex = Content.Load<Texture2D>("HUD/splashPage.png");
            splashScreen = new Sprite(splashTex); 
            
            var playButtonTex = Content.Load<Texture2D>("HUD/PlayButton.png");
            playButton = new Sprite(playButtonTex);
            
            var playTextureCenter = new Vector2(playButtonTex.Width / 2f, playButtonTex.Height / 2f);
            playButton.Position = ViewportCenter - playTextureCenter;

            var endScreenTex = Content.Load<Texture2D>("HUD/endScreenOverlay.png");
            endScreen = new Sprite(endScreenTex);
            endScreen.Visible = false;

            var startScreenTex = Content.Load<Texture2D>("HUD/startScreen.png");
            startScreen = new Sprite(startScreenTex);

            var hudTex = Content.Load<Texture2D>("HUD/HUD.png");
            hudContainer = new Sprite(hudTex);
            hudContainer.Position.Y = GraphicsViewport.Height - hudTex.Height;

            for(int i = 0; i < 3; i++)
            {
                var texture = Content.Load<Texture2D>("HUD/heart.png");
                hearts[i] = new Sprite(texture);
                hearts[i].Position.Y = hudContainer.Position.Y;
                hearts[i].Position.X = GraphicsViewport.Bounds.Right / 1.5f - texture.Width * (i + 1);
            }

            List<Texture2D> numberTextures = new List<Texture2D>();

            for (int i = 0; i < 10; i++)
                numberTextures.Add(Content.Load<Texture2D>("HUD/number_" + i + ".png"));

            SpeedDigits[0] = new Sprite(numberTextures);
            SpeedDigits[1] = new Sprite(numberTextures);

            SpeedDigits[0].Frame = SpeedDigits[1].Frame = 0;

            SpeedDigits[0].Position.Y = SpeedDigits[1].Position.Y = hudContainer.Position.Y;
            SpeedDigits[0].Position.X = 100;
            SpeedDigits[1].Position.X = SpeedDigits[0].Position.X + 26;

            Arrow = new Sprite(Content.Load<Texture2D>("HUD/arrow_01.png"));
        }

        public void CreateEnvironment()
        {
            var backgroundTex = Content.Load<Texture2D>("BG/Background.png");
            background = new Sprite(backgroundTex);

            List<Texture2D> fireTextures = new List<Texture2D>();

            for(int i = 1; i < 13; i++)
                fireTextures.Add(Content.Load<Texture2D>("wallOfFire/wallOfFire_" + i + ".png"));

            fire = new Explosion(fireTextures);
            fire.AddAnimation("Idle", new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}, 100, true);
            fire.Position.Y = hudContainer.Position.Y;
            fire.PlayAnimation("Idle");
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
            CreateEnemies();            
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
                        gameState++;
                        splashScreen.Visible = false;
                        break;
                    case GameState.Menu:                        
                        playButton.Visible = false;
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
                
                player.Ascend();
                player.Speed = 25;
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
                player.Attack();

            #endregion
        }        

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.Splash)
            {
                splashTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (splashTime <= 0)
                    gameState++;
            }

            // Get current states of mouse and keyboard
            UpdateInput(Keyboard.GetState(), Mouse.GetState());            

            TimeSpan deltaTime = gameTime.ElapsedGameTime;
            float deltaSeconds = (float)deltaTime.Milliseconds;

            // If the game is running, update the main systems
            if (gameState == GameState.Running)
            {
                if (spawnTimer <= 0)
                {
                    spawnTimer = 2000;

                    if (!enemies[enemyIndex].HasSpawned)
                        enemies[enemyIndex].Spawn();

                    enemyIndex++;

                    if (enemyIndex >= enemies.Count)
                        enemyIndex = 0;
                }
                else
                {
                    spawnTimer -= gameTime.ElapsedGameTime.Milliseconds;
                }

                if (player.Position.Y >= fire.Position.Y && fire.Position.Y <= 122 && fire.Visible)
                {
                    player.Die();
                    gameState = GameState.GameOver;
                    endScreen.Visible = true;
                }

                if (player.Position.Y >= 424 && !player.IsAscending)
                {
                    player.Speed -= 10;
                    player.Ascend();
                }

                launcher.Update(gameTime);
                player.Update(gameTime);
                fire.Update(gameTime);

                foreach (var enemy in enemies)
                {
                    enemy.Update(gameTime);
                    if (enemy.HasSpawned && enemy.Collideable)
                    {
                        var result = enemy.Collides(player);

                        if (result)
                            player.Bounce();
                    }
                }

                if (player.Speed < 10)
                    fire.IsRising = true;
                else
                    fire.IsRising = false;

                scrollY += scrollRate * deltaSeconds;

                SpeedDigits[0].Frame = int.Parse(player.Speed.ToString().Substring(0, 1));

                if (player.Speed > 9)
                {
                    SpeedDigits[1].Visible = true;
                    SpeedDigits[1].Frame = int.Parse(player.Speed.ToString().Substring(1, 1));
                }
                else
                    SpeedDigits[1].Visible = false;               
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
                    startScreen.Draw(spriteBatch);
                    playButton.Draw(spriteBatch);
                    break;
                case GameState.Launch:
                case GameState.GameOver:
                case GameState.Running:
                    spriteBatch.Draw(background.Textures[0], background.Position, new Rectangle(0, (int)-scrollY, background.Textures[0].Bounds.Width, background.Textures[0].Bounds.Height), Color.White);                    
                    launcher.Draw(spriteBatch);
                    player.Draw(spriteBatch);                    
                    fire.Draw(spriteBatch);
                    
                    foreach (var enemy in enemies)
                        enemy.Draw(spriteBatch);                    

                    hudContainer.Draw(spriteBatch);

                    foreach (var digit in SpeedDigits)
                        digit.Draw(spriteBatch);

                    foreach(var heart in hearts)
                        heart.Draw(spriteBatch);

                    endScreen.Draw(spriteBatch);
                    break;                
            }
                        
            // End drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
