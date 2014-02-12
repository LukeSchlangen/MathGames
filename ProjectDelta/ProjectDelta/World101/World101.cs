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
    public class World101
    {
        private static int COUNT_TO_CONTINUE = 10;
        private static int MAX_STAGE = 10;
        DynamoDBContext context;
        private int correctInARow = 0;
        private int worldStage = 1;

        private float scale;

        private float planetSpeed = .01f;
        //private float shipSpeed = .25f;
        private float backgroundSpeed = .1f;
        private float backupPlanetSpeed = .01f;
        private float backupBackgroundSpeed = .1f;

        //Textures for level 1
        private Texture2D backgroundOne;
        private Texture2D backgroundTwo;
        private Texture2D backgroundThree;
        //private Texture2D planetTwo;
        //private Texture2D planetThree;
        //private Texture2D planetFour;
        //private Texture2D planetFive;
        //private Texture2D shipOne;
        //private Texture2D shipTwo;
        //private Texture2D shipThree;
        //private Texture2D shipFour;

        ////Vectors for level 1
        //private Vector2 movingPlanetTwoPosition;
        //private Vector2 movingPlanetThreePosition;
        //private Vector2 movingPlanetFourPosition;
        //private Vector2 movingPlanetFivePosition;
        //private Vector2 shipOnePosition;
        //private Vector2 shipTwoPosition;
        //private Vector2 shipThreePosition;
        //private Vector2 shipFourPosition;
        private Vector2 backgroundOnePosition;
        private Vector2 backgroundTwoPosition;
        private Vector2 backgroundThreePosition;

        //Mouse states
        private MouseState current;
        private MouseState previous;

        private Hero hero;
        private World101Monster monsterOne;
        private World101Monster monsterTwo;
        private int currentMonster;
        private World101Input world101Input;
        private World101Text world101Text = new World101Text();
        private Random random = new Random();

        private bool answerDone = false;
        private bool showQuestion = true;
        private bool badInput = false;
        private bool heroDead = false;

        //This is an array of HashSets that should allow you store data
        //as you explained in email.
        private Dictionary<string, int>[] stageProblems;

        public World101(DynamoDBContext context)
        {
            this.context = context;
        }

        public void Initialize(float scale, int screenX)
        {
            this.scale = scale;
            monsterOne = new World101Monster(1600, 800, scale, backgroundSpeed, screenX);
            monsterTwo = new World101Monster(2600, 800, scale, backgroundSpeed, screenX);
            hero = new Hero();
            hero.Initialize(scale);
            world101Input = new World101Input(scale);
            world101Text.Initialize(scale);
            currentMonster = 1;
        }

        //Specifies which content is loaded for level 1

        public void LoadContent(ContentManager content, int worldStage)
        {
            this.worldStage = worldStage;
            loadExtraObjects(content);
            world101Input.LoadContent(content);
            hero.LoadContent(content);
            monsterOne.LoadContent(content);
            monsterTwo.LoadContent(content);

            //After the world stage has been initialized (directly above this)
            //and before the first set of factors is determined, you'll want to
            //load your first set of values into the array

            stageProblems = Problems.determineProblems(worldStage);

            //Load up the first set of factors into the monster objects
            //Note: when worldStage = -1 is the hook for endless mode.
            //It can be ignored if there is not going to be an endless mode.

            if (worldStage == -1)
            {
                monsterOne.setFactors(stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"]);
                monsterTwo.setFactors(stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);
            }
            else
            {
                monsterOne.setFactors(stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"]);
                monsterTwo.setFactors(stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);
            }

            world101Text.LoadContent(content);

            //Play music in repeating loop
            Song backgroundMusic;
            backgroundMusic = content.Load<Song>("Level1/level1_background_music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
        }

        public bool Update(GameTime gameTime)
        {
            if (correctInARow >= COUNT_TO_CONTINUE && worldStage != -1)
            {
                //This is where we go when we finish a stage and
                //want to wrap up the level (notice the sole return condition!)
                //You might want to throw your logic for determining what the next
                //Set of questions is in here somewhere.
                //Also, if you need to make an additional DB reads or writes, this
                //is a great place to do it, as this block of code is only executed once
                //instead of the usual 60 times per second.
                monsterOne.monsterDeath();
                monsterTwo.monsterDeath();
                monsterOne.Update(gameTime);
                monsterTwo.Update(gameTime);

                hero.stageSuccess();
                hero.Update(gameTime);

                updateExtraObjects(gameTime);

                //stopAll();
                KeyboardState keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    if (worldStage >= MAX_STAGE)
                    {
                        Game1.globalUser.world101 = MAX_STAGE;
                        context.Save<User>(Game1.globalUser);
                        return true;
                    }
                    if (worldStage > Game1.globalUser.world101)
                    {
                        Game1.globalUser.world101 = worldStage;
                        context.Save<User>(Game1.globalUser);
                    }
                    worldStage++;
                    resetStage();
                }
                if (keyboard.IsKeyDown(Keys.Escape))
                {
                    Game1.globalUser.world101 = worldStage;
                    context.Save<User>(Game1.globalUser);

                    return true;
                }
            }
            else
            {
                updateExtraObjects(gameTime);
                cycleBackground(gameTime);
                hero.Update(gameTime);

                monsterOne.Update(gameTime);
                monsterTwo.Update(gameTime);

                answerDone = world101Input.Update(gameTime, heroDead);

                if (answerDone == true)
                {
                    checkAnswer();
                }

                if (currentMonster == 1)
                {
                    if (hero.getShieldCollisionBox().Intersects(monsterOne.getCollisionBox()))
                    {
                        hero.questionUp();
                        hero.deactivateShield();
                        currentMonster = 2;
                        monsterOne.monsterDeath();
                        //monsterOne.setX((int)(2600*scale));

                        //Any time we need to set the factors for the monster when it dies,
                        //You'll need to make a call similar to the one in the load content method
                        //I'm not 100% if you'll need correctInARow or correctInARow+1, play around with it

                        if (worldStage == -1)
                        {
                            //still for endless mode
                            monsterOne.setFactors(stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);
                        }
                        else
                        {
                            monsterOne.setFactors(stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);
                        }
                        showQuestion = true;
                    }

                    if (hero.getHeroCollisionBox().Intersects(monsterOne.getCollisionBox()))
                    {
                        stopAll();

                        KeyboardState keyboard = Keyboard.GetState();
                        if (keyboard.IsKeyDown(Keys.Space))
                        {
                            resetStage();
                        }
                        if (keyboard.IsKeyDown(Keys.Escape))
                        {
                            return true;
                        }
                    }

                    //You'll also need to make some changes here to the text class to properly display
                    //the questions operator (right now it always assumes its the + operator)
                    world101Text.Update(monsterOne.getFactorOne(), monsterOne.getFactorTwo(), world101Input.getCurrentInput(), correctInARow, worldStage);
                }

                if (currentMonster == 2)
                {
                    if (hero.getShieldCollisionBox().Intersects(monsterTwo.getCollisionBox()))
                    {
                        hero.questionUp();
                        hero.deactivateShield();
                        currentMonster = 1;
                        monsterTwo.monsterDeath();

                        //Don't forget to check both monsterOne and monsterTwo or you
                        //might end up with some funky bugs!

                        if (worldStage == -1)
                        {
                            monsterTwo.setFactors(stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"]);
                        }
                        else
                        {
                            monsterTwo.setFactors(stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);
                        }
                        showQuestion = true;
                    }

                    if (hero.getHeroCollisionBox().Intersects(monsterTwo.getCollisionBox()))
                    {
                        stopAll();

                        KeyboardState keyboard = Keyboard.GetState();
                        if (keyboard.IsKeyDown(Keys.Space))
                        {
                            resetStage();
                        }
                        if (keyboard.IsKeyDown(Keys.Escape))
                        {
                            return true;
                        }
                    }

                    //The other text update. Don't forget about me!
                    world101Text.Update(monsterTwo.getFactorOne(), monsterTwo.getFactorTwo(), world101Input.getCurrentInput(), correctInARow, worldStage);
                }

                if (badInput)
                {
                    KeyboardState keyboard = Keyboard.GetState();
                    if (keyboard.IsKeyDown(Keys.Space))
                    {
                        resetStage();
                    }
                    if (keyboard.IsKeyDown(Keys.Escape))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            drawExtraObjects(spriteBatch);
            monsterOne.Draw(spriteBatch);
            monsterTwo.Draw(spriteBatch);
            hero.Draw(spriteBatch);
            world101Text.DrawAnswerCount(spriteBatch);
            if (showQuestion)
            {
                world101Text.Draw(spriteBatch);
            }
            if (correctInARow >= COUNT_TO_CONTINUE)
            {
                world101Text.DrawCongratsMsg(spriteBatch);
            }
            if (heroDead)
            {
                world101Text.DrawDeadMsg(spriteBatch);
            }
        }

        private void checkClick()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);
        }

        private void stopAll()
        {
            monsterOne.setSpeed(0);
            monsterTwo.setSpeed(0);
            backgroundSpeed = 0;
            planetSpeed = 0;
            heroDead = true;
            hero.die();
        }

        private void cycleBackground(GameTime gameTime)
        {
            backgroundOnePosition.X -= backgroundSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            backgroundTwoPosition.X -= backgroundSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            backgroundThreePosition.X -= backgroundSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (backgroundOnePosition.X < -backgroundOne.Width * scale)
            {
                backgroundOnePosition.X = backgroundThreePosition.X + backgroundThree.Width * scale;
            }
            if (backgroundTwoPosition.X < -backgroundTwo.Width * scale)
            {
                backgroundTwoPosition.X = backgroundOnePosition.X + backgroundOne.Width * scale;
            }
            if (backgroundThreePosition.X < -backgroundThree.Width * scale)
            {
                backgroundThreePosition.X = backgroundTwoPosition.X + backgroundTwo.Width * scale;
            }
        }

        private void loadExtraObjects(ContentManager content)
        {
            backgroundOne = content.Load<Texture2D>("Level1/background_level_1a");
            backgroundTwo = content.Load<Texture2D>("Level1/background_level_1b");
            backgroundThree = content.Load<Texture2D>("Level1/background_level_1c");
            backgroundOnePosition = new Vector2(0, 0);
            backgroundTwoPosition = new Vector2(backgroundOne.Width * scale, 0);
            backgroundThreePosition = new Vector2(backgroundOne.Width * scale + backgroundTwo.Width * scale, 0);

            //planetTwo = content.Load<Texture2D>("General/Planets/planet_2");
            //planetThree = content.Load<Texture2D>("General/Planets/planet_3");
            //planetFour = content.Load<Texture2D>("General/Planets/planet_4");
            //planetFive = content.Load<Texture2D>("General/Planets/planet_5");

            //shipOne = content.Load<Texture2D>("General/Ships/good_drone");
            //shipTwo = content.Load<Texture2D>("General/Ships/enemy_drones");
            //shipThree = content.Load<Texture2D>("General/Ships/good_fleet_1");
            //shipFour = content.Load<Texture2D>("General/Ships/enemy_fleet_1");

            ////any scalar value needs to take into consideration
            ////the scale factor to fix resolution issues
            //movingPlanetTwoPosition = new Vector2(-2800 * scale, 150 * scale);
            //movingPlanetThreePosition = new Vector2(-5250 * scale, 200 * scale);
            //movingPlanetFourPosition = new Vector2(-9000 * scale, 1200 * scale);
            //movingPlanetFivePosition = new Vector2(-12000 * scale, -800 * scale);

            //shipOnePosition = new Vector2(-2000 * scale, 150 * scale);
            //shipTwoPosition = new Vector2(6000 * scale, 200 * scale);
            //shipThreePosition = new Vector2(-15000 * scale, 500 * scale);
            //shipFourPosition = new Vector2(-23000 * scale, 300 * scale);
        }

        private void updateExtraObjects(GameTime gameTime)
        {
            //movingPlanetTwoPosition.X -= (float)(planetSpeed * 100 * Math.Sin((double)gameTime.ElapsedGameTime.TotalMilliseconds) * MathHelper.Pi / 2);
            //movingPlanetTwoPosition.Y -= planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //movingPlanetThreePosition.X -= (float)(planetSpeed * 100 * Math.Sin((double)gameTime.ElapsedGameTime.TotalMilliseconds) * MathHelper.Pi / 2);
            //movingPlanetThreePosition.Y -= planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //movingPlanetFourPosition.X -= (float)(planetSpeed * 100 * Math.Sin((double)gameTime.ElapsedGameTime.TotalMilliseconds) * MathHelper.Pi / 2);
            //movingPlanetFourPosition.Y -= planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //movingPlanetFivePosition.X -= (float)(planetSpeed * 100 * Math.Sin((double)gameTime.ElapsedGameTime.TotalMilliseconds) * MathHelper.Pi / 2);
            //movingPlanetFivePosition.Y += planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //shipOnePosition.X += 4 * shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //shipTwoPosition.X -= 5 * shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //shipThreePosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //shipFourPosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        private void drawExtraObjects(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundOne, backgroundOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backgroundTwo, backgroundTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backgroundThree, backgroundThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(planetTwo, movingPlanetTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(planetThree, movingPlanetThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(planetFour, movingPlanetFourPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(planetFive, movingPlanetFivePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(shipOne, shipOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(shipTwo, shipTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(shipThree, shipThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(shipFour, shipFourPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void resetStage()
        {
            monsterOne.setX((int)(1600 * scale));
            monsterTwo.setX((int)(2600 * scale));
            backgroundSpeed = backupBackgroundSpeed;
            monsterOne.setSpeed(backgroundSpeed);
            monsterTwo.setSpeed(backgroundSpeed);
            planetSpeed = backupPlanetSpeed;
            hero.live();
            hero.questionUp();
            stageProblems = Problems.determineProblems(worldStage);
            monsterOne.setFactors(stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"]);
            monsterTwo.setFactors(stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);
            currentMonster = 1;
            showQuestion = true;
            heroDead = false;
            world101Input.resetInput();
            correctInARow = 0;
            badInput = false;
            hero.deactivateShield();
        }

        private void checkAnswer()
        {
            if (world101Input.getLastInput().Equals("") == false)
            {
                if (currentMonster == 1)
                {
                    //This is where the hook for the expected answer takes place.
                    //You'll need to pull from your dictionary the expect answer here.
                    //I won't change this particular line, because I don't know what you'll
                    //call the keys in the dictionary pair (I just assumed factorOne factorTwo for the factors
                    //change as neccessary
                    if (monsterOne.getExpectedAnswer() == Int32.Parse(world101Input.getLastInput()))
                    {
                        correctAnswer();
                    }
                    else
                    {
                        incorrectAnswer();
                    }
                }
                if (currentMonster == 2)
                {

                    //The other place where we check answers, don't forget it!
                    if (monsterTwo.getExpectedAnswer() == Int32.Parse(world101Input.getLastInput()))
                    {
                        correctAnswer();
                    }
                    else
                    {
                        incorrectAnswer();
                    }
                }
            }
        }

        private void correctAnswer()
        {
            hero.shieldAnimate();
            hero.activateShield();
            showQuestion = false;
            answerDone = false;
            correctInARow++;
        }

        private void incorrectAnswer()
        {
            stopAll();
            badInput = true;
        }
    }
}
