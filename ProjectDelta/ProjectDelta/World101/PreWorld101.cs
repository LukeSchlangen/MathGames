using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class PreWorld101
    {
        private DynamoDBContext context;
        private float scale;
        private int screenHeight;
        private int screenWidth;

        private Animation hero;

        private SpriteFont font;

        private Texture2D backgroundOne;
        private Texture2D backgroundTwo;
        private Texture2D backgroundThree;
        private Texture2D heroSprite;
        private Texture2D ship;
        private Texture2D questionBox;

        private Vector2 heroPosition;
        private Vector2 shipPosition;
        private Vector2 backgroundOnePosition;
        private Vector2 backgroundTwoPosition;
        private Vector2 backgroundThreePosition;
        private Vector2 questionBoxPosition;
        private Vector2 questionStringPosition;

        private Rectangle heroCollisionBox;
        private Rectangle shipCollisionBox;

        private World101Input input;
        private bool answerDone;
        private World101Monster questionMonster;
        private String questionString;

        private float backgroundSpeed = .1f;

        private int currentlyTesting;
        Dictionary<string, int>[] problem;

        public PreWorld101(DynamoDBContext context, float scale)
        {
            this.context = context;
            this.scale = scale;
            input = new World101Input(scale);           
        }

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;

            font = content.Load<SpriteFont>("large_input_font");
            questionStringPosition = new Vector2(screenWidth / 2 - (200 * scale), 75 * scale);

            heroSprite = content.Load<Texture2D>("General/Hero/running_sprite_sheet_5x5");
            backgroundOne = content.Load<Texture2D>("Story/story_background");
            backgroundTwo = content.Load<Texture2D>("Story/story_background");
            backgroundThree = content.Load<Texture2D>("Story/story_background");
            ship = content.Load<Texture2D>("Story/hero_ship_engines_off");
            questionBox = content.Load<Texture2D>("Home/question_box_image");

            backgroundOnePosition = new Vector2(0, 0);
            backgroundTwoPosition = new Vector2(backgroundOne.Width * scale, 0);
            backgroundThreePosition = new Vector2(backgroundOne.Width * scale + backgroundTwo.Width * scale, 0);
            heroPosition = new Vector2(275 * scale, 850 * scale);
            shipPosition = new Vector2(8300 * scale, 280 * scale);
            questionBoxPosition = new Vector2((screenWidth / 2 - questionBox.Width * scale / 2), (screenHeight / 4 - questionBox.Height * scale / 2));

            hero = new Animation(heroSprite, heroPosition, 5, 5, scale, 10f);
            heroCollisionBox = new Rectangle((int)((heroPosition.X) - 150 * scale), (int)(heroPosition.Y), (int)(hero.getWidth() * scale), hero.getHeight());
            shipCollisionBox = new Rectangle(((int)(shipPosition.X - ship.Width / 2)), ((int)(shipPosition.Y - ship.Height / 2)), (ship.Width), (ship.Height));

            input.LoadContent(content);
            questionMonster = new World101Monster(0, 0, scale, 0f, 0);
            currentlyTesting = 0;
            setUpProblem();
        }

        public bool Update(GameTime gameTime)
        {
            cycleBackground(gameTime);
            hero.animateLoop(gameTime, heroPosition);
            updateCollisionBoxes();

            answerDone = input.Update(gameTime, false);

            //If the answer is submitted and the string isn't the empty string...
            if (answerDone == true && input.getLastInput().Equals("") == false)
            {
                //If the answer is correct, progress to a harder question...
                if (questionMonster.getExpectedAnswer() == Int32.Parse(input.getLastInput()))
                {
                    currentlyTesting += 4;
                    setUpProblem();
                }
                //Otherwise, maybe they got lucky, scale it back to a slightly easier question
                else
                {
                    //Can't go lower than 0 for the level to test
                    if (currentlyTesting > 0)
                    {
                        currentlyTesting -= 2;
                    }

                    setUpProblem();
                }
            }

            //Once the hero arrives at the ship, the students time is up.
            //Figure out what level they are at, and then return to the home screen.
            if (shipCollisionBox.Intersects(heroCollisionBox))
            {
                Game1.globalUser.world101 = currentlyTesting;
                context.Save<User>(Game1.globalUser);
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            drawBackground(spriteBatch);
            hero.Draw(spriteBatch, heroPosition);
            spriteBatch.DrawString(font, questionString + " " + input.getCurrentInput(), questionStringPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void cycleBackground(GameTime gameTime)
        {
            shipPosition.X -= backgroundSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
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

        private void drawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundOne, backgroundOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backgroundTwo, backgroundTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backgroundThree, backgroundThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(questionBox, questionBoxPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(ship, shipPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void updateCollisionBoxes()
        {
            heroCollisionBox.Y = (int)heroPosition.Y;
            heroCollisionBox.X = (int)heroPosition.X;
            shipCollisionBox.Y = (int)shipPosition.Y;
            shipCollisionBox.X = (int)shipPosition.X;
        }

        private void setUpProblem()
        {
            problem = Problems.determineProblems(currentlyTesting, 1);
            questionMonster.setFactors(problem[0]["operation"], problem[0]["factorOne"], problem[0]["factorTwo"]);
            questionString = new QuestionFormat().question(questionMonster.getOperationValue(), questionMonster.getFactorOne(), questionMonster.getFactorTwo()) + " = ";
        }
    }
}
