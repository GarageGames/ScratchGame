using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    public class Barrel : Enemy
    {
        /// <summary>
        /// Toggle for whether the Barrel is ready to explode or not
        /// </summary>
        bool IsPrimed = false;

        /// <summary>
        /// Time between primings
        /// </summary>
        float PrimeTimer = 1300;
        
        /// <summary>
        /// Sound effect to play when the barrel explodes
        /// </summary>
        public SoundEffect ExplodeSound = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Barrel() : base()
        {
            SpawnTimeRemaining = SpawnTiming = 3500;
        }

        /// <summary>
        /// Constructor used when a Barrel only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Barrel(Texture2D texture) : base(texture)
        {
            SpawnTimeRemaining = SpawnTiming = 3500;
        }

        /// <summary>
        /// Constructor used when a Barrel uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Barrel(List<Texture2D> textures)
            : base(textures)
        {
            SpawnTimeRemaining = SpawnTiming = 3500;
        }
        
        public override void Update(GameTime gameTime)
        {
            // If the barrel has spawned
            if (HasSpawned)
            {
                // If it is not primed for explosion
                if (!IsPrimed)
                {
                    // Update it's position based on the decaying velocity
                    Position += Velocity;

                    // Changed velocity to make the object start dropping
                    Velocity.Y += 0.03f;
                }

                // Reduce the time until it's ready to explode or reset
                PrimeTimer -= gameTime.ElapsedGameTime.Milliseconds;

                // If the primer has run out and it has not been damaged...
                if (PrimeTimer <= 0 && !IsDamaged)
                {
                    // Switch to the opposite primed state
                    IsPrimed = !IsPrimed;

                    // If it was just primed, play the Prime animation. Go to idle otherwise
                    if (IsPrimed)
                        PlayAnimation("Prime");
                    else
                        PlayAnimation("Idle");

                    // Reset the prime timer
                    PrimeTimer = 1300;
                }
            }

            base.Update(gameTime);
        }
        
        /// <summary>
        /// Custom Collides method for Barrel
        /// </summary>
        /// <param name="sprite">Sprite to check collision against</param>
        /// <returns>The result of the collision</returns>
        public override bool Collides(Sprite sprite)
        {            
            // First, check for the basic collision via its parent (Enemy)
            bool collided = base.Collides(sprite);
            
            // If the collision occurred while the barrel was primed
            if (IsPrimed && collided)
            {
                // Play the explosion animation and sound
                PlayAnimation("Explode");
                ExplodeSound.Play();

                // Cast the sprite as a Player object and hurt it
                var player = (Player)sprite;                
                player.Damage();
                
            }

            // Return the result of the collision
            return collided;
        }

        /// <summary>
        /// Custom Spawn method for Barrel
        /// </summary>
        /// <param name="position">Starting position</param>
        /// <param name="velocity">Starting velocity</param>
        public override void Spawn(Vector2 position, Vector2 velocity)
        {           
            // Start off not primed
            IsPrimed = false;
            PrimeTimer = 1300;
            base.Spawn(position, velocity);
        }

        /// <summary>
        /// Custom onAnimationEnd for barrel
        /// </summary>
        public override void OnAnimationEnd()
        {
            // If the animation was Break or Explode, despawn
            if (CurrentAnimation.Name == "Break" || CurrentAnimation.Name == "Explode")
            {
                Visible = false;
                HasSpawned = false;
            }
        }
    }
}
