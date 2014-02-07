using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class Balloons
    {
        // Animation representing the enemy
        public Texture2D BalloonTexture;

        // The position of the enemy ship relative to the top left corner of thescreen
        public Vector2 Position;

        // The state of the Enemy Ship
        public bool Active;

        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Health;

        // The amount of damage the enemy inflicts on the player ship
        public int Damage;

        // The amount of score the enemy will give to the player
        public int Value;

        public int Up, Down, UpDown;

        // Get the width of the enemy ship
        public int Width
        {
            get { return BalloonTexture.Width; }
        }

        // Get the height of the enemy ship
        public int Height
        {
            get { return BalloonTexture.Height; }
        }

        // The speed at which the enemy moves
        float enemyMoveSpeed;

        public void Initialize(Texture2D texture, Vector2 position)
        {
            // Load the enemy ship texture
            BalloonTexture = texture;

            // Set the position of the enemy
            Position = position;

            // We initialize the enemy to be active so it will be update in the game
            Active = true;


            // Set the health of the enemy
            Health = 24;

            // Set the amount of damage the enemy can do
            Damage = 20;

            // Set how fast the enemy moves
            enemyMoveSpeed = 1.0f;


            // Set the score value of the enemy
            Value = 100;

            Down = 0;
            Up = 125;
            UpDown = 0;

        }

        public void Update(GameTime gameTime)
        { 
            // The enemy always moves to the left so decrement it's xposition
            
            Position.X -= enemyMoveSpeed;
            //.Y = Position.Y - (float)gameTime.ElapsedGameTime.TotalSeconds - .1f;

            if (UpDown == 0)
            {
                if (Up > 0)
                {
                    Position.X -= enemyMoveSpeed;
                    Position.Y -= enemyMoveSpeed/1.5f;
                    Up--;
                    UpDown += 1;

                    if (Up == 0)
                    {
                        Down = 75;
                    }
                }

                if (Down > 0)
                {
                    Position.X -= enemyMoveSpeed/3;
                    Position.Y += enemyMoveSpeed;
                    Down--;
                    UpDown++;

                    if (Down == 0)
                    {
                        Up = 125;
                    }
                }

            }
            else
            {
                Position.X -= enemyMoveSpeed;
                UpDown--;
            }

            
            // Update the position of the Animation
            //BalloonTexture. = Position;

            // Update Animation
            //EnemyAnimation.Update(gameTime);

            // If the enemy is past the screen or its health reaches 0 then deactivateit
            if (Position.X < -Width || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet fromthe 
                // active game list
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            spriteBatch.Draw(BalloonTexture, Position, null, Color.White, 0f, Vector2.Zero, 1.15f, SpriteEffects.None, 0f);
        }

    }
}
