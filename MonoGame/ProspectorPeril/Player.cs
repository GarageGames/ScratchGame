﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ProspectorPeril
{
    class Player : Sprite
    {
        public enum PlayerState
        {
            Idle,
            Ascending,
            Descending,
            Attacking,
            Bounce,
            Dead
        };

        public PlayerState State;
        public int Speed = 0;
        public int Lives = 3;

        float playerVerticalVelocity = 0.0f;
        float speedTimer = 2000;        
        int speedDecay = 1;
        
        public bool IsAscending
        {
            get
            {
                return State == PlayerState.Ascending;
            }
        }

        public void Damage()
        {
            Lives--;
        }

        public void Die()
        {
            State = PlayerState.Dead;
        }

        public void Ascend()
        {
            State = PlayerState.Ascending;
            PlayAnimation("Launch");
        }

        public void Attack()
        {
            if (State == PlayerState.Descending || State == PlayerState.Bounce)
                State = PlayerState.Attacking;
        }

        public void Bounce()
        {
            Speed++;
            PlayAnimation("Attack");
            State = PlayerState.Bounce;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Player() { }

        /// <summary>
        /// Constructor used when a sprite only uses a single texture
        /// </summary>
        /// <param name="texture">A valid Texture2D this sprite will render</param>
        public Player(Texture2D texture) : base(texture) {}

        /// <summary>
        /// Constructor used when a sprite uses multiple textures
        /// </summary>
        /// <param name="textures">Generic list of Texture2D objects</param>
        public Player(List<Texture2D> textures) : base(textures) {}

        public override void Update(GameTime gameTime)
        {            
            if (Speed <= 0)
                Speed = 0;
            else if (Speed > 99)
                Speed = 99;

            // Update the player based on his state
            switch (State)
            {
                case PlayerState.Idle:
                    break;

                case PlayerState.Ascending:
                    playerVerticalVelocity = -3;
                    Collideable = false;
                    if (Position.Y <= 0)
                    {
                        State = PlayerState.Descending;
                        PlayAnimation("Float");
                    }

                    break;

                case PlayerState.Descending:
                    playerVerticalVelocity = 1.5f;
                    Collideable = true;
                    speedTimer -= gameTime.ElapsedGameTime.Milliseconds;

                    if (speedTimer <= 0)
                    {
                        speedTimer = 2000;
                        Speed -= speedDecay;
                    }

                    break;

                case PlayerState.Bounce:
                    playerVerticalVelocity = -3;

                    if (Position.Y <= 0)
                    {
                        State = PlayerState.Descending;
                        PlayAnimation("Float");
                    }

                    break;
                case PlayerState.Attacking:
                    Collideable = true;
                    playerVerticalVelocity = 9;
                    break;

                case PlayerState.Dead:
                    break;
            }

            Position.Y += playerVerticalVelocity;

            base.Update(gameTime);
        }
    }
}
