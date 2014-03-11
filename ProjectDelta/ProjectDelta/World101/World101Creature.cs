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
        private Texture2D creatureImage;
        private Vector2 position;
        private float speed;
        private float scale;
        private float constantlyIncreasingNumber;
        private int powerupUseCount = 0;
        private int maxNumberOfPowerUpUses;
        private int creatureNumber;
        private int creatureLevel;
        private string creatureName;
        private string creatureType;

        private bool isAvailable;
        private bool usingPowerup = false;

        private Rectangle collisionBox;

        public World101Creature(int creatureNumber, int worldStage, int numberOfCorrectAnswers, float scale, float speed)
        {
            this.scale = scale;
            this.speed = speed;
            this.creatureNumber = creatureNumber;
            setAttributes(worldStage);
            maxNumberOfPowerUpUses = creatureLevel / 20;
            if (maxNumberOfPowerUpUses > 3) { maxNumberOfPowerUpUses = 3; }
        }

        public void LoadContent(ContentManager content)
        {
            creatureImage = content.Load<Texture2D>("Creatures/wild_creature_" + creatureNumber);
            collisionBox = new Rectangle(((int)(position.X - creatureImage.Width / 2)), ((int)(position.Y - creatureImage.Height / 2)), (creatureImage.Width), (creatureImage.Height));
        }

        public void Update(GameTime gameTime, Vector2 lowerRightCorner)
        {
            position.X = lowerRightCorner.X - creatureImage.Width * scale + 130 * scale;
            position.Y = lowerRightCorner.Y - creatureImage.Height * scale;
            //bouncing
            constantlyIncreasingNumber += speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            position.Y += 10 * (float)Math.Sin(constantlyIncreasingNumber / 6) * scale;

            ////code from wild creature
            //position.X -= 5 / 2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            collisionBox.Y = (int)position.Y;
            collisionBox.X = (int)position.X;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(creatureImage, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void stop()
        {
            speed = 0f;
        }

        public void reset(int worldStage)
        {
            setAttributes(worldStage);
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

        public void setPosition(Vector2 position)
        {
            this.position = position;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public Rectangle getCollisionBox()
        {
            return collisionBox;
        }

        public void setCollisionBox(Rectangle collisionBox)
        {
            this.collisionBox = collisionBox;
        }

        public float getWidth()
        {
            return creatureImage.Width;
        }

        public float getHeight()
        {
            return creatureImage.Height;
        }
        public bool getAvailability()
        {
            return isAvailable;
        }

        private bool mostEvolvedCreature(int worldStage)
        {
            bool mostEvolved = false;

            switch (creatureNumber)
            {
                case 15:
                    mostEvolved = true;
                    break;
                case 24:
                    mostEvolved = true;
                    break;
                case 29:
                    mostEvolved = true;
                    break;
                case 38:
                    mostEvolved = true;
                    break;
                case 44:
                    mostEvolved = true;
                    break;
                case 49:
                    mostEvolved = true;
                    break;
                case 55:
                    mostEvolved = true;
                    break;
                case 62:
                    mostEvolved = true;
                    break;
                case 67:
                    mostEvolved = true;
                    break;
                case 72:
                    mostEvolved = true;
                    break;
                case 78:
                    mostEvolved = true;
                    break;
                case 86:
                    mostEvolved = true;
                    break;
                case 93:
                    mostEvolved = true;
                    break;
                case 101:
                    mostEvolved = true;
                    break;
                case 109:
                    mostEvolved = true;
                    break;
                case 115:
                    mostEvolved = true;
                    break;
                case 123:
                    mostEvolved = true;
                    break;
                case 130:
                    mostEvolved = true;
                    break;
                case 138:
                    mostEvolved = true;
                    break;
                case 144:
                    mostEvolved = true;
                    break;
                case 149:
                    mostEvolved = true;
                    break;
                case 157:
                    mostEvolved = true;
                    break;
                default:
                    mostEvolved = false;
                    break;
            }

            if (creatureNumber == (worldStage - 1))
            {
                mostEvolved = true;
            }

            return mostEvolved;
        }

        private bool isCreatureAvailable(int worldStage)
        {
            //if they are the most evolved state
            bool available = mostEvolvedCreature(worldStage);

            //if the creature hasn't been collected yet,
            //the creature is not available
            if (creatureNumber >= worldStage)
            {
                available = false;
            }

            return available;
        }

        private void setAttributes(int worldStage)
        {
            if (creatureNumber < 16)
            {
                creatureName = "Begino";
                creatureType = "Tackler";
            }
            else if (creatureNumber < 25)
            {
                creatureName = "Addingo";
                creatureType = "Spiker";
            }
            else if (creatureNumber < 30)
            {
                creatureName = "Lotolegs";
                creatureType = "Strecher";
            }
            else if (creatureNumber < 39)
            {
                creatureName = "Toradd";
                creatureType = "Tackler";
            }
            else if (creatureNumber < 45)
            {
                creatureName = "Diamon";
                creatureType = "Freezer";
            }
            else if (creatureNumber < 50)
            {
                creatureName = "Torton";
                creatureType = "Tackler";
            }
            else if (creatureNumber < 56)
            {
                creatureName = "Harehat";
                creatureType = "Digger";
            }
            else if (creatureNumber < 63)
            {
                creatureName = "Umbrello";
                creatureType = "Stomper";
            }
            else if (creatureNumber < 68)
            {
                creatureName = "Springo";
                creatureType = "Spinner";
            }
            else if (creatureNumber < 73)
            {
                creatureName = "Vamp";
                creatureType = "Scarer";
            }
            else if (creatureNumber < 79)
            {
                creatureName = "Chargo";
                creatureType = "Zapper";
            }
            else if (creatureNumber < 87)
            {
                creatureName = "Lanwal";
                creatureType = "Stomper";
            }
            else if (creatureNumber < 94)
            {
                creatureName = "Slinko";
                creatureType = "Stretcher";
            }
            else if (creatureNumber < 102)
            {
                creatureName = "Hingo";
                creatureType = "Shooter";
            }
            else if (creatureNumber < 110)
            {
                creatureName = "Downgo";
                creatureType = "Digger";
            }
            else if (creatureNumber < 116)
            {
                creatureName = "Slicko";
                creatureType = "Zapper";
            }
            else if (creatureNumber < 124)
            {
                creatureName = "Fuzzbump";
                creatureType = "Stomper";
            }
            else if (creatureNumber < 131)
            {
                creatureName = "Arrow";
                creatureType = "Tackler";
            }
            else if (creatureNumber < 139)
            {
                creatureName = "Flamingren";
                creatureType = "Zapper";
            }
            else if (creatureNumber < 145)
            {
                creatureName = "Twisto";
                creatureType = "Spinner";
            }
            else if (creatureNumber < 150)
            {
                creatureName = "Hoverbo";
                creatureType = "Shooter";
            }
            else if (creatureNumber < 158)
            {
                creatureName = "Wisdo";
                creatureType = "Freezer";
            }
            else
            {
                creatureName = "Unnamed";
                creatureType = "Tackler";
            }

            isAvailable = isCreatureAvailable(worldStage);
            //isAvailable = true;
        }

        private int determineCreatureLevel(int worldStage, int creature, int numberOfAttemptedProblems, int numberOfCorrectProblems)
        {
            int creatureLevel = worldStage - creature;

            if (creatureLevel > 99)
            {
                creatureLevel = 99;
            }
            return creatureLevel;
        }

        public string getCreatureType()
        {
            return creatureType;
        }

        public string getCreatureName()
        {
            return creatureName;
        }

        public int getCreatureLevel(int worldStage, int numberOfAttemptedProblems, int numberOfCorrectProblems)
        {
            return creatureLevel;
        }
    }
}
