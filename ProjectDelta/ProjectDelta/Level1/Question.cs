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
    class Question
    {
        SpriteFont font;
        Vector2 questionFontPosition;

        int factorOne = 5;
        int factorTwo = 10;
        
        float scale;

        string question = "asdf";

        public void Initialize(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("input_font");
            questionFontPosition = new Vector2(100*scale, 600*scale);
        }

        public void Update(string myAnswer)
        {            
            question = factorOne + " x " + factorTwo + " = " + myAnswer;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, question, questionFontPosition, Color.SandyBrown, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
