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
    class World101FreindlyCreature
    {
        private Texture2D friendlyCreature;
        private Vector2 position;
        private float speed = 0f;
        private float startSpeed = 0f;
        private float scale;
        private int x;
        private int y;
        private int friendlyCreatureWidth;
        private int screenX;
        private int powerupUseCount = 0;
        private int maxNumberOfPowerUpUses;

        private bool usingPowerup = false;

        public World101FreindlyCreature(int x, int y, float scale, float speed, int screenX)
        {
            this.scale = scale;
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.startSpeed = speed;
            this.screenX = screenX;

            Debug.WriteLine(position);
            position.X = x - 200;
            position.Y = y * scale + 10 * (float)Math.Sin(position.X / 15) * scale;
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

        }

        public void Update(GameTime gameTime, int heroPosition, int creatureSelected)
        {
            Debug.WriteLine(position);
            position.X = heroPosition - 200 * scale;
            position.Y = y * scale + 10 * (float)Math.Sin(position.X / 15) * scale;
            friendlyCreatureWidth = friendlyCreature.Width;
        }

        public void Draw(SpriteBatch spriteBatch, int worldStage)
        {
            spriteBatch.Draw(friendlyCreature, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void stop()
        {
            startSpeed = speed;
            speed = 0f;
        }

        public void reset(int heroPosition)
        {
            position = new Vector2(heroPosition - 200 * scale, 800 * scale);
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
