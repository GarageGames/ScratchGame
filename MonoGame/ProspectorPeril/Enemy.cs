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
        public Vector2 SpawnPosition;
        public Vector2 SpawnVelocity;
        public bool HasSpawned;
        public Vector2 Velocity;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Enemy()
        {
            Position = new Vector2(-500, -500);
        }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Enemy(Texture2D texture) : base(texture)
        {
            Position = new Vector2(-500, -500);
        }

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Enemy(List<Texture2D> textures)
            : base(textures)
        {
            Position = new Vector2(-500, -500);
        }

        public override void Update(GameTime gameTime)
        {
            if (Position.X < -240 || Position.X > 500 || Position.Y > 400)
            {
                HasSpawned = false;
                Position = SpawnPosition;
                Velocity = SpawnVelocity;
            }

            base.Update(gameTime);
        }

        virtual public void Spawn()
        {
            Frame = 0;
            HasSpawned = true;
            Visible = true;
            Position = SpawnPosition;
            Velocity = SpawnVelocity;

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
                Velocity = Vector2.Zero;
                EnableCollision(false);
                PlayAnimation("Break");                
            }

            return result;
        }

        public override void OnAnimationEnd()
        {            
            Visible = false;
            HasSpawned = false;
        }
    }
}
