using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    class HomeText
    {
        SpriteFont font;
        Vector2 worldFontPosition;

        float scale;

        string world = "";



        public HomeText(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("large_input_font");
            worldFontPosition = new Vector2(375 * scale, 660 * scale);
                    }

        public void Update(String whereTo)
        {
            world = "Travel to world: " + whereTo;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, world, worldFontPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        }

    }
}
