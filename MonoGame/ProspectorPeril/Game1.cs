using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProspectorPeril
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        /// <summary>
        /// Game state enumeration from start (Splash) to finish (End)
        /// </summary>
        enum GameState
        {
            Splash,
            Menu,
            Launch,
            Running,
            GameOver,
            End
        };

        #region MonoGame/XNA variables
        
        // Handles the configuration and management of the graphics device.
        GraphicsDeviceManager graphics;

        // Main drawing object. Enables a group of sprites to be drawn using the same settings.
        SpriteBatch spriteBatch;

        // The graphics device render target to receive draw calls. 
        Viewport GraphicsViewport;

        // The middle position of the viewport
        Vector2 ViewportCenter;
        
        // Last mouse state recorded in Update
        MouseState lastMouseState;        

        // Random number generator (only one instance allowed per thread)
        Random random = new Random();
        #endregion

        #region Interface Variables
        Sprite splashScreen; 
        float splashTime = 3000; 
        
        // Start and end screens
        Sprite endScreen;
        Sprite startScreen;
        
        // Buttons
        Sprite playButton;
        Sprite nextButton;
        Sprite scoreButton;
        Sprite restartButton;
        
        // Hud (container, current speed, lives remaining, current time)
        Sprite hudContainer;
        Sprite[] SpeedDigits = new Sprite[2]; 
        Sprite[] hearts = new Sprite[3];
        Sprite[] timer = new Sprite[4];        
        #endregion

        #region Game variables
        
        // Current state of the game
        GameState gameState; 
        
        // Main prospector player
        Player player;        

        // List of all eneemies to track, draw, update, etc
        List<Enemy> rocks = new List<Enemy>();
        List<Enemy> carts = new List<Enemy>();
        List<Enemy> barrels = new List<Enemy>();
        
        // Current enemy index in enemies list
        int cartIndex = 0;
        int rockIndex = 0;
        int barrelIndex = 0;

        // Fastest speed player has gotten
        int topSpeed = 0;

        // The longest time a player has stayed alive
        float topTime = 0.0f;

        // Amount of time that has elapsed since launching
        float currentTime = 0.0f;

        // Scrolling background
        Sprite background;

        // The rate at which the background image is scrolled per ms
        float backgroundScrollSpeed = 0.25f;

        // The current background scroll value (updated each frame)
        float currentBackgroundScroll;

        // Launcher object
        Launcher launcher;

        // The deadly explosion
        Explosion explosion;
                
        // Array of arrow objects, show up only when player collides with enemy
        OneUp[] arrows = new OneUp[3];

        // Available index into arrows array
        int availableOneUp = 0;

        // Enemy variables
        
        #endregion

        #region Audio variables
        SoundEffectInstance currentMusicInstance;
        SoundEffect menuMusic;
        SoundEffect backgroundMusic;
        SoundEffect barrelBreak;
        SoundEffect cartBreak;
        SoundEffect death;
        SoundEffect fall;
        SoundEffect launch;
        SoundEffect rockBreak;
        SoundEffect barrelExplode;
        #endregion

        /// <summary>
        /// Main game constructor
        /// </summary>
        public Game1() : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
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
            graphics.PreferredBackBufferWidth = 480; 
            graphics.PreferredBackBufferHeight = 360;            
            graphics.ApplyChanges();

            // Store the viewport information, used elsewhere
            GraphicsViewport = graphics.GraphicsDevice.Viewport;
            ViewportCenter = new Vector2(240, 180);

            base.Initialize();
        }

        /// <summary>
        /// Create a single Player
        /// </summary>
        void CreatePlayer()
        {
            // Load all the textures
            string[] playerTextures = {   "character_prospector/PPeril_character_Static", 
                                          "character_prospector/PPeril_character_launch01", 
                                          "character_prospector/PPeril_character_launch02", 
                                          "character_prospector/PPeril_character_float", 
                                          "character_prospector/PPeril_character_attack01", 
                                          "character_prospector/PPeril_character_attack02"                                           
                                      };
            var textures = new List<Texture2D>();
            foreach (var textureString in playerTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            // Create the new Player and add animations
            player = new Player(textures);            
            player.AddAnimation("Idle", new int[] { 0 }, 0);
            player.AddAnimation("Launch", new int[] { 1, 2 }, 100);
            player.AddAnimation("Float", new int[] { 3 }, 0);
            player.AddAnimation("Attack", new int[] { 4, 5, 3 }, 100);

            // Scale and reposition player
            player.Scale = new Vector2(0.6f, 0.6f);
            player.Position = new Vector2(178, 100);

            // Turn on its collision
            player.EnableCollision();
        }

        /// <summary>
        /// Create the Launcher
        /// </summary>
        void CreateLauncher()
        {
            // Load all the textures
            string[] launcherTextures = {   "launcher/launcher01", 
                                          "launcher/launcher02", 
                                          "launcher/launcher03", 
                                          "launcher/launcher04", 
                                          "launcher/launcher05", 
                                          "launcher/launcher06",
                                          "launcher/launcher07" 
                                      };
            var textures = new List<Texture2D>();
            foreach (var textureString in launcherTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            // Create the new Launcher and add animations
            launcher = new Launcher(textures);
            launcher.AddAnimation("Idle", new int[] { 0 }, 0);
            launcher.AddAnimation("Launch", new int[] { 0, 1, 2, 3, 4, 5, 6 }, 100);

            // Scale and reposition
            launcher.Scale = new Vector2(0.75f, 0.75f);
            launcher.Position = new Vector2(8, 85);
        }

        /// <summary>
        ///  Create a new Barrel enemy
        /// </summary>
        /// <returns>The new Barrel *Enemy*</returns>
        Enemy CreateBarrel()
        {
            // Load all the Barrel textures
            string[] barrelTextures = {   "enemy_barrel/enemyBarrel_01", 
                                          "enemy_barrel/enemyBarrel_02", 
                                          "enemy_barrel/enemyBarrel_03", 
                                          "enemy_barrel/enemyBarrel_04", 
                                          "enemy_barrel/enemyBarrel_05",
                                          "enemy_barrel_exploding/enemyBarellExplode_02",
                                          "enemy_barrel_exploding/enemyBarellExplode_03",
                                          "enemy_barrel_exploding/enemyBarellExplode_04",
                                          "enemy_barrel_exploding/enemyBarellExplode_05",
                                          "enemy_barrel_exploding/enemyBarellExplode_06",
                                          "enemy_barrel_exploding/enemyBarellExplode_07",
                                          "enemy_barrel_exploding/enemyBarellExplode_08",
                                      };
            var textures = new List<Texture2D>();
            foreach (var textureString in barrelTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            // Create a new Barrel and add its animations
            var barrel = new Barrel(textures);
            barrel.AddAnimation("Idle", new int[] { 0 }, 0);
            barrel.AddAnimation("Break", new int[] { 0, 1, 2, 3, 4 }, 70);
            barrel.AddAnimation("Prime", new int[] { 0, 5, 0, 5, 0, 5 }, 70, true);
            barrel.AddAnimation("Explode", new int[] { 5, 6, 7, 8, 9, 10, 11 }, 70);

            // Assign its audio effects
            barrel.BreakSound = barrelBreak;
            barrel.ExplodeSound = barrelExplode;

            // Return the new instance
            return barrel;
        }

        /// <summary>
        ///  Create a new Rock enemy
        /// </summary>
        /// <returns>The new Rock *Enemy*</returns>
        Enemy CreateRock()
        {
            // Load all the Rock textures
            string[] rockTextures = {   "enemy_rock/enemy_rock01", 
                                          "enemy_rock/enemy_rock02", 
                                          "enemy_rock/enemy_rock03", 
                                          "enemy_rock/enemy_rock04", 
                                          "enemy_rock/enemy_rock05" 
                                      };
            var textures = new List<Texture2D>();
            foreach (var textureString in rockTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            // Create a new Rock and add its animations
            var rock = new Rock(textures);
            rock.AddAnimation("Idle", new int[] { 0 }, 0);
            rock.AddAnimation("Break", new int[] { 0, 1, 2, 3, 4 }, 70);

            // Assign its audio effect
            rock.BreakSound = rockBreak;

            // Return the new instance
            return rock;
        }

        /// <summary>
        ///  Create a new Barrel enemy
        /// </summary>
        /// <returns>The new Barrel *Enemy*</returns>
        Enemy CreateCart()
        {
            // Load all the Cart textures
            string[] cartTextures = {   "enemy_cart/enemy_cart01", 
                                          "enemy_cart/enemy_cart02", 
                                          "enemy_cart/enemy_cart03", 
                                          "enemy_cart/enemy_cart04", 
                                          "enemy_cart/enemy_cart05" 
                                      };
            var textures = new List<Texture2D>();
            foreach (var textureString in cartTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            // Create a new Cart and add its animations
            var cart = new Cart(textures);
            cart.AddAnimation("Idle", new int[] { 0 }, 0);
            cart.AddAnimation("Break", new int[] { 0, 1, 2, 3, 4 }, 70);
            
            // Assign its audio effect
            cart.BreakSound = cartBreak;

            // Return the new instance
            return cart;
        }

        /// <summary>
        /// Create all the enemies for the game (three of each type, in varying order)
        /// </summary>
        void CreateEnemies()
        {
            // Create carts
            carts.Add(CreateCart());
            carts.Add(CreateCart());
            carts.Add(CreateCart());
            carts.Add(CreateCart());
            carts.Add(CreateCart());

            // Create rocks
            rocks.Add(CreateRock());
            rocks.Add(CreateRock());
            rocks.Add(CreateRock());
            rocks.Add(CreateRock());
            rocks.Add(CreateRock());

            // Create barrels
            barrels.Add(CreateBarrel());
            barrels.Add(CreateBarrel());
            barrels.Add(CreateBarrel());
            barrels.Add(CreateBarrel());
            barrels.Add(CreateBarrel());
        }

        /// <summary>
        /// Create the speed visualization for the game's interface
        /// </summary>
        void CreateSpeedGui()
        {
            // Load all the number textures
            var speedTextures = new List<Texture2D>();
            for (var i = 0; i < 10; i++)
                speedTextures.Add(Content.Load<Texture2D>("HUD/number_" + i));

            // Create the sprites
            SpeedDigits[0] = new Sprite(speedTextures);            
            SpeedDigits[1] = new Sprite(speedTextures);

            // Set their layer and frame
            SpeedDigits[0].Layer = SpeedDigits[1].Layer = 3;
            SpeedDigits[0].Frame = SpeedDigits[1].Frame = 0;

            // Position all the speed digits to be on the bottom left
            SpeedDigits[0].Position.Y = SpeedDigits[1].Position.Y = hudContainer.Position.Y;
            SpeedDigits[0].Position.X = 100;
            SpeedDigits[1].Position.X = SpeedDigits[0].Position.X + 26;
        }

        /// <summary>
        /// Create the time visualization for the game's interface
        /// </summary>
        void CreateTimeGUI()
        {
            // Load all the number textures
            List<Texture2D> timeTextures = new List<Texture2D>();
            for (int i = 0; i < 10; i++)
                timeTextures.Add(Content.Load<Texture2D>("HUD/number_" + i + "b"));

            // Create 4 sprites to show the current time
            for (var i = 0; i < 4; i++)
            {
                timer[i] = new Sprite(timeTextures);
                timer[i].Layer = 3;
                timer[i].Frame = 0;
                timer[i].Position.Y = hudContainer.Position.Y;
            }
            
            // Position all the time digits to be on the bottom right 
            timer[0].Position.X = 448;
            timer[1].Position.X = timer[0].Position.X - 25;
            timer[2].Position.X = timer[0].Position.X - 60;
            timer[3].Position.X = timer[0].Position.X - 85;
        }

        /// <summary>
        /// Build the game interface sprites (splash, buttons, etc)
        /// </summary>
        void CreateInterface()
        {
            // Splash screen
            splashScreen = new Sprite(Content.Load<Texture2D>("HUD/splashPage"));
            splashScreen.Layer = 1;

            // Start screen
            startScreen = new Sprite(Content.Load<Texture2D>("HUD/startScreen"));
            startScreen.Layer = 3;

            // Play button
            playButton = new Sprite(Content.Load<Texture2D>("HUD/PlayButton"));
            playButton.Layer = 0;            
            var buttonTextureCenter = new Vector2(playButton.Textures[0].Width / 2f, playButton.Textures[0].Height / 2f);
            playButton.Position = new Vector2(ViewportCenter.X - buttonTextureCenter.X, 280 - buttonTextureCenter.Y);

            // Next button
            nextButton = new Sprite(Content.Load<Texture2D>("Hud/NextButton"));
            nextButton.Position = playButton.Position;
            nextButton.Layer = 0;

            // Score button
            scoreButton = new Sprite(Content.Load<Texture2D>("Hud/ScoreButton"));
            scoreButton.Position = new Vector2(175, 200);
            scoreButton.Layer = 0;

            // Restart button
            restartButton = new Sprite(Content.Load<Texture2D>("Hud/RestartButton"));
            restartButton.Position = new Vector2(175, 250);
            restartButton.Layer = 0;

            // End screen
            endScreen = new Sprite(Content.Load<Texture2D>("HUD/endScreenOverlay"));
            endScreen.Visible = false;
            endScreen.Layer = 2;

            // HUD container
            hudContainer = new Sprite(Content.Load<Texture2D>("HUD/HUD"));
            hudContainer.Position.Y = 360 - hudContainer.Textures[0].Height;
            hudContainer.Layer = 4;

            // Hearts (lives)
            for (int i = 0; i < 3; i++)
            {
                hearts[i] = new Sprite(Content.Load<Texture2D>("HUD/heart"));
                hearts[i].Position.Y = hudContainer.Position.Y;
                hearts[i].Position.X = 480 / 1.5f - hearts[i].Textures[0].Width * (i + 1);
                hearts[i].Layer = 3;
            }

            // Speed
            CreateSpeedGui();

            // Timer
            CreateTimeGUI();

            // One up arrow
            var arrowTex = Content.Load<Texture2D>("HUD/arrow_01");
            for (int i = 0; i < 3; i++)
                arrows[i] = new OneUp(arrowTex);
        }

        /// <summary>
        /// Create the "environment" (background and explosion)
        /// </summary>
        public void CreateEnvironment()
        {
            // Load background textures and create the sprite
            var backgroundTex = Content.Load<Texture2D>("BG/Background");
            background = new Sprite(backgroundTex);
            background.Layer = 8;

            // Load fire textures
            List<Texture2D> fireTextures = new List<Texture2D>();
            for (int i = 1; i < 13; i++)
                fireTextures.Add(Content.Load<Texture2D>("wallOfFire/wallOfFire_" + i));

            // Create the explosion and add its animations
            explosion = new Explosion(fireTextures);            
            explosion.AddAnimation("Idle", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 100, true);
            explosion.Layer = 5;

            // Position the explosion off screen
            explosion.Position.Y = hudContainer.Position.Y;
            explosion.PlayAnimation("Idle");
        }

        /// <summary>
        /// Loads all the audio files
        /// </summary>
        void CreateAudio()
        {
            // Menu music
            menuMusic = Content.Load<SoundEffect>("Audio/MenuMusic.wav");
            currentMusicInstance = menuMusic.CreateInstance();
            currentMusicInstance.IsLooped = true;

            // Background music
            backgroundMusic = Content.Load<SoundEffect>("Audio/BackgroundMusic");            

            // Enemy sounds
            rockBreak = Content.Load<SoundEffect>("Audio/rock_break");
            barrelExplode = Content.Load<SoundEffect>("Audio/volatile_barrel_explosion");
            barrelBreak = Content.Load<SoundEffect>("Audio/barrel_break");
            cartBreak = Content.Load<SoundEffect>("Audio/cart_break");

            // Player sounds
            death = Content.Load<SoundEffect>("Audio/death");
            fall = Content.Load<SoundEffect>("Audio/fall_off_screen");

            // Launcher sound
            launch = Content.Load<SoundEffect>("Audio/launch_explosion");            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Call helper functions to set up the game
            CreateAudio();
            CreateInterface();
            CreateEnvironment();
            CreatePlayer();
            CreateLauncher();
            CreateEnemies();
        }

        /// <summary>
        /// Updates game and objects based on current input
        /// </summary>
        /// <param name="currentKeyState">Current keyboard state</param>
        /// <param name="currentMouseState">Current mouse state</param>
        void UpdateInput(KeyboardState currentKeyState, MouseState currentMouseState)
        {   
            #region Mouse Handling
            // Store the mouse state if the left mouse button was pressed
            if (currentMouseState.LeftButton == ButtonState.Pressed)
                lastMouseState = currentMouseState;

            // If the left mouse button was down previously and is now up, register the click
            if (currentMouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
            {
                // Store the release state
                lastMouseState = currentMouseState;

                // Get the mouse point
                var mousePoint = new Point(lastMouseState.X, lastMouseState.Y);

                // React to mouse click based on current game state
                switch (gameState)
                {
                    case GameState.Menu:

                        // Play button was clicked, move to launch state
                        if (playButton.IsClicked(mousePoint))
                        {
                            gameState++;

                            currentMusicInstance.Stop();
                            currentMusicInstance = backgroundMusic.CreateInstance();
                            currentMusicInstance.IsLooped = true;
                            currentMusicInstance.Play();
                        }
                        break;
                    case GameState.GameOver:

                        // Next button was clicked, show end screen
                        if (nextButton.IsClicked(mousePoint))
                        {
                            splashScreen.Visible = true;
                            gameState++;
                        }
                        break;
                    case GameState.End:

                        // Score button was clicked, show score screen
                        if (scoreButton.IsClicked(mousePoint))
                            gameState--;

                        // Restart button was clicked, go to the menu screen
                        if (restartButton.IsClicked(mousePoint))
                            Restart();
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
                launch.Play();
                player.Speed = 25;
                launcher.PlayAnimation("Launch");
                

                player.PlayAnimation("Launch");
                player.Ascend();                
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
            if (currentKeyState.IsKeyDown(Keys.Down) && player.State == Player.PlayerState.Descending)
                player.Attack();

            #endregion
        }

        /// <summary>
        /// Updates the timer images based on amount of seconds that have passed
        /// </summary>
        /// <param name="seconds">Number of seconds that have passed since launching</param>
        void UpdateTimer(float seconds)
        {
            timer[0].Frame = (int)seconds % 60 % 10;
            timer[1].Frame = (int)seconds % 60 / 10;
            timer[2].Frame = (int)seconds / 60 % 10;
            timer[3].Frame = (int)seconds / 60 / 10;
        }

        void UpdateEnemy(List<Enemy> enemies, int enemyIndex, GameTime gameTime)
        {
            // If there is time remaining before spawn
            if (enemies[enemyIndex].SpawnTimeRemaining > 0)
            {
                // Reduce the time remaining before spawning
                enemies[enemyIndex].SpawnTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                // Time to spawn an enemy. Reset the timer
                enemies[enemyIndex].SpawnTimeRemaining = enemies[enemyIndex].SpawnTiming;

                // Check to see if the current enemy has not spawned yet
                if (!enemies[enemyIndex].HasSpawned)
                {
                    var position = Vector2.Zero;
                    var velocity = Vector2.Zero;

                    var enemyType = enemies[enemyIndex].GetType();

                    // Spawn position and velocity based on Enemy type
                    if (enemyType == typeof(Rock))
                    {
                        // Generate a random double between 0.0 and 1.0. Greater than 0.5, spawn rock on right
                        if (random.NextDouble() > 0.5)
                        {
                            // Start outside of the right view, move to the left
                            position = new Vector2(480, random.Next(-15, -5));
                            velocity = new Vector2(random.Next(-3, -2), random.Next(2, 3));
                        }
                        else
                        {
                            // Start outside of the left view, move to the right
                            position = new Vector2(-100, random.Next(-15, -5));
                            velocity = new Vector2(random.Next(2, 3), random.Next(2, 3));
                        }
                    }
                    else if (enemyType == typeof(Barrel))
                    {
                        position = new Vector2(random.Next(10, 250), 360);
                        velocity = new Vector2(1.2f, -3.5f);
                    }
                    else if (enemyType == typeof(Cart))
                    {
                        position = new Vector2(random.Next(0, 250), 360);
                        velocity = new Vector2(random.Next(1, 2), -4);
                    }

                    enemies[enemyIndex].Spawn(position, velocity);
                    enemies[enemyIndex].Rotation = (float)random.NextDouble();
                }

                // Increase enemy index to the next slot
                enemyIndex++;

                // If the enemy index is greater than the number of we have created, reset to zero
                if (enemyIndex >= enemies.Count)
                    enemyIndex = 0;
            }

            // Update the enemy
            enemies[enemyIndex].Update(gameTime);

            // If it has spawned and can collide... 
            if (enemies[enemyIndex].HasSpawned && enemies[enemyIndex].Collideable)
            {
                // ...check for collision against the player
                if (enemies[enemyIndex].Collides(player))
                {
                    // Enemy hit, show the oneup arrow
                    arrows[availableOneUp].Play(enemies[enemyIndex].Position);
                    availableOneUp++;

                    if (availableOneUp >= arrows.Length)
                        availableOneUp = 0;

                    // Bounce the player
                    player.Bounce();
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // If the splash screen is showing
            if (gameState == GameState.Splash)
            {
                // Reduce the time remaining
                splashTime -= gameTime.ElapsedGameTime.Milliseconds;

                // If there is no more time left, go to the menu
                if (splashTime <= 0)
                {
                    currentMusicInstance.Play();
                    gameState++;
                }
            }

            // Update the input
            UpdateInput(Keyboard.GetState(), Mouse.GetState());

            // If the game is running, update the main systems
            if (gameState == GameState.Running)
            {
                // Get elapsed time since last update
                var deltaSeconds = (float)gameTime.ElapsedGameTime.Milliseconds;

                // Increase the amount of time since launch
                currentTime += deltaSeconds;

                // Current time is longer than the best time, so make it the top time
                if (currentTime > topTime)
                    topTime = currentTime;

                // Update the timer interface
                UpdateTimer(currentTime / 1000);

                // If the player below the fire, the game is over
                if ((player.Position.Y >= explosion.Position.Y && explosion.Position.Y <= 122 && explosion.Visible) || player.Lives <= 0)
                {
                    // Kill player and play his sound
                    player.Die();
                    death.Play();

                    // Go to game over state
                    gameState = GameState.GameOver;

                    // Stop the game music
                    currentMusicInstance.Stop();
                    
                    // Show the end screen
                    endScreen.Visible = true;
                }

                // If the player goes too far down, shoot him back up
                if (player.Position.Y >= 300 && !player.IsAscending)
                {
                    // Change his speed so that he moves up
                    player.Speed -= 10;

                    // Play his falling sound
                    fall.Play();

                    // Start his ascension
                    player.Ascend();
                }

                // Update the launcher
                launcher.Update(gameTime);

                // Update the player
                player.Update(gameTime);
                
                // Update the one-up arrows
                foreach (var arrow in arrows)
                    arrow.Update(gameTime);

                // If the current speed is the fastest yet, store that into the top speed
                if (player.Speed >= topSpeed)
                    topSpeed = player.Speed;

                // Update the explosion
                explosion.Update(gameTime);

                UpdateEnemy(rocks, rockIndex, gameTime);
                UpdateEnemy(barrels, barrelIndex, gameTime);
                UpdateEnemy(carts, cartIndex, gameTime);                

                // If the player isn't moving fast enough (10), start moving the explosion
                if (player.Speed < 10)
                    explosion.IsRising = true;
                else
                    explosion.IsRising = false;

                // Increase the background image scroll value. This does NOT update position in the game. It updates the image itself
                currentBackgroundScroll += backgroundScrollSpeed * deltaSeconds;

                // Get the current player speed convert it to two separate digits to render as textures
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
            // Clear to black
            GraphicsDevice.Clear(Color.Black);

            // Begin drawing. The SamplerState.LinearWrap is an option that allows an image to be wrapped when its UVs are moved
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.LinearWrap, null, null);

            // Draw certain objects based on game state
            switch (gameState)
            {
                case GameState.Splash:
                    splashScreen.Draw(spriteBatch);
                    break;
                case GameState.Menu:
                    startScreen.Draw(spriteBatch);
                    playButton.Draw(spriteBatch);
                    break;
                case GameState.Launch:
                case GameState.Running:

                    // Draw the scrolling background
                    // background.Textures[0] - The background image itself
                    // background.Position - The position in the game to draw.
                    // sourceRect - A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.
                    //            - The currentBackgroundScroll value is what makes the image move, in this case, vertically up
                    var sourceRect = new Rectangle(0, (int)-currentBackgroundScroll, background.Textures[0].Bounds.Width, background.Textures[0].Bounds.Height);
                    spriteBatch.Draw(background.Textures[0], background.Position, sourceRect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, (float)background.Layer / 10f);
                    
                    launcher.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    explosion.Draw(spriteBatch);

                    foreach (var cart in carts)
                        cart.Draw(spriteBatch);

                    foreach (var barrel in barrels)
                        barrel.Draw(spriteBatch);

                    foreach (var rock in rocks)
                        rock.Draw(spriteBatch);

                    hudContainer.Draw(spriteBatch);

                    foreach (var digit in SpeedDigits)
                        digit.Draw(spriteBatch);

                    foreach (var arrow in arrows)
                        arrow.Draw(spriteBatch);

                    hearts[0].Alpha = (player.Lives < 3) ? 0.5f : 1.0f;
                    hearts[0].Draw(spriteBatch);

                    hearts[1].Alpha = (player.Lives < 2) ? 0.5f : 1.0f;
                    hearts[1].Draw(spriteBatch);

                    hearts[2].Alpha = (player.Lives < 1) ? 0.5f : 1.0f;
                    hearts[2].Draw(spriteBatch);

                    //for (var i = 0; i < hearts.Length; i++)
                    //{
                    //    if (player.Lives <= i)
                    //        hearts[i].Alpha = 0.5f;

                    //    hearts[i].Draw(spriteBatch);
                    //}

                    foreach (var timeDigit in timer)
                        timeDigit.Draw(spriteBatch);
                    
                    break;
                case GameState.GameOver:
                    // Draw the scrolling background
                    // background.Textures[0] - The background image itself
                    // background.Position - The position in the game to draw.
                    // sourceRect - A rectangle that specifies (in texels) the source texels from a texture. Use null to draw the entire texture.
                    //            - The currentBackgroundScroll value is what makes the image move, in this case, vertically up
                    sourceRect = new Rectangle(0, (int)-currentBackgroundScroll, background.Textures[0].Bounds.Width, background.Textures[0].Bounds.Height);
                    spriteBatch.Draw(background.Textures[0], background.Position, sourceRect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, (float)background.Layer / 10f);

                    explosion.Draw(spriteBatch);
                    hudContainer.Draw(spriteBatch);

                    foreach (var digit in SpeedDigits)
                        digit.Draw(spriteBatch);

                    for (int i = 0; i < hearts.Length; i++)
                    {
                        if (player.Lives <= i)
                            hearts[i].Alpha = 0.5f;

                        hearts[i].Draw(spriteBatch);
                    }

                    foreach (var timeDigit in timer)
                        timeDigit.Draw(spriteBatch);

                    endScreen.Draw(spriteBatch);

                    spriteBatch.Draw(timer[3].Textures[timer[3].Frame], new Vector2(310, 208), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(timer[2].Textures[timer[2].Frame], new Vector2(335, 208), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(timer[1].Textures[timer[1].Frame], new Vector2(370, 208), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(timer[0].Textures[timer[0].Frame], new Vector2(395, 208), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                    var speedFrame = int.Parse(topSpeed.ToString().Substring(0, 1));
                    spriteBatch.Draw(SpeedDigits[0].Textures[speedFrame], new Vector2(311, 157), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                    if (topSpeed > 9)
                    {
                        speedFrame = int.Parse(topSpeed.ToString().Substring(1, 1));
                        spriteBatch.Draw(SpeedDigits[1].Textures[speedFrame], new Vector2(338, 157), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    }

                    nextButton.Draw(spriteBatch);
                    break;
                case GameState.End:
                    splashScreen.Draw(spriteBatch);
                    scoreButton.Draw(spriteBatch);
                    restartButton.Draw(spriteBatch);
                    break;
            }

            // End drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Restart the game
        /// </summary>
        void Restart()
        {
            #region Game Reset
            gameState = GameState.Menu;
            currentTime = 0.0f;
            splashTime = 3000;
            player.Lives = 3;
            player.Speed = 0;

            explosion.Position.Y = hudContainer.Position.Y;
            explosion.Frame = 0;

            launcher.Position = new Vector2(8, 85);
            launcher.Frame = 0;

            UpdateTimer(0);
            #endregion

            #region Player Reset
            player.Position = Vector2.Zero;
            player.Position = new Vector2(178, 100);
            player.State = Player.PlayerState.Idle;
            player.Frame = 0;
            #endregion

            #region Enemy Reset
            foreach (var barrel in barrels)
            {
                barrel.IsDamaged = false;
                barrel.HasSpawned = false;
                barrel.Visible = false;
            }

            foreach (var cart in carts)
            {
                cart.IsDamaged = false;
                cart.HasSpawned = false;
                cart.Visible = false;
            }

            foreach (var rock in rocks)
            {
                rock.IsDamaged = false;
                rock.HasSpawned = false;
                rock.Visible = false;
            }

            foreach (var arrow in arrows)
            {
                arrow.Stop();                
            }
            #endregion

            #region Music Reset
            currentMusicInstance = menuMusic.CreateInstance();
            currentMusicInstance.IsLooped = true;
            currentMusicInstance.Play();
            #endregion
        }
    }
}