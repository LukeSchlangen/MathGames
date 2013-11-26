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
        private Texture2D creature;
        private Vector2 position;
        private Rectangle collisionBox;
        private float speed = 0f;
        private float scale;

        private int y;
        private int factorOne;
        private int factorTwo;
        private int deathTrajectory;
        private int screenX;

        private bool dead;

        private Random random = new Random();

        public World101Creature(int x, int y, float scale, float speed, int screenX)
        {
            this.scale = scale;
            this.speed = speed;
            this.y = y;
            this.screenX = screenX;

            position = new Vector2(x * scale, y * scale);
        }

        public void LoadContent(ContentManager content)
        {
            creature = content.Load<Texture2D>("Creatures/Creature1/Creature1_Stage1");
            collisionBox = new Rectangle(((int)(position.X - creature.Width / 2)), ((int)(position.Y - creature.Height / 2)), (creature.Width), (creature.Height));
        }

        public void Update(GameTime gameTime)
        {
            //if (position.X > screenX + (800 * scale))
            //{
            //    dead = false;
            //    position.Y = y * scale;
            //}
            //if (dead)
            //{
            //    position.X += 40 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //    position.Y += deathTrajectory * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //}
            if (position.X > screenX - (200 * scale))
            {
                position.X -= 5 / 2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            
            position.Y = y * scale + 10 * (float)Math.Sin(position.X / 15) * scale;

            collisionBox.Y = (int)position.Y;
            collisionBox.X = (int)position.X;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(creature, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        //public void setFactors(int factorOne, int factorTwo)
        //{
        //    this.factorOne = factorOne;
        //    this.factorTwo = factorTwo;
        //}

        //public void creatureDeath()
        //{
        //    dead = true;
        //    deathTrajectory = random.Next(0, 2);
        //    switch (deathTrajectory)
        //    {
        //        case 1:
        //            deathTrajectory = -55;
        //            break;
        //        case 2:
        //            deathTrajectory = -15;
        //            break;
        //        default:
        //            deathTrajectory = 25;
        //            break;
        //    }

        //}

        public int getFactorOne()
        {
            return factorOne;
        }

        public int getFactorTwo()
        {
            return factorTwo;
        }

        public int getExpectedAnswer()
        {
            return factorOne + factorTwo;
        }

        public Rectangle getCollisionBox()
        {
            return collisionBox;
        }

        public void setSpeed(float speed)
        {
            this.speed = speed;
        }

        public void setX(int x)
        {
            position.X = x;
        }

        public void setY(int y)
        {
            position.Y = y;
        }
    }
}
