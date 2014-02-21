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
    class World101Monster
    {
        private Texture2D monster;
        private Vector2 position;
        private Vector2 startingPosition;
        private Rectangle collisionBox;
        private float speed = 0f;
        private float startingSpeed = 0f;
        private float scale;

        private int y;


        private int factorOne;
        private int factorTwo;
        private int operationValue;

        private int deathTrajectory;
        private int screenX;

        public bool dead;

        private Random random = new Random();

        public World101Monster(int x, int y, float scale, float speed, int screenX)
        {
            this.scale = scale;
            this.speed = speed;
            this.startingSpeed = speed;
            this.y = y;
            this.screenX = screenX;

            position = new Vector2(x * scale, y * scale);
            startingPosition = position;
        }

        public void LoadContent(ContentManager content)
        {
            monster = content.Load<Texture2D>("General/Monsters/enemy_1");
            collisionBox = new Rectangle(((int)(position.X - monster.Width / 2)), ((int)(position.Y - monster.Height / 2)), (monster.Width), (monster.Height));
        }

        public void Update(GameTime gameTime)
        {
            if (position.X > screenX + (100 * scale))
            {
                dead = false;
                position.Y = y * scale;
                speed = startingSpeed;
            }

            if (dead)
            {
                position.X += 40 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                position.Y += deathTrajectory * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                position.X -= 5 / 2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                position.Y = y * scale + 10 * (float)Math.Sin(position.X / 15) * scale;
            }
            collisionBox.Y = (int)position.Y;
            collisionBox.X = (int)position.X;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(monster, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void setFactors(int operationValue, int factorOne, int factorTwo)
        {
            this.operationValue = operationValue;
            this.factorOne = factorOne;
            this.factorTwo = factorTwo;
        }

        public void monsterDeath()
        {
            dead = true;
            deathTrajectory = random.Next(0, 3);
            switch (deathTrajectory)
            {
                case 1:
                    deathTrajectory = -55;
                    break;
                case 2:
                    deathTrajectory = -15;
                    break;
                default:
                    deathTrajectory = 25;
                    break;
            }

        }

        public int getFactorOne()
        {
            return factorOne;
        }

        public int getFactorTwo()
        {
            return factorTwo;
        }

        public int getOperationValue()
        {
            return operationValue;
        }

        public int getExpectedAnswer()
        {
            switch (operationValue)
            {
                case 0:
                    return factorOne + factorTwo;
                case 1:
                    return factorOne - factorTwo;
                case 2:
                    return factorOne * factorTwo;
                case 3:
                    return factorOne / factorTwo;
                default:
                    return 0;
            }
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

        public void reset(int operationValue, int factorOne, int factorTwo, float speed)
        {
            dead = false;
            position = startingPosition;
            setFactors(operationValue, factorOne, factorTwo);
            setSpeed(speed);
        }
    }
}
