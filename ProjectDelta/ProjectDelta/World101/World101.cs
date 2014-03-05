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
            //InternetConnectionError,
            ResetTimer,
        }

        private State state;

        private static int MAX_STAGE = 1000;
        DynamoDBContext context;
        private int countToContinue = 10;
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
        private Texture2D internetConnectionWarning;

        //Vectors for level 1
        private Vector2 backgroundOnePosition;
        private Vector2 backgroundTwoPosition;
        private Vector2 backgroundThreePosition;
        private Vector2 internetConnectionWarningPosition;

        private Hero hero;
        private World101Monster monsterOne;
        private World101Monster monsterTwo;
        private World101Monster currentMonster;
        private World101WildCreature wildCreature;
        private World101FriendlyCreature friendlyCreature;
        private World101Input world101Input;
        private World101Text world101Text = new World101Text();
        private Random random = new Random();

        private bool answerDone = false;
        private bool showQuestion = true;
        private bool heroDead = false;
        private bool collected = false;
        private bool internetConnection = true;

        private int bgToDraw = 1;

        private string myAnswer;

        private int sessionTimePlayed;
        private int sessionAnswersAttempted;
        private int sessionAnswersCorrect;

        //This is an array of HashSets to store problems
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
            friendlyCreature = new World101FriendlyCreature(scale, backgroundSpeed, screenX);
            currentMonster = monsterOne;
            hero.Initialize(scale);
            world101Input = new World101Input(scale);
            world101Text.Initialize(scale);
            errorCounter = 1000;
        }

        //Specifies which content is loaded for level 1

        public void LoadContent(ContentManager content, int worldStage, int COUNT_TO_CONTINUE)
        {
            this.worldStage = worldStage;
            this.countToContinue = COUNT_TO_CONTINUE;
            loadExtraObjects(content);
            world101Input.LoadContent(content);
            hero.LoadContent(content);
            monsterOne.LoadContent(content);
            monsterTwo.LoadContent(content);
            wildCreature.LoadContent(content);
            friendlyCreature.LoadContent(content);

            //internetConnectionError = content.Load<Texture2D>("Login/internet_connection_error");
            //internetConnectionErrorPosition = new Vector2((1920 / 2 * scale - internetConnectionError.Width * scale / 2), (1080 / 2 * scale - internetConnectionError.Height * scale / 2));

            internetConnectionWarning = content.Load<Texture2D>("General/internet_connection_warning");
            internetConnectionWarningPosition = new Vector2(50 * scale, 50 * scale);


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

            sessionTimePlayed = 0;
            sessionAnswersAttempted = 0;
            sessionAnswersCorrect = 0;
        }

        public bool Update(GameTime gameTime)
        {
            if (sessionAnswersAttempted > 0)
            {
                sessionTimePlayed += gameTime.ElapsedGameTime.Milliseconds;
            }

            updateCharacters(gameTime); //update the positions of all of the characters

            KeyboardState keyboard = Keyboard.GetState(); //determine what button is being pressed

            //if the player hits esc, save, and return them to the main level
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                saveStats();
                saveStage();
                return true;
            }

            //if the player hits space, save and restart the level
            if (keyboard.IsKeyDown(Keys.Space))
            {
                saveStats();
                saveStage();
                resetStage();
            }

            //if they have answered all of the questions and the level is over...
            else if (correctInARow >= countToContinue)
            {
                //if the level is over, get rid of the monsters
                monsterOne.monsterDeath();
                monsterTwo.monsterDeath();
                hero.stageSuccess(); //hero goes accross screen to collect creature
                resetTimer(); //reset the error timer

                //when the hero gets to the creature, have him stop
                if (hero.getHeroCollisionBox().Intersects(wildCreature.getCollisionBox()))
                {
                    hero.stop();
                    wildCreature.stop();
                }
            }
            else
            {
                cycleBackground(gameTime); //advance the background to make it look like the hero is moving

                //if the creature has a powerup remaining, and the player presses "S", use the powerup
                if (friendlyCreature.remainingPowerUp() && keyboard.IsKeyDown(Keys.S))
                {
                    useCreaturePowerUp(scale);
                }

                //if both monsters are off screen, make sure they are in order (special powers can make this out of sync)
                if (monsterOne != currentMonster)
                {
                    if (monsterOne.getCollisionBox().X < currentMonster.getCollisionBox().X + 300 * scale && !monsterOne.dead)
                    {
                        monsterOne.setX(currentMonster.getCollisionBox().X + (int)(300 * scale));
                    }
                }
                else if (monsterTwo != currentMonster)
                {
                    if (monsterTwo.getCollisionBox().X < currentMonster.getCollisionBox().X + 300 * scale && !monsterTwo.dead)
                    {
                        monsterTwo.setX(currentMonster.getCollisionBox().X + (int)(300 * scale));
                    }
                }

                answerDone = world101Input.Update(gameTime, heroDead); //if the player has entered an answer

                if (answerDone == true) //if the player has entered an answer, do some stuff
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
                            sessionAnswersAttempted++;
                            stopAll(); //if the player has the wrong answer, stop everything
                        }
                    }
                }

                if (hero.getShieldCollisionBox().Intersects(currentMonster.getCollisionBox()))
                {
                    beatMonster(); //if the monster hits the shield, send the monster flying
                }

                if (hero.getHeroCollisionBox().Intersects(currentMonster.getCollisionBox()))
                {
                    stopAll(); //if the monster collides with the hero, stop everything
                }

                //You'll also need to make some changes here to the text class to properly display
                //the questions operator (right now it always assumes its the + operator)
            }

            //This determines if the level should restart on the student
            //it's to make sure the student doesn't just stop playing,
            //but nothing bad happens if they walk away
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
                    saveStage(); //save their game (in case they just made it to the end of a level
                    resetStage();
                }
            }

            //if the hero is dead, show the correct answer
            if (hero.getDead())
            {
                myAnswer = currentMonster.getExpectedAnswer().ToString();
            }
            else
            {
                myAnswer = world101Input.getCurrentInput(); //if the hero is alive, show the current input
            }

            //show the problem
            world101Text.Update(currentMonster.getOperationValue(), currentMonster.getFactorOne(), currentMonster.getFactorTwo(), myAnswer, correctInARow, worldStage, countToContinue);

            return false; //don't go back to home screen, keep playing the game

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //draw the backgrounds and characters
            drawExtraObjects(spriteBatch);
            monsterOne.Draw(spriteBatch);
            monsterTwo.Draw(spriteBatch);
            hero.Draw(spriteBatch);
            friendlyCreature.Draw(spriteBatch, worldStage);
            world101Text.DrawAnswerCount(spriteBatch);

            if (showQuestion && correctInARow < countToContinue)
            {
                world101Text.Draw(spriteBatch); //show the question to the student
            }
            if (correctInARow >= countToContinue)
            {
                world101Text.DrawCongratsMsg(spriteBatch); //draw the congratulations message to the student upon completion
            }
            if (heroDead)
            {
                world101Text.DrawDeadMsg(spriteBatch); //if the hero is dead, show them the message telling them how to restart
            }
            if (!collected)
            {
                wildCreature.Draw(spriteBatch, worldStage); //show the wild creature if it hasn't been collected (not used right now, but might be later)
            }

            if (internetConnection == false)
            {
                spriteBatch.Draw(internetConnectionWarning, internetConnectionWarningPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }


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

        //make the background move and cycle through the three images that make up the background
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
            if (correctInARow >= countToContinue)
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
            if (correctInARow >= countToContinue)
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
            stageProblems = Problems.determineProblems(worldStage, countToContinue);
            backgroundSpeed = backupBackgroundSpeed;
            monsterOne.reset(stageProblems[correctInARow]["operation"], stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"], backgroundSpeed);
            monsterTwo.reset(stageProblems[correctInARow]["operation"], stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"], backgroundSpeed);
            friendlyCreature.reset();
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
            sessionAnswersAttempted++;
            sessionAnswersCorrect++;
            //Update factors after an answer is correct (it's here instead of beatmonster because a powerup could be used for that)
            currentMonster.setFactors(stageProblems[correctInARow + 1]["operation"], stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);

        }

        private void saveStats()
        {
            if (sessionAnswersAttempted > 0)
            {
                try
                {
                    new DailyStats(context).resetDailyStats();
                    Game1.globalUser.answersAttempted += sessionAnswersAttempted;
                    Game1.globalUser.answersAttemptedToday += sessionAnswersAttempted;
                    Game1.globalUser.answersCorrect += sessionAnswersCorrect;
                    Game1.globalUser.answersCorrectToday += sessionAnswersCorrect;
                    Game1.globalUser.timePlayed += sessionTimePlayed;
                    Game1.globalUser.timePlayedToday += sessionTimePlayed;
                    Game1.globalUser.lastDatePlayed = DateTime.Today;
                    context.Save<User>(Game1.globalUser);
                    sessionAnswersAttempted = 0;
                    sessionAnswersCorrect = 0;
                    sessionTimePlayed = 0;
                    internetConnection = true;
                }
                catch
                {
                    internetConnection = false;
                }
            }
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
                    internetConnection = true;
                }
                catch
                {
                    //state = State.InternetConnectionError;
                    internetConnection = false;
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
            monsterOne.setX((int)(monsterOne.getCollisionBox().X + 600 * scale));
            monsterTwo.setX((int)(monsterTwo.getCollisionBox().X + 600 * scale));
            currentMonster.setX((int)(currentMonster.getCollisionBox().X + 600 * scale));
        }

        private void beatMonster()
        {

            hero.questionUp();

            currentMonster.monsterDeath();
            world101Input.resetInput();

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