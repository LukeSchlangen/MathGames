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
    class Animation
    {
        private Texture2D spriteSheet;
        private int spriteWidth;
        private int spriteHeight;
        private Rectangle spriteRectangle;
        private float timer = 0f;
        private float interval = 70f;
        private int xFrame = 0;
        private int yFrame = 0;

        public Animation(Texture2D spriteSheet, int imageInX, int imageInY)
        {
            this.spriteSheet = spriteSheet;
            spriteWidth = spriteSheet.Width / imageInX;
            spriteHeight = spriteSheet.Height / imageInY;

            spriteRectangle = new Rectangle(xFrame * spriteWidth, yFrame, spriteWidth, spriteHeight);
        }

        public void stationaryScroll(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer >= interval)
            {
                timer = 0;
                xFrame++;
                if (xFrame > 2)
                {
                    yFrame++;
                    if (yFrame > 6)
                    {
                        yFrame = 0;
                    }

                    xFrame = 0;
                }
            }

            spriteRectangle.Y = yFrame * spriteHeight;
            spriteRectangle.X = xFrame * spriteWidth;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Rectangle(100, 100, spriteWidth, spriteHeight), spriteRectangle, Color.White);
        }
    }
}
