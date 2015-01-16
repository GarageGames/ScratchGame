using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    public class Barrel : Enemy
    {
        bool IsPrimed = false;
        float PrimeTimer = 2000;

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
            // Prime stuff

            // Base stuff
            base.Update(gameTime);
        }

        //public override void Spawn(Vector2 position, Vector2 velocity)
        //{
        //    base.Spawn(position, velocity);
        //}
        
        //public bool Collides(Sprite sprite)
        //{
        //    if (!sprite.Collideable)
        //        return false;

        //    var result = CollisionSphere.Intersects(sprite.CollisionSphere);

        //    if (result)
        //    {                
        //        EnableCollision(false);
        //        PlayAnimation("Break");
        //        Velocity = Vector2.Zero;
        //    }

        //    return result;
        //}

        //public override void OnAnimationEnd()
        //{
        //    Position = new Vector2(500, 500);
        //    Visible = false;
        //    HasSpawned = false;
        //}
        
        public override bool Collides(Sprite sprite)
        {
            var player = (Player)sprite;
            bool collided = CollisionSphere.Intersects(sprite.CollisionSphere);

            if (IsPrimed && collided)
                player.Damage();

            return collided;
        }

        public void Spawn(Random rand)
        {
            IsPrimed = true;
            PrimeTimer = 2000;
        }
    }
}
