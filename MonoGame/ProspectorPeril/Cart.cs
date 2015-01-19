using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    class Cart : Enemy
    {
        Vector2 DecayingVelocity = Vector2.Zero;

        public Cart() : base()
        {            
        }

        public Cart(Texture2D texture) : base(texture)
        {
        }

        public Cart(List<Texture2D> textures)
            : base(textures)
        {            
        }

        public override void Spawn()
        {
            DecayingVelocity = Velocity;
            base.Spawn();
        }

        public override void Update(GameTime gameTime)
        {
            if (HasSpawned)
            {
                Position += DecayingVelocity;
                DecayingVelocity.Y += 0.03f;
            }

            base.Update(gameTime);
        }
    }
}
