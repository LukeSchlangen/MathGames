using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    class Home
    {
        private float scale;

        private HomeText text;

        //Mouse states
        private MouseState current;
        private MouseState previous;

        //Textures
        private Texture2D background;
        private Texture2D mainMenuBox;
        private Texture2D nextLevelButton;

        //Vectors
        private Vector2 mainMenuBoxPosition;
        private Vector2 nextLevelButtonPosition;

        //Collision Boxes
        private Rectangle nextLevelButtonCollisionBox;

        public void Initialize(float scale)
        {
            this.scale = scale;
            text = new HomeText(scale);
        }

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            text.LoadContent(content, screenHeight, screenWidth);
            background = content.Load<Texture2D>("Login/login_background");
            mainMenuBox = content.Load<Texture2D>("Home/main_menu");
            nextLevelButton = content.Load<Texture2D>("Home/start_next_level_button");
            mainMenuBoxPosition = new Vector2((screenWidth / 2 - mainMenuBox.Width * scale / 2), (screenHeight / 2 - mainMenuBox.Height * scale / 2)); //hardcoded values for screenwidth and screenheight need to be replaced
            nextLevelButtonPosition = new Vector2((screenWidth / 2 - nextLevelButton.Width * scale / 2), (screenHeight / 2 +  (125 * scale)));
            nextLevelButtonCollisionBox = new Rectangle(((int)(nextLevelButtonPosition.X)), ((int)(nextLevelButtonPosition.Y)), (int) (nextLevelButton.Width * scale), (int) (nextLevelButton.Height * scale));

            // Play music in repeating loop
            Song backgroundMusic;
            backgroundMusic = content.Load<Song>("Login/Background_Music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;  
        }

        public int Update(GameTime gameTime)
        {
            if (checkClick())
            {
                return 101;
            }

            return 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainMenuBox, mainMenuBoxPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(nextLevelButton, nextLevelButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            text.Draw(spriteBatch);
        }

        private bool checkClick()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(nextLevelButtonCollisionBox))
            {
                return true;
            }

            return false;
        }
    }
}
