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
            ResetTimer,
        }

        private State state;

        private int MAX_STAGE;
        DynamoDBContext context;
        private int countToContinue;
        private int correctInARow = 0;
        private int worldStage;
        private int lifetimeAnswersCorrect;
        private int lifetimeMinutesPlayed;
        private int spikePositionCounter;
        private int dropCounter;
        private int resetCounter;
        private int tabletCreatureNumber;
        private int totalEnergyBubbles;
        private int energyBubblesForDisplay;

        private SpriteFont font;

        private float scale;
        private float backgroundSpeed = .15f;
        private float startingBackgroundSpeed = .15f;
        private float standardSpeed = .1f; //speed of background for game, also used to set monster speed
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
        private Texture2D shootBubblesKeyboardImage;
        private Texture2D creatureDefendImage;
        private Texture2D creatureStrongerImage;
        private Texture2D bubbleCounter;
        private Texture2D statusBar;
        private Texture2D creatureTablet;
        private Texture2D spikeOne;
        private Texture2D spikeTwo;
        private Texture2D spike;
        private Texture2D hole;
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
        private Vector2 creatureTextPosition;
        private Vector2 spikePosition;
        private Vector2 holePosition;
        private Vector2 internetConnectionWarningPosition;
        private Vector2 panValue;

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
        private World101Monster nonCurrentMonster;
        private World101EnergyBubbles energyBubbles;
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
        private bool bubbleShooting = false;

        private int bgToDraw = 1;

        private string myAnswer;
        private string creatureText;

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
            monsterOne = new World101Monster(1600, 800, scale, standardSpeed, screenX);
            monsterTwo = new World101Monster(2600, 800, scale, standardSpeed, screenX);
            currentMonster = monsterOne;
            nonCurrentMonster = monsterTwo;
            hero.Initialize(scale);
            energyBubbles = new World101EnergyBubbles(hero.getHeroPosition(), new Vector2(1500 * scale, 75 * scale), scale);
            world101Input = new World101Input(scale);
            world101Text.Initialize(scale);
            resetCounter = 1000;
        }

        //Specifies which content is loaded for level 1

        public void LoadContent(ContentManager content, int worldStage, int COUNT_TO_CONTINUE, int MAX_STAGE)
        {
            this.MAX_STAGE = MAX_STAGE;
            this.worldStage = worldStage;
            for (int i = 0; i < creatures.Length; i++)
            {
                creatures[i] = new World101Creature(i, worldStage, lifetimeAnswersCorrect, lifetimeMinutesPlayed, scale, standardSpeed);
            }
            this.countToContinue = COUNT_TO_CONTINUE;
            loadExtraObjects(content);
            world101Input.LoadContent(content);
            hero.LoadContent(content);
            monsterOne.LoadContent(content);
            monsterTwo.LoadContent(content);
            energyBubbles.LoadContent(content);

            for (int i = 0; i < creatures.Length; i++)
            {
                creatures[i].LoadContent(content);
            }

            currentFriendlyCreature = Game1.globalUser.currentFriendlyCreature;
            lifetimeAnswersCorrect = Game1.globalUser.answersCorrect;
            lifetimeMinutesPlayed = Game1.globalUser.timePlayed / 60000;
            totalEnergyBubbles = Game1.globalUser.energyBubbles;

            soundEffectShieldUp = content.Load<SoundEffect>("Level1/shield_up");
            soundEffectThud = content.Load<SoundEffect>("Level1/thud");
            soundEffectZap = content.Load<SoundEffect>("Level1/zap");

            bubbleCounter = content.Load<Texture2D>("Level1/bubble_counter");
            statusBar = content.Load<Texture2D>("Level1/status_bar");
            statusBarPosition = new Vector2(700 * scale, 50 * scale);

            keyboardImage = content.Load<Texture2D>("Level1/keyboard_image");
            nextLevelKeyboardImage = content.Load<Texture2D>("Level1/next_level_keyboard");
            shootBubblesKeyboardImage = content.Load<Texture2D>("Level1/shoot_bubbles_keyboard");
            keyboardImagePosition = new Vector2((1920 * 2 / 3 * scale - keyboardImage.Width * scale / 2), (1080 * 2 / 5 * scale - keyboardImage.Height * scale / 2));

            creatureTablet = content.Load<Texture2D>("Level1/creature_tablet");
            creatureTabletPosition = new Vector2(10 * scale, 10 * scale);

            font = content.Load<SpriteFont>("input_font");
            creatureText = "";
            creatureTextPosition = new Vector2(creatureTabletPosition.X + 405 * scale, creatureTabletPosition.Y + 90 * scale);

            spikeOne = content.Load<Texture2D>("Level1/spike");
            spikeTwo = content.Load<Texture2D>("Level1/lightning");

            updateSpikeImage();

            hole = content.Load<Texture2D>("Level1/hole");
            spikePosition = new Vector2(2000 * scale, 0);
            holePosition = new Vector2(hero.getHeroPosition().X + 320 * scale, hero.getHeroPosition().Y - 150 * scale);
            if (worldStage > 0)
            {
                tabletCreatureNumber = worldStage - 1;
            }
            else
            {
                tabletCreatureNumber = 0;
            }

            tabletCreaturePosition = new Vector2(40 * scale, 40 * scale);

            startButton = content.Load<Texture2D>("Level1/creature_tablet_start_button");
            startButtonPosition = new Vector2((creatureTabletPosition.X + creatureTablet.Width / 2 - startButton.Width / 2) * scale, (creatureTabletPosition.Y + creatureTablet.Height * 6 / 7 - startButton.Height / 2) * scale);
            startButtonCollisionBox = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, startButton.Width, startButton.Height);

            internetConnectionWarning = content.Load<Texture2D>("General/internet_connection_warning");
            creatureDefendImage = content.Load<Texture2D>("Level1/creatures_defend_text");
            creatureStrongerImage = content.Load<Texture2D>("Level1/creatures_stronger_text");
            internetConnectionWarningPosition = new Vector2(275 * scale, 250 * scale);

            panValue = new Vector2(-75 * scale, 0 * scale);

            //load your first set of values into the array
            stageProblems = Problems.determineProblems(worldStage, COUNT_TO_CONTINUE);

            //Load up the first set of factors into the monster objects
            //Note: when worldStage = -1 is the hook for endless mode.
            //It can be ignored if there is not going to be an endless mode.

            monsterOne.setFactors(stageProblems[correctInARow]["operation"], stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"]);
            monsterTwo.setFactors(stageProblems[correctInARow + 1]["operation"], stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"]);

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

            KeyboardState keyboard = Keyboard.GetState(); //determine what button is being pressed

            //if the player hits esc, save, and return them to the main level
            if (keyboard.IsKeyDown(Keys.Escape) || worldStage == MAX_STAGE)
            {
                saveStats();
                return true;
            }

            //if the player hits space, save and restart the level
            else if (keyboard.IsKeyDown(Keys.Space) || checkClick())
            {
                saveStats();
                resetStage();
            }

            //if they have answered all of the questions and the level is over...
            else if (correctInARow >= countToContinue)
            {
                //if the level is over, get rid of the monsters
                updateCharacters(gameTime); //update the positions of all of the characters
                monsterOne.monsterDeath();
                monsterTwo.monsterDeath();
                hero.stageSuccess(); //hero goes accross screen to collect creature
                resetTimer(); //reset the error timer
                creatureEvolution(gameTime);
            }
            else
            {
                if (sessionAnswersAttempted > 0)
                {
                    sessionTimePlayed += gameTime.ElapsedGameTime.Milliseconds;
                }

                if (!hero.getDead() && totalEnergyBubbles > 0 && (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Left)))
                {
                    bubbleShooting = true;
                    monsterOne.setX((int)(monsterOne.getCollisionBox().X + 30 * scale));
                    monsterTwo.setX((int)(monsterTwo.getCollisionBox().X + 30 * scale));
                    totalEnergyBubbles -= 1;
                }
                else
                {
                    bubbleShooting = false;
                    cycleBackground(gameTime); //advance the background to make it look like the hero is moving
                }

                hero.setHeroShooting(bubbleShooting);

                updateCharacters(gameTime); //update the positions of all of the characters
                showMostEvolvedCreature();

                if (spikePositionCounter > 0)
                {

                    spikePosition = new Vector2(nonCurrentMonster.getCollisionBox().X - spike.Width * scale / 2, nonCurrentMonster.getCollisionBox().Y + nonCurrentMonster.getHeight() / 2 - spike.Height * scale / 2);

                    spikePositionCounter++;
                    if (spikePositionCounter > 5)
                    {
                        spikePositionCounter = 0;
                        spikePosition = new Vector2(2000 * scale, 0);
                    }
                }
                else if (dropCounter > 0)
                {
                    dropCounter++;
                    if (dropCounter > 50 || heroDead)
                    {
                        dropCounter = 0;
                        beatMonster();
                    }
                    else
                    {
                        if (monsterOne == currentMonster)
                        {
                            monsterOne.setX((int)(monsterOne.getCollisionBox().X + 10 * scale));
                            currentMonster.setX((int)(currentMonster.getCollisionBox().X + 10 * scale));
                        }
                        else
                        {
                            monsterTwo.setX((int)(monsterTwo.getCollisionBox().X + 10 * scale));
                            currentMonster.setX((int)(currentMonster.getCollisionBox().X + 10 * scale));
                        }
                        if (monsterOne == currentMonster)
                        {
                            monsterOne.setY((int)(monsterOne.getCollisionBox().Y + 25 * dropCounter * scale));
                            currentMonster.setY((int)(currentMonster.getCollisionBox().Y + 25 * dropCounter * scale));
                        }
                        else
                        {
                            monsterTwo.setY((int)(monsterTwo.getCollisionBox().Y + 25 * dropCounter * scale));
                            currentMonster.setY((int)(currentMonster.getCollisionBox().Y + 25 * dropCounter * scale));
                        }
                        creatures[currentFriendlyCreature].setPosition(new Vector2(currentMonster.getCollisionBox().X, currentMonster.getCollisionBox().Y));
                    }
                }

                //make sure they are in order (special powers can make this out of sync)
                keepMonstersInOrder();

                answerDone = world101Input.Update(gameTime, heroDead); //if the player has entered an answer

                if (answerDone == true) //if the player has entered an answer, do some stuff
                {
                    if (world101Input.getLastInput().Equals("") == false)
                    {
                        if (currentMonster.getExpectedAnswer() == Int32.Parse(world101Input.getLastInput()))
                        {
                            correctAnswer(); //if the answer is the same as the expected answer, it was the correct answer
                            currentMonster.setSpeed(standardSpeed * 3); //monster speeds up so player doesn't have to wait
                            backgroundSpeed *= 4;
                            currentMonster.setX(currentMonster.getCollisionBox().X - 10);
                        }
                        else
                        {
                            sessionAnswersAttempted++;
                            stopAll(); //if the player has the wrong answer, stop everything
                        }
                    }
                }
                else //this is an else, because if they get the correct answer at the exact time the monster collides with them, it would act up
                {
                    if (hero.getShieldCollisionBox().Intersects(currentMonster.getCollisionBox()))
                    {
                        beatMonster(); //if the monster hits the shield, send the monster flying
                    }

                    if (hero.getHeroCollisionBox().Intersects(currentMonster.getCollisionBox()))
                    {
                        if (world101Input.getInput().Equals("") == false && currentMonster.getExpectedAnswer() == Int32.Parse(world101Input.getInput()))
                        {
                            correctAnswer(); //if the answer is the same as the expected answer, it was the correct answer
                            //currentMonster.setSpeed(backgroundSpeed * 2); //monster speeds up so player doesn't have to wait
                        }
                        else
                        {
                            //if the creature has a powerup remaining, use it now
                            if (creatures[currentFriendlyCreature].getPowerUpsRemaining() > 0)
                            {
                                useCreaturePowerUp(scale);
                            }
                            else
                            {
                                sessionAnswersAttempted++;
                                soundEffectThud.Play();
                                stopAll(); //if the player has the wrong answer, stop everything
                            }
                        }
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
            if (totalEnergyBubbles == 1000)
            {
                energyBubblesForDisplay = 1000;
            }
            else
            {
                energyBubblesForDisplay = Math.Max(totalEnergyBubbles - energyBubbles.energyBubblesInTransit(), 0);
            }

            world101Text.Update(currentMonster.getOperationValue(), currentMonster.getFactorOne(), currentMonster.getFactorTwo(), myAnswer, correctInARow, worldStage, countToContinue, energyBubblesForDisplay);

            return false; //don't go back to home screen, keep playing the game

        }

        private void keepMonstersInOrder()
        {
            if (currentMonster.getCollisionBox().X > 2000 * scale)
            {
                currentMonster.setX((int)(2000 * scale));
            }
            if (nonCurrentMonster.getCollisionBox().X > 2500 * scale)
            {
                nonCurrentMonster.setX((int)(2500 * scale));
            }
            if (nonCurrentMonster.getCollisionBox().X < currentMonster.getCollisionBox().X + 300 * scale && !nonCurrentMonster.dead)
            {
                nonCurrentMonster.setX(currentMonster.getCollisionBox().X + (int)(300 * scale));
            }


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
                if (((int)(resetCounter / -200)) % 2 == 0 && resetCounter < -13000)
                {
                    tabletCreatureNumber = worldStage - 1;
                }
                else
                {
                    tabletCreatureNumber = worldStage;
                }
            }
            else
            {

                tabletCreatureNumber = worldStage;//show new baby creature in creature tablet
                if (hero.getHeroPosition().X < 2600 * scale - hero.getHeroPosition().X)
                {
                    creatures[tabletCreatureNumber].Update(gameTime, new Vector2(2600 * scale - hero.getHeroPosition().X, hero.getHeroPosition().Y));
                }
                else
                {
                    creatures[tabletCreatureNumber].Update(gameTime, hero.getHeroPosition());
                }
            }

            tabletCreaturePosition = new Vector2(creatureTabletPosition.X + creatureTablet.Width * scale * 30 / 100 - creatures[tabletCreatureNumber].getWidth() / 2, creatureTabletPosition.Y + creatureTablet.Height * scale * 41 / 100 - creatures[tabletCreatureNumber].getHeight() / 2);

            creatureText = "Name: " + creatures[tabletCreatureNumber].getCreatureName() + "\n" +
        "Type: " + creatures[tabletCreatureNumber].getCreatureType() + "\n" +
        "Level: " + creatures[tabletCreatureNumber].getCreatureLevel() + "\n" +
        "Power Ups: " + creatures[tabletCreatureNumber].getPowerUpsRemaining();

        }

        private void updateSpikeImage()
        {
            if (creatures[currentFriendlyCreature].getCreatureType() == "Spiker")
            {
                spike = spikeOne;
            }
            else if (creatures[currentFriendlyCreature].getCreatureType() == "Zapper")
            {
                spike = spikeTwo;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            //draw the backgrounds and characters
            drawExtraObjects(spriteBatch);
            energyBubbles.Draw(spriteBatch, energyBubblesForDisplay);
            monsterOne.Draw(spriteBatch);
            monsterTwo.Draw(spriteBatch);
            if (correctInARow < countToContinue && bubbleShooting)
            {
                energyBubbles.DrawBubbleGun(spriteBatch);
            }

            if (worldStage > 0 && correctInARow < countToContinue)
            {
                creatures[currentFriendlyCreature].Draw(spriteBatch);
            }
            if (dropCounter > 0)
            {
                spriteBatch.Draw(hole, holePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            hero.Draw(spriteBatch);
            if (spikePositionCounter > 0)
            {
                spriteBatch.Draw(spike, spikePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(bubbleCounter, new Vector2(statusBarPosition.X + 745 * scale, statusBarPosition.Y + 85 * scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(statusBar, statusBarPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            world101Text.DrawAnswerCount(spriteBatch);

            if (!Keyboard.GetState().IsKeyDown(Keys.Space) && showQuestion && correctInARow < countToContinue)
            {

                world101Text.Draw(spriteBatch); //show the question to the student

            }




            if (correctInARow >= countToContinue && worldStage < MAX_STAGE)
            {
                if (creatures[currentFriendlyCreature].getCreatureName() == creatures[worldStage].getCreatureName())
                {
                    creatures[tabletCreatureNumber].Draw(spriteBatch);
                }
                else
                {
                    creatures[currentFriendlyCreature].Draw(spriteBatch);
                    if (creatures[worldStage - 1].getCreatureName() != creatures[worldStage].getCreatureName())
                    {
                        creatures[tabletCreatureNumber].Draw(spriteBatch);
                    }
                }



                spriteBatch.Draw(nextLevelKeyboardImage, keyboardImagePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(creatureTablet, creatureTabletPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(creatures[tabletCreatureNumber].getCreatureImage(), tabletCreaturePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(startButton, startButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, creatureText, creatureTextPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else if (worldStage == 1)
            {
                spriteBatch.Draw(creatureDefendImage, internetConnectionWarningPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else if (worldStage == 2)
            {
                spriteBatch.Draw(creatureStrongerImage, internetConnectionWarningPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else if (worldStage == 3)
            {
                spriteBatch.Draw(shootBubblesKeyboardImage, keyboardImagePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            if (heroDead)
            {
                spriteBatch.Draw(keyboardImage, keyboardImagePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

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
            energyBubbles.Update(gameTime, hero.getHeroPosition());
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

            if (worldStage < MAX_STAGE)
            {
                correctInARow = 0;
                stageProblems = Problems.determineProblems(worldStage, countToContinue);
                backgroundSpeed = startingBackgroundSpeed;
                monsterOne.reset(stageProblems[correctInARow]["operation"], stageProblems[correctInARow]["factorOne"], stageProblems[correctInARow]["factorTwo"], standardSpeed);
                monsterTwo.reset(stageProblems[correctInARow]["operation"], stageProblems[correctInARow + 1]["factorOne"], stageProblems[correctInARow + 1]["factorTwo"], standardSpeed);
                for (int i = 0; i < creatures.Length; i++)
                {
                    creatures[i].reset(worldStage, lifetimeAnswersCorrect, lifetimeMinutesPlayed);
                }

                creatures[worldStage].setPosition(new Vector2((2200) * scale, hero.getHeroPosition().Y));
                currentMonster = monsterOne;
                nonCurrentMonster = monsterTwo;
                hero.live();
                hero.questionUp();
                hero.start();
                energyBubbles.reset();
                showQuestion = true;
                heroDead = false;
                world101Input.resetInput();
                state = State.None;
                dropCounter = 0;
                resetCounter = 1000;
            }
        }

        private void correctAnswer()
        {
            soundEffectShieldUp.Play();
            hero.shieldAnimate();
            hero.activateShield();
            showQuestion = false;
            answerDone = false;
            correctInARow++;
            if (correctInARow >= countToContinue)
            {
                energyBubbles.newBubbles(150);
                totalEnergyBubbles += 150;
            }
            energyBubbles.newBubbles(10);
            totalEnergyBubbles += 10;
            if (totalEnergyBubbles > 1000)
            {
                totalEnergyBubbles = 1000;
            }
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
                    lifetimeMinutesPlayed += Game1.globalUser.timePlayed / 60000;
                    Game1.globalUser.timePlayedToday += sessionTimePlayed;
                    Game1.globalUser.lastDatePlayed = DateTime.Today;
                    for (int i = 0; i < creatures.Length; i++)
                    {
                        creatures[i].reset(worldStage, lifetimeAnswersCorrect, lifetimeMinutesPlayed); //makes the new creature available and the old creature not if it just evolved
                    }
                    showMostEvolvedCreature(); //updates to most evolved creature
                    if ((currentFriendlyCreature > worldStage - 1) || (currentFriendlyCreature == worldStage - 2)) { currentFriendlyCreature = worldStage - 1; } //makes sure weird things don't happen and that player gets most recent creature
                    Game1.globalUser.currentFriendlyCreature = currentFriendlyCreature;
                    Game1.globalUser.world101 = worldStage;
                    Game1.globalUser.energyBubbles = totalEnergyBubbles;
                    context.Save<User>(Game1.globalUser);
                    updateSpikeImage();
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
            string creatureType = creatures[currentFriendlyCreature].getCreatureType();
            if (creatureType == "Tackler" || creatureType == "Spinner")
            {
                tackle(scale);
            }
            else if (creatureType == "Spiker" || creatureType == "Zapper")
            {
                shootSpike();
                beatMonster();
            }
            else if (creatureType == "Digger")
            {
                monsterBack();
                monsterDrop();
            }
            else if (creatureType == "Stomper")
            {
                monsterBack();
                beatMonster();
            }
            else
            {
                beatMonster();
            }
        }

        private void tackle(float scale)
        {
            monsterOne.setX((int)(monsterOne.getCollisionBox().X + (400 + creatures[currentFriendlyCreature].getCreatureLevel() * 10) * scale));
            monsterTwo.setX((int)(monsterTwo.getCollisionBox().X + (400 + creatures[currentFriendlyCreature].getCreatureLevel() * 10) * scale));
            currentMonster.setX((int)(currentMonster.getCollisionBox().X + (400 + creatures[currentFriendlyCreature].getCreatureLevel() * 10) * scale));
            nonCurrentMonster.setX((int)(nonCurrentMonster.getCollisionBox().X + (400 + creatures[currentFriendlyCreature].getCreatureLevel() * 10) * scale));
        }

        private void shootSpike()
        {
            spikePositionCounter += 1;
        }

        private void monsterBack()
        {
            //send them back
            if (monsterOne == currentMonster)
            {
                monsterOne.setX((int)(monsterOne.getCollisionBox().X + 200 * scale));
                currentMonster.setX((int)(currentMonster.getCollisionBox().X + 200 * scale));
                monsterOne.setY((int)(monsterOne.getCollisionBox().Y - 100 * scale));
                currentMonster.setY((int)(currentMonster.getCollisionBox().Y - 100 * scale));
            }
            else
            {
                monsterTwo.setX((int)(monsterTwo.getCollisionBox().X + 200 * scale));
                currentMonster.setX((int)(currentMonster.getCollisionBox().X + 200 * scale));
                monsterTwo.setY((int)(monsterTwo.getCollisionBox().Y - 100 * scale));
                currentMonster.setY((int)(currentMonster.getCollisionBox().Y - 100 * scale));
            }
        }
        private void monsterDrop()
        {
            dropCounter += 1;
        }

        private void beatMonster()
        {
            soundEffectZap.Play();

            hero.questionUp();
            backgroundSpeed = startingBackgroundSpeed;
            currentMonster.monsterDeath();
            world101Input.resetInput();

            showQuestion = true;

            if (currentMonster == monsterOne)
            {
                currentMonster = monsterTwo;
                nonCurrentMonster = monsterOne;
            }
            else
            {
                currentMonster = monsterOne;
                nonCurrentMonster = monsterTwo;
            }

            currentMonster.setSpeed((currentMonster.getCollisionBox().X - hero.getHeroCollisionBox().X) / timePerProblem);

        }
    }
}