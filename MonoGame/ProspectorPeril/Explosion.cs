using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    class Explosion : Sprite
    {
        public bool IsRising = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Explosion()
        {
        }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Explosion(Texture2D texture) : base(texture)
        {            
        }

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Explosion(List<Texture2D> textures)
            : base(textures)
        {            
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            TimeSpan deltaTime = gameTime.ElapsedGameTime;
            float deltaSeconds = (float)deltaTime.Milliseconds;

            if (IsRising)
            {
                Position.Y -= deltaSeconds * 0.05f;
                Visible = true;
            }
            else
            {
                if (Position.Y < 122)
                    Position.Y += deltaSeconds * 0.05f;
                else
                    Visible = false;
            }
        }
    }
}
