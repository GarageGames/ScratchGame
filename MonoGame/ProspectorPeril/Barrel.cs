using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ProspectorPeril
{
    public class Barrel : Sprite, Enemy
    {
        public bool HasSpawned { get; set; }
        bool IsPrimed = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Barrel()
        {
        }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Barrel(Texture2D texture) : base(texture)
        {            
        }

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Barrel(List<Texture2D> textures) : base(textures)
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

        public bool Collides(Sprite sprite)
        {
            var player = (Player)sprite;
            bool collided = CollisionBox.Intersects(sprite.CollisionBox);

            if (IsPrimed && collided)
                player.Damage();

            return collided;
        }

        public void Spawn(Vector2 position)
        {
            HasSpawned = true;
        }
    }
}
