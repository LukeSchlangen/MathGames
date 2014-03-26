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
    class Hero
    {
        private enum State
        {
            None,
            Question,
            ShieldAnimation,
            Shield,
            StageSuccess,
            Death,
            Stop
        }

        private float scale;
        private float speed = .1f;
        private State state;
        private bool shieldAnimationDone = false;
        private bool heroStop = false;
        private bool dead = false;
        private bool isShooting = false;

        private int shieldShake = 1;

        private float constantlyIncreasingNumber;

        private Vector2 heroPosition;
        private Vector2 heroStartingPosition;
        private Vector2 shieldPosition;
        private Vector2 shieldStartingPosition;

        private Texture2D heroRunning;
        private Texture2D shield;
        private Texture2D malfunctionShield;

        private Rectangle heroCollisionBox;
        private Rectangle shieldCollisionBox;
        private Rectangle activatedShieldCollisionBoxPosition;

        private Animation heroAnimation;
        private Animation shieldAnimation;

        public void Initialize(float scale)
        {
            this.scale = scale;
            state = State.Question;
            heroStartingPosition = new Vector2(275 * scale, 800 * scale);
        }

        public void LoadContent(ContentManager content)
        {
            heroRunning = content.Load<Texture2D>("General/Hero/running_sprite_sheet_5x5");
            shield = content.Load<Texture2D>("General/Shield/question_box_to_shield_3x3");
            malfunctionShield = content.Load<Texture2D>("General/Shield/malfunction_shield");
            heroPosition = heroStartingPosition;
            heroAnimation = new Animation(heroRunning, heroStartingPosition, 5, 5, scale, 10f);
            heroCollisionBox = new Rectangle((int)((heroStartingPosition.X) - 150 * scale), (int)(heroStartingPosition.Y), (int)(heroAnimation.getWidth() * scale), heroAnimation.getHeight());
            shieldPosition.X = heroAnimation.getAnimationPosition().X - 40 * scale;
            shieldPosition.Y = heroAnimation.getAnimationPosition().Y - 200 * scale;
            shieldStartingPosition = new Vector2(shieldPosition.X, shieldPosition.Y);
            shieldAnimation = new Animation(shield, shieldPosition, 3, 3, scale, 30f);
            shieldCollisionBox = new Rectangle(((int)(shieldPosition.X) + (int)(275 * scale)), ((int)(shieldPosition.Y)), (int)(100 * scale), (int)(1000 * scale));
            activatedShieldCollisionBoxPosition = shieldCollisionBox;
            deactivateShield();
        }

        public void Update(GameTime gameTime)
        {

            heroAnimation.animateLoop(gameTime, heroPosition);

            if (state == State.Question)
            {
                shieldAnimation.getFirstState();
                heroStop = false;
            }

            if (state == State.ShieldAnimation)
            {
                shieldAnimationDone = shieldAnimation.animateOnce(gameTime);
                if (shieldAnimationDone == true)
                {
                    state = State.Shield;
                    shieldAnimationDone = false;
                    shieldAnimation.resetAnimation();
                }
            }

            if (state == State.Shield)
            {
                shieldAnimation.getLastState();
            }

            if (!heroStop)
            {
                if (state == State.StageSuccess)
                {
                        shieldAnimation.getLastState();
                    shieldPosition.X += 20 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    heroPosition.X += 5 / 2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                                    }
                
                if (state == State.Death)
                {
                    heroPosition.X -= 25 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    heroPosition.Y -= 25 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (shieldShake <= 9){
                        if (shieldShake % 3 == 1)
                        {
                            shieldPosition.X -= 10 * scale;
                        }
                        if (shieldShake % 3 == 2)
                        {
                            shieldPosition.X += 5 * scale;
                            shieldPosition.Y += 10 * scale;
                        }
                        if (shieldShake % 3 == 0)
                        {
                            shieldPosition.X += 5 * scale;
                            shieldPosition.Y -= 10 * scale;
                        }
                    }
                    shieldShake += 1;
                }
                else
                {
                    constantlyIncreasingNumber += speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    heroPosition.Y += (float)Math.Sin(constantlyIncreasingNumber / 8) * scale;
                }
            }

            heroCollisionBox.X = (int)(heroPosition.X - 150 * scale);
            heroCollisionBox.Y = (int)heroPosition.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            heroAnimation.Draw(spriteBatch, heroPosition);
            shieldAnimation.Draw(spriteBatch, shieldPosition);
            if (state == State.Death)
            {
                spriteBatch.Draw(malfunctionShield, new Vector2(shieldPosition.X - 100*scale, shieldPosition.Y - 75 *scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

        }

        public void shieldAnimate()
        {
            state = State.ShieldAnimation;
            shieldAnimation.resetAnimation();
        }

        public void questionUp()
        {
            state = State.Question;
            deactivateShield();
        }

        public void shieldCollision()
        {
            state = State.Question;
        }

        public void stageSuccess()
        {
            state = State.StageSuccess;
        }

        public void collectWildCreature()
        {
            heroAnimation.stopAnimation();
            heroStop = true;
        }

        public void die()
        {
            heroAnimation.stopAnimation();
            state = State.Death;
            dead = true;
            shieldShake = 1;
        }

        public void live()
        {
            heroAnimation.startAnimation();
            dead = false;
        }

        public Rectangle getHeroCollisionBox()
        {
            return heroCollisionBox;
        }

        public Rectangle getShieldCollisionBox()
        {
            return shieldCollisionBox;
        }

        public bool getShieldAnimationDone()
        {
            return shieldAnimationDone;
        }

        public bool getDead()
        {
            return dead;
        }

        public void deactivateShield()
        {
            shieldCollisionBox.X = activatedShieldCollisionBoxPosition.X - 1000;
        }

        public void activateShield()
        {
            shieldCollisionBox.X = activatedShieldCollisionBoxPosition.X;
        }

        public void stop()
        {
            heroAnimation.stopAnimation();
            speed = 0f;
            heroStop = true;
        }

        public void start()
        {
            heroPosition = heroStartingPosition;
            shieldPosition = shieldStartingPosition;
            state = State.Question;
            speed = .1f;
            heroStop = false;
        }

        public Vector2 getHeroPosition()
        {
            //return the position where the hero's "back foot" is for friendly creature to follow
            return new Vector2(heroPosition.X, heroPosition.Y + 200 * scale);
        }
    }
}
