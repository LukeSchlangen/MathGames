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
        private float spinRotation = 0;
        private int powerupUseCount = 0;
        private int maxNumberOfPowerUpUses;
        private int creatureNumber;
        private int creatureLevel;
        private int holdCounter;
        private string creatureName;
        private string creatureType;
        private string creatureDescription;

        private bool isAvailable = false;
        private bool usingPowerup = false;
        private bool hold = false;

        private Rectangle collisionBox;

        public World101Creature(int creatureNumber, int worldStage, int numberOfCorrectAnswers, int totalTimePlayed, float scale, float speed)
        {
            this.scale = scale;
            this.speed = speed;
            this.creatureNumber = creatureNumber;
            this.creatureLevel = worldStage - creatureNumber;
            setAttributes(worldStage, numberOfCorrectAnswers, totalTimePlayed);
        }

        public void LoadContent(ContentManager content)
        {
            creatureImage = content.Load<Texture2D>("Creatures/wild_creature_" + creatureNumber);
            collisionBox = new Rectangle(((int)(position.X - creatureImage.Width / 2)), ((int)(position.Y - creatureImage.Height / 2)), (creatureImage.Width), (creatureImage.Height));
        }

        public void Update(GameTime gameTime, Vector2 lowerRightCorner)
        {
            if (usingPowerup && (creatureType == "Tackler" || creatureType == "Spinner"))
            {
                position.X += 250;
                if (position.X > 2500 * scale)
                {
                    usingPowerup = false;
                }
                if (creatureType == "Spinner")
                {
                    position.X -= 100;
                    spinRotation += 700f;
                }
            }
            else if (usingPowerup && (creatureType == "Spiker" || creatureType == "Zapper"))
            {
                position.X += 30;
                if (position.X > lowerRightCorner.X)
                {
                    usingPowerup = false;
                }
            }
            else if (usingPowerup && (creatureType == "Stomper"))
            {
                position.X += 600 * scale;
                position.Y -= 500 * scale;
                if (position.X > lowerRightCorner.X + 800 * scale)
                {
                    position.X = lowerRightCorner.X + 200 * scale;
                    position.Y += 1500 * scale;
                    if (position.Y > lowerRightCorner.Y - getHeight() + 200*scale)
                    {
                        position.Y = lowerRightCorner.Y - getHeight() + 200 * scale;
                        holdCounter = 0;
                        hold = true;
                        usingPowerup = false;
                    }
                }
            }
            else if (hold)
            {
                holdCounter += 1;
                if (holdCounter > 10)
                {
                    hold = false;
                }
            }
            else
            {
                position.X = lowerRightCorner.X - creatureImage.Width * scale + 130 * scale;
                position.Y = lowerRightCorner.Y - creatureImage.Height * scale;
                //bouncing
                constantlyIncreasingNumber += speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                position.Y += 10 * (float)Math.Sin(constantlyIncreasingNumber / 6) * scale;
            }

            collisionBox.Y = (int)position.Y;
            collisionBox.X = (int)position.X;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (usingPowerup && getCreatureType() == "Spinner")
            {
                spriteBatch.Draw(creatureImage, position, null, Color.White, spinRotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(creatureImage, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        public void stop()
        {
            speed = 0f;
        }

        public void reset(int worldStage, int numberOfCorrectAnswers, int lifetimeMinutesPlayed)
        {
            setAttributes(worldStage, numberOfCorrectAnswers, lifetimeMinutesPlayed);
            speed = .1f;
            powerupUseCount = 0;
            usingPowerup = false;
        }

        public void usePowerup()
        {
                powerupUseCount += 1;
                usingPowerup = true;
        }

        public int getPowerUpsRemaining()
        {
            return maxNumberOfPowerUpUses - powerupUseCount;
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
            return creatureImage.Width * scale;
        }

        public float getHeight()
        {
            return creatureImage.Height * scale;
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
                case 139:
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
                case 163:
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

        private void setAttributes(int worldStage, int correctAnswers, int totalTimePlayed)
        {
            if (creatureNumber < 16)
            {
                creatureName = "Begino";
                creatureType = "Tackler";
                creatureDescription = "Impressed by hard work, Begino \nbecomes a stronger tackler every minute you play!";
                if (totalTimePlayed <= 10)
                {
                    creatureLevel = totalTimePlayed;
                }
                else if (totalTimePlayed <= 30)
                {
                    creatureLevel = 5 + (totalTimePlayed - 10) / 2;
                }
                else
                {
                    creatureLevel = 10 + (totalTimePlayed - 30) / 4;
                }
            }
            else if (creatureNumber < 25)
            {
                creatureName = "Addingo";
                creatureType = "Spiker";
                creatureDescription = "Inspired by new locations, Addingo becomes stronger as you move from stage to stage.";
                if (worldStage < 20)
                {
                    creatureLevel = worldStage;
                }
                else
                {
                    creatureLevel = 14 + worldStage / 3;
                }
            }
            else if (creatureNumber < 30)
            {
                creatureName = "Lotolegs";
                creatureType = "Spinner";
                creatureDescription = "Nothing makes Lotolegs happier than new friends and he gets stronger with every new creature you collect!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 39)
            {
                creatureName = "Toradd";
                creatureType = "Tackler";
                creatureDescription = "Toradd loves watching you get the right answers and grows with every correct answer!";
                if (correctAnswers < 1000)
                {
                    creatureLevel = correctAnswers / 40;
                }
                else
                {
                    creatureLevel = 50 + (correctAnswers - 1000) / 60;
                }
            }
            else if (creatureNumber < 45)
            {
                creatureName = "Diamon";
                creatureType = "Zapper";
                creatureDescription = "Mixing diamond and ice, Diamon is tough and can stop the enemy in their tracks!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 50)
            {
                creatureName = "Torton";
                creatureType = "Tackler";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 56)
            {
                creatureName = "Harehat";
                creatureType = "Digger";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 63)
            {
                creatureName = "Umbrello";
                creatureType = "Stomper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 68)
            {
                creatureName = "Springo";
                creatureType = "Spinner";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 73)
            {
                creatureName = "Vamp";
                creatureType = "Stomper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 79)
            {
                creatureName = "Chargo";
                creatureType = "Zapper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 87)
            {
                creatureName = "Lanwal";
                creatureType = "Stomper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 94)
            {
                creatureName = "Slinko";
                creatureType = "Stomper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 102)
            {
                creatureName = "Hingo";
                creatureType = "Spiker";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 110)
            {
                creatureName = "Downgo";
                creatureType = "Digger";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 116)
            {
                creatureName = "Slicko";
                creatureType = "Zapper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 124)
            {
                creatureName = "Fuzzbump";
                creatureType = "Stomper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 131)
            {
                creatureName = "Arrawo";
                creatureType = "Tackler";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 140)
            {
                creatureName = "Flamingren";
                creatureType = "Zapper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 145)
            {
                creatureName = "Twisto";
                creatureType = "Spinner";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 150)
            {
                creatureName = "Hoverbo";
                creatureType = "Spiker";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 158)
            {
                creatureName = "Wisdo";
                creatureType = "Zapper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else if (creatureNumber < 164)
            {
                creatureName = "Zwiggle";
                creatureType = "Zapper";
                creatureDescription = "This creature is very nice!";
                creatureLevel = worldStage - creatureNumber;
            }
            else
            {
                creatureName = "Unnamed";
                creatureType = "Tackler";
                creatureDescription = "This creature is very nice!";
                creatureLevel = (worldStage - creatureNumber) / 4;
            }

            isAvailable = isCreatureAvailable(worldStage);



            if (creatureLevel > 99)
            {
                creatureLevel = 99;
            }

            maxNumberOfPowerUpUses = creatureLevel / 25 + 1;

            if (maxNumberOfPowerUpUses > 5)
            {
                maxNumberOfPowerUpUses = 5;
            }

        }

        public string getCreatureType()
        {
            return creatureType;
        }

        public string getCreatureName()
        {
            return creatureName;
        }

        public int getCreatureLevel()
        {
            return creatureLevel;
        }

        public string getCreatureDescription()
        {
            return creatureDescription;
        }

        public Texture2D getCreatureImage()
        {
            return creatureImage;
        }
    }
}
