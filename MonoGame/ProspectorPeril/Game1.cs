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
        Sprite nextButton;
        Sprite scoreButton;
        Sprite restartButton;
        float splashTime = 3000;
        #endregion

        #region Game variables
        GameState gameState;
        int topSpeed = 0;
        float currentTime = 0.0f;
        float topTime = 0.0f;
        Sprite background;
        Launcher launcher;
        Explosion fire;
        Sprite hudContainer;
        Sprite[] hearts = new Sprite[3];
        Sprite[] timer = new Sprite[4];
        

        // The rate at which the background image is scrolled per ms
        float backgroundScrollSpeed = 0.25f;

        // The current background scroll value (updated each frame)
        float currentBackgroundScroll;

        Sprite[] SpeedDigits = new Sprite[2];
        OneUp[] arrows = new OneUp[3];
        int availableOneUp = 0;

        // Enemy variables
        List<Enemy> enemies = new List<Enemy>();
        float spawnTimer = 1000;
        int enemyIndex = 0;
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

        public Game1()
            : base()
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
            graphics.PreferredBackBufferHeight = 360;
            graphics.PreferredBackBufferWidth = 480;
            graphics.ApplyChanges();

            GraphicsViewport = graphics.GraphicsDevice.Viewport;
            ViewportCenter = new Vector2(240, 180);

            base.Initialize();
        }

        void CreatePlayer()
        {
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

            player = new Player(textures);            
            player.AddAnimation("Idle", new int[] { 0 }, 0);
            player.AddAnimation("Launch", new int[] { 1, 2 }, 100);
            player.AddAnimation("Float", new int[] { 3 }, 0);
            player.AddAnimation("Attack", new int[] { 4, 5, 3 }, 100);

            player.Scale = new Vector2(0.6f, 0.6f);
            player.Position.X = ViewportCenter.X - player.SpriteCenter.X;
            player.Position.Y = 116;

            player.EnableCollision();
        }

        void CreateLauncher()
        {
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

            launcher = new Launcher(textures);
            launcher.Scale = new Vector2(0.75f, 0.75f);            
            launcher.Position = new Vector2(36.5f, 106.5f);
            launcher.AddAnimation("Idle", new int[] { 0 }, 0);
            launcher.AddAnimation("Launch", new int[] { 0, 1, 2, 3, 4, 5, 6 }, 100);
        }

        Enemy CreateBarrel()
        {
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

            var barrel = new Barrel(textures);
            barrel.AddAnimation("Idle", new int[] { 0 }, 0);
            barrel.AddAnimation("Break", new int[] { 0, 1, 2, 3, 4 }, 70);
            barrel.AddAnimation("Prime", new int[] { 0, 5, 0, 5, 0, 5 }, 70, true);
            barrel.AddAnimation("Explode", new int[] { 5, 6, 7, 8, 9, 10, 11 }, 70);
            barrel.Position = barrel.SpawnPosition = new Vector2(random.Next(20, 300), 366);
            barrel.Velocity = barrel.SpawnVelocity = new Vector2(1.2f, -3.5f);
            barrel.BreakSound = barrelBreak;
            barrel.ExplodeSound = barrelExplode;
            return barrel;
        }

        Enemy CreateRock()
        {
            string[] rockTextures = {   "enemy_rock/enemy_rock01", 
                                          "enemy_rock/enemy_rock02", 
                                          "enemy_rock/enemy_rock03", 
                                          "enemy_rock/enemy_rock04", 
                                          "enemy_rock/enemy_rock05" 
                                      };

            var textures = new List<Texture2D>();

            foreach (var textureString in rockTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            var rock = new Rock(textures);
            rock.AddAnimation("Idle", new int[] { 0 }, 0);
            rock.AddAnimation("Break", new int[] { 0, 1, 2, 3, 4 }, 70);
            rock.BreakSound = rockBreak;
            return rock;
        }

        Enemy CreateCart()
        {
            string[] cartTextures = {   "enemy_cart/enemy_cart01", 
                                          "enemy_cart/enemy_cart02", 
                                          "enemy_cart/enemy_cart03", 
                                          "enemy_cart/enemy_cart04", 
                                          "enemy_cart/enemy_cart05" 
                                      };

            var textures = new List<Texture2D>();

            foreach (var textureString in cartTextures)
                textures.Add(Content.Load<Texture2D>(textureString));

            var cart = new Cart(textures);
            cart.AddAnimation("Idle", new int[] { 0 }, 0);
            cart.AddAnimation("Break", new int[] { 0, 1, 2, 3, 4 }, 70);
            cart.Position = cart.SpawnPosition = new Vector2(random.Next(0, 480), 360);
            cart.Velocity = cart.SpawnVelocity = new Vector2(random.Next(1, 2), -4);
            cart.BreakSound = cartBreak;
            return cart;
        }

        void CreateEnemies()
        {
            var rock = CreateRock();
            rock.SpawnPosition = new Vector2(-100, random.Next(-15, -5));
            rock.SpawnVelocity = new Vector2(random.Next(2, 3), random.Next(2, 3));
            enemies.Add(rock);
            enemies.Add(CreateBarrel());
            enemies.Add(CreateCart());

            enemies.Add(CreateBarrel());
            enemies.Add(CreateCart());
            rock = CreateRock();
            rock.SpawnPosition = new Vector2(480, random.Next(-15, -5));
            rock.SpawnVelocity = new Vector2(random.Next(-3, -2), random.Next(2, 3));
            enemies.Add(rock);

            rock = CreateRock();
            rock.SpawnPosition = new Vector2(-100, random.Next(-15, -5));
            rock.SpawnVelocity = new Vector2(random.Next(2, 3), random.Next(2, 3));
            enemies.Add(rock);
            enemies.Add(CreateCart());
            enemies.Add(CreateBarrel());
        }

        void CreateSpeedGui()
        {
            List<Texture2D> speedTextures = new List<Texture2D>();

            for (int i = 0; i < 10; i++)
                speedTextures.Add(Content.Load<Texture2D>("HUD/number_" + i));

            SpeedDigits[0] = new Sprite(speedTextures);            
            SpeedDigits[1] = new Sprite(speedTextures);

            SpeedDigits[0].Layer = SpeedDigits[1].Layer = 3;

            SpeedDigits[0].Frame = SpeedDigits[1].Frame = 0;

            SpeedDigits[0].Position.Y = SpeedDigits[1].Position.Y = hudContainer.Position.Y;
            SpeedDigits[0].Position.X = 100;
            SpeedDigits[1].Position.X = SpeedDigits[0].Position.X + 26;
        }

        void CreateTimeGUI()
        {
            List<Texture2D> timeTextures = new List<Texture2D>();

            for (int i = 0; i < 10; i++)
                timeTextures.Add(Content.Load<Texture2D>("HUD/number_" + i + "b"));

            for (int i = 0; i < 4; i++)
            {
                timer[i] = new Sprite(timeTextures);
                timer[i].Layer = 3;
                timer[i].Frame = 0;
                timer[i].Position.Y = hudContainer.Position.Y;
            }
            
            timer[0].Position.X = 448;
            timer[1].Position.X = timer[0].Position.X - 25;
            timer[2].Position.X = timer[0].Position.X - 60;
            timer[3].Position.X = timer[0].Position.X - 85;
        }

        void CreateInterface()
        {
            splashScreen = new Sprite(Content.Load<Texture2D>("HUD/splashPage"));
            splashScreen.Layer = 1;

            playButton = new Sprite(Content.Load<Texture2D>("HUD/PlayButton"));
            playButton.Layer = 0;

            var buttonTextureCenter = new Vector2(playButton.Textures[0].Width / 2f, playButton.Textures[0].Height / 2f);
            playButton.Position = new Vector2(ViewportCenter.X - buttonTextureCenter.X, 280 - buttonTextureCenter.Y);

            nextButton = new Sprite(Content.Load<Texture2D>("Hud/NextButton"));
            nextButton.Position = playButton.Position;
            nextButton.Layer = 0;

            scoreButton = new Sprite(Content.Load<Texture2D>("Hud/ScoreButton"));
            scoreButton.Position = new Vector2(175, 200);
            scoreButton.Layer = 0;

            restartButton = new Sprite(Content.Load<Texture2D>("Hud/RestartButton"));
            restartButton.Position = new Vector2(175, 250);
            restartButton.Layer = 0;

            endScreen = new Sprite(Content.Load<Texture2D>("HUD/endScreenOverlay"));
            endScreen.Visible = false;
            endScreen.Layer = 2;

            startScreen = new Sprite(Content.Load<Texture2D>("HUD/startScreen"));
            startScreen.Layer = 3;

            hudContainer = new Sprite(Content.Load<Texture2D>("HUD/HUD"));
            hudContainer.Position.Y = 360 - hudContainer.Textures[0].Height;
            hudContainer.Layer = 4;

            for (int i = 0; i < 3; i++)
            {
                hearts[i] = new Sprite(Content.Load<Texture2D>("HUD/heart"));
                hearts[i].Position.Y = hudContainer.Position.Y;
                hearts[i].Position.X = 480 / 1.5f - hearts[i].Textures[0].Width * (i + 1);
                hearts[i].Layer = 3;
            }

            CreateSpeedGui();
            CreateTimeGUI();

            var arrowTex = Content.Load<Texture2D>("HUD/arrow_01");

            for (int i = 0; i < 3; i++)
                arrows[i] = new OneUp(arrowTex);
        }

        public void CreateEnvironment()
        {
            var backgroundTex = Content.Load<Texture2D>("BG/Background");
            background = new Sprite(backgroundTex);
            background.Layer = 8;

            List<Texture2D> fireTextures = new List<Texture2D>();

            for (int i = 1; i < 13; i++)
                fireTextures.Add(Content.Load<Texture2D>("wallOfFire/wallOfFire_" + i));

            fire = new Explosion(fireTextures);
            fire.Layer = 5;
            fire.AddAnimation("Idle", new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, 100, true);
            fire.Position.Y = hudContainer.Position.Y;
            fire.PlayAnimation("Idle");
        }

        void CreateAudio()
        {

            menuMusic = Content.Load<SoundEffect>("Audio/MenuMusic.wav");
            currentMusicInstance = menuMusic.CreateInstance();
            currentMusicInstance.IsLooped = true;

            backgroundMusic = Content.Load<SoundEffect>("Audio/BackgroundMusic");
            //instance = backgroundMusic.CreateInstance();
            //instance.IsLooped = true;

            barrelBreak = Content.Load<SoundEffect>("Audio/barrel_break");
            cartBreak = Content.Load<SoundEffect>("Audio/cart_break");
            death = Content.Load<SoundEffect>("Audio/death");
            fall = Content.Load<SoundEffect>("Audio/fall_off_screen");
            launch = Content.Load<SoundEffect>("Audio/launch_explosion");
            rockBreak = Content.Load<SoundEffect>("Audio/rock_break");
            barrelExplode = Content.Load<SoundEffect>("Audio/volatile_barrel_explosion");
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            CreateAudio();
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
                var mousePoint = new Point(lastMouseState.X, lastMouseState.Y);
                switch (gameState)
                {
                    case GameState.Menu:
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
                        if (nextButton.IsClicked(mousePoint))
                        {
                            splashScreen.Visible = true;
                            gameState++;
                        }
                        break;
                    case GameState.End:
                        if (scoreButton.IsClicked(mousePoint))
                            gameState--;

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
            if (currentKeyState.IsKeyDown(Keys.Down))
                player.Attack();

            #endregion
        }

        void UpdateTimer(float seconds)
        {
            timer[0].Frame = (int)seconds % 60 % 10;
            timer[1].Frame = (int)seconds % 60 / 10;
            timer[2].Frame = (int)seconds / 60 % 10;
            timer[3].Frame = (int)seconds / 60 / 10;
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
                {
                    currentMusicInstance.Play();
                    gameState++;
                }
            }

            // Get current states of mouse and keyboard
            UpdateInput(Keyboard.GetState(), Mouse.GetState());

            // If the game is running, update the main systems
            if (gameState == GameState.Running)
            {
                TimeSpan deltaTime = gameTime.ElapsedGameTime;
                float deltaSeconds = (float)deltaTime.Milliseconds;

                currentTime += deltaSeconds;

                if (currentTime > topTime)
                    topTime = currentTime;

                UpdateTimer(currentTime / 1000);
                if (spawnTimer <= 0)
                {
                    spawnTimer = 1000;

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
                    currentMusicInstance.Stop();
                    death.Play();
                    endScreen.Visible = true;
                }

                if (player.Position.Y >= 424 && !player.IsAscending)
                {
                    player.Speed -= 10;
                    fall.Play();
                    player.Ascend();
                }

                launcher.Update(gameTime);
                player.Update(gameTime);
                
                foreach (var arrow in arrows)
                    arrow.Update(gameTime);

                if (player.Speed >= topSpeed)
                    topSpeed = player.Speed;

                fire.Update(gameTime);

                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Update(gameTime);
                    if (enemies[i].HasSpawned && enemies[i].Collideable)
                    {
                        var result = enemies[i].Collides(player);

                        if (result)
                        {
                            arrows[availableOneUp].Play(enemies[i].Position);

                            availableOneUp++;

                            if (availableOneUp >= arrows.Length)
                                availableOneUp = 0;

                            player.Bounce();
                        }
                    }
                }

                if (player.Speed < 10)
                    fire.IsRising = true;
                else
                    fire.IsRising = false;

                // Increase the background image scroll value. This does NOT update position in the game. It updates the image itself
                currentBackgroundScroll += backgroundScrollSpeed * deltaSeconds;

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

            // Begin drawing. The SamplerState.LinearWrap is an option that allows an image to be wrapped when its UVs are moved
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.LinearWrap, null, null);

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
                    fire.Draw(spriteBatch);

                    foreach (var enemy in enemies)
                        enemy.Draw(spriteBatch);

                    hudContainer.Draw(spriteBatch);

                    foreach (var digit in SpeedDigits)
                        digit.Draw(spriteBatch);

                    foreach (var arrow in arrows)
                        arrow.Draw(spriteBatch);

                    for (int i = 0; i < hearts.Length; i++)
                    {
                        if (player.Lives <= i)
                            hearts[i].Alpha = 0.5f;

                        hearts[i].Draw(spriteBatch);
                    }

                    for (int i = 0; i < timer.Length; i++)
                    {
                        timer[i].Draw(spriteBatch);
                    }
                    break;
                case GameState.GameOver:
                    sourceRect = new Rectangle(0, (int)-currentBackgroundScroll, background.Textures[0].Bounds.Width, background.Textures[0].Bounds.Height);
                    spriteBatch.Draw(background.Textures[0], background.Position, sourceRect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, (float)background.Layer / 10f);
                    fire.Draw(spriteBatch);
                    hudContainer.Draw(spriteBatch);

                    foreach (var digit in SpeedDigits)
                        digit.Draw(spriteBatch);

                    for (int i = 0; i < hearts.Length; i++)
                    {
                        if (player.Lives <= i)
                            hearts[i].Alpha = 0.5f;

                        hearts[i].Draw(spriteBatch);
                    }

                    for (int i = 0; i < timer.Length; i++)
                        timer[i].Draw(spriteBatch);

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

        void Restart()
        {
            // Game reset
            gameState = GameState.Menu;
            currentTime = 0.0f;
            splashTime = 3000;
            player.Lives = 3;
            player.Speed = 0;
            spawnTimer = 1000;

            fire.Position.Y = hudContainer.Position.Y;
            fire.Frame = 0;

            launcher.Position = new Vector2(36.5f, 106.5f);
            launcher.Frame = 0;

            UpdateTimer(0);

            // Player reset
            player.Position = Vector2.Zero;
            player.Position.X = ViewportCenter.X - player.SpriteCenter.X;
            player.Position.Y = 116;
            player.State = Player.PlayerState.Idle;
            player.Frame = 0;

            foreach (var enemy in enemies)
            {
                enemy.IsDamaged = false;
                enemy.HasSpawned = false;
                enemy.Visible = false;
            }
            
            currentMusicInstance = menuMusic.CreateInstance();
            currentMusicInstance.IsLooped = true;
            currentMusicInstance.Play();
        }
    }
}