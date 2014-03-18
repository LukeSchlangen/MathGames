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
    class World101Text
    {
        SpriteFont font;
        Vector2 questionFontPosition;
        Vector2 stageStringPosition;
        Vector2 correctAnswerCountPosition;
        Vector2 congratsPosition;
        Vector2 energyBubblesPosition;


        float scale;

        QuestionFormat questionObject = new QuestionFormat();
        string questionString = "";
        string stageString = "";
        string correctAnswerCount = "";
        string congrats = "";
        string dead = "";
        string energyBubbles = "";

        public void Initialize(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("large_input_font");
            questionFontPosition = new Vector2(285 * scale, 660 * scale);
            stageStringPosition = new Vector2(1000 * scale, 70 * scale);
            correctAnswerCountPosition = new Vector2(1525 * scale, 70 * scale);
            congratsPosition = new Vector2((1920 / 16) * scale, (1080 / 4) * scale);
            energyBubblesPosition = new Vector2(1525 * scale, 230 * scale);
        }

        public void Update(int operationValue, int factorOne, int factorTwo, string myAnswer, int answerCount, int stage, int countToContinue, int energyBubbleCount)
        {
            //question string is created here, but it is done through publicly available class
            //this is so it can be used in stats as well without the answer
            questionString = questionObject.question(operationValue, factorOne, factorTwo) + " = " + myAnswer;

            stageString = stage.ToString();
            correctAnswerCount = answerCount + "/" + countToContinue;
            congrats = "Congratulations on finishing stage " + stage + "! \nPress SPACE to continue forward.\nPress ESC to return home.";
            dead = "Aww... you died. \nPress SPACE to try again.\nPress ESC to return home.";
            energyBubbles = energyBubbleCount.ToString();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, questionString, questionFontPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void DrawAnswerCount(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, stageString, stageStringPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, correctAnswerCount, correctAnswerCountPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, energyBubbles, energyBubblesPosition, Color.White, 0f, Vector2.Zero, scale / 2, SpriteEffects.None, 0f);
        }

        public void DrawCongratsMsg(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, congrats, congratsPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void DrawDeadMsg(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, dead, congratsPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
