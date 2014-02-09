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
    class World201Text
    {
        SpriteFont font;
        Vector2 questionFontPosition;
        Vector2 correctAnswerCountPosition;
        Vector2 congratsPosition;
        
        
        float scale;

        string question = "";
        string correctAnswerCount = "";
        string congrats = "";
        string dead = "";

        public void Initialize(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("large_input_font");
            questionFontPosition = new Vector2(345*scale, 660*scale);
            correctAnswerCountPosition = new Vector2(1250 * scale, 50 * scale);
            congratsPosition = new Vector2((1920/16) * scale, (1080/4) * scale);
        }

        public void Update(int factorOne, int factorTwo, string myAnswer, int answerCount, int stage)
        {
            if (stage == -1)
            {
                question = factorOne + " + " + factorTwo + " = " + myAnswer;
                correctAnswerCount = "Endless score: " + answerCount;
                dead = "You vanquished " + answerCount + " monsters before death! \nPress SPACE to try again.\nPress ESC to return home.";
            }
            else
            {
                question = factorOne + " + " + factorTwo + " = " + myAnswer;
                correctAnswerCount = "Stage " + stage + ": " + answerCount + "/10";
                congrats = "Congratulations on finishing stage " + stage + "! \nPress SPACE to continue forward.\nPress ESC to return home.";
                dead = "Aww... you died. \nPress SPACE to try again.\nPress ESC to return home.";
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, question, questionFontPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void DrawAnswerCount(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, correctAnswerCount, correctAnswerCountPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
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
