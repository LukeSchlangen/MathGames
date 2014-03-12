//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

//using Amazon;
//using Amazon.S3;
//using Amazon.S3.Model;
//using Amazon.DynamoDBv2;
//using Amazon.DynamoDBv2.DocumentModel;
//using Amazon.DynamoDBv2.DataModel;

//namespace ProjectDelta
//{
//    class World101WildCreature
//    {
//        private Texture2D[] wildCreatures = new Texture2D[163];
//        private Vector2 position;
//        private Vector2 startingPosition;
//        private Rectangle collisionBox;
//        private float speed = 0f;
//        private float startSpeed = 0f;
//        private float scale;
//        private int x;
//        private int y;
//        private int screenX;

//        public World101WildCreature(int x, int y, float scale, float speed, int screenX)
//        {
//            this.scale = scale;
//            this.x = x;
//            this.y = y;
//            this.speed = speed;
//            this.startSpeed = speed;
//            this.screenX = screenX;

//            startingPosition = new Vector2(x * scale, y * scale);
//            position = new Vector2(x * scale, y * scale);
//        }

//        public void LoadContent(ContentManager content)
//        {

//            for (int i = 0; i < wildCreatures.Length; i++)
//            {
//                Texture2D wildCreatureToLoad = content.Load<Texture2D>("Creatures/wild_creature_" + i); //load the creatures in order so that the creature that appears corresponds with the world stage
//                wildCreatures[i] = wildCreatureToLoad;
//            }

//            collisionBox = new Rectangle(((int)(position.X - wildCreatures[0].Width / 2)), ((int)(position.Y - wildCreatures[0].Height / 2)), (wildCreatures[0].Width), (wildCreatures[0].Height));
//        }

//        public void Update(GameTime gameTime)
//        {
//            Debug.WriteLine(position);
//            position.X -= 5 / 2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
//            position.Y = y * scale + 10 * (float)Math.Sin(position.X / 15) * scale;
//            collisionBox.Y = (int)position.Y;
//            collisionBox.X = (int)position.X;
//        }

//        public void Draw(SpriteBatch spriteBatch, int worldStage)
//        {
//            spriteBatch.Draw(wildCreatures[worldStage], position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
//        }

//        public Rectangle getCollisionBox()
//        {
//            return collisionBox;
//        }

//        public void setCollisionBox(Rectangle collisionBox)
//        {
//            this.collisionBox = collisionBox;
//        }



//        public void stop()
//        {
//            startSpeed = speed;
//            speed = 0f;
//        }

//        public void reset()
//        {
//            position = new Vector2(2600 * scale, 800 * scale);
//            speed = .1f;
//        }
//    }
//}
