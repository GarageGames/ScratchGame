using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProspectorPeril
{
    /// <summary>
    /// Very basic Sprite class
    /// </summary>
    class Sprite
    {
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
        /// Size of sprite on screen
        /// </summary>
        public Vector2 Size = new Vector2(1.0f, 1.0f);

        /// <summary>
        /// Whether the sprite should be drawn to the screen or not
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// Order/depth of sprite on screen
        /// </summary>
        public int Layer = 0;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Sprite()
        {

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
        /// Draw the sprite to the screen using the XNA/MonoGame stock drawing object
        /// </summary>
        /// <param name="spriteBatch">A valid SpriteBatch that does all the drawing for the game</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            var currentTexture = Textures[Frame];

            if (currentTexture != null)
                spriteBatch.Draw(currentTexture, Position, null, Color.White, 0, Vector2.Zero, Size, SpriteEffects.None, Layer);
        }
    }
}
