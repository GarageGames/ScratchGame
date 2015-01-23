using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProspectorPeril
{
    class Rock : Enemy
    {
        public Rock() : base()
        {
        }

        public Rock(Texture2D texture) : base(texture)
        {
        }

        public Rock(List<Texture2D> textures) : base(textures)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (HasSpawned)
                Position += Velocity;                                   

            base.Update(gameTime);
        }        
    }
}
