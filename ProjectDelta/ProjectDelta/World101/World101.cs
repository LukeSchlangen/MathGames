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
        private enum State
        {
            None,
            InternetConnectionError,
            ResetTimer,
        }

        private State state;

        private static int COUNT_TO_CONTINUE = 10;
        private static int MAX_STAGE = 1000;
        DynamoDBContext context;
        private int correctInARow = 0;
        private int worldStage;
        private int errorCounter;

        private float scale;

        private float backgroundSpeed = .1f;
        private float backupBackgroundSpeed = .1f; //speed of background for game, also used to set monster speed
        private float timePerProblem = 10000f; //10000f is about 3.5 seconds per problem

        //Textures for level 1
        private Texture2D backgroundOne;
        private Texture2D backgroundTwo;
        private Texture2D backgroundThree;
        private Texture2D secondBackgroundOne;
        private Texture2D secondBackgroundTwo;
        private Texture2D secondBackgroundThree;
        private Texture2D thirdBackgroundOne;
        private Texture2D thirdBackgroundTwo;
        private Texture2D thirdBackgroundThree;
        private Texture2D internetConnectionError;

        //Vectors for level 1
        private Vector2 backgroundOnePosition;
        private Vector2 backgroundTwoPosition;
        private Vector2 backgroundThreePosition;
        private Vector2 internetConnectionErrorPosition;

        //Mouse states
        private MouseState current;
        private MouseState previous;

        private Hero hero;
        private World101Monster monsterOne;
        private World101Monster monsterTwo;
        private World101Monster currentMonster;
        private World101WildCreature wildCreature;
        private World101FreindlyCreature friendlyCreature;
        private World101Input world101Input;
        private World101Text world101Text = new World101Text();
        private Random random = new Random();

        private bool answerDone = false;
        private bool showQuestion = true;
        private bool heroDead = false;
        private bool collected = false;
        private int bgToDraw = 1;

        private string myAnswer;

        //This is an array of HashSets that should allow you store data
        //as you explained in email.
        private Dictionary<string, int>[] stageProblems;

        public World101(DynamoDBContext context)
        {
            this.context = context;
        }

        public void Initialize(float scale, int screenX)
        {
            state = State.None;
            this.scale = scale;
            hero = new Hero();
            monsterOne = new World101Monster(1600, 800, scale, backgroundSpeed, screenX);
            monsterTwo = new World101Monster(2600, 800, scale, backgroundSpeed, screenX);
            wildCreature = new World101WildCreature(2600, 800, scale, backgroundSpeed, screenX);
            friendlyCreature = new World101FreindlyCreature(hero.getHeroPosition(), 800, scale, backgroundSpeed, screenX);
            currentMonster = monsterOne;
            hero.Initialize(scale);
            world101Input = new World101Input(scale);
            world101Text.Initialize(scale);
            errorCounter = 1000;
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
            wildCreature.LoadContent(content);
            friendlyCreature.LoadContent(content);

            internetConnectionError = content.Load<Texture2D>("Login/internet_connection_error");
            internetConnectionErrorPosition = new Vector2((1920 / 2 * scale - internetConnectionError.Width * scale / 2), (1080 / 2 * scale - internetConnectionError.Height * scale / 2));

            //load your first set of values into the array

            stageProblems = Problems.determineProblems(worldStage, COUNT_TO_CONTINUE);

            //Load up the first set of factors into the monster objects
            //Note: when worldStage = -1 is the hook for endless mode.
            //It can be ignored if there is not going to be an endless mode.

            monsterOne.setFactors(stageProblems[correctInARow]["operation"], stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"]);
            monsterTwo.setFactors(stageProblems[correctInARow + 1]["operation"], stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);

            friendlyCreature.setMaxNumberOfPowerupUses(1);

            world101Text.LoadContent(content);

            //Play music in repeating loop
            Song backgroundMusic;
            backgroundMusic = content.Load<Song>("Level1/level1_background_music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
        }

        public bool Update(GameTime gameTime)
        {
            updateCharacters(gameTime);

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                saveStage();
                return true;
            }

            if (keyboard.IsKeyDown(Keys.Space))
            {
                saveStage();
                resetStage();
            }

            else if (correctInARow >= COUNT_TO_CONTINUE)
            {
                monsterOne.monsterDeath();
                monsterTwo.monsterDeath();
                hero.stageSuccess();
                resetTimer();
                if (hero.getHeroCollisionBox().Intersects(wildCreature.getCollisionBox()))
                {
                    hero.stop();
                    wildCreature.stop();
                }
            }
            else
            {
                cycleBackground(gameTime);

                if (friendlyCreature.remainingPowerUp() && keyboard.IsKeyDown(Keys.S))
                {
                    useCreaturePowerUp(scale);
                }

                //if both monsters are off screen, make sure they are in order
                if (monsterOne.getCollisionBox().X < currentMonster.getCollisionBox().X && !monsterOne.dead)
                {
                    monsterOne.setX(currentMonster.getCollisionBox().X + (int)(500 * scale));
                }
                if (monsterTwo.getCollisionBox().X < currentMonster.getCollisionBox().X && !monsterTwo.dead)
                {
                    monsterTwo.setX(currentMonster.getCollisionBox().X + (int)(500 * scale));
                }

                answerDone = world101Input.Update(gameTime, heroDead);

                if (answerDone == true)
                {
                    if (world101Input.getLastInput().Equals("") == false)
                    {
                        if (currentMonster.getExpectedAnswer() == Int32.Parse(world101Input.getLastInput()))
                        {
                            correctAnswer(); //if the answer is the same as the expected answer, it was the correct answer
                            currentMonster.setSpeed(backgroundSpeed * 2); //monster speeds up so player doesn't have to wait
                        }
                        else
                        {
                            stopAll();
                        }
                    }
                }

                if (hero.getShieldCollisionBox().Intersects(currentMonster.getCollisionBox()))
                {
                    beatMonster();
                }

                if (hero.getHeroCollisionBox().Intersects(currentMonster.getCollisionBox()))
                {
                    stopAll();
                }

                //You'll also need to make some changes here to the text class to properly display
                //the questions operator (right now it always assumes its the + operator)
            }

            if (state == State.InternetConnectionError)
            {
                if (errorCounter == 1000)
                {
                    errorCounter = -5000;
                }
                if (errorCounter < 0)
                {
                    errorCounter += gameTime.ElapsedGameTime.Milliseconds;
                }
                if (errorCounter >= 0)
                {
                    state = State.None;
                    errorCounter = 1000;
                    return true;
                }
            }

            if (state == State.ResetTimer)
            {
                if (errorCounter == 1000)
                {
                    errorCounter = -15000;
                }
                if (errorCounter < 0)
                {
                    errorCounter += gameTime.ElapsedGameTime.Milliseconds;
                }
                if (errorCounter >= 0)
                {
                    state = State.None;
                    errorCounter = 1000;
                    saveStage();
                    resetStage();
                }
            }
            if (hero.getDead())
            {
                myAnswer = currentMonster.getExpectedAnswer().ToString();
            }
            else
            {
                myAnswer = world101Input.getCurrentInput();
            }
            world101Text.Update(currentMonster.getOperationValue(), currentMonster.getFactorOne(), currentMonster.getFactorTwo(), myAnswer, correctInARow, worldStage, COUNT_TO_CONTINUE);

            return false;

        }

        public void Draw(SpriteBatch spriteBatch)
        {

            drawExtraObjects(spriteBatch);
            monsterOne.Draw(spriteBatch);
            monsterTwo.Draw(spriteBatch);
            hero.Draw(spriteBatch);
            friendlyCreature.Draw(spriteBatch, worldStage);
            world101Text.DrawAnswerCount(spriteBatch);
            if (showQuestion && correctInARow < COUNT_TO_CONTINUE)
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
            if (!collected)
            {
                wildCreature.Draw(spriteBatch, worldStage);
            }

            if (state == State.InternetConnectionError)
            {
                spriteBatch.Draw(internetConnectionError, internetConnectionErrorPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
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
            heroDead = true;
            hero.die();
            resetTimer();
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
            if (worldStage < 10)
            {
                bgToDraw = 1;
            }
            if (worldStage >= 10 && worldStage < 20)
            {
                bgToDraw = 2;
            }
            if (worldStage >= 20)
            {
                bgToDraw = 3;
            }

            backgroundOne = content.Load<Texture2D>("Level1/background_level_1a");
            backgroundTwo = content.Load<Texture2D>("Level1/background_level_1b");
            backgroundThree = content.Load<Texture2D>("Level1/background_level_1c");

            backgroundOnePosition = new Vector2(0, 0);
            backgroundTwoPosition = new Vector2(backgroundOne.Width * scale, 0);
            backgroundThreePosition = new Vector2(backgroundOne.Width * scale + backgroundTwo.Width * scale, 0);

            secondBackgroundOne = content.Load<Texture2D>("Level1/background_level_2a");
            secondBackgroundTwo = content.Load<Texture2D>("Level1/background_level_2b");
            secondBackgroundThree = content.Load<Texture2D>("Level1/background_level_2c");

            thirdBackgroundOne = content.Load<Texture2D>("Level1/background_level_3a");
            thirdBackgroundTwo = content.Load<Texture2D>("Level1/background_level_3b");
            thirdBackgroundThree = content.Load<Texture2D>("Level1/background_level_3c");
        }

        private void updateCharacters(GameTime gameTime)
        {
            hero.Update(gameTime);
            monsterOne.Update(gameTime);
            monsterTwo.Update(gameTime);
            friendlyCreature.Update(gameTime, hero.getHeroPosition());
            if (correctInARow >= COUNT_TO_CONTINUE)
            {
                wildCreature.Update(gameTime);
            }
        }

        private void drawExtraObjects(SpriteBatch spriteBatch)
        {
            if (bgToDraw == 1)
            {
                spriteBatch.Draw(backgroundOne, backgroundOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(backgroundTwo, backgroundTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(backgroundThree, backgroundThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (bgToDraw == 2)
            {
                spriteBatch.Draw(secondBackgroundOne, backgroundOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(secondBackgroundTwo, backgroundTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(secondBackgroundThree, backgroundThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (bgToDraw == 3)
            {
                spriteBatch.Draw(thirdBackgroundOne, backgroundOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(thirdBackgroundTwo, backgroundTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(thirdBackgroundThree, backgroundThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        public void resetStage()
        {
            if (correctInARow >= COUNT_TO_CONTINUE)
            {
                //This block of code is only executed once between successful level
                //completion instead of the usual 60 times per second.              
                worldStage++;
                if (worldStage < 10)
                {
                    bgToDraw = 1;
                }
                if (worldStage >= 10 && worldStage < 20)
                {
                    bgToDraw = 2;
                }
                if (worldStage >= 20)
                {
                    bgToDraw = 3;
                }

            }
            correctInARow = 0;
            stageProblems = Problems.determineProblems(worldStage, COUNT_TO_CONTINUE);
            backgroundSpeed = backupBackgroundSpeed;
            monsterOne.reset(stageProblems[correctInARow]["operation"], stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"], backgroundSpeed);
            monsterTwo.reset(stageProblems[correctInARow]["operation"], stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"], backgroundSpeed);
            friendlyCreature.reset(hero.getHeroPosition());
            wildCreature.reset();
            currentMonster = monsterOne;
            hero.live();
            hero.questionUp();
            hero.start();
            showQuestion = true;
            heroDead = false;
            collected = false;
            world101Input.resetInput();
            state = State.None;
        }

        private void correctAnswer()
        {
            hero.shieldAnimate();
            hero.activateShield();
            showQuestion = false;
            answerDone = false;
            correctInARow++;
        }

        private void saveStage()
        {
            stopAll();
            if (worldStage >= MAX_STAGE)
            {
                worldStage = MAX_STAGE;
            }

            if (worldStage > Game1.globalUser.world101)
            {
                try
                {
                    Game1.globalUser.world101 = worldStage;
                    context.Save<User>(Game1.globalUser);
                }
                catch
                {
                    state = State.InternetConnectionError;
                }
            }


        }
        private void resetTimer()
        {
            state = State.ResetTimer;
        }

        private void useCreaturePowerUp(float scale)
        {
            friendlyCreature.usePowerup();
            if (worldStage < 20)
            {
                shockWave(scale);
            }
            else
            {
                beatMonster();
            }

        }

        private void shockWave(float scale)
        {
            monsterOne.setX((int)(monsterOne.getCollisionBox().X + 700 * scale));
            monsterTwo.setX((int)(monsterTwo.getCollisionBox().X + 700 * scale));
            currentMonster.setX((int)(currentMonster.getCollisionBox().X + 700 * scale));
        }

        private void beatMonster()
        {

            hero.questionUp();

            currentMonster.monsterDeath();
            world101Input.resetInput();

            //Update factors when a monster dies
            currentMonster.setFactors(stageProblems[correctInARow + 1]["operation"], stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);

            showQuestion = true;

            if (currentMonster == monsterOne)
            {
                currentMonster = monsterTwo;
            }
            else
            {
                currentMonster = monsterOne;
            }

            currentMonster.setSpeed((currentMonster.getCollisionBox().X - hero.getHeroCollisionBox().X) / timePerProblem);

        }
    }
}