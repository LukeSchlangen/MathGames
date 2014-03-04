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
    class ViewCreatures
    {
        private QuestionFormat question = new QuestionFormat();

        private DynamoDBContext context;
        private MouseState current;
        private MouseState previous;

        private SpriteFont font;

        private float scale;
        private int screenHeight;
        private int screenWidth;

        private Texture2D background;
        private Texture2D backButton;
        private Texture2D largeWhiteBoard;
        private Texture2D[] creature = new Texture2D[157];

        private Vector2 backButtonPosition;
        private Vector2 largeWhiteBoardPosition;
        private Vector2[] creaturePosition = new Vector2[157];

        private Rectangle backButtonCollisionBox;
        private Rectangle[] creatureCollisionBox = new Rectangle[157];

        public ViewCreatures(DynamoDBContext context, float scale)
        {
            this.context = context;
            this.scale = scale;
        }

        public void LoadContent(ContentManager content, int worldStage, int screenX, int screenY)
        {
            screenHeight = screenY;
            screenWidth = screenX;

            background = content.Load<Texture2D>("Login/login_background");
            backButton = content.Load<Texture2D>("Login/back_button");
            largeWhiteBoard = content.Load<Texture2D>("Home/large_white_board");
            backButtonPosition = new Vector2(screenWidth / 9, screenHeight * 4 / 5);
            backButtonCollisionBox = new Rectangle((int)(backButtonPosition.X), (int)(backButtonPosition.Y), (int)(backButton.Width * scale), (int)(backButton.Height * scale));
            largeWhiteBoardPosition = new Vector2((screenWidth / 2 - largeWhiteBoard.Width * scale / 2), (screenHeight / 2 - largeWhiteBoard.Height * scale / 2));

            if (Game1.globalUser.world101 - 1 >= 0)
            {

                int j = 1;
                int k = 1;
                for (int i = 0; i < creature.Length; i++)
                {

                    //simply put the creatures in the numerical order that you want them to appear
                    //following the naming convention wild_creature_i.png in the creatures directory
                    //for final version replace j with i
                    Texture2D wildCreatureToLoad = content.Load<Texture2D>("Creatures/wild_creature_" + i); //load the creatures in order so that the creature that appears corresponds with the world stage
                    creature[i] = wildCreatureToLoad;

                    creaturePosition[i] = new Vector2((150 * j * scale), (150 * k * scale));

                    creatureCollisionBox[i] = new Rectangle(((int)(creaturePosition[i].X)), ((int)(creaturePosition[i].Y)), ((int)(creature[i].Width)), ((int)(creature[i].Height)));
                    j++;
                    if (j > 10) { j = 1; k++; }
                }

            }
        }

        public bool Update(GameTime gameTime)
        {
            return checkBack();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(largeWhiteBoard, largeWhiteBoardPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backButton, backButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            if (Game1.globalUser.world101 - 1 >= 0)
            {


                for (int i = 0; i < creature.Length; i++)
                {
                    spriteBatch.Draw(creature[i], creaturePosition[i], null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                }
            }
        }

        private bool checkBack()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(backButtonCollisionBox))
            {
                return true;
            }

            return false;
        }
    }
}
