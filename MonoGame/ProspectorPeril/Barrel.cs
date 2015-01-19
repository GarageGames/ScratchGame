using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    public class Barrel : Enemy
    {
        bool IsPrimed = false;
        float PrimeTimer = 1300;
        Vector2 DecayingVelocity = Vector2.Zero;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Barrel() : base()
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
        public Barrel(List<Texture2D> textures)
            : base(textures)
        {            
        }
        
        public override void Update(GameTime gameTime)
        {
            if (HasSpawned)
            {
                if (!IsPrimed)
                {
                    Position += DecayingVelocity;
                    DecayingVelocity.Y += 0.03f;
                }

                PrimeTimer -= gameTime.ElapsedGameTime.Milliseconds;

                if (PrimeTimer <= 0 && !IsDamaged)
                {
                    IsPrimed = !IsPrimed;

                    if (IsPrimed)
                        PlayAnimation("Prime");
                    else
                        PlayAnimation("Idle");

                    PrimeTimer = 2000;
                }
            }

            base.Update(gameTime);
        }
        
        public override bool Collides(Sprite sprite)
        {            
            bool collided = base.Collides(sprite);

            var player = (Player)sprite;

            if (IsPrimed && collided)
            {
                PlayAnimation("Explode");
                player.Damage();
            }

            return collided;
        }

        public override void Spawn()
        {
            DecayingVelocity = Velocity;
            IsPrimed = false;
            PrimeTimer = 2000;
            base.Spawn();
        }

        public override void OnAnimationEnd()
        {
            if (CurrentAnimation.Name == "Break" || CurrentAnimation.Name == "Explode")
            {
                Visible = false;
                HasSpawned = false;
            }
        }
    }
}
