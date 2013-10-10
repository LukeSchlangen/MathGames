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
    class Text
    {
        SpriteFont font;
        Vector2 questionFontPosition;
        Vector2 correctAnswerCountPosition;
        Vector2 congratsPosition;
        
        float scale;

        string question = "";
        string correctAnswerCount = "";
        string congrats = "";

        public void Initialize(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("large_input_font");
            questionFontPosition = new Vector2(200*scale, 600*scale);
            correctAnswerCountPosition = new Vector2(1700 * scale, 50 * scale);
            congratsPosition = new Vector2((1920/16) * scale, (1080/2) * scale);
        }

        public void Update(int factorOne, int factorTwo, string myAnswer, int answerCount, int stage)
        {            
            question = factorOne + " + " + factorTwo + " = " + myAnswer;
            correctAnswerCount = answerCount + "";
            congrats = "Congratulations on finishing stage " + stage + "! \n Press SPACE to continue forward!";
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
    }
}
