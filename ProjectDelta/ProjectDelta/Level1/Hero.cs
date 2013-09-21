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
        }

        float scale;
        private State state;
        private bool shieldAnimationDone = false;

        private Vector2 heroPosition;
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
            shield = content.Load<Texture2D>("General/Shield/question_box_to_shield_sheet_3x3");
            heroPosition = new Vector2(275 * scale, 800 * scale);
            heroAnimation = new Animation(heroRunning, heroPosition, 5, 5, scale, 10f);
            heroCollisionBox = new Rectangle(((int)(heroPosition.X - heroAnimation.getWidth() / 4)), ((int)(heroPosition.Y - heroAnimation.getHeight() / 2)), heroAnimation.getWidth(), heroAnimation.getHeight());
            shieldPosition.X = heroAnimation.getAnimationPosition().X * scale - 250 * scale;
            shieldPosition.Y = heroAnimation.getAnimationPosition().Y * scale - 300 * scale;
            shieldAnimation = new Animation(shield, shieldPosition, 3, 3, scale, 30f);
            shieldCollisionBox = new Rectangle(((int)(shieldPosition.X - shieldAnimation.getWidth() / 16)), ((int)(shieldPosition.Y - shieldAnimation.getHeight() / 2)), shieldAnimation.getWidth(), shieldAnimation.getHeight() + 1000);
            deactivateShield();
        }

        public void Update(GameTime gameTime)
        {
            heroAnimation.animateLoop(gameTime);

            if (state == State.Question)
            {
                shieldAnimation.getFirstState();
                //Debug.WriteLine("QUESTION");
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
                //Debug.WriteLine("ANIMATE");
            }

            if (state == State.Shield)
            {
                shieldAnimation.getLastState();
                //Debug.WriteLine("SHIELD");
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            heroAnimation.Draw(spriteBatch);            
            shieldAnimation.Draw(spriteBatch);          
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

        public void die()
        {
            heroAnimation.stopAnimation();
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
