using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProspectorPeril
{    
    public class Enemy : Sprite
    {
        /// <summary>
        /// Whether the Enemy has spawned or not
        /// </summary>
        public bool HasSpawned = false;

        /// <summary>
        /// Whether the Enemy has collided with the player or not
        /// </summary>
        public bool IsDamaged = false;

        /// <summary>
        /// Enemy velocity
        /// </summary>
        public Vector2 Velocity;
        
        /// <summary>
        /// Sound to play when damaged
        /// </summary>
        public SoundEffect BreakSound = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Enemy()
        {
            Visible = false;
            Layer = 6;            
        }

        /// <summary>
        /// Constructor used when an Enemy only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Enemy(Texture2D texture) : base(texture)
        {
            Visible = false;
            Layer = 6;            
        }

        /// <summary>
        /// Constructor used when an Enemy uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Enemy(List<Texture2D> textures)
            : base(textures)
        {
            Visible = false;
            Layer = 6;            
        }

        /// <summary>
        /// Update the Enemy
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public override void Update(GameTime gameTime)
        {
            // Check to see if enemy is far enough out of view to despawn
            if (Position.X < -200 || Position.X > 500 || Position.Y > 400)
                HasSpawned = false;
            
            // Sprite Update
            base.Update(gameTime);
        }

        /// <summary>
        /// Spawn the Enemy
        /// </summary>
        /// <param name="position">Starting position</param>
        /// <param name="velocity">Starting velocity</param>
        virtual public void Spawn(Vector2 position, Vector2 velocity)
        {
            Frame = 0;
            HasSpawned = true;
            Visible = true;
            Position = position;
            Velocity = velocity;
            IsDamaged = false;

            EnableCollision();
            UpdateCollision();
        }

        /// <summary>
        /// Check to see if the Enemy collides with another Sprite
        /// </summary>
        /// <param name="sprite">The Sprite to check collision against</param>
        /// <returns>True if their spheres intersect, false otherwise</returns>
        virtual public bool Collides(Sprite sprite)
        {
            // Make sure these sprites can even collide
            if (!Collideable || !sprite.Collideable)
                return false;

            // BoundingSphere collision check
            var result = CollisionSphere.Intersects(sprite.CollisionSphere);

            // If there was a collision
            if (result)
            {
                // Stop moving
                Velocity = Vector2.Zero;

                // Disable collision on this Enemy
                EnableCollision(false);

                // Play the Break animation
                PlayAnimation("Break");

                // It has been damaged
                IsDamaged = true;

                // Play the break sound
                BreakSound.Play();
            }

            // Return the collision result
            return result;
        }

        /// <summary>
        /// Called when the Sprite animation has ended (Break)
        /// </summary>
        public override void OnAnimationEnd()
        {
            // Hide and despawn the Enemy
            Visible = false;
            HasSpawned = false;
        }
    }
}
