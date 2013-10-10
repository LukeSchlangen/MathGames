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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public enum State
        {
            //add any relevant game states here
            Login,
            World1,
            Home,
            Exit,
        }

        private int screenWidth;
        private int screenHeight;

        //holds the scale ratio of the screen compared
        //to the max resolution available for gameplay (1920x1080)
        private float scale;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //The database context. Use this to
        //utilize database functionality
        private DynamoDBContext context;

        public State state;
        
        private Login login;
        private World1 world1;

        ContentManager loginContentManager;
        ContentManager level1ContentManager;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            loginContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            level1ContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);

            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //For Tom debug
            //screenWidth = 1366;
            //screenHeight = 768;

            //this specifies the actual resolution that the game displays at
            //we want to leave this at the natural screen resolution
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;

            //set the scale factor
            scale = (float) screenHeight / 1080;
            
            //Initializes the game in full screen
            //graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            connectToDatabase();

            //for debug can edit this to go to desired state
            //default is State.Login
            state = State.Login;
            //state = State.Level1;

            login = new Login(context);
            world1 = new World1(context);

            //when we initialize the login screen (and any screens
            //from here on out), we pass in the scale value to allow
            //us to scale the textures
            login.Initialize(scale);
            world1.Initialize(scale);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            login.LoadContent(loginContentManager, screenHeight, screenWidth);            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //General Architecture:
            //Use the state variable to track where in the game
            //the user is and what needs to be updated
            
            if (state == State.Login)
            {
                bool success = login.Update(gameTime);
                if (success == true)
                {
                    state = State.World1;
                    loginContentManager.Unload();
                    world1.LoadContent(Content);
                }
            }

            if (state == State.Home)
            {
                world1.LoadContent(Content);
                state = State.World1;
            }

            if (state == State.World1)
            {
                bool success = world1.Update(gameTime);
                if (success == true)
                {
                    state = State.Home;
                }
            }

            if (state == State.Exit)
            {
                this.Exit();
            }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (state == State.Login)
            {
                login.Draw(spriteBatch);
            }
            
            if (state == State.Home)
            {
                //home screen code
                login = null;   
            }

            if (state == State.World1)
            {
                world1.Draw(spriteBatch);
            }

            if (state == State.Exit)
            {
                this.Exit();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void connectToDatabase()
        {
            var config = new AmazonDynamoDBConfig();
            config.ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"];
            AmazonDynamoDB client = new AmazonDynamoDBClient(config);
            context = new DynamoDBContext(client);
        }
    }
}
