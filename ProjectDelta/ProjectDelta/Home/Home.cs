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

        private Texture2D background;

        private HomeInput input;
        private HomeText text;

        private Texture2D mainMenuBox;

        private Vector2 mainMenuBoxPosition;

        public void Initialize(float scale)
        {
            this.scale = scale;
            input = new HomeInput(scale);
            text = new HomeText(scale);
        }

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            input.LoadContent(content);
            text.LoadContent(content);
            background = content.Load<Texture2D>("Login/login_background");
            mainMenuBox = content.Load<Texture2D>("home/main_menu");
            mainMenuBoxPosition = new Vector2((screenWidth / 2 - mainMenuBox.Width * scale / 2), (screenHeight / 2 - mainMenuBox.Height * scale / 2)); //hardcoded values for screenwidth and screenheight need to be replaced
        }

        public int Update(GameTime gameTime)
        {
            input.Update(gameTime);
            text.Update(input.getCurrentInput());
            if (!input.getLastInput().Equals(""))
            {
                return Int32.Parse(input.getLastInput());
            }

            return 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            text.Draw(spriteBatch);
            spriteBatch.Draw(mainMenuBox, mainMenuBoxPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void resetUpdate()
        {
            input.resetLastInput();
        }
    }
}