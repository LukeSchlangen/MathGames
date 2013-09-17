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

        LoginInput input = new LoginInput();
        Random random = new Random();

        //Textures for level 1
        private Texture2D background;
        private Texture2D hero;
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
            background = content.Load<Texture2D>("Level1/resized_level1_background");

            //Play music in repeating loop
            Song backgroundMusic;
            backgroundMusic = content.Load<Song>("Level1/level1_background_music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;

            hero = content.Load<Texture2D>("General/Hero/math_hero_character");
            heroPosition = new Vector2(-200 * scale, 800 * scale);
            heroCollisionBox = new Rectangle(((int)(heroPosition.X)), ((int)(heroPosition.Y)), (int)(hero.Width), (hero.Height));

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

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //always draw the login background
            //remember to scale stuff when appropriate!

            spriteBatch.Draw(background, new Vector2(0,0), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(hero, heroPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetTwo, movingPlanetTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetThree, movingPlanetThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetFour, movingPlanetFourPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetFive, movingPlanetFivePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipOne, shipOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipTwo, shipTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipThree, shipThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipFour, shipFourPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void checkClick()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);

        }
    }
}
