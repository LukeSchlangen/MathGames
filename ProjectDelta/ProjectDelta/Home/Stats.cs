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
    class Stats
    {
        private QuestionFormat question = new QuestionFormat();

        private DynamoDBContext context;
        private MouseState current;
        private MouseState previous;

        private SpriteFont textFont;
        private SpriteFont stageFont;

        private float scale;
        private int screenHeight;
        private int screenWidth;

        private Texture2D background;
        private Texture2D statsBackground;
        private Texture2D backButton;

        private Vector2 backButtonPosition;
        private Vector2 statsBackgroundPosition;
        private Vector2 todayStatsPosition;
        private Vector2 lifetimeStatsPosition;
        private Vector2 firstHalfPosition;
        private Vector2 secondHalfPosition;
        private Vector2 worldStagePosition;

        private Rectangle backButtonCollisionBox;

        private string todayStats = "";
        private string lifetimeStats = "";
        private string stage = "";
        private string currentlyPracticingFirstHalf = "";
        private string currentlyPracticingSecondHalf = "";


        private Dictionary<string, int>[] stageProblems;

        public Stats(DynamoDBContext context, float scale)
        {
            this.context = context;
            this.scale = scale;
        }

        public void LoadContent(ContentManager content, int worldStage, int COUNT_TO_CONTINUE, int screenX, int screenY)
        {
            new DailyStats(context).resetDailyStats();
            screenHeight = screenY;
            screenWidth = screenX;

            background = content.Load<Texture2D>("Login/login_background");
            backButton = content.Load<Texture2D>("Login/back_button");
            statsBackground = content.Load<Texture2D>("Home/stats_background");
            backButtonPosition = new Vector2(screenWidth / 7, screenHeight * 5 / 6);
            backButtonCollisionBox = new Rectangle(((int)(backButtonPosition.X)), (int)(backButtonPosition.Y), (int)(backButton.Width), (backButton.Height));

            textFont = content.Load<SpriteFont>("input_font");
            stageFont = content.Load<SpriteFont>("huge_input_font");

            stageProblems = Problems.determineProblems(worldStage, COUNT_TO_CONTINUE);

            todayStats =
                "Time Played: " + Game1.globalUser.timePlayedToday / 60000 + " minutes\n" +
                "Answers Attempted: " + Game1.globalUser.answersAttemptedToday + "\n" +
                "Answers Correct: " + Game1.globalUser.answersCorrectToday;

            lifetimeStats =
                "Time Played: " + Game1.globalUser.timePlayed / 60000 + " minutes\n" +
                "Answers Attempted: " + Game1.globalUser.answersAttempted + "\n" +
                "Answers Correct: " + Game1.globalUser.answersCorrect;

            stage = Game1.globalUser.world101.ToString();


            for (int i = 0; i < COUNT_TO_CONTINUE + 2; i++)
            {
                if (i < (COUNT_TO_CONTINUE + 2) / 2)
                {
                    currentlyPracticingFirstHalf += question.question(stageProblems[i]["operation"], stageProblems[i]["factorOne"], stageProblems[i]["factorTwo"]) + "        ";
                }
                else
                {
                    currentlyPracticingSecondHalf += question.question(stageProblems[i]["operation"], stageProblems[i]["factorOne"], stageProblems[i]["factorTwo"]) + "        ";
                }
            }

            statsBackgroundPosition = new Vector2((screenWidth / 2 - statsBackground.Width * scale / 2), (screenHeight / 2 - statsBackground.Height * scale / 2));
            todayStatsPosition = new Vector2(850*scale, 300*scale);
            lifetimeStatsPosition = new Vector2(1350*scale, 300*scale);
            firstHalfPosition = new Vector2(900*scale, 800*scale);
            secondHalfPosition = new Vector2(900*scale, 900*scale);
            worldStagePosition = new Vector2(300*scale, 300*scale);
        }

        public bool Update(GameTime gameTime)
        {
            return checkBack();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(statsBackground, statsBackgroundPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(backButton, backButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(textFont, todayStats, todayStatsPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(textFont, lifetimeStats, lifetimeStatsPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(textFont, currentlyPracticingFirstHalf, firstHalfPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(textFont, currentlyPracticingSecondHalf, secondHalfPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(stageFont, stage, worldStagePosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private bool checkBack()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(backButtonCollisionBox))
            {
                currentlyPracticingFirstHalf = "";
                currentlyPracticingSecondHalf = "";
                return true;
            }

            return false;
        }
    }
}
