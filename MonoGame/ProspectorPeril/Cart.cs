using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    class Cart : Enemy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Cart() : base()
        {
            SpawnTimeRemaining = SpawnTiming = 1000f;
        }

        /// <summary>
        /// Constructor used when a Cart only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Cart(Texture2D texture) : base(texture)
        {
            SpawnTimeRemaining = SpawnTiming = 1000f;
        }

        /// <summary>
        /// Constructor used when a Cart uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Cart(List<Texture2D> textures)
            : base(textures)
        {
            SpawnTimeRemaining = SpawnTiming = 1000f;
        }

        /// <summary>
        /// Spawn the Cart
        /// </summary>
        /// <param name="position">Starting position</param>
        /// <param name="velocity">Starting velocity</param>
        public override void Spawn(Vector2 position, Vector2 velocity)
        {
            base.Spawn(position, velocity);            
        }

        /// <summary>
        /// Update the Cart
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public override void Update(GameTime gameTime)
        {
            // If it has spawned
            if (HasSpawned)
            {
                // Update it's position based on the decaying velocity
                Position += Velocity;

                // Changed velocity to make the object start dropping
                Velocity.Y += 0.03f;
            }

            // Enemy Update routine
            base.Update(gameTime);
        }
    }
}
