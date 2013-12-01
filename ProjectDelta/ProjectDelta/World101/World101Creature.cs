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
        private String creatureString;
        private float speed = 0f;
        private float scale;
        private float bounceTimerFloat;

        private int y;
        //private int factorOne;
        //private int factorTwo;
        //private int deathTrajectory;
        private int screenX;

        //private bool dead;

        private Random random = new Random();

        public World101Creature(int x, int y, float scale, float speed, int screenX)
        {
            this.scale = scale;
            this.speed = speed;
            this.y = y;
            this.screenX = screenX;

            position = new Vector2(x * scale, y * scale);
            bounceTimerFloat = position.X;
        }

        public void LoadContent(ContentManager content, int worldStage)
        {
            creatureString = "Creatures/Creature1/Creature1_Stage" + worldStage;
            creature = content.Load<Texture2D>(creatureString);
            collisionBox = new Rectangle(((int)(position.X - creature.Width / 2)), ((int)(position.Y - creature.Height / 2)), (creature.Width), (creature.Height));
        }

        public void Update(GameTime gameTime)
        {

            if (position.X > screenX - (200 * scale))
            {
                position.X -= 5 / 2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            bounceTimerFloat -= 5 / 2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            position.Y = y * scale + 10 * (float)Math.Sin(bounceTimerFloat / 15) * scale;

            collisionBox.Y = (int)position.Y;
            collisionBox.X = (int)position.X;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(creature, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public Rectangle getCollisionBox()
        {
            return collisionBox;
        }

        public void setSpeed(float speed)
        {
            this.speed = speed;
        }

        public void displayNewCreature()
        {
            //setX((int)(1000 * scale));
            //setY((int)(200 * scale));
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
