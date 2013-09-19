using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;

namespace ProjectDelta
{
    public class Level1
    {
        User user;
        DynamoDBContext context;

        private enum State
        {
            None,
        }

        private State state;
        private float scale;

        private float planetSpeed = .01f;
        private float shipSpeed = .25f;
        private float backgroundSpeed = .1f;
                
        Random random = new Random();
        Animation animation;

        //Textures for level 1
        private Texture2D backgroundOne;
        private Texture2D backgroundTwo;
        private Texture2D backgroundThree;
        private Texture2D heroRunning;
        private Texture2D planetTwo;
        private Texture2D planetThree;
        private Texture2D planetFour;
        private Texture2D planetFive;
        private Texture2D shipOne;
        private Texture2D shipTwo;
        private Texture2D shipThree;
        private Texture2D shipFour;

        //Font for level 1
        private SpriteFont font;

        //Vectors for level 1
        private Vector2 heroPosition;
        private Vector2 movingPlanetTwoPosition;
        private Vector2 movingPlanetThreePosition;
        private Vector2 movingPlanetFourPosition;
        private Vector2 movingPlanetFivePosition;
        private Vector2 shipOnePosition;
        private Vector2 shipTwoPosition;
        private Vector2 shipThreePosition;
        private Vector2 shipFourPosition;
        private Vector2 backgroundOnePosition;
        private Vector2 backgroundTwoPosition;
        private Vector2 backgroundThreePosition;

        //Rectangles for the collision boxes for the login
        private Rectangle heroCollisionBox;
        
        //Mouse states
        private MouseState current;
        private MouseState previous;

        public Level1(DynamoDBContext context)
        {
            this.context = context;
        }

        public void Initialize(float scale)
        {
            this.scale = scale;
            state = State.None;
        }

        //Specifies which content is loaded for the login screen

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            backgroundOne = content.Load<Texture2D>("Level1/background_level_1a");
            backgroundTwo = content.Load<Texture2D>("Level1/background_level_1b");
            backgroundThree = content.Load<Texture2D>("Level1/background_level_1c");
            backgroundOnePosition = new Vector2(0,0);
            backgroundTwoPosition = new Vector2(backgroundOne.Width*scale, 0);
            backgroundThreePosition = new Vector2(backgroundOne.Width*scale + backgroundTwo.Width*scale, 0);

            //Play music in repeating loop
            Song backgroundMusic;
            backgroundMusic = content.Load<Song>("Level1/level1_background_music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;

            heroRunning = content.Load<Texture2D>("General/Hero/running_sprite_sheet_5x5");
            heroPosition = new Vector2(-200 * scale, 800 * scale);
            
            planetTwo = content.Load<Texture2D>("General/Planets/planet_2");
            planetThree = content.Load<Texture2D>("General/Planets/planet_3");
            planetFour = content.Load<Texture2D>("General/Planets/planet_4");
            planetFive = content.Load<Texture2D>("General/Planets/planet_5");

            shipOne = content.Load<Texture2D>("General/Ships/good_ship_1");
            shipTwo = content.Load<Texture2D>("General/Ships/enemy_ship_1");
            shipThree = content.Load<Texture2D>("General/Ships/good_fleet_1");
            shipFour = content.Load<Texture2D>("General/Ships/enemy_fleet_1");

            //any scalar value needs to take into consideration
            //the scale factor to fix resolution issues
            movingPlanetTwoPosition = new Vector2(-2800 * scale, 100 * scale);
            movingPlanetThreePosition = new Vector2(-5250 * scale, 1000 * scale);
            movingPlanetFourPosition = new Vector2(-9000 * scale, 1200 * scale);
            movingPlanetFivePosition = new Vector2(-12000 * scale, -800 * scale);

            shipOnePosition = new Vector2(-2000 * scale, 100 * scale);
            shipTwoPosition = new Vector2(-6000 * scale, 900 * scale);
            shipThreePosition = new Vector2(-15000 * scale, 500 * scale);
            shipFourPosition = new Vector2(-23000 * scale, 300 * scale);

            animation = new Animation(heroRunning, 5, 5, scale);
        }

        public bool Update(GameTime gameTime)
        {
            //always update the planets flying
                        
            movingPlanetTwoPosition.X -= (float)(planetSpeed * 100 * Math.Sin((double)gameTime.ElapsedGameTime.TotalMilliseconds) * MathHelper.Pi / 2);
            movingPlanetTwoPosition.Y -= planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            movingPlanetThreePosition.X -= (float)(planetSpeed * 100 * Math.Sin((double)gameTime.ElapsedGameTime.TotalMilliseconds) * MathHelper.Pi / 2);
            movingPlanetThreePosition.Y -= planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            movingPlanetFourPosition.X -= (float)(planetSpeed * 100 * Math.Sin((double)gameTime.ElapsedGameTime.TotalMilliseconds) * MathHelper.Pi / 2);
            movingPlanetFourPosition.Y -= planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            movingPlanetFivePosition.X -= (float)(planetSpeed * 100 * Math.Sin((double)gameTime.ElapsedGameTime.TotalMilliseconds) * MathHelper.Pi / 2);
            movingPlanetFivePosition.Y += planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            shipOnePosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            shipTwoPosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            shipThreePosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            shipFourPosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            cycleBackground(gameTime);          

            animation.stationaryScroll(gameTime);

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //always draw the login background
            //remember to scale stuff when appropriate!

            spriteBatch.Draw(backgroundOne, backgroundOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backgroundTwo, backgroundTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backgroundThree, backgroundThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetTwo, movingPlanetTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetThree, movingPlanetThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetFour, movingPlanetFourPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetFive, movingPlanetFivePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipOne, shipOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipTwo, shipTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipThree, shipThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipFour, shipFourPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            animation.Draw(spriteBatch);
        }

        private void checkClick()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);

        }

        private void cycleBackground(GameTime gameTime)
        {
            backgroundOnePosition.X -= backgroundSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            backgroundTwoPosition.X -= backgroundSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            backgroundThreePosition.X -= backgroundSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            if (backgroundOnePosition.X < -backgroundOne.Width*scale)
            {
                backgroundOnePosition.X = backgroundThreePosition.X + backgroundThree.Width*scale;
            }
            if (backgroundTwoPosition.X < -backgroundTwo.Width*scale)
            {
                backgroundTwoPosition.X = backgroundOnePosition.X + backgroundOne.Width*scale;
            }
            if (backgroundThreePosition.X < -backgroundThree.Width*scale)
            {
                backgroundThreePosition.X = backgroundTwoPosition.X + backgroundTwo.Width*scale;
            }
        }
    }
}
