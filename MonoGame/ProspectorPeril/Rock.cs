using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    class Rock : Enemy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Rock() : base()
        {
            SpawnTimeRemaining = SpawnTiming = 5750;
        }

        /// <summary>
        /// Constructor used when a Rock only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Rock(Texture2D texture) : base(texture)
        {
            SpawnTimeRemaining = SpawnTiming = 5750;
        }

        /// <summary>
        /// Constructor used when a Rock uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Rock(List<Texture2D> textures) : base(textures)
        {
            SpawnTimeRemaining = SpawnTiming = 5750;
        }

        /// <summary>
        /// Update the Rock
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public override void Update(GameTime gameTime)
        {
            // If the Rock has spawned, update its position based on its velocity
            if (HasSpawned)
                Position += Velocity;                                   

            // Enemy Update method
            base.Update(gameTime);
        }        
    }
}
