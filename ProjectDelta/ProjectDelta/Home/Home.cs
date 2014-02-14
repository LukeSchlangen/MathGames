﻿using System;
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
        private Texture2D world101Box;
        private Texture2D world101Button;
        private Texture2D logoutButton;
        
        //Vectors
        private Vector2 world101BoxPosition;
        private Vector2 world101ButtonPosition;
        private Vector2 logoutButtonPosition;
        
        //Collision Boxes
        private Rectangle world101ButtonCollisionBox;
        private Rectangle logoutButtonCollisionBox;

        public void Initialize(float scale)
        {
            this.scale = scale;
            text = new HomeText(scale);
        }

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            text.LoadContent(content, screenHeight, screenWidth);
            background = content.Load<Texture2D>("Login/login_background");
            world101Box = content.Load<Texture2D>("Home/menu_box");
            world101Button = content.Load<Texture2D>("Home/start_next_level_button");
            logoutButton = content.Load<Texture2D>("Home/log_out_button");
            world101BoxPosition = new Vector2((screenWidth / 2 - world101Box.Width * scale / 2), (screenHeight / 2 - world101Box.Height * scale / 2));
            world101ButtonPosition = new Vector2((screenWidth / 2 - world101Button.Width * scale / 2), (screenHeight / 2 +  (125 * scale)));
            logoutButtonPosition = new Vector2(screenWidth / 8, screenHeight * 3 / 4);
            world101ButtonCollisionBox = new Rectangle(((int)(world101ButtonPosition.X)), ((int)(world101ButtonPosition.Y)), (int) (world101Button.Width * scale), (int) (world101Button.Height * scale));
            logoutButtonCollisionBox = new Rectangle(((int)(logoutButtonPosition.X)), ((int)(logoutButtonPosition.Y)), (int)(logoutButton.Width * scale), (int)(logoutButton.Height * scale));

            // Play music in repeating loop
            Song backgroundMusic;
            backgroundMusic = content.Load<Song>("Login/Background_Music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;  
        }

        public int Update(GameTime gameTime)
        {
            return checkClick();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(world101Box, world101BoxPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(world101Button, world101ButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(logoutButton, logoutButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            text.Draw(spriteBatch);
        }

        private int checkClick()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(world101ButtonCollisionBox))
            {
                return 101;
            }

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(logoutButtonCollisionBox))
            {
                return -1;
            }

            return 0;
        }
    }
}
