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
        private Vector2 position;
        private int spriteWidth;
        private int spriteHeight;
        float scale;
        private int imageInX;
        private int imageInY;
        private Rectangle spriteRectangle;
        private float timer = 0f;
        private float interval = 10f;
        private int xFrame = 0;
        private int yFrame = 0;

        public Animation(Texture2D spriteSheet, Vector2 position, int imageInX, int imageInY, float scale)
        {
            this.spriteSheet = spriteSheet;
            this.position = position;
            this.imageInX = imageInX;
            this.imageInY = imageInY;
            this.scale = scale;
            spriteWidth = spriteSheet.Width / imageInX;
            spriteHeight = spriteSheet.Height / imageInY;

            spriteRectangle = new Rectangle(xFrame * spriteWidth, yFrame, spriteWidth, spriteHeight);

            //position = new Vector2(300 * scale, 800 * scale);
        }

        public void stationaryScroll(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer >= interval)
            {
                timer = 0;
                xFrame++;
                if (xFrame > imageInX-1)
                {
                    yFrame++;
                    if (yFrame > imageInY-1)
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
            spriteBatch.Draw(spriteSheet, position, spriteRectangle, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public Vector2 getAnimationPosition()
        {
            return position;
        }
    }
}
