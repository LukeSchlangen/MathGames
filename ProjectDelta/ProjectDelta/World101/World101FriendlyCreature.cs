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
    class World101FriendlyCreature
    {
        private Texture2D friendlyCreature;
        private Vector2 position;
        private float speed = 0f;
        private float startSpeed = 0f;
        private float scale;
        private float constantlyIncreasingNumber;
        private int screenX;
        private int powerupUseCount = 0;
        private int maxNumberOfPowerUpUses;

        private bool usingPowerup = false;

        public World101FriendlyCreature(float scale, float speed, int screenX)
        {
            this.scale = scale;
            this.speed = speed;
            this.startSpeed = speed;
            this.screenX = screenX;
        }

        public void LoadContent(ContentManager content)
        {
            if (Game1.globalUser.world101 - 1 >= 0)
            {
                if (Game1.globalUser.currentFriendlyCreature == -1)
                {
                    friendlyCreature = content.Load<Texture2D>("Creatures/wild_creature_" + (Game1.globalUser.world101 - 1));
                }
                else
                {
                    friendlyCreature = content.Load<Texture2D>("Creatures/wild_creature_" + (Game1.globalUser.currentFriendlyCreature));
                }
            }
            else
            {
                friendlyCreature = content.Load<Texture2D>("Creatures/wild_creature_0");
            }
        }

        public void Update(GameTime gameTime, Vector2 heroPosition)
        {
                position.X = heroPosition.X - friendlyCreature.Width * scale + 130 * scale;
                position.Y = heroPosition.Y - friendlyCreature.Height * scale;

            //bouncing
                constantlyIncreasingNumber += speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                position.Y += 10* (float)Math.Sin(constantlyIncreasingNumber / 6) * scale;
        }

        public void Draw(SpriteBatch spriteBatch, int worldStage)
        {
            if (Game1.globalUser.world101 - 1 >= 0)
            {
                spriteBatch.Draw(friendlyCreature, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        public void stop()
        {
            startSpeed = speed;
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
    }
}
