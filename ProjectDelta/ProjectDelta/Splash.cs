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

namespace ProjectDelta
{
    class Splash
    {
        private int splashCounter = 1000;
        private Texture2D logo;
        private Vector2 logoPosition;

        private int screenX;
        private int screenY;
        private float scale;

        public Splash(int screenX, int screenY, float scale)
        {
            this.screenX = screenX;
            this.screenY = screenY;
            this.scale = scale;
        }

        public void LoadContent(ContentManager content)
        {
            logo = content.Load<Texture2D>("General/abamath");
            logoPosition = new Vector2((screenX / 2 - logo.Width * scale / 4), (screenY / 2 - logo.Height * scale / 4));
        }

        public bool Update(GameTime gameTime)
        {
            if (splashCounter == 1000)
            {
                splashCounter = -2000;
            }
            if (splashCounter < 0)
            {
                splashCounter += gameTime.ElapsedGameTime.Milliseconds;
            }
            if (splashCounter >= 0)
            {
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(logo, logoPosition, null, Color.White, 0f, Vector2.Zero, scale/2, SpriteEffects.None, 0f);
        }
    }
}
