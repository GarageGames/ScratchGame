using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    class Rock : Sprite, Enemy
    {
        public bool HasSpawned { get; set; }

        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Rock()
        {
            Position = new Vector2(500, 500);
        }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Rock(Texture2D texture) : base(texture)
        {
            Position = new Vector2(500, 500);
        }

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Rock(List<Texture2D> textures) : base(textures)
        {
            Position = new Vector2(500, 500);
        }

        public void UpdateEnemy(GameTime gameTime)
        {
            Update(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (HasSpawned)
            {
                Position += Velocity;
            }

            base.Update(gameTime);
        }

        public void DrawEnemy(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public void Spawn(Vector2 position)
        {
            Random rand = new Random();
            Frame = 0;

            if (rand.Next(0, 1) == 1)
            {
                Position.X = -1 * Textures[0].Width;                
                Velocity = new Vector2(rand.Next(1, 3), rand.Next(1, 2));
            }
            else
            {
                Position.X = 320 + Textures[0].Width;
                Velocity = new Vector2(rand.Next(-3, -1), rand.Next(1, 2));
            }

            Position.Y = rand.Next(25, 75);
            EnableCollision();
            UpdateCollision();            
            HasSpawned = true;
            Visible = true;
        }

        public bool Collides(Sprite sprite)
        {
            var result = CollisionBox.Intersects(sprite.CollisionBox);

            if (result)
            {                
                EnableCollision(false);
                PlayAnimation("Break");
                Velocity = Vector2.Zero;
            }

            return result;
        }

        public override void OnAnimationEnd()
        {
            Position = new Vector2(500, 500);
            Visible = false;
            HasSpawned = false;
        }
    }
}
