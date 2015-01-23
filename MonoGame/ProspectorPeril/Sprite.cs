using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    /// <summary>
    /// Very basic Sprite class
    /// </summary>
    public class Sprite
    {
        // Helper class for holding animation data
        public class Animation
        {
            // Name of the animation
            public string Name;

            // Array of frame numbers (indexes into Sprite::Textures
            public int[] Frames;

            // Time in between each frame of animation
            public float TimePerFrame;

            // Whether it should loop back to the beginning and start playing again
            public bool Looping;            

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="name"></param>
            /// <param name="frames"></param>
            /// <param name="timePerFrame"></param>
            /// <param name="looping"></param>
            public Animation(string name, int[] frames, float timePerFrame, bool looping)
            {
                Name = name;
                Frames = frames;
                TimePerFrame = timePerFrame;
                Looping = looping;
            }
        }

        /// <summary>
        /// A collection of Animation objects, referenced by name
        /// </summary>
        private Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        /// <summary>
        /// All the textures this sprite can use
        /// </summary>
        public List<Texture2D> Textures = new List<Texture2D>();

        /// <summary>
        /// Current sprite texture to use
        /// </summary>
        public int Frame = 0;
        
        /// <summary>
        /// Position on screen for the sprite
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// Scale of sprite texture on screen
        /// </summary>
        public Vector2 Scale = new Vector2(1.0f, 1.0f);

        /// <summary>
        /// The rotation of the sprite
        /// </summary>
        public float Rotation = 0.0f;

        /// <summary>
        /// Sprite transparency
        /// </summary>
        public float Alpha = 1f;

        /// <summary>
        /// Whether the sprite should be drawn to the screen or not
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// Determines if the Sprite should be running an animation
        /// </summary>
        bool IsAnimating = false;

        /// <summary>
        /// Timer for the currently playing animation
        /// </summary>
        float AnimationTimer = 0.0f;

        /// <summary>
        /// The index used to increment through the array of animation frames
        /// </summary>
        int CurrentAnimFrame = 0;

        /// <summary>
        /// The current animation being played
        /// </summary>
        public Animation CurrentAnimation = null;

        /// <summary>
        /// Order/depth of sprite on screen
        /// </summary>
        public int Layer = 0;

        /// <summary>
        /// The real render depth used by the SpriteBatch::draw method. It expects a range of 0 to 1.
        /// The 0 to 100 range of the Layer property is for usability purposes.
        /// </summary>
        float RenderLayer
        {
            get 
            { 
                return (float)Layer / 10f; 
            }
        }

        /// <summary>
        /// Whether the Sprite can be collided with or not
        /// </summary>
        public bool Collideable;

        /// <summary>
        /// Basic Collision Sphere object
        /// </summary>
        public BoundingSphere CollisionSphere;

        /// <summary>
        /// Width of the Sprite (texture width * X scale)
        /// </summary>
        public float Width
        {
            get { return Textures[Frame].Width * Scale.X; }
        }

        /// <summary>
        /// Height of the Sprite (texture height * Y scale)
        /// </summary>
        public float Height
        {
            get { return Textures[Frame].Height * Scale.Y; }
        }

        /// <summary>
        /// Width and Height of Sprite
        /// </summary>
        public Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }

        /// <summary>
        /// Center of the Sprite
        /// </summary>
        public Vector2 SpriteCenter
        {
            get
            {
                return new Vector2(Width / 2f, Height / 2f) + Position;
            }
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public Sprite()
        {

        }

        /// <summary>
        /// Turn on collision for this Sprite and update its collision sphere
        /// </summary>
        /// <param name="shouldEnable"></param>
        public void EnableCollision(bool shouldEnable = true)
        {
            Collideable = shouldEnable;

            var tempPos = new Vector3(SpriteCenter, 0.0f);
            CollisionSphere = new BoundingSphere(tempPos, Width / 2.5f);            
        }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Sprite(Texture2D texture)
        {
            if (texture != null)
                Textures.Add(texture);
        }

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Sprite(List<Texture2D> textures)
        {
            Textures = textures;
        }

        /// <summary>
        /// Add a new animation to the Sprite's animation collection
        /// </summary>
        /// <param name="name">ID of the animation</param>
        /// <param name="frames">An array of texture frame indexes</param>
        /// <param name="time">Time between each frame (milliseconds)</param>
        public void AddAnimation(string name, int[] frames, float time, bool looping = false)
        {
            Animations.Add(name, new Animation(name, frames, time, looping));
        }

        /// <summary>
        /// Play an animation
        /// </summary>
        /// <param name="animationname">The ID of the animation</param>
        public void PlayAnimation(string animationname)
        {
            CurrentAnimation = Animations[animationname];

            if (CurrentAnimation != null)
            {
                IsAnimating = true;
                AnimationTimer = CurrentAnimation.TimePerFrame;
                CurrentAnimFrame = 0;
            }
        }
        
        void UpdateAnimation(float time)
        {
            // Set the Sprite's texture frame to the current animation "cell"
            Frame = CurrentAnimation.Frames[CurrentAnimFrame];

            // Reduce the animation timer
            AnimationTimer -= time;

            // If the timer is less than 0, we are ready for the next frame
            if (AnimationTimer <= 0)
            {
                // Increase the current animation frame by 1
                CurrentAnimFrame++;
                AnimationTimer = CurrentAnimation.TimePerFrame;

                // If the current animation frame is more than the number of frames in the animation, stop animating
                if (CurrentAnimFrame >= CurrentAnimation.Frames.Length)
                {
                    if (CurrentAnimation.Looping)
                    {
                        CurrentAnimFrame = 0;
                    }
                    else
                    {
                        OnAnimationEnd();
                        IsAnimating = false;
                    }
                }
            }
        }

        virtual public void OnAnimationEnd() { }

        public void UpdateCollision()
        {
            // Update bounding struct positions
            CollisionSphere.Center.X = Position.X;
            CollisionSphere.Center.Y = Position.Y;            
        }

        /// <summary>
        /// Basic update routine for Sprite
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public virtual void Update(GameTime gameTime)
        {            
            // If the Sprite should be animating
            if (IsAnimating)
                UpdateAnimation(gameTime.ElapsedGameTime.Milliseconds);                            

            if (Collideable)
                UpdateCollision();            
        }

        /// <summary>
        /// Draw the sprite to the screen using the XNA/MonoGame stock drawing object
        /// </summary>
        /// <param name="spriteBatch">A valid SpriteBatch that does all the drawing for the game</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Textures[Frame] != null && Visible)
                spriteBatch.Draw(Textures[Frame], Position, null, Color.White * Alpha, Rotation, Vector2.Zero, Scale, SpriteEffects.None, RenderLayer);
        }

        /// <summary>
        /// Check to see if the Sprite was clicked
        /// </summary>
        /// <param name="point">Point from the current mouse state</param>
        /// <returns></returns>
        public bool IsClicked(Point point)
        {
            return ((point.X >= Position.X) && point.X <= (Position.X + Width) && point.Y >= Position.Y && point.Y <= (Position.Y + Height));
        }
    }
}
