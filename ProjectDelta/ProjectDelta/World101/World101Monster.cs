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
        private Rectangle collisionBox;
        private float speed = 0f;
        private float scale;

        private int factorOne;
        private int factorTwo;

        public World101Monster(int x, int y, float scale, float speed)
        {
            this.scale = scale;
            this.speed = speed;
            position = new Vector2(x*scale, y*scale);
        }

        public void LoadContent(ContentManager content)
        {
            monster = content.Load<Texture2D>("General/Monsters/enemy_1");
            collisionBox = new Rectangle(((int)(position.X - monster.Width / 2)), ((int)(position.Y - monster.Height / 2)), (monster.Width), (monster.Height));
        }

        public void Update(GameTime gameTime)
        {
            position.X -= 5/2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            collisionBox.Y = (int)position.Y;
            collisionBox.X = (int)position.X;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(monster, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void setFactors(int factorOne, int factorTwo)
        {
            this.factorOne = factorOne;
            this.factorTwo = factorTwo;
        }

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
