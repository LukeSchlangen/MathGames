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
    class World101Input
    {
        float scale;

        SpriteFont font;
        Vector2 fontPosition;

        KeyboardState keyboard;
        KeyboardState prevKeyboard;

        string input = "";
        string lastInput = "";

        private bool heroDead = false;

        public World101Input(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("input_font");
            fontPosition = new Vector2(50 * scale, 50 * scale);
        }

        public bool Update(GameTime gameTime, bool heroDead)
        {
            this.heroDead = heroDead;

            prevKeyboard = keyboard;
            keyboard = Keyboard.GetState();

            if (input.Length < 3)
            {
                if ((keyboard.IsKeyDown(Keys.NumPad0) && prevKeyboard.IsKeyDown(Keys.NumPad0) == false) || (keyboard.IsKeyDown(Keys.D0) && prevKeyboard.IsKeyDown(Keys.D0) == false))
                {
                    input = input + "0";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
                if ((keyboard.IsKeyDown(Keys.NumPad1) && prevKeyboard.IsKeyDown(Keys.NumPad1) == false) || (keyboard.IsKeyDown(Keys.D1) && prevKeyboard.IsKeyDown(Keys.D1) == false))
                {
                    input = input + "1";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
                if ((keyboard.IsKeyDown(Keys.NumPad2) && prevKeyboard.IsKeyDown(Keys.NumPad2) == false) || (keyboard.IsKeyDown(Keys.D2) && prevKeyboard.IsKeyDown(Keys.D2) == false))
                {
                    input = input + "2";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
                if ((keyboard.IsKeyDown(Keys.NumPad3) && prevKeyboard.IsKeyDown(Keys.NumPad3) == false) || (keyboard.IsKeyDown(Keys.D3) && prevKeyboard.IsKeyDown(Keys.D3) == false))
                {
                    input = input + "3";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
                if ((keyboard.IsKeyDown(Keys.NumPad4) && prevKeyboard.IsKeyDown(Keys.NumPad4) == false) || (keyboard.IsKeyDown(Keys.D4) && prevKeyboard.IsKeyDown(Keys.D4) == false))
                {
                    input = input + "4";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
                if ((keyboard.IsKeyDown(Keys.NumPad5) && prevKeyboard.IsKeyDown(Keys.NumPad5) == false) || (keyboard.IsKeyDown(Keys.D5) && prevKeyboard.IsKeyDown(Keys.D5) == false))
                {
                    input = input + "5";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
                if ((keyboard.IsKeyDown(Keys.NumPad6) && prevKeyboard.IsKeyDown(Keys.NumPad6) == false) || (keyboard.IsKeyDown(Keys.D6) && prevKeyboard.IsKeyDown(Keys.D6) == false))
                {
                    input = input + "6";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
                if ((keyboard.IsKeyDown(Keys.NumPad7) && prevKeyboard.IsKeyDown(Keys.NumPad7) == false) || (keyboard.IsKeyDown(Keys.D7) && prevKeyboard.IsKeyDown(Keys.D7) == false))
                {
                    input = input + "7";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
                if ((keyboard.IsKeyDown(Keys.NumPad8) && prevKeyboard.IsKeyDown(Keys.NumPad8) == false) || (keyboard.IsKeyDown(Keys.D8) && prevKeyboard.IsKeyDown(Keys.D8) == false))
                {
                    input = input + "8";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
                if ((keyboard.IsKeyDown(Keys.NumPad9) && prevKeyboard.IsKeyDown(Keys.NumPad9) == false) || (keyboard.IsKeyDown(Keys.D9) && prevKeyboard.IsKeyDown(Keys.D9) == false))
                {
                    input = input + "9";

                    prevKeyboard = keyboard;
                    keyboard = Keyboard.GetState();
                }
            }
            if (keyboard.IsKeyDown(Keys.Back) && prevKeyboard.IsKeyDown(Keys.Back) == false)
            {
                input = "";

                prevKeyboard = keyboard;
                keyboard = Keyboard.GetState();
            }

            if (keyboard.IsKeyDown(Keys.Enter) && prevKeyboard.IsKeyDown(Keys.Enter) == false && !input.Equals("") && !heroDead)
            {
                lastInput = input;
                input = "";
                return true;
            }

            return false;
        }

        public string getCurrentInput()
        {
            return input;
        }

        public string getLastInput()
        {
            return lastInput;
        }

        public string getInput()
        {
            return input;
        }

        public void resetInput()
        {
            input = "";
        }
    }
}
