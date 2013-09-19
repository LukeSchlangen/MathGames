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
    class Level1Monster
    {
        private Texture2D monster;
        private Vector2 position;
        private Rectangle collisionBox;
        private float speed = 0f;
        private float scale;

        private int factorOne;
        private int factorTwo;

        Random random = new Random();

        public Level1Monster(int x, int y, float scale, float speed)
        {
            this.scale = scale;
            this.speed = speed;
            position = new Vector2(x*scale, y*scale);
        }

        public void LoadContent(ContentManager content)
        {
            monster = content.Load<Texture2D>("General/Monsters/enemy_1");
            collisionBox = new Rectangle(((int)(position.X - monster.Width / 2)), ((int)(position.Y - monster.Height / 2)), (int)(monster.Width), (monster.Height));
        }

        public void Update(GameTime gameTime)
        {
            position.X -= 5/2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            position.Y += (float)Math.Sin(position.X/15);
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

        public void newRandomFactors(int min, int max)
        {
            factorOne = random.Next(min, max);
            factorTwo = random.Next(min, max);
        }
    }
}
