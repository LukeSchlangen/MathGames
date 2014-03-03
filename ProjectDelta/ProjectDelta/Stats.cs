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

        private SpriteFont font;

        private float scale;
        private int screenHeight;
        private int screenWidth;

        private Texture2D background;
        private Texture2D backButton;

        private Vector2 backButtonPosition;
        private Vector2 statsPosition;

        private Rectangle backButtonCollisionBox;

        private string displayedStats = "";

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
            backButtonPosition = new Vector2(screenWidth / 8, screenHeight * 3 / 4);
            backButtonCollisionBox = new Rectangle(((int)(backButtonPosition.X)), (int)(backButtonPosition.Y), (int)(backButton.Width), (backButton.Height));

            font = content.Load<SpriteFont>("input_font");

            stageProblems = Problems.determineProblems(worldStage, COUNT_TO_CONTINUE);

            displayedStats =
                "Stats for " + Game1.globalUser.username + "\n\n" +
                "Time Played Today: " + Game1.globalUser.timePlayedToday / 60000 + " minutes\n" +
                "Answers Attempted Today: " + Game1.globalUser.answersAttemptedToday + "\n" +
                "Answers Correct Today: " + Game1.globalUser.answersCorrectToday + "\n" +
                //"Percent Correct Today: " + (float)((int)(1000 * (float)Game1.globalUser.answersCorrectToday / (float)Game1.globalUser.answersAttemptedToday)) / 10 + "%" + "\n" +
                "\n" +
                "Time Played: " + Game1.globalUser.timePlayed / 60000 + " minutes\n" +
                "Answers Attempted: " + Game1.globalUser.answersAttempted + "\n" +
                "Answers Correct: " + Game1.globalUser.answersCorrect + "\n" +
                //"Percent Correct: " + (float)((int)(1000 * (float)Game1.globalUser.answersCorrect / (float)Game1.globalUser.answersAttempted)) / 10 + "%" + "\n" +
                "\nCurrently Working On:\n";

            for (int i = 0; i < COUNT_TO_CONTINUE + 2; i++)
            {
                displayedStats += question.question(stageProblems[i]["operation"], stageProblems[i]["factorOne"], stageProblems[i]["factorTwo"]) + "\n";
            }

            statsPosition = new Vector2((screenWidth / 2 - (font.MeasureString(displayedStats).X) / 2 * scale), 100 * scale);
        }

        public bool Update(GameTime gameTime)
        {
            return checkBack();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backButton, backButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, displayedStats, statsPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private bool checkBack()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(backButtonCollisionBox))
            {
                return true;
            }

            return false;
        }
    }
}
