using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ProspectorPeril
{
    class Launcher : Sprite
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Launcher()
        {
        }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Launcher(Texture2D texture) : base(texture)
        {            
        }

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Launcher(List<Texture2D> textures)
            : base(textures)
        {            
        }

        public override void Update(GameTime gameTime)
        {
            if (Position.Y < 328)
            {
                Position.Y += 1;
                base.Update(gameTime);                
            }
        }
    }
}
