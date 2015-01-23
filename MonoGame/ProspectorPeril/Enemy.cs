using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProspectorPeril
{    
    public class Enemy : Sprite
    {
        public bool HasSpawned;
        public bool IsDamaged = false; 
        public Vector2 Velocity;        
        public SoundEffect BreakSound = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Enemy()
        {
            Layer = 6;
            Position = new Vector2(-500, -500);
        }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Enemy(Texture2D texture) : base(texture)
        {
            Layer = 6;
            Position = new Vector2(-500, -500);
        }

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Enemy(List<Texture2D> textures)
            : base(textures)
        {
            Layer = 6;
            Position = new Vector2(-500, -500);
        }

        public override void Update(GameTime gameTime)
        {
            if (Position.X < -200 || Position.X > 500 || Position.Y > 400)
                HasSpawned = false;
            
            base.Update(gameTime);
        }

        virtual public void Spawn(Vector2 position, Vector2 velocity)
        {
            Frame = 0;
            HasSpawned = true;
            Visible = true;
            Position = position;
            Velocity = velocity;
            IsDamaged = false;

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
                IsDamaged = true;
                BreakSound.Play();
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
