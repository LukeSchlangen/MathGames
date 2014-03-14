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
        private int timer;

        private HomeText text;

        //Mouse states
        private MouseState current;
        private MouseState previous;

        //Textures
        private Texture2D background;
        private Texture2D world101Box;
        private Texture2D world101Button;
        private Texture2D logoutButton;
        private Texture2D statsButton;
        private Texture2D creature;
        private Texture2D spaceshipPointer;
        private Texture2D highlighter;


        //Vectors
        private Vector2 world101BoxPosition;
        private Vector2 world101ButtonPosition;
        private Vector2 logoutButtonPosition;
        private Vector2 statsButtonPosition;
        private Vector2 creaturePosition;
        private Vector2 spaceShipPosition;

        //Collision Boxes
        private Rectangle world101ButtonCollisionBox;
        private Rectangle logoutButtonCollisionBox;
        private Rectangle statsButtonCollisionBox;
        private Rectangle creatureCollisionBox;

        public void Initialize(float scale)
        {
            this.scale = scale;
            text = new HomeText(scale);
        }

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            timer = 0;
            text.LoadContent(content, screenHeight, screenWidth);
            background = content.Load<Texture2D>("Login/login_background");
            world101Box = content.Load<Texture2D>("Home/menu_box");
            world101Button = content.Load<Texture2D>("Home/start_next_level_button");
            logoutButton = content.Load<Texture2D>("Home/log_out_button");
            statsButton = content.Load<Texture2D>("Home/stats_button");
            spaceshipPointer = content.Load<Texture2D>("General/Ships/good_ship_1");
            highlighter = content.Load<Texture2D>("Login/login_highlighter");
            world101BoxPosition = new Vector2((screenWidth / 2 - world101Box.Width * scale / 2), (screenHeight / 2 - world101Box.Height * scale / 2));
            world101ButtonPosition = new Vector2((screenWidth / 2 - world101Button.Width * scale / 2), (screenHeight / 2 + (125 * scale)));
            logoutButtonPosition = new Vector2(screenWidth / 8, screenHeight * 3 / 4);
            statsButtonPosition = new Vector2(3 * screenWidth / 4, screenHeight * 3 / 4);
            spaceShipPosition = new Vector2(-screenWidth / 4, world101ButtonPosition.Y);
            world101ButtonCollisionBox = new Rectangle(((int)(world101ButtonPosition.X)), ((int)(world101ButtonPosition.Y)), (int)(world101Button.Width * scale), (int)(world101Button.Height * scale));
            logoutButtonCollisionBox = new Rectangle(((int)(logoutButtonPosition.X)), ((int)(logoutButtonPosition.Y)), (int)(logoutButton.Width * scale), (int)(logoutButton.Height * scale));
            statsButtonCollisionBox = new Rectangle(((int)(statsButtonPosition.X)), ((int)(statsButtonPosition.Y)), (int)(statsButton.Width * scale), (int)(statsButton.Height * scale));

            //NOTE: This will cause an exception if using an account that is higher than level 7, until all the creature images are added...
            if (Game1.globalUser.world101 > 0)
            {
                creature = content.Load<Texture2D>("Creatures/wild_creature_" + (Game1.globalUser.currentFriendlyCreature));
                creaturePosition = new Vector2((screenWidth / 2 - creature.Width * scale / 2), (screenHeight / 2 - creature.Height * scale / 2));
                creatureCollisionBox = new Rectangle(((int)(world101BoxPosition.X)), ((int)(world101BoxPosition.Y)), ((int)(world101ButtonPosition.X)), ((int)(world101ButtonPosition.Y)));
            }


        }

        public int Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.Milliseconds;

            if (spaceShipPosition.X < world101ButtonPosition.X - 200 * scale)
            {
                spaceShipPosition.X += gameTime.ElapsedGameTime.Milliseconds * scale / 2;
            }
            return checkClick();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(world101Box, world101BoxPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(world101Button, world101ButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(logoutButton, logoutButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            if (Game1.globalUser.world101 > 0)
            {
                spriteBatch.Draw(statsButton, statsButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(creature, creaturePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (spaceShipPosition.X > world101ButtonPosition.X - 250 * scale)
            {
                spriteBatch.Draw(highlighter, new Vector2(world101ButtonPosition.X - 17 * scale, world101ButtonPosition.Y - 18 * scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(spaceshipPointer, spaceShipPosition, null, Color.White, 0f, Vector2.Zero, scale / 2, SpriteEffects.None, 0f);
            text.Draw(spriteBatch);

        }

        private int checkClick()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(world101ButtonCollisionBox) || (timer > 1000 && ((Keyboard.GetState().IsKeyDown(Keys.Space)) || Keyboard.GetState().IsKeyDown(Keys.Enter))))
            {
                return 101;
            }

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(logoutButtonCollisionBox))
            {
                return -1;
            }

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(statsButtonCollisionBox))
            {
                return -2;
            }

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(creatureCollisionBox))
            {
                return -3;
            }

            return 0;
        }
    }
}
