using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ProspectorPeril
{
    class Rock : Sprite, Enemy
    {
        public bool HasSpawned { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Rock()
        {
        }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Rock(Texture2D texture) : base(texture)
        {            
        }

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Rock(List<Texture2D> textures) : base(textures)
        {            
        }

        public void UpdateEnemy(GameTime gameTime)
        {
            Update(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void DrawEnemy(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public void Spawn(Vector2 position)
        {
            HasSpawned = true;
        }

        public bool Collides(Sprite sprite)
        {
            return CollisionBox.Intersects(sprite.CollisionBox);
        }
    }
}
