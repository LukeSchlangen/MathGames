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
    class World101Creature
    {
        private Texture2D creatureImage;
        private Vector2 position;
        private float speed;
        private float scale;
        private float constantlyIncreasingNumber;
        private int screenX;
        private int powerupUseCount = 0;
        private int maxNumberOfPowerUpUses;
        private int creatureNumber;

        private bool usingPowerup = false;

        private Rectangle collisionBox;

        public World101Creature(int creatureNumber, float scale, float speed, int screenX)
        {
            this.scale = scale;
            this.speed = speed;
            this.screenX = screenX;
            this.creatureNumber = creatureNumber;
        }

        public void LoadContent(ContentManager content)
        {
            creatureImage = content.Load<Texture2D>("Creatures/wild_creature_" + creatureNumber);
            collisionBox = new Rectangle(((int)(position.X - creatureImage.Width / 2)), ((int)(position.Y - creatureImage.Height / 2)), (creatureImage.Width), (creatureImage.Height));
        }

        public void Update(GameTime gameTime, Vector2 heroPosition)
        {
            position.X = heroPosition.X - creatureImage.Width * scale + 130 * scale;
            position.Y = heroPosition.Y - creatureImage.Height * scale;
            //bouncing
            constantlyIncreasingNumber += speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            position.Y += 10 * (float)Math.Sin(constantlyIncreasingNumber / 6) * scale;

            ////code from wild creature
            //position.X -= 5 / 2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //collisionBox.Y = (int)position.Y;
            //collisionBox.X = (int)position.X;
        }

        public void Draw(SpriteBatch spriteBatch, int worldStage)
        {
                spriteBatch.Draw(creatureImage, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void stop()
        {
            speed = 0f;
        }

        public void reset()
        {
            speed = .1f;
            powerupUseCount = 0;
            usingPowerup = false;
        }

        public void setMaxNumberOfPowerupUses(int maxNumberOfPowerUpUses)
        {
            this.maxNumberOfPowerUpUses = maxNumberOfPowerUpUses;
        }

        public void usePowerup()
        {
            if (usingPowerup == false)
            {
                powerupUseCount += 1;
                usingPowerup = true;
            }
        }

        public bool remainingPowerUp()
        {
            if (powerupUseCount < maxNumberOfPowerUpUses)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void setPosition(Vector2 position)
        {
            this.position = position;
        }
        
        public Vector2 getPosition()
        {
            return position;
        }
    }
}
