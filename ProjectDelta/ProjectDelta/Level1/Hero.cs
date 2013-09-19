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
            ShieldUp,
        }

        float scale;
        private State state;

        private Vector2 heroPosition;
        private Vector2 shieldPosition;

        private Texture2D heroRunning;
        private Texture2D shield;

        private Rectangle heroCollisionBox;
        private Rectangle shieldCollisionBox;

        private Animation animation;

        public void Initialize(float scale)
        {
            this.scale = scale;
            state = State.None;
            Debug.WriteLine("DOWN");
        }

        public void LoadContent(ContentManager content)
        {
            heroRunning = content.Load<Texture2D>("General/Hero/running_sprite_sheet_5x5");
            shield = content.Load<Texture2D>("General/Hero/shield");
            heroPosition = new Vector2(500 * scale, 800 * scale);
            heroCollisionBox = new Rectangle();
            animation = new Animation(heroRunning, new Vector2(300 * scale, 800 * scale), 5, 5, scale);
            shieldPosition.X = animation.getAnimationPosition().X + 250*scale;
            shieldPosition.Y = animation.getAnimationPosition().Y - 100*scale;
            shieldCollisionBox = new Rectangle(((int)(shieldPosition.X - shield.Width / 2)), ((int)(shieldPosition.Y - shield.Height / 2)), (int)(shield.Width), (shield.Height));
        }

        public void Update(GameTime gameTime)
        {
            animation.stationaryScroll(gameTime);

            if (state == State.ShieldUp)
            {

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch);

            if (state == State.ShieldUp)
            {
                spriteBatch.Draw(shield, shieldPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        public void shieldUp()
        {
            state = State.ShieldUp;
        }

        public void shieldDown()
        {
            state = State.None;
        }
    }
}
