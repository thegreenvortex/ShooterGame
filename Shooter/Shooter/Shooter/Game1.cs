using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        Player player;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        // A movement speed for the player
        float playerMoveSpeed;

        // Image used to display the static background
        Texture2D mainBackground;

        // Parallaxing Layers
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

        // Enemies
        Texture2D balloonTexture;
        List<Balloons> balloons;

        Texture2D bulletBillTexture;
        List<BulletBill> bulletBills;


        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan bulletSpawnTime;
        TimeSpan previousSpawnTime;
        TimeSpan previousBulletSpawnTime;

        // A random number generator
        Random random;

        Texture2D projectileTexture;
        Texture2D fireballTexture;
        List<Projectile> projectiles;
        List<BalloonProjectile> balloonProjectiles;

        // The rate of fire of the player laser
        //TimeSpan fireTime;
        //TimeSpan previousFireTime;
        int SpawnProjectile;
        int SpawnBProjectile;

        Texture2D explosionTexture;
        List<Animation> explosions;

        // The sound that is played when a laser is fired
        SoundEffect laserSound;

        // The sound used when the player or an enemy dies
        SoundEffect explosionSound;

        // The music played during gameplay
        Song gameplayMusic;

        //Number that holds the player score
        int score;
        // The font used to display UI elements
        SpriteFont font;

        //bool bPlayer;

        int bProjectileCount;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1600;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player();

            playerMoveSpeed = 8.0f;

            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();


            // Initialize the enemies list
            balloons = new List<Balloons>();

            bulletBills = new List<BulletBill>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;
            previousBulletSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(3.5f);

            bulletSpawnTime = TimeSpan.FromSeconds(.75f);

            // Initialize our random number generator
            random = new Random();

            projectiles = new List<Projectile>();
            balloonProjectiles = new List<BalloonProjectile>();

            SpawnProjectile = 10;
            SpawnBProjectile = 75;

            //bPlayer = false;

            explosions = new List<Animation>();

            //Set player's score to zero
            score = 0;

            // Set the laser to fire every quarter second
            //fireTime = TimeSpan.FromSeconds(.15f);

            bProjectileCount = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            player.Initialize(Content.Load<Texture2D>("BowsersAirship"), playerPosition);

            balloonTexture = Content.Load<Texture2D>("Boo");
            bulletBillTexture = Content.Load<Texture2D>("bullet_bill");

            // Load the parallaxing background
            bgLayer1.Initialize(Content, "bgLayer1", GraphicsDevice.Viewport.Width, -1);
            bgLayer2.Initialize(Content, "bgLayer2", GraphicsDevice.Viewport.Width, -2);

            mainBackground = Content.Load<Texture2D>("mainbackground");

            projectileTexture = Content.Load<Texture2D>("laser");
            fireballTexture = Content.Load<Texture2D>("FireballMissle");

            explosionTexture = Content.Load<Texture2D>("explosion");

            // Load the music
            gameplayMusic = Content.Load<Song>("sound/gameMusic");

            // Load the laser and explosion sound effect
            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");

            // Start the music right away
            PlayMusic(gameplayMusic);

            // Load the score font
            font = Content.Load<SpriteFont>("gameFont");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            //bPlayer = true;
            //Update the player
            UpdatePlayer(gameTime);

            // Update the parallaxing background
         
            bgLayer1.Update();
            bgLayer2.Update();

            //bPlayer = false;
            // Update the enemies
            UpdateEnemies(gameTime);
            UpdateBulletBill(gameTime);

            // Update the collision
            

            UpdateProjectiles();
            UpdateBalloonProjectiles();

            UpdateCollision();

            UpdateExplosions(gameTime);
            UpdatePartExplosions(gameTime);

            base.Update(gameTime);
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            //bPlayer = true;
            // Get Thumbstick Controls
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            if (currentKeyboardState.IsKeyDown(Keys.Up) ||
            currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
            currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }



            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

            // Fire only every interval we set as the fireTime
            if (currentKeyboardState.IsKeyDown(Keys.A) || 
            currentGamePadState.IsButtonDown(Buttons.X))
            {
                //bPlayer = true;
                // Reset our current time
                //previousFireTime = gameTime.TotalGameTime;

                if (SpawnProjectile <= 0)
                {
                    SpawnProjectile = 10;
                    // Play the laser sound
                    laserSound.Play();
                    AddProjectile(player.Position + new Vector2(player.Width / 2, 0));

                }

                if (SpawnProjectile > 0)
                {
                    SpawnProjectile--;
                }
            }








            // reset score if player health goes to zero
            if (player.Health <= 0)
            {

                AddExplosion(player.Position);
                Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
                player.Initialize(Content.Load<Texture2D>("BowsersAirship"), playerPosition);

                for (int i = balloons.Count - 1; i >= 0; i--)
                {
                    balloons.RemoveAt(i);
                }

                for (int i = bulletBills.Count - 1; i >= 0; i--)
                {
                    bulletBills.RemoveAt(i);
                }

                for (int i = balloonProjectiles.Count - 1; i >= 0; i--)
                {
                    balloonProjectiles.RemoveAt(i);
                }

                player.Health = 100;
                score = 0;

            }
            //bPlayer = false;
        }


        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);

            for (int i = 0; i < balloonProjectiles.Count; i++)
            {
                rectangle2 = new Rectangle((int)balloonProjectiles[i].Position.X -
                balloonProjectiles[i].Width / 2, (int)balloonProjectiles[i].Position.Y -
                balloonProjectiles[i].Height / 2, balloonProjectiles[i].Width, balloonProjectiles[i].Height);

                if (rectangle2.Intersects(rectangle1))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Health -= balloonProjectiles[i].Damage;
                    balloonProjectiles[i].Active = false;
                    bProjectileCount--;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                        player.Active = false;
                }



            }

            // Do the collision between the player and the balloons
            for (int i = 0; i < balloons.Count; i++)
            {
                rectangle2 = new Rectangle((int)balloons[i].Position.X, (int)balloons[i].Position.Y, balloons[i].Width, balloons[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Health -= balloons[i].Damage;

                    // Since the enemy collided with the player
                    // destroy it
                    balloons[i].Health = 0;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                        player.Active = false;
                }

            }

            for (int i = 0; i < bulletBills.Count; i++)
            {
                rectangle2 = new Rectangle((int)bulletBills[i].Position.X, (int)bulletBills[i].Position.Y, bulletBills[i].Width / 4, bulletBills[i].Height / 4);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Health -= bulletBills[i].Damage;

                    // Since the enemy collided with the player
                    // destroy it
                    bulletBills[i].Health = 0;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                        player.Active = false;
                }

            }

            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < balloons.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)balloons[j].Position.X, (int)balloons[j].Position.Y, balloons[j].Width, balloons[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        balloons[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }

                for (int j = 0; j < bulletBills.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)bulletBills[j].Position.X, (int)bulletBills[j].Position.Y, bulletBills[j].Width / 4, bulletBills[j].Height / 4);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        bulletBills[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }




            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < balloonProjectiles.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)balloonProjectiles[j].Position.X, (int)balloonProjectiles[j].Position.Y, balloonProjectiles[j].Width, balloonProjectiles[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        balloonProjectiles[j].Active = false;
                        projectiles[i].Active = false;
                    }
                }
            }


        }

        private void AddPartExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 0.4f, false);
            explosions.Add(explosion);
        }

        private void UpdatePartExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        private void AddEnemy()
        {
            // Create the animation object
            //Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            //enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
           

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + balloonTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

            // Create an enemy
            Balloons balloon = new Balloons();

            // Initialize the enemy
            balloon.Initialize(balloonTexture, position);

            // Add the enemy to the active enemies list
            balloons.Add(balloon);
        }


        private void UpdateEnemies(GameTime gameTime)
        {
            //bPlayer = false;
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                if(balloons.Count < 4)
                    AddEnemy();
            }

            // Update the Enemies
            for (int i = balloons.Count - 1; i >= 0; i--)
            {
                balloons[i].Update(gameTime);

                if (SpawnBProjectile <= 0)
                {
                    SpawnBProjectile = 75;
                    // Play the laser sound
                    laserSound.Play();
                    AddBalloonProjectile(balloons[i].Position + new Vector2(balloons[i].Width / 2, 0));

                }




                if (balloons[i].Active == false)
                {
 
                    // If not active and health <= 0
                    if (balloons[i].Health <= 0)
                    {
                        // Add an explosion
                        // Play the explosion sound
                        explosionSound.Play();
                        //Add to the player's score
                        score += balloons[i].Value;
                        AddExplosion(balloons[i].Position);
                    }

                    balloons.RemoveAt(i);
                }
            }

            if (SpawnBProjectile > 0)
            {
                SpawnBProjectile--;
            }

            //wwwbPlayer = true;

        }



        private void AddBulletBill()
        {
            // Create the animation object
            //Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            //enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);


            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + balloonTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

            // Create an enemy
            BulletBill bulletBill = new BulletBill();

            // Initialize the enemy
            bulletBill.Initialize(bulletBillTexture, position);

            // Add the enemy to the active enemies list
            bulletBills.Add(bulletBill);
        }

        private void UpdateBulletBill(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousBulletSpawnTime > bulletSpawnTime)
            {
                previousBulletSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                if(balloons.Count < 8)
                    AddBulletBill();
            }

            // Update the Enemies
            for (int i = bulletBills.Count - 1; i >= 0; i--)
            {
                bulletBills[i].Update(gameTime);

                if (bulletBills[i].Active == false)
                {
                    // If not active and health <= 0
                    if (bulletBills[i].Health <= 0)
                    {
                        // Add an explosion
                        // Play the explosion sound
                        explosionSound.Play();
                        //Add to the player's score
                        score += bulletBills[i].Value;
                        AddExplosion(bulletBills[i].Position);
                    }

                    bulletBills.RemoveAt(i);
                }
            }
        }




        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(GraphicsDevice.Viewport, fireballTexture, position);
            projectiles.Add(projectile);
        }

        private void UpdateProjectiles()
        {
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false)
                {
                    AddPartExplosion(projectiles[i].Position);
                    projectiles.RemoveAt(i);
                }
            }
        }

        private void AddBalloonProjectile(Vector2 position)
        {
            BalloonProjectile projectile = new BalloonProjectile();
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            balloonProjectiles.Add(projectile);
        }

        private void UpdateBalloonProjectiles()
        {
            // Update the Projectiles
            for (int i = balloonProjectiles.Count - 1; i >= 0; i--)
            {
                balloonProjectiles[i].Update();

                if (balloonProjectiles[i].Active == false)
                {
                    balloonProjectiles[i].Position.X -= 30;
                    AddPartExplosion(balloonProjectiles[i].Position);
                    balloonProjectiles.RemoveAt(i);
                }
            }
        }


        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing
            spriteBatch.Begin();

            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

            bgLayer1.Draw(spriteBatch);
            
            bgLayer2.Draw(spriteBatch);

            // Draw the Enemies
            for (int i = 0; i < balloons.Count; i++)
            {
                //if(i %2 == 0)
                balloons[i].Draw(spriteBatch);
            }

            for (int i = 0; i < bulletBills.Count; i++)
            {
                //if(i %2 == 0)
                bulletBills[i].Draw(spriteBatch);
            }

            // Draw the Projectiles
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(spriteBatch);
            }

            for (int i = 0; i < balloonProjectiles.Count; i++)
            {
                balloonProjectiles[i].Draw(spriteBatch);
            }

            // Draw the explosions


            // Draw the score
            spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

            spriteBatch.DrawString(font, "Time: " + gameTime.TotalGameTime.Minutes + " : " + gameTime.TotalGameTime.Seconds + " : " + gameTime.TotalGameTime.Milliseconds, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 1350, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

            // Draw the Player
            player.Draw(spriteBatch);

            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }

            // Stop drawing
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
