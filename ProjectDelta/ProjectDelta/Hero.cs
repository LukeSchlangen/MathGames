﻿using System;
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
        }

        private float scale;
        private float speed = .1f;
        private State state;
        private bool shieldAnimationDone = false;
        private bool heroStop = false;

        private Vector2 heroPosition;
        private Vector2 heroStartingPosition;
        private Vector2 shieldPosition;

        private Texture2D heroRunning;
        private Texture2D shield;

        private Rectangle heroCollisionBox;
        private Rectangle shieldCollisionBox;

        private Animation heroAnimation;
        private Animation shieldAnimation;

        public void Initialize(float scale)
        {
            this.scale = scale;
            state = State.Question;
        }

        public void LoadContent(ContentManager content)
        {
            heroRunning = content.Load<Texture2D>("General/Hero/running_sprite_sheet_5x5");
            shield = content.Load<Texture2D>("General/Shield/question_box_to_shield_3x3");
            heroStartingPosition = new Vector2(275*scale, 800*scale);
            heroPosition = heroStartingPosition;
            heroAnimation = new Animation(heroRunning, heroStartingPosition, 5, 5, scale, 10f);
            heroCollisionBox = new Rectangle((int)((heroStartingPosition.X) - 100*scale), (int)(heroStartingPosition.Y), (int)(heroAnimation.getWidth()*scale), heroAnimation.getHeight());
            shieldPosition.X = heroAnimation.getAnimationPosition().X - 40*scale;
            shieldPosition.Y = heroAnimation.getAnimationPosition().Y - 200 * scale;
            shieldAnimation = new Animation(shield, shieldPosition, 3, 3, scale, 30f);
            shieldCollisionBox = new Rectangle(((int)(shieldPosition.X) + (int)(275*scale)), ((int)(shieldPosition.Y)), (int)(100*scale), (int)(1000*scale));
            deactivateShield();
        }

        public void Update(GameTime gameTime)
        {
            heroAnimation.animateLoop(gameTime);

            if (state == State.Question)
            {
                shieldAnimation.getFirstState();
                heroPosition = heroStartingPosition;
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

            if (state == State.StageSuccess && heroStop == false)
            {
                heroPosition.X += 5 / 2 * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            heroCollisionBox.X = (int)(heroPosition.X - 150 * scale);
            heroCollisionBox.Y = (int)heroPosition.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            heroAnimation.Draw(spriteBatch, heroPosition);
     
            if (state != State.StageSuccess)
            {
                shieldAnimation.Draw(spriteBatch, shieldPosition);
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
        }

        public void shieldCollision()
        {
            state = State.Question;
        }

        public void stageSuccess()
        {
            state = State.StageSuccess;
        }

        public void collectCreature()
        {
            heroAnimation.stopAnimation();
            heroStop = true;
        }

        public void die()
        {
            heroAnimation.stopAnimation();
        }

        public void live()
        {
            heroAnimation.startAnimation();
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

        public void deactivateShield()
        {
            shieldCollisionBox.X -= 1000;
        }

        public void activateShield()
        {
            shieldCollisionBox.X += 1000;
        }
    }
}