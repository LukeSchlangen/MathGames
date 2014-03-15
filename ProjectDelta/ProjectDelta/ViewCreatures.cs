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
        private bool hover;
        private float scale;
        private int screenHeight;
        private int screenWidth;
        private int lifetimeAnswersCorrect;
        private int lifetimeMinutesPlayed;
        public int worldStage;

        private string creatureText;

        private Texture2D background;
        private Texture2D backButton;
        private Texture2D largeWhiteBoard;
        private Texture2D textBubble;
        private World101Creature[] creatures;

        private Vector2 backButtonPosition;
        private Vector2 largeWhiteBoardPosition;
        private Vector2 textBubblePosition;
        private Vector2 fontPosition;

        private Rectangle backButtonCollisionBox;

        public ViewCreatures(DynamoDBContext context, float scale)
        {
            this.context = context;
            this.scale = scale;
        }

        public void LoadContent(ContentManager content, int worldStage, int screenX, int screenY)
        {
            this.worldStage = worldStage;
            creatures = new World101Creature[worldStage];

            font = content.Load<SpriteFont>("input_font");

            screenHeight = screenY;
            screenWidth = screenX;

            lifetimeAnswersCorrect = Game1.globalUser.answersCorrect;
            lifetimeMinutesPlayed = Game1.globalUser.timePlayed / 60000;

            background = content.Load<Texture2D>("Login/login_background");
            backButton = content.Load<Texture2D>("Login/back_button");
            largeWhiteBoard = content.Load<Texture2D>("Home/large_white_board");
            textBubble = content.Load<Texture2D>("Home/text_bubble");
            backButtonPosition = new Vector2(screenWidth / 9, screenHeight * 4 / 5);
            backButtonCollisionBox = new Rectangle((int)(backButtonPosition.X), (int)(backButtonPosition.Y), (int)(backButton.Width * scale), (int)(backButton.Height * scale));
            largeWhiteBoardPosition = new Vector2((screenWidth / 2 - largeWhiteBoard.Width * scale / 2), (screenHeight / 2 - largeWhiteBoard.Height * scale / 2));

            if (worldStage - 1 >= 0)
            {

                for (int i = 0; i < creatures.Length; i++)
                {
                    creatures[i] = new World101Creature(i, worldStage, lifetimeAnswersCorrect, lifetimeMinutesPlayed, scale, 0);
                    creatures[i].LoadContent(content);
                }

                int j = 0;
                int k = 0;
                for (int i = 0; i < creatures.Length; i++)
                {
                    if (creatures[i].getAvailability())
                    {
                        creatures[i].setPosition(new Vector2(((100 + 300 * j) * scale), ((100 + 300 * k) * scale)));
                        creatures[i].setCollisionBox(new Rectangle(((int)(creatures[i].getPosition().X)), ((int)(creatures[i].getPosition().Y)), ((int)(creatures[i].getWidth())), ((int)(creatures[i].getHeight()))));
                        j++;
                        if (j > 5) { j = 0; k++; }
                    }
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
            if (Game1.globalUser.world101 - 1 >= 0)
            {



                for (int i = 0; i < worldStage; i++)
                {
                    if (creatures[i].getAvailability())
                    {
                        creatures[i].Draw(spriteBatch);
                    }
                }
            }
            if (hover)
            {
                spriteBatch.Draw(textBubble, textBubblePosition, null, Color.White, 0f, Vector2.Zero, scale * 2, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, creatureText, fontPosition, Color.SteelBlue, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(backButton, backButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private bool checkBack()
        {
            previous = current;
            current = Mouse.GetState();
            Rectangle mousePosition = new Rectangle(current.X, current.Y, 1, 1);
            hover = false;
            for (int i = 0; i < creatures.Length; i++)
            {
                if (creatures[i].getAvailability())
                {

                    if (mousePosition.Intersects(creatures[i].getCollisionBox()))
                    {
                        textBubblePosition = new Vector2(creatures[i].getCollisionBox().X + 150 * scale, creatures[i].getCollisionBox().Y);
                        creatureText = "Name: " + creatures[i].getCreatureName() + "\n" +
                            "Type: " + creatures[i].getCreatureType() + "\n" +
                            "Level: " + creatures[i].getCreatureLevel() + "\n" +
                            "Description: " + creatures[i].getCreatureDescription();
                        fontPosition = new Vector2(textBubblePosition.X + 130 * scale, textBubblePosition.Y + 100 * scale);
                        hover = true;
                    }
                }
            }

            if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && mousePosition.Intersects(backButtonCollisionBox))
            {
                return true;
            }
            else if (current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released)
            {
                for (int i = 0; i < creatures.Length; i++)
                {
                    if (mousePosition.Intersects(creatures[i].getCollisionBox()) && creatures[i].getAvailability())
                    {
                        try
                        {
                            Game1.globalUser.currentFriendlyCreature = i;
                            context.Save<User>(Game1.globalUser);
                            return true;
                        }
                        catch
                        {
                            //internetConnection = false;
                        }
                    }
                }
            }
            return false;
        }
    }
}
