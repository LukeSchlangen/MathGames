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
        private float interval = 0f;
        private int xFrame = 0;
        private int yFrame = 0;
        private bool done = false;

        public Animation(Texture2D spriteSheet, Vector2 position, int imageInX, int imageInY, float scale, float interval)
        {
            this.spriteSheet = spriteSheet;
            this.position = position;
            this.imageInX = imageInX;
            this.imageInY = imageInY;
            this.scale = scale;
            this.interval = interval;
            spriteWidth = spriteSheet.Width / imageInX;
            spriteHeight = spriteSheet.Height / imageInY;

            spriteRectangle = new Rectangle(xFrame * spriteWidth, yFrame, spriteWidth, spriteHeight);

        }

        public void animateLoop(GameTime gameTime)
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

        public bool animateOnce(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (done == true)
            {
                return true;
            }
            
            if (timer >= interval)
            {
                timer = 0;
                xFrame++;
                if (xFrame > imageInX - 1)
                {
                    yFrame++;
                    if (yFrame > imageInY - 1)
                    {
                        done = true;
                    }

                    xFrame = 0;
                }
            }

            spriteRectangle.Y = yFrame * spriteHeight;
            spriteRectangle.X = xFrame * spriteWidth;

            return false;
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
