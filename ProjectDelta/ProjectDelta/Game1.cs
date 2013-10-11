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
        public static User globalUser = null;

        public enum State
        {
            //add any relevant game states here
            Login,
            Home,
            World101,
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
        private bool success;
        
        private Login login;
        private Home home;
        private World101 world101;

        //ContentManagers: One manager for each set of 
        //content (worlds, login, home, etc)
        ContentManager loginContentManager;
        ContentManager homeContentManager;
        ContentManager world101ContentManager;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            loginContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            homeContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            world101ContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);

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
            home = new Home();
            world101 = new World101(context);

            //when we initialize the login screen (and any screens
            //from here on out), we pass in the scale value to allow
            //us to scale the textures
            login.Initialize(scale);
            home.Initialize(scale);
            world101.Initialize(scale);

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
                success = login.Update(gameTime);
                if (success == true)
                {
                    state = State.Home;
                    loginContentManager.Unload();
                    home.LoadContent(homeContentManager);
                    success = false;
                }
            }

            if (state == State.Home)
            {
                int whereTo = home.Update(gameTime);

                if (whereTo == 101)
                {
                    state = State.World101;
                    homeContentManager.Unload();
                    world101.LoadContent(world101ContentManager);
                    success = false;
                    whereTo = 0;
                    home.resetUpdate();
                }               
            }

            if (state == State.World101)
            {
                bool success = world101.Update(gameTime);
                if (success == true)
                {
                    state = State.Home;
                    world101ContentManager.Unload();
                    home.LoadContent(homeContentManager);
                    success = false;
                    world101.resetWorld();
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
                home.Draw(spriteBatch);      
            }

            if (state == State.World101)
            {
                world101.Draw(spriteBatch);
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
