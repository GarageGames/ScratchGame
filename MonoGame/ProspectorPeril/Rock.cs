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
            Visible = false;
        }

        public Rock(Texture2D texture) : base(texture)
        {
            Visible = false;
        }

        public Rock(List<Texture2D> textures) : base(textures)
        {
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (HasSpawned)
                Position += Velocity;                                   

            base.Update(gameTime);
        }        
    }
}
