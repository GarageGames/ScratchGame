using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProspectorPeril
{    
    public class Enemy : Sprite
    {
        public bool HasSpawned { get; set; }

        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Enemy()
        {
            Position = new Vector2(500, 500);
        }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Enemy(Texture2D texture) : base(texture)
        {
            Position = new Vector2(500, 500);
        }

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Enemy(List<Texture2D> textures)
            : base(textures)
        {
            Position = new Vector2(500, 500);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        virtual public void Spawn(Vector2 position, Vector2 velocity)
        {
            Frame = 0;
            Position = position;
            HasSpawned = true;
            Visible = true;
            Velocity = velocity;            
            EnableCollision();
            UpdateCollision();            
        }

        virtual public bool Collides(Sprite sprite)
        {
            if (!sprite.Collideable)
                return false;

            var result = CollisionSphere.Intersects(sprite.CollisionSphere);

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
