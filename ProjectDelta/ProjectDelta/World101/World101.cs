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
        private int countToContinue;
        private int correctInARow = 0;
        private int worldStage;
        private int lifetimeAnswersCorrect;
        private int resetCounter;
        private int tabletCreatureNumber;

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
        private Texture2D keyboardImage;
        private Texture2D nextLevelKeyboardImage;
        private Texture2D statusBar;
        private Texture2D creatureTablet;
        private Texture2D startButton;
        private Texture2D internetConnectionWarning;

        //Vectors for level 1
        private Vector2 backgroundOnePosition;
        private Vector2 backgroundTwoPosition;
        private Vector2 backgroundThreePosition;
        private Vector2 keyboardImagePosition;
        private Vector2 statusBarPosition;
        private Vector2 creatureTabletPosition;
        private Vector2 startButtonPosition;
        private Vector2 tabletCreaturePosition;
        private Vector2 internetConnectionWarningPosition;

        //Collision Boxes
        private Rectangle startButtonCollisionBox;

        //Mouse states
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        private Hero hero;
        private World101Monster monsterOne;
        private World101Monster monsterTwo;
        private World101Creature[] creatures = new World101Creature[163];
        private World101Monster currentMonster;
        //private World101Creature wildCreature;
        //private CreatureOrganizer creatureOrganizer = new CreatureOrganizer();
        //private World101FriendlyCreature friendlyCreature;
        private World101Input world101Input;
        private World101Text world101Text = new World101Text();
        private Random random = new Random();

        //sound effects for World 101
        private SoundEffect soundEffectShieldUp;
        private SoundEffect soundEffectThud;
        private SoundEffect soundEffectZap;

        private bool answerDone = false;
        private bool showQuestion = true;
        private bool heroDead = false;
        private bool internetConnection = true;
        private bool spaceDown = false;

        private int bgToDraw = 1;

        private string myAnswer;

        private int sessionTimePlayed;
        private int sessionAnswersAttempted;
        private int sessionAnswersCorrect;
        private int currentFriendlyCreature;

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
            //wildCreature = new World101WildCreature(2600, 800, scale, backgroundSpeed, screenX);
            currentMonster = monsterOne;
            hero.Initialize(scale);
            world101Input = new World101Input(scale);
            world101Text.Initialize(scale);
            resetCounter = 1000;
        }

        //Specifies which content is loaded for level 1

        public void LoadContent(ContentManager content, int worldStage, int COUNT_TO_CONTINUE)
        {
            this.worldStage = worldStage;
            for (int i = 0; i < creatures.Length; i++)
            {
                creatures[i] = new World101Creature(i, worldStage, 0, scale, backgroundSpeed);
            }
            this.countToContinue = COUNT_TO_CONTINUE;
            loadExtraObjects(content);
            world101Input.LoadContent(content);
            hero.LoadContent(content);
            monsterOne.LoadContent(content);
            monsterTwo.LoadContent(content);

            for (int i = 0; i < creatures.Length; i++)
            {
                creatures[i].LoadContent(content);
            }

            //wildCreature = creatures[worldStage];
            //wildCreature.setPosition(new Vector2(2600 * scale, 800 * scale));
            //wildCreature.LoadContent(content);

            currentFriendlyCreature = Game1.globalUser.currentFriendlyCreature;
            lifetimeAnswersCorrect = Game1.globalUser.answersCorrect;

            soundEffectShieldUp = content.Load<SoundEffect>("Level1/shield_up");
            soundEffectThud = content.Load<SoundEffect>("Level1/thud");
            soundEffectZap = content.Load<SoundEffect>("Level1/zap");

            statusBar = content.Load<Texture2D>("Level1/status_bar");
            statusBarPosition = new Vector2(700 * scale, 50 * scale);

            keyboardImage = content.Load<Texture2D>("Level1/keyboard_image");
            nextLevelKeyboardImage = content.Load<Texture2D>("Level1/next_level_keyboard");
            keyboardImagePosition = new Vector2((1920 * 2 / 3 * scale - keyboardImage.Width * scale / 2), (1080 * 2 / 5 * scale - keyboardImage.Height * scale / 2));

            creatureTablet = content.Load<Texture2D>("Level1/creature_tablet");
            creatureTabletPosition = new Vector2(10 * scale, 10 * scale);

            if (worldStage > 0)
            {
                tabletCreatureNumber = worldStage - 1;
            }
            else
            {
                tabletCreatureNumber = 0;
            }

            tabletCreaturePosition = new Vector2(20 * scale, 20 * scale);

            startButton = content.Load<Texture2D>("Level1/creature_tablet_start_button");
            startButtonPosition = new Vector2((creatureTabletPosition.X + creatureTablet.Width / 2 - startButton.Width / 2) * scale, (creatureTabletPosition.Y + creatureTablet.Height * 6 / 7 - startButton.Height / 2) * scale);
            startButtonCollisionBox = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, startButton.Width, startButton.Height);

            internetConnectionWarning = content.Load<Texture2D>("General/internet_connection_warning");
            internetConnectionWarningPosition = new Vector2(50 * scale, 50 * scale);


            //load your first set of values into the array

            stageProblems = Problems.determineProblems(worldStage, COUNT_TO_CONTINUE);

            //Load up the first set of factors into the monster objects
            //Note: when worldStage = -1 is the hook for endless mode.
            //It can be ignored if there is not going to be an endless mode.

            monsterOne.setFactors(stageProblems[correctInARow]["operation"], stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"]);
            monsterTwo.setFactors(stageProblems[correctInARow + 1]["operation"], stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);

            for (int i = 0; i < creatures.Length; i++)
            {
                creatures[i].setMaxNumberOfPowerupUses(1);
            }

            creatures[worldStage].setPosition(new Vector2((2200) * scale, hero.getHeroPosition().Y));

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
                return true;
            }

            //if the player hits space, save and restart the level
            spaceDown = keyboard.IsKeyDown(Keys.Space);
            if (spaceDown || checkClick())
            {
                saveStats();
                resetStage();
                spaceDown = false;
            }

            //if they have answered all of the questions and the level is over...
            if (correctInARow >= countToContinue)
            {
                //if the level is over, get rid of the monsters
                monsterOne.monsterDeath();
                monsterTwo.monsterDeath();
                hero.stageSuccess(); //hero goes accross screen to collect creature
                resetTimer(); //reset the error timer
                creatureEvolution(gameTime);
            }
            else
            {
                showMostEvolvedCreature();
                cycleBackground(gameTime); //advance the background to make it look like the hero is moving

                //if the creature has a powerup remaining, and the player presses "S", use the powerup
                if (creatures[currentFriendlyCreature].remainingPowerUp() && keyboard.IsKeyDown(Keys.S) && !heroDead)
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
                    if (world101Input.getInput().Equals("") == false && currentMonster.getExpectedAnswer() == Int32.Parse(world101Input.getInput()))
                    {
                        correctAnswer(); //if the answer is the same as the expected answer, it was the correct answer
                        currentMonster.setSpeed(backgroundSpeed * 2); //monster speeds up so player doesn't have to wait
                    }
                    else
                    {
                        sessionAnswersAttempted++;
                        soundEffectThud.Play();
                        stopAll(); //if the player has the wrong answer, stop everything
                    }
                }
            }



            //This determines if the level should restart on the student
            //it's to make sure the student doesn't just stop playing,
            //but nothing bad happens if they walk away
            if (state == State.ResetTimer)
            {
                if (resetCounter == 1000)
                {
                    resetCounter = -15000;
                }
                if (resetCounter < 0)
                {
                    resetCounter += gameTime.ElapsedGameTime.Milliseconds;
                }
                if (resetCounter >= 0)
                {
                    state = State.None;
                    resetCounter = 1000;
                    saveStats(); //save their game (in case they just made it to the end of a level
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

        private void showMostEvolvedCreature()
        {
            if (currentFriendlyCreature < 0 || currentFriendlyCreature > worldStage)
            {
                currentFriendlyCreature = worldStage - 1;
            }

            if (worldStage < 0 || currentFriendlyCreature < 0)
            { currentFriendlyCreature = 0; }
            else if (creatures[currentFriendlyCreature].getAvailability() || currentFriendlyCreature == worldStage - 1)
            {
                //do nothing, don't need to change creature if the creature is available
            }
            else
            {
                currentFriendlyCreature += 1;
            }
        }

        private void creatureEvolution(GameTime gameTime)
        {
            
            if (worldStage > 0 && creatures[worldStage - 1].getCreatureName() == creatures[worldStage].getCreatureName())
            {
                //show evolution in creature tablet
                if (((int)(resetCounter / -200))%2 == 0 && resetCounter<-13000)
                {
                    tabletCreatureNumber = worldStage - 1;
                }
                else
                {
                    tabletCreatureNumber = worldStage;
                }


                if (creatures[currentFriendlyCreature].getCreatureName() == creatures[tabletCreatureNumber].getCreatureName())
                {
                    
                    //currentFriendlyCreature = tabletCreatureNumber;
                }

            }
            else
            {

                //TODO
                tabletCreatureNumber = worldStage;//show new baby creature in creature tablet
                if (hero.getHeroPosition().X < 2600 * scale - hero.getHeroPosition().X)
                {
                    creatures[tabletCreatureNumber].Update(gameTime, new Vector2(2600 * scale - hero.getHeroPosition().X, hero.getHeroPosition().Y));
                }
                else
                {
                    creatures[tabletCreatureNumber].Update(gameTime, hero.getHeroPosition());
                }
                //collect creature
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //draw the backgrounds and characters
            drawExtraObjects(spriteBatch);
            monsterOne.Draw(spriteBatch);
            monsterTwo.Draw(spriteBatch);
            if (worldStage > 0 && correctInARow < countToContinue)
            {
                creatures[currentFriendlyCreature].Draw(spriteBatch);
            }
            hero.Draw(spriteBatch);
            spriteBatch.Draw(statusBar, statusBarPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            world101Text.DrawAnswerCount(spriteBatch);

            if (showQuestion && correctInARow < countToContinue)
            {
                world101Text.Draw(spriteBatch); //show the question to the student
            }
            if (correctInARow >= countToContinue)
            {
                if (creatures[currentFriendlyCreature].getCreatureName() == creatures[worldStage].getCreatureName())
                {
                    creatures[tabletCreatureNumber].Draw(spriteBatch);
                }
                else
                {
                    creatures[currentFriendlyCreature].Draw(spriteBatch);
                    creatures[tabletCreatureNumber].Draw(spriteBatch);
                }

                spriteBatch.Draw(nextLevelKeyboardImage, keyboardImagePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(creatureTablet, creatureTabletPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(startButton, startButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(creatures[tabletCreatureNumber].getCreatureImage(), tabletCreaturePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                //spriteBatch.Draw(creatures[worldStage].getCreatureImage(), creatures[worldStage].getPosition(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (heroDead)
            {
                spriteBatch.Draw(keyboardImage, keyboardImagePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            //wildCreature.Draw(spriteBatch); //show the wild creature if it hasn't been collected (not used right now, but might be later)
            

            if (internetConnection == false)
            {
                spriteBatch.Draw(internetConnectionWarning, internetConnectionWarningPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }


        }

        private bool checkClick()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (state == State.ResetTimer)
            {
                if (rectangleClick(startButtonCollisionBox))
                {
                    return true;
                }
            }
            return false;
        }

        private bool rectangleClick(Rectangle rectangle) //see if that button (collisionBox rectangle) is being clicked
        {
            Rectangle mousePosition = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released && mousePosition.Intersects(rectangle))
            {
                return true;
            }
            return false;
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
            showMostEvolvedCreature();
            if (creatures[currentFriendlyCreature].getCreatureName() == creatures[worldStage].getCreatureName() && correctInARow >= countToContinue)
            {
                creatures[tabletCreatureNumber].Update(gameTime, hero.getHeroPosition());
            }
            else
            {
                creatures[currentFriendlyCreature].Update(gameTime, hero.getHeroPosition());
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
            correctInARow = 0;
            stageProblems = Problems.determineProblems(worldStage, countToContinue);
            backgroundSpeed = backupBackgroundSpeed;
            monsterOne.reset(stageProblems[correctInARow]["operation"], stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"], backgroundSpeed);
            monsterTwo.reset(stageProblems[correctInARow]["operation"], stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"], backgroundSpeed);
            for (int i = 0; i < creatures.Length; i++)
            {
                creatures[i].reset(worldStage);
            }
            creatures[worldStage].setPosition(new Vector2((2200) * scale, hero.getHeroPosition().Y));
            currentMonster = monsterOne;
            hero.live();
            hero.questionUp();
            hero.start();
            showQuestion = true;
            heroDead = false;
            world101Input.resetInput();
            state = State.None;
            resetCounter = 1000;
        }

        private void correctAnswer()
        {
            soundEffectShieldUp.Play();
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
            stopAll();
            if (sessionAnswersAttempted > 0)
            {
                if (correctInARow >= countToContinue)
                {
                    //This block of code is only executed once between successful level
                    //completion instead of the usual 60 times per second.              
                    worldStage++;
                }

                if (worldStage >= MAX_STAGE)
                {
                    worldStage = MAX_STAGE;
                }

                try
                {
                    new DailyStats(context).resetDailyStats();
                    Game1.globalUser.answersAttempted += sessionAnswersAttempted;
                    Game1.globalUser.answersAttemptedToday += sessionAnswersAttempted;
                    Game1.globalUser.answersCorrect += sessionAnswersCorrect;
                    lifetimeAnswersCorrect += sessionAnswersCorrect;
                    Game1.globalUser.answersCorrectToday += sessionAnswersCorrect;
                    Game1.globalUser.timePlayed += sessionTimePlayed;
                    Game1.globalUser.timePlayedToday += sessionTimePlayed;
                    Game1.globalUser.lastDatePlayed = DateTime.Today;
                    for (int i = 0; i < creatures.Length; i++)
                    {
                        creatures[i].reset(worldStage);
                    }
                    showMostEvolvedCreature();
                    if (currentFriendlyCreature > worldStage - 1) { currentFriendlyCreature = worldStage - 1; }
                    Game1.globalUser.currentFriendlyCreature = currentFriendlyCreature;
                    Game1.globalUser.world101 = worldStage;
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

        private void resetTimer()
        {

            state = State.ResetTimer;
        }

        private void useCreaturePowerUp(float scale)
        {
            creatures[currentFriendlyCreature].usePowerup();
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
            soundEffectZap.Play();

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