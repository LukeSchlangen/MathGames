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
        public static bool abort = false;

        public enum State
        {
            //add any relevant game states here
            Splash,
            Login,
            Home,
            Story,
            World101,
            Stats,
            ViewCreatures,
            Exit,
        }

        private static int COUNT_TO_CONTINUE = 2;

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

        private Splash splash;
        private Login login;
        private Story story;
        private Home home;
        private World101 world101;
        private Stats stats;
        private ViewCreatures viewCreatures;

        //ContentManagers: One manager for each set of 
        //content (worlds, login, home, etc)
        ContentManager splashContentManager;
        ContentManager loginContentManager;
        ContentManager storyContentManager;
        ContentManager homeContentManager;
        ContentManager world101ContentManager;
        ContentManager statsContentManager;
        ContentManager viewCreaturesContentManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            splashContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            loginContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            storyContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            homeContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            world101ContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            statsContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            viewCreaturesContentManager = new ContentManager(Content.ServiceProvider, Content.RootDirectory);

            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //For Tom debug
            screenWidth = 1366;
            screenHeight = 768;

            //this specifies the actual resolution that the game displays at
            //we want to leave this at the natural screen resolution
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;

            //set the scale factor
            scale = (float)screenHeight / 1080;

            //Initializes the game in full screen
            //graphics.IsFullScreen = true;

            this.IsFixedTimeStep = false;
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

            //How to encrpyt an unencrypted password, only run uncommented one time
            //GameUtils.databaseEncryptionHelper("usernameOfPasswordNeedingEncryption", context);

            //for debug can edit this to go to desired state
            //default is State.Login
            state = State.Splash;
            //state = State.Level1;

            splash = new Splash(screenWidth, screenHeight, scale);
            login = new Login(context);
            story = new Story(screenWidth, screenHeight, scale);
            home = new Home();
            world101 = new World101(context);
            stats = new Stats(context, scale);
            viewCreatures = new ViewCreatures(context, scale);

            //when we initialize the login screen (and any screens
            //from here on out), we pass in the scale value to allow
            //us to scale the textures
            login.Initialize(scale);
            home.Initialize(scale);
            world101.Initialize(scale, screenWidth);
            story.Initialize(scale, screenWidth);

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

            splash.LoadContent(splashContentManager);
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

            if (state == State.Splash)
            {
                success = splash.Update(gameTime);
                if (success == true)
                {
                    state = State.Login;
                    splashContentManager.Unload();
                    login.LoadContent(loginContentManager, screenHeight, screenWidth);
                    success = false;
                }
            }

            if (state == State.Login)
            {
                success = login.Update(gameTime);
                if (success == true)
                {
                    if (Game1.globalUser.world101 != 0)
                    {
                        state = State.Home;
                        loginContentManager.Unload();
                        home.LoadContent(homeContentManager, screenHeight, screenWidth);
                        success = false;
                    }
                    else
                    {
                        state = State.Story;
                        loginContentManager.Unload();
                        story.LoadContent(storyContentManager, screenHeight, screenWidth);
                        success = false;
                    }
                }
            }

            if (state == State.Story)
            {
                success = story.Update(gameTime);
                if (success == true)
                {
                    state = State.Home;
                    storyContentManager.Unload();
                    home.LoadContent(loginContentManager, screenHeight, screenWidth);
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
                    int difficulty = globalUser.world101;

                    world101.LoadContent(world101ContentManager, globalUser.world101, COUNT_TO_CONTINUE);

                    success = false;
                    whereTo = 0;
                }

                if (whereTo == -1)
                {
                    state = State.Login;
                    homeContentManager.Unload();
                    globalUser = null;
                    login.LoadContent(loginContentManager, screenHeight, screenWidth);
                    success = false;
                    whereTo = 0;
                }

                if (whereTo == -2)
                {
                    state = State.Stats;
                    homeContentManager.Unload();
                    stats.LoadContent(statsContentManager, globalUser.world101, COUNT_TO_CONTINUE, screenWidth, screenHeight);
                    success = false;
                    whereTo = 0;
                }

                if (whereTo == -3)
                {
                    state = State.ViewCreatures;
                    homeContentManager.Unload();
                    viewCreatures.LoadContent(viewCreaturesContentManager, globalUser.world101, screenWidth, screenHeight);
                    success = false;
                    whereTo = 0;
                }
            }

            if (state == State.Stats)
            {
                success = stats.Update(gameTime);
                if (success == true)
                {
                    state = State.Home;
                    statsContentManager.Unload();
                    home.LoadContent(homeContentManager, screenHeight, screenWidth);
                    success = false;
                }
            }

            if (state == State.ViewCreatures)
            {
                success = viewCreatures.Update(gameTime);
                if (success == true)
                {
                    state = State.Home;
                    statsContentManager.Unload();
                    home.LoadContent(homeContentManager, screenHeight, screenWidth);
                    success = false;
                }
            }


            if (state == State.World101)
            {
                bool success = world101.Update(gameTime);
                if (success == true)
                {
                    state = State.Home;
                    world101ContentManager.Unload();
                    home.LoadContent(homeContentManager, screenHeight, screenWidth);
                    success = false;
                    world101.resetStage();
                }
            }

            if (abort)
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
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            if (state == State.Splash)
            {
                splash.Draw(spriteBatch);
            }

            if (state == State.Login)
            {
                login.Draw(spriteBatch);
            }

            if (state == State.Story)
            {
                story.Draw(spriteBatch);
            }

            if (state == State.Home)
            {
                home.Draw(spriteBatch);
            }

            if (state == State.World101)
            {
                world101.Draw(spriteBatch);
            }

            if (state == State.Stats)
            {
                stats.Draw(spriteBatch);
            }

            if (state == State.ViewCreatures)
            {
                viewCreatures.Draw(spriteBatch);
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
