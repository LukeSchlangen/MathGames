using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProjectDelta
{
    class Instructions
    {
        private float scale;
        private int instCounter = 1000;
        private String instString = "";

        private SpriteFont font;
        private Vector2 fontPosition;
        private Texture2D background;

        public Instructions(float scale)
        {
            this.scale = scale;
        }


        public void LoadContent(ContentManager content, int screenX, int screenY)
        {
            background = content.Load<Texture2D>("Login/background_stars");
            font = content.Load<SpriteFont>("input_font");
            fontPosition = new Vector2(50 * scale, 50 * scale);
            instString = "Before you being, let's find out where you should begin your journey to save the planet.\nWhen you see a question, answer it using the number pad and then press the Enter key.\nAnswer as many questions as you can before the hero reaches his ship!";
        }

        public bool Update(GameTime gameTime)
        {
            if (instCounter == 1000)
            {
                instCounter = -8000;
            }
            if (instCounter < 0)
            {
                instCounter += gameTime.ElapsedGameTime.Milliseconds;
            }
            if (instCounter >= 0)
            {
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(background, new Vector2(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, instString, fontPosition, Color.Blue);
        }
    }
}
