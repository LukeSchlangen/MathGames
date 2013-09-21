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
            QuestionUp,
            ShieldAnimation,
            ShieldUp,
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
            state = State.QuestionUp;
        }

        public void LoadContent(ContentManager content)
        {
            heroRunning = content.Load<Texture2D>("General/Hero/running_sprite_sheet_5x5");
            shield = content.Load<Texture2D>("General/Shield/question_box_to_shield_sheet_3x3");
            heroPosition = new Vector2(275 * scale, 800 * scale);
            heroCollisionBox = new Rectangle();
            heroAnimation = new Animation(heroRunning, heroPosition, 5, 5, scale, 10f);
            shieldPosition.X = heroAnimation.getAnimationPosition().X * scale - 250 * scale;
            shieldPosition.Y = heroAnimation.getAnimationPosition().Y * scale - 300 * scale;
            shieldAnimation = new Animation(shield, shieldPosition, 3, 3, scale, 30f);
            shieldCollisionBox = new Rectangle(((int)(shieldPosition.X - shield.Width / 2)), ((int)(shieldPosition.Y - shield.Height / 2)), (int)(shield.Width), (shield.Height));
        }

        public void Update(GameTime gameTime)
        {
            heroAnimation.animateLoop(gameTime);

            if (state == State.QuestionUp)
            {
                shieldAnimation.getFirstState();
            }

            if (state == State.ShieldAnimation)
            {
                Debug.WriteLine(shieldAnimationDone);
                shieldAnimationDone = shieldAnimation.animateOnce(gameTime);
                if (shieldAnimationDone == true)
                {
                    state = State.ShieldUp;
                    shieldAnimationDone = false;
                    shieldAnimation.resetAnimation();
                }        
            }

            if (state == State.ShieldUp)
            {
                shieldAnimation.getLastState();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            heroAnimation.Draw(spriteBatch);            
            shieldAnimation.Draw(spriteBatch);
            
        }

        public void shieldUp()
        {
            state = State.ShieldAnimation;
            shieldAnimation.resetAnimation();
        }

        public void shieldDown()
        {
            state = State.QuestionUp;
        }
    }
}
