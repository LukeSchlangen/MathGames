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
        private Texture2D[] freindlyCreatures = new Texture2D[200];
        private Vector2 position;
        private float speed = 0f;
        private float startSpeed = 0f;
        private float scale;
        private int x;
        private int y;
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
            for (int i = 0; i < freindlyCreatures.Length; i++)
            {
                Texture2D FreindlyCreatureToLoad = content.Load<Texture2D>("Creatures/Creature1/Creature1_Stage1"); //can do some clever text manipulation here to quickly load the FreindlyCreatures
                freindlyCreatures[i] = FreindlyCreatureToLoad;
            }

                    }

        public void Update(GameTime gameTime, int heroPosition)
        {
            Debug.WriteLine(position);
            position.X = heroPosition - 200 * scale;
            position.Y = y * scale + 10 * (float)Math.Sin(position.X / 15) * scale;

        }

        public void Draw(SpriteBatch spriteBatch, int worldStage)
        {
            spriteBatch.Draw(freindlyCreatures[worldStage], position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void stop()
        {
            startSpeed = speed;
            speed = 0f;
        }

        public void reset(int heroPosition)
        {
            position = new Vector2(heroPosition - 200*scale, 800 * scale);
            speed = .1f;
            powerupUseCount = 0;
        }
        public void setMaxNumberOfPowerupUses(int maxNumberOfPowerUpUses)
        {
            this.maxNumberOfPowerUpUses = maxNumberOfPowerUpUses;
        }

        public void usePowerup()
        {
            powerupUseCount += 1;
            usingPowerup = true;
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
