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
    public class Login
    {
        DynamoDBContext context;

        private enum State
        {
            None,
            UsernameEntered,
            PasswordEntered,
            ExitButtonPressed,
            SignupButtonPressed,
            UsernameCreated,
            PasswordCreated,
            CreateUser,
            LoginError,
            CreationError,
            PasswordMatchError,
            InternetConnectionError,
        }

        private State state;
        private float scale;
        private int loginErrorCounter = 1000;

        private float planetSpeed = .075f;
        private float shipSpeed = .25f;

        private string username = null;
        private string password = null;
        private string checkPassword = null;

        LoginInput input = new LoginInput();
        Random random = new Random();

        //Textures for the login
        private Texture2D background;
        private Texture2D loginBox;
        private Texture2D signupButton;
        private Texture2D signupBox;
        private Texture2D backButton;
        private Texture2D goButton;
        private Texture2D loginHighlighter;
        private Texture2D loginError;
        private Texture2D signupError;
        private Texture2D passwordMatchError;
        private Texture2D internetConnectionError;
        private Texture2D planetOne;
        private Texture2D planetTwo;
        private Texture2D planetThree;
        private Texture2D planetFour;
        private Texture2D planetFive;
        private Texture2D shipOne;
        private Texture2D shipTwo;
        private Texture2D shipThree;
        private Texture2D shipFour;

        //Font for the login
        private SpriteFont font;

        //Vectors for the login
        private Vector2 loginBoxPosition;
        private Vector2 loginUsernameTextPosition;
        private Vector2 loginPasswordTextPosition;
        private Vector2 signupButtonPosition;
        private Vector2 signupBoxPosition;
        private Vector2 signupBoxUsernamePosition;
        private Vector2 signupBoxPasswordPosition;
        private Vector2 signupBoxPasswordConfirmPosition;
        private Vector2 signupUsernameTextPosition;
        private Vector2 signupPasswordTextPosition;
        private Vector2 signupPasswordConfirmTextPosition;
        private Vector2 goButtonPosition;
        private Vector2 backButtonPosition;
        private Vector2 loginUsernameHighlighterPosition;
        private Vector2 loginPasswordHighlighterPosition;
        private Vector2 signupUsernameHighlighterPosition;
        private Vector2 signupPasswordHighlighterPosition;
        private Vector2 signupConfirmHighlighterPosition;
        private Vector2 loginErrorPosition;
        private Vector2 signupErrorPosition;
        private Vector2 passwordMatchErrorPosition;
        private Vector2 internetConnectionErrorPosition;
        private Vector2 movingPlanetOnePosition;
        private Vector2 movingPlanetTwoPosition;
        private Vector2 movingPlanetThreePosition;
        private Vector2 movingPlanetFourPosition;
        private Vector2 movingPlanetFivePosition;
        private Vector2 shipOnePosition;
        private Vector2 shipTwoPosition;
        private Vector2 shipThreePosition;
        private Vector2 shipFourPosition;

        //Rectangles for the collision boxes for the login
        private Rectangle signupButtonCollisionBox;
        private Rectangle goButtonCollisionBox;
        private Rectangle backButtonCollisionBox;
        private Rectangle usernameCollisionBox;
        private Rectangle passwordCollisionBox;

        private Rectangle signupUsernameCollisionBox;
        private Rectangle signupPasswordCollisionBox;
        private Rectangle passwordConfirmCollisionBox;

        //Mouse states
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        //keyboard states
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        public Login(DynamoDBContext context)
        {
            this.context = context;
        }

        public void Initialize(float scale)
        {
            username = "";
            password = "";
            checkPassword = "";

            this.scale = scale;
            state = State.None;
        }

        //Specifies which content is loaded for the login screen

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            background = content.Load<Texture2D>("Login/login_background");

            // Play music in repeating loop
            Song backgroundMusic;
            backgroundMusic = content.Load<Song>("Login/Background_Music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;

            planetOne = content.Load<Texture2D>("General/Planets/planet_1");
            planetTwo = content.Load<Texture2D>("General/Planets/planet_2");
            planetThree = content.Load<Texture2D>("General/Planets/planet_3");
            planetFour = content.Load<Texture2D>("General/Planets/planet_4");
            planetFive = content.Load<Texture2D>("General/Planets/planet_5");

            shipOne = content.Load<Texture2D>("General/Ships/good_ship_1");
            shipTwo = content.Load<Texture2D>("General/Ships/enemy_ship_1");
            shipThree = content.Load<Texture2D>("General/Ships/good_fleet_1");
            shipFour = content.Load<Texture2D>("General/Ships/enemy_fleet_1");

            //any scalar value needs to take into consideration
            //the scale factor to fix resolution issues
            movingPlanetOnePosition = new Vector2(-200 * scale, 0 * scale);
            movingPlanetTwoPosition = new Vector2(-2400 * scale, 0 * scale);
            movingPlanetThreePosition = new Vector2(-4500 * scale, -1000 * scale);
            movingPlanetFourPosition = new Vector2(-6800 * scale, 0 * scale);
            movingPlanetFivePosition = new Vector2(-9000 * scale, 0 * scale);

            shipOnePosition = new Vector2(-2000 * scale, 100 * scale);
            shipTwoPosition = new Vector2(-6000 * scale, 900 * scale);
            shipThreePosition = new Vector2(-15000 * scale, 500 * scale);
            shipFourPosition = new Vector2(-23000 * scale, 300 * scale);

            loginError = content.Load<Texture2D>("Login/login_error");
            loginErrorPosition = new Vector2((screenWidth / 2 - loginError.Width * scale / 2), (screenHeight / 2 - loginError.Height * scale / 2));
            signupError = content.Load<Texture2D>("Login/signup_error");
            signupErrorPosition = new Vector2((screenWidth / 2 - signupError.Width * scale / 2), (screenHeight / 2 - signupError.Height * scale / 2));
            passwordMatchError = content.Load<Texture2D>("Login/password_match_error");
            passwordMatchErrorPosition = new Vector2((screenWidth / 2 - passwordMatchError.Width * scale / 2), (screenHeight / 2 - passwordMatchError.Height * scale / 2));
            internetConnectionError = content.Load<Texture2D>("Login/internet_connection_error");
            internetConnectionErrorPosition = new Vector2((screenWidth / 2 - internetConnectionError.Width * scale / 2), (screenHeight / 2 - internetConnectionError.Height * scale / 2));

            goButton = content.Load<Texture2D>("Login/go_button");
            goButtonPosition = new Vector2(screenWidth * 2 / 3, screenHeight / 2);
            goButtonCollisionBox = new Rectangle(((int)(goButtonPosition.X)), (int)(goButtonPosition.Y), (int)(goButton.Width), (int)(goButton.Height));

            backButton = content.Load<Texture2D>("Login/back_button");
            backButtonPosition = new Vector2(screenWidth / 8, screenHeight * 3 / 4);
            backButtonCollisionBox = new Rectangle(((int)(backButtonPosition.X)), (int)(backButtonPosition.Y), (int)(backButton.Width), (backButton.Height));

            signupBox = content.Load<Texture2D>("Login/signup_box");
            signupBoxPosition = new Vector2((screenWidth / 2 - signupBox.Width * scale / 2), (screenHeight / 2 - signupBox.Height * scale / 2));

            signupButton = content.Load<Texture2D>("Login/signup_button");
            signupButtonPosition = new Vector2(screenWidth * 3 / 4, screenHeight * 3 / 4);
            signupButtonCollisionBox = new Rectangle(((int)(signupButtonPosition.X)), ((int)(signupButtonPosition.Y)), (int)(signupButton.Width), (signupButton.Height));

            signupBoxUsernamePosition = new Vector2((screenWidth / 2 - signupBox.Width * scale / 4), (screenHeight / 2 - signupBox.Height * scale / 5));
            signupBoxPasswordPosition = new Vector2((screenWidth / 2 - signupBox.Width * scale / 4), (screenHeight / 2 + signupBox.Height * scale / 16));
            signupBoxPasswordConfirmPosition = new Vector2((screenWidth / 2 - signupBox.Width * scale / 4), (screenHeight / 2 + signupBox.Height * scale / 3));

            loginBox = content.Load<Texture2D>("Login/login_box");
            loginBoxPosition = new Vector2((screenWidth / 2 - loginBox.Width * scale / 2), (screenHeight / 2 - loginBox.Height * scale / 2));

            loginHighlighter = content.Load<Texture2D>("Login/login_highlighter");
            loginUsernameHighlighterPosition = new Vector2((screenWidth / 2 - (loginBox.Width * scale * 2 / 5)), (screenHeight / 2 - loginBox.Height * scale * 19 / 90));
            loginPasswordHighlighterPosition = new Vector2((screenWidth / 2 - (loginBox.Width * scale * 2 / 5)), (screenHeight / 2 + loginBox.Height * scale * 1 / 9));
            signupUsernameHighlighterPosition = new Vector2((screenWidth / 2 - (signupBox.Width * scale * 2 / 5)), (screenHeight / 2 - signupBox.Height * 18 / 90));
            signupPasswordHighlighterPosition = new Vector2((screenWidth / 2 - (signupBox.Width * scale * 2 / 5)), (screenHeight / 2 - signupBox.Height * 2 / 90));
            signupConfirmHighlighterPosition = new Vector2((screenWidth / 2 - (signupBox.Width * scale * 2 / 5)), (screenHeight / 2 + signupBox.Height * 31 / 180));

            font = content.Load<SpriteFont>("input_font");
            loginUsernameTextPosition = new Vector2((screenWidth / 2 - (loginBox.Width * scale * 28 / 90)), (screenHeight / 2 - loginBox.Height * scale * 12 / 90));
            loginPasswordTextPosition = new Vector2((screenWidth / 2 - (loginBox.Width * scale * 28 / 90)), (screenHeight / 2 + loginBox.Height * scale * 19 / 90));

            signupUsernameTextPosition = new Vector2((screenWidth / 2 - (loginBox.Width * scale * 28 / 90)), (screenHeight / 2 - loginBox.Height * scale * 25 / 90));
            signupPasswordTextPosition = new Vector2((screenWidth / 2 - (loginBox.Width * scale * 28 / 90)), (screenHeight / 2 + loginBox.Height * scale * 6 / 90));
            signupPasswordConfirmTextPosition = new Vector2((screenWidth / 2 - (loginBox.Width * scale * 28 / 90)), (screenHeight / 2 + loginBox.Height * scale * 38 / 90));

            usernameCollisionBox = new Rectangle((int)(loginUsernameHighlighterPosition.X), (int)(loginUsernameHighlighterPosition.Y), loginHighlighter.Width, loginHighlighter.Height);
            passwordCollisionBox = new Rectangle((int)(loginPasswordHighlighterPosition.X), (int)(loginPasswordHighlighterPosition.Y), loginHighlighter.Width, loginHighlighter.Height);

            signupUsernameCollisionBox = new Rectangle((int)(signupUsernameHighlighterPosition.X), (int)(signupUsernameHighlighterPosition.Y), loginHighlighter.Width, loginHighlighter.Height);
            signupPasswordCollisionBox = new Rectangle((int)(signupPasswordHighlighterPosition.X), (int)(signupPasswordHighlighterPosition.Y), loginHighlighter.Width, loginHighlighter.Height);
            passwordConfirmCollisionBox = new Rectangle((int)(signupConfirmHighlighterPosition.X), (int)(signupConfirmHighlighterPosition.Y), loginHighlighter.Width, loginHighlighter.Height);
        }

        public bool Update(GameTime gameTime)
        {
            //This first part is just pretty background things
            //always update the planets flying
            movingPlanetOnePosition.X += planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            movingPlanetOnePosition.Y += (float)Math.Sin(movingPlanetOnePosition.X / 1000) * scale;
            movingPlanetTwoPosition.X += planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            movingPlanetTwoPosition.Y += (float)Math.Sin(movingPlanetOnePosition.X / 1000) * scale;
            movingPlanetThreePosition.X += planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            movingPlanetThreePosition.Y += (float)Math.Sin(movingPlanetOnePosition.X / 1000) * scale;
            movingPlanetFourPosition.X += planetSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            movingPlanetFourPosition.Y += (float)Math.Sin(movingPlanetOnePosition.X / 1000) * scale;
            movingPlanetFivePosition.X += planetSpeed * (float)(1.3 * gameTime.ElapsedGameTime.TotalMilliseconds);
            movingPlanetFivePosition.Y += (float)Math.Sin(movingPlanetOnePosition.X / 500) * scale;

            shipOnePosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            shipTwoPosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            shipThreePosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            shipFourPosition.X += shipSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //get the current keyboard state 
            //(we'll use it later to see if it's different from the last one "clicked")
            currentKeyboardState = Keyboard.GetState();

            //checkClick() looks to see if any of the buttons are being clicked
            checkClick();

            //player enters into State.None
            if (state == State.None)
            {
                username = updateText(username);
                if (checkEnterAndTab())
                {
                    state = State.UsernameEntered;
                }
            }
            if (state == State.UsernameEntered)
            {
                password = updateText(password);
                if (checkEnterAndTab())
                {
                    if (password != "" && username != "")
                    {
                        state = State.PasswordEntered;
                    }
                }
            }
            if (state == State.PasswordEntered)
            {
                try
                {

                    Game1.globalUser = context.Load<User>(username);
                    if (Game1.globalUser == null)
                    {
                        state = State.LoginError;
                    }
                    else if (password != Game1.globalUser.password)
                    {
                        state = State.LoginError;
                    }
                    else
                    {
                        state = State.None;
                        return true;
                    }
                }
                catch
                {
                    state = State.InternetConnectionError;
                }
            }
            if (state == State.SignupButtonPressed)
            {
                username = updateText(username);
                if (checkEnterAndTab())
                {
                    state = State.UsernameCreated;
                }
            }
            if (state == State.UsernameCreated)
            {
                password = updateText(password);
                if (checkEnterAndTab() && password != "")
                {
                    state = State.PasswordCreated;
                }

            }
            if (state == State.PasswordCreated)
            {
                checkPassword = updateText(checkPassword);
                if (username != "" && password != "" && checkPassword != "" && checkEnterAndTab())
                {
                    state = State.CreateUser;
                }
            }
            if (state == State.CreateUser)
            {
                if (password.Equals(checkPassword))
                {
                    try
                    {
                        if (!checkUserExists(username))
                        {
                            Game1.globalUser = new User
                            {
                                username = username,
                                password = password,
                                skill = "0",
                            };

                            context.Save<User>(Game1.globalUser);
                            state = State.None;
                        }
                        else
                        {
                            state = State.CreationError;
                        }
                    }
                    catch
                    {
                        state = State.InternetConnectionError;
                    }
                }
                else
                {
                    state = State.PasswordMatchError;
                }
            }


            //below are a series of errors that could occur as a user is logging in
            //this section is only the timer for these errors
            //the first error addressed is if there is not a matching username and password
            if (state == State.LoginError || state == State.CreationError || state == State.PasswordMatchError || state == State.InternetConnectionError)
            {
                if (loginErrorCounter == 1000) //loginErrorCounter == 1000 is the trigger for "no current error"
                {
                    loginErrorCounter = -5000; //this controls how long the error message will stay
                }
                if (loginErrorCounter < 0)
                {
                    loginErrorCounter += gameTime.ElapsedGameTime.Milliseconds;
                }

                if (loginErrorCounter >= 0 || (loginErrorCounter >= -4000 && (previousKeyboardState != currentKeyboardState || previousMouseState != currentMouseState)))
                {
                    loginErrorCounter = 1000;
                    password = "";
                    checkPassword = "";
                    switch (state)
                    {
                        case State.LoginError:
                            state = State.None;
                            break;
                        case State.CreationError:
                            state = State.SignupButtonPressed;
                            break;
                        case State.PasswordMatchError:
                            state = State.SignupButtonPressed;
                            break;
                        case State.InternetConnectionError:
                            state = State.None;
                            break;
                    }
                }

            }

            previousKeyboardState = currentKeyboardState;

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //always draw the login background
            //remember to scale stuff when appropriate!

            spriteBatch.Draw(background, new Vector2(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetOne, movingPlanetOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetTwo, movingPlanetTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetThree, movingPlanetThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetFour, movingPlanetFourPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(planetFive, movingPlanetFivePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipOne, shipOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipTwo, shipTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipThree, shipThreePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(shipFour, shipFourPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            if (state == State.None || state == State.UsernameEntered)
            {
                spriteBatch.Draw(signupButton, signupButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(loginBox, loginBoxPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                if (currentMouseState.LeftButton != ButtonState.Pressed)
                {
                    if (state == State.UsernameEntered)
                    {
                        spriteBatch.Draw(loginHighlighter, loginPasswordHighlighterPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        spriteBatch.Draw(loginHighlighter, loginUsernameHighlighterPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                }
                if (username != "" && password != "")
                {
                    spriteBatch.Draw(goButton, goButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
                spriteBatch.DrawString(font, username, loginUsernameTextPosition, Color.SteelBlue, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, hidePassword(password), loginPasswordTextPosition, Color.SteelBlue, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (state == State.LoginError)
            {
                spriteBatch.Draw(loginError, loginErrorPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (state == State.SignupButtonPressed || state == State.UsernameCreated || state == State.PasswordCreated)
            {
                spriteBatch.Draw(backButton, backButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(signupBox, signupBoxPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                if (currentMouseState.LeftButton != ButtonState.Pressed)
                {
                    if (state == State.SignupButtonPressed)
                    {
                        spriteBatch.Draw(loginHighlighter, signupUsernameHighlighterPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                    else if (state == State.UsernameCreated)
                    {
                        spriteBatch.Draw(loginHighlighter, signupPasswordHighlighterPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        spriteBatch.Draw(loginHighlighter, signupConfirmHighlighterPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                }

                if (username != "" && password != "" && checkPassword != "")
                {
                    spriteBatch.Draw(goButton, goButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }

                spriteBatch.DrawString(font, username, signupUsernameTextPosition, Color.SteelBlue, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, hidePassword(password), signupPasswordTextPosition, Color.SteelBlue, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, hidePassword(checkPassword), signupPasswordConfirmTextPosition, Color.SteelBlue, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            if (state == State.CreationError)
            {
                spriteBatch.Draw(signupError, signupErrorPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (state == State.PasswordMatchError)
            {
                spriteBatch.Draw(passwordMatchError, passwordMatchErrorPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (state == State.InternetConnectionError)
            {
                spriteBatch.Draw(internetConnectionError, internetConnectionErrorPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        private void checkClick()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (rectangleClick(signupButtonCollisionBox))
            {
                state = State.SignupButtonPressed;
            }
            if (rectangleClick(backButtonCollisionBox))
            {
                state = State.None;
                password = "";
                checkPassword = "";
            }
            if (state == State.None || state == State.UsernameEntered)
            {
                if (rectangleClick(usernameCollisionBox))
                {
                    state = State.None;
                }
                else if (rectangleClick(passwordCollisionBox))
                {
                    state = State.UsernameEntered;
                }
                else if (rectangleClick(goButtonCollisionBox) && password != "" && username != "")
                {
                    state = State.PasswordEntered;
                }
            }
            if (state == State.SignupButtonPressed || state == State.UsernameCreated || state == State.PasswordCreated)
            {
                if (rectangleClick(signupUsernameCollisionBox))
                {
                    state = State.SignupButtonPressed;
                }
                else if (rectangleClick(signupPasswordCollisionBox))
                {
                    state = State.UsernameCreated;
                }
                else if (rectangleClick(passwordConfirmCollisionBox))
                {
                    state = State.PasswordCreated;
                }
                else if (rectangleClick(goButtonCollisionBox) && password != "" && username != "")
                {
                    state = State.CreateUser;
                }
            }
        }

        private bool rectangleClick(Rectangle rectangle)
        {

            Rectangle mousePosition = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released && mousePosition.Intersects(rectangle))
            {
                return true;
            }
            return false;
        }

        private string hidePassword(string password)
        {
            string hiddenPassword = "";

            for (int i = 0; i < password.Length; i++)
            {
                hiddenPassword += "*";
            }

            return hiddenPassword;
        }

        private bool checkUserExists(string username)
        {

            Game1.globalUser = context.Load<User>(username);
            if (Game1.globalUser == null)
            {
                return false;
            }

            return true;

        }
        private string updateText(string textString)
        {

            textString = textString + input.Update();

            if (currentKeyboardState.IsKeyDown(Keys.Back) && previousKeyboardState.IsKeyDown(Keys.Back) == false)
            {
                if (textString.Length > 0)
                {
                    textString = textString.Substring(0, textString.Length - 1);
                }
            }
            return textString;
        }
        private bool checkEnterAndTab()
        {
            if ((currentKeyboardState.IsKeyDown(Keys.Tab) && previousKeyboardState.IsKeyDown(Keys.Tab) == false) || (currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyDown(Keys.Enter) == false))
            {
                return true;
            }
            return false;
        }
    }
}
