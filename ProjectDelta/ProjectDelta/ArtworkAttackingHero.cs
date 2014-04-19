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
    class ArtworkAttackingHero
    {
        private Texture2D monster;
        private Texture2D enemyBombOne;
        private Texture2D enemyBombTwo;
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

        public bool dead;

        private Random random = new Random();

        public ArtworkAttackingHero(int x, int y, float scale, float speed)
        {
            this.scale = scale;
            this.speed = speed;
            this.startingSpeed = speed;
            this.y = y;

            position = new Vector2(x * scale, y * scale);
            startingPosition = position;
        }

        public void LoadContent(ContentManager content)
        {
            monster = content.Load<Texture2D>("General/Monsters/enemy_3");
            enemyBombOne = content.Load<Texture2D>("General/Monsters/enemy_bomb_one");
            enemyBombTwo = content.Load<Texture2D>("General/Monsters/enemy_bomb_two");
            collisionBox = new Rectangle(((int)(position.X - monster.Width / 2)), ((int)(position.Y - monster.Height / 2)), (monster.Width), (monster.Height));
        }

        public void Update(GameTime gameTime, Vector2 heroPosition)
        {
            if (position.X < -1000 * scale)
            {
                position = startingPosition;
            }
            else if (position.X > 2200 * scale)
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
                position.Y = y * scale + 100 * (float)Math.Sin(position.X / 200) * scale;
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
            int expectedAnswer = new QuestionFormat().getExpectedAnswer(operationValue, factorOne, factorTwo);
            return expectedAnswer;
        }

        public Rectangle getCollisionBox()
        {
            return collisionBox;
        }

        public float getHeight()
        {
            return monster.Height * scale;
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

        public void reset()
        {
            dead = false;
            position = startingPosition;
        }
    }
}
