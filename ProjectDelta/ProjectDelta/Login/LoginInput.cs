﻿using System;
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

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;

namespace ProjectDelta
{
    class LoginInput
    {
        private SpriteFont font;

        private KeyboardState current;
        private KeyboardState previous;

        private string input = "";

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("input_font");
        }

        public string Update()
        {
            previous = current;
            current = Keyboard.GetState();
            input = "";

            if (current.IsKeyDown(Keys.A) && previous.IsKeyDown(Keys.A) == false)
            {
                input = "a";
            }
            if (current.IsKeyDown(Keys.B) && previous.IsKeyDown(Keys.B) == false)
            {
                input = "b";
            }
            if (current.IsKeyDown(Keys.C) && previous.IsKeyDown(Keys.C) == false)
            {
                input = "c";
            }
            if (current.IsKeyDown(Keys.D) && previous.IsKeyDown(Keys.D) == false)
            {
                input = "d";
            }
            if (current.IsKeyDown(Keys.E) && previous.IsKeyDown(Keys.E) == false)
            {
                input = "e";
            }
            if (current.IsKeyDown(Keys.F) && previous.IsKeyDown(Keys.F) == false)
            {
                input = "f";
            }
            if (current.IsKeyDown(Keys.G) && previous.IsKeyDown(Keys.G) == false)
            {
                input = "g";
            }
            if (current.IsKeyDown(Keys.H) && previous.IsKeyDown(Keys.H) == false)
            {
                input = "h";
            }
            if (current.IsKeyDown(Keys.I) && previous.IsKeyDown(Keys.I) == false)
            {
                input = "i";
            }
            if (current.IsKeyDown(Keys.J) && previous.IsKeyDown(Keys.J) == false)
            {
                input = "j";
            }
            if (current.IsKeyDown(Keys.K) && previous.IsKeyDown(Keys.K) == false)
            {
                input = "k";
            }
            if (current.IsKeyDown(Keys.L) && previous.IsKeyDown(Keys.L) == false)
            {
                input = "l";
            }
            if (current.IsKeyDown(Keys.M) && previous.IsKeyDown(Keys.M) == false)
            {
                input = "m";
            }
            if (current.IsKeyDown(Keys.N) && previous.IsKeyDown(Keys.N) == false)
            {
                input = "n";
            }
            if (current.IsKeyDown(Keys.O) && previous.IsKeyDown(Keys.O) == false)
            {
                input = "o";
            }
            if (current.IsKeyDown(Keys.P) && previous.IsKeyDown(Keys.P) == false)
            {
                input = "p";
            }
            if (current.IsKeyDown(Keys.Q) && previous.IsKeyDown(Keys.Q) == false)
            {
                input = "q";
            }
            if (current.IsKeyDown(Keys.R) && previous.IsKeyDown(Keys.R) == false)
            {
                input = "r";
            }
            if (current.IsKeyDown(Keys.S) && previous.IsKeyDown(Keys.S) == false)
            {
                input = "s";
            }
            if (current.IsKeyDown(Keys.T) && previous.IsKeyDown(Keys.T) == false)
            {
                input = "t";
            }
            if (current.IsKeyDown(Keys.U) && previous.IsKeyDown(Keys.U) == false)
            {
                input = "u";
            }
            if (current.IsKeyDown(Keys.V) && previous.IsKeyDown(Keys.V) == false)
            {
                input = "v";
            }
            if (current.IsKeyDown(Keys.W) && previous.IsKeyDown(Keys.W) == false)
            {
                input = "w";
            }
            if (current.IsKeyDown(Keys.X) && previous.IsKeyDown(Keys.X) == false)
            {
                input = "x";
            }
            if (current.IsKeyDown(Keys.Y) && previous.IsKeyDown(Keys.Y) == false)
            {
                input = "y";
            }
            if (current.IsKeyDown(Keys.Z) && previous.IsKeyDown(Keys.Z) == false)
            {
                input = "z";
            }
            if ((current.IsKeyDown(Keys.NumPad0) && previous.IsKeyDown(Keys.NumPad0) == false) || (current.IsKeyDown(Keys.D0) && previous.IsKeyDown(Keys.D0) == false))
            {
                input = "0";
            }
            if ((current.IsKeyDown(Keys.NumPad1) && previous.IsKeyDown(Keys.NumPad1) == false) || (current.IsKeyDown(Keys.D1) && previous.IsKeyDown(Keys.D1) == false))
            {
                input = "1";
            }
            if ((current.IsKeyDown(Keys.NumPad2) && previous.IsKeyDown(Keys.NumPad2) == false) || (current.IsKeyDown(Keys.D2) && previous.IsKeyDown(Keys.D2) == false))
            {
                input = "2";
            }
            if ((current.IsKeyDown(Keys.NumPad3) && previous.IsKeyDown(Keys.NumPad3) == false) || (current.IsKeyDown(Keys.D3) && previous.IsKeyDown(Keys.D3) == false))
            {
                input = "3";
            }
            if ((current.IsKeyDown(Keys.NumPad4) && previous.IsKeyDown(Keys.NumPad4) == false) || (current.IsKeyDown(Keys.D4) && previous.IsKeyDown(Keys.D4) == false))
            {
                input = "4";
            }
            if ((current.IsKeyDown(Keys.NumPad5) && previous.IsKeyDown(Keys.NumPad5) == false) || (current.IsKeyDown(Keys.D5) && previous.IsKeyDown(Keys.D5) == false))
            {
                input = "5";
            }
            if ((current.IsKeyDown(Keys.NumPad6) && previous.IsKeyDown(Keys.NumPad6) == false) || (current.IsKeyDown(Keys.D6) && previous.IsKeyDown(Keys.D6) == false))
            {
                input = "6";
            }
            if ((current.IsKeyDown(Keys.NumPad7) && previous.IsKeyDown(Keys.NumPad7) == false) || (current.IsKeyDown(Keys.D7) && previous.IsKeyDown(Keys.D7) == false))
            {
                input = "7";
            }
            if ((current.IsKeyDown(Keys.NumPad8) && previous.IsKeyDown(Keys.NumPad8) == false) || (current.IsKeyDown(Keys.D8) && previous.IsKeyDown(Keys.D8) == false))
            {
                input = "8";
            }
            if ((current.IsKeyDown(Keys.NumPad9) && previous.IsKeyDown(Keys.NumPad9) == false) || (current.IsKeyDown(Keys.D9) && previous.IsKeyDown(Keys.D9) == false))
            {
                input = "9";
            }
            if (current.IsKeyDown(Keys.Back) && previous.IsKeyDown(Keys.Back) == false)
            {
                input = "";
            }


            previous = current;
            current = Keyboard.GetState();



            return input;
        }

        public string getInput()
        {
            return input;
        }
    }
}
