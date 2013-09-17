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

            if (current.IsKeyDown(Keys.A) && previous.IsKeyDown(Keys.A) == false)
            {
                input = input + "a";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.B) && previous.IsKeyDown(Keys.B) == false)
            {
                input = input + "b";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.C) && previous.IsKeyDown(Keys.C) == false)
            {
                input = input + "c";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.D) && previous.IsKeyDown(Keys.D) == false)
            {
                input = input + "d";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.E) && previous.IsKeyDown(Keys.E) == false)
            {
                input = input + "e";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.F) && previous.IsKeyDown(Keys.F) == false)
            {
                input = input + "f";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.G) && previous.IsKeyDown(Keys.G) == false)
            {
                input = input + "g";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.H) && previous.IsKeyDown(Keys.H) == false)
            {
                input = input + "h";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.I) && previous.IsKeyDown(Keys.I) == false)
            {
                input = input + "i";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.J) && previous.IsKeyDown(Keys.J) == false)
            {
                input = input + "j";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.K) && previous.IsKeyDown(Keys.K) == false)
            {
                input = input + "k";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.L) && previous.IsKeyDown(Keys.L) == false)
            {
                input = input + "l";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.M) && previous.IsKeyDown(Keys.M) == false)
            {
                input = input + "m";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.N) && previous.IsKeyDown(Keys.N) == false)
            {
                input = input + "n";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.O) && previous.IsKeyDown(Keys.O) == false)
            {
                input = input + "o";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.P) && previous.IsKeyDown(Keys.P) == false)
            {
                input = input + "p";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.Q) && previous.IsKeyDown(Keys.Q) == false)
            {
                input = input + "q";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.R) && previous.IsKeyDown(Keys.R) == false)
            {
                input = input + "r";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.S) && previous.IsKeyDown(Keys.S) == false)
            {
                input = input + "s";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.T) && previous.IsKeyDown(Keys.T) == false)
            {
                input = input + "t";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.U) && previous.IsKeyDown(Keys.U) == false)
            {
                input = input + "u";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.V) && previous.IsKeyDown(Keys.V) == false)
            {
                input = input + "v";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.W) && previous.IsKeyDown(Keys.W) == false)
            {
                input = input + "w";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.X) && previous.IsKeyDown(Keys.X) == false)
            {
                input = input + "x";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.Y) && previous.IsKeyDown(Keys.Y) == false)
            {
                input = input + "y";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.Z) && previous.IsKeyDown(Keys.Z) == false)
            {
                input = input + "z";

                previous = current;
                current = Keyboard.GetState();
            }
            if (current.IsKeyDown(Keys.Back) && previous.IsKeyDown(Keys.Back) == false)
            {
                if (input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                }
            }
            if (current.IsKeyDown(Keys.Enter) && previous.IsKeyDown(Keys.Enter) == false)
            {
                string temp = input;
                input = "";
                return temp;
            }
            if (current.IsKeyDown(Keys.Tab) && previous.IsKeyDown(Keys.Tab) == false)
            {
                string temp = input;
                input = "";
                return temp;
            }

            return null;
        }

        public string getInput()
        {
            return input;
        }
    }
}
