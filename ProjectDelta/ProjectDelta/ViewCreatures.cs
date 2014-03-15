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
        private bool flipped;
        private float scale;
        private float xSpacing;
        private float totalPreviousEvolutionsWidth;
        private float widthSum;
        private int screenHeight;
        private int screenWidth;
        private int lifetimeAnswersCorrect;
        private int lifetimeMinutesPlayed;
        private int previousEvolutions;
        private int previousEvolutionsHolder;
        private int currentHoverCreature;
        private int previousEvolutionCreature;
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
            creatures = new World101Creature[163];

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
            largeWhiteBoardPosition = new Vector2(screenWidth / 2 - largeWhiteBoard.Width * scale / 2, screenHeight / 2 - largeWhiteBoard.Height * scale / 2);

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
                        creatures[i].setPosition(new Vector2(((150 + 200 * j) * scale - creatures[i].getWidth() / 3), ((125 + 200 * k) * scale - creatures[i].getHeight() / 3)));
                        creatures[i].setCollisionBox(new Rectangle(((int)(creatures[i].getPosition().X)), ((int)(creatures[i].getPosition().Y)), (int)(creatures[i].getWidth() * 2 / 3), (int)(creatures[i].getHeight() * 2 / 3)));
                        j++;
                        if (j > 8) { j = 0; k++; }
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
            if (worldStage - 1 >= 0)
            {
                for (int i = 0; i < worldStage; i++)
                {
                    if (creatures[i].getAvailability())
                    {
                        spriteBatch.Draw(creatures[i].getCreatureImage(), creatures[i].getPosition(), null, Color.White, 0f, Vector2.Zero, scale * 2 / 3, SpriteEffects.None, 0f);

                    }
                }
            }
            spriteBatch.Draw(backButton, backButtonPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            if (hover)
            {
                if (flipped)
                {
                    spriteBatch.Draw(textBubble, textBubblePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0f);
                }
                else
                {
                    spriteBatch.Draw(textBubble, textBubblePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
                spriteBatch.DrawString(font, creatureText, fontPosition, Color.SteelBlue, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                //draw previous evolutions of hover creature
                previousEvolutionsHolder = previousEvolutions;
                totalPreviousEvolutionsWidth = 0;

                while (previousEvolutions > 0)
                {
                    previousEvolutionCreature = currentHoverCreature - previousEvolutions;
                    totalPreviousEvolutionsWidth += creatures[previousEvolutionCreature].getWidth();
                    previousEvolutions -= 1;
                }

                previousEvolutions = previousEvolutionsHolder;
                previousEvolutionCreature = currentHoverCreature - previousEvolutions;
                widthSum = creatures[previousEvolutionCreature].getWidth();
                while (previousEvolutions > 0)
                {
                    xSpacing = 225 * scale + (575 - creatures[currentHoverCreature - 1].getWidth() * 2 / 3) * widthSum / totalPreviousEvolutionsWidth * scale;
                    previousEvolutionCreature = currentHoverCreature - previousEvolutions;
                    spriteBatch.Draw(creatures[previousEvolutionCreature].getCreatureImage(), new Vector2(fontPosition.X + xSpacing, fontPosition.Y + textBubble.Height * scale /3 - creatures[previousEvolutionCreature].getHeight() / 3), null, Color.White, 0f, Vector2.Zero, scale * 2 / 3, SpriteEffects.None, 0f);
                    widthSum += creatures[previousEvolutionCreature + 1].getWidth();
                    previousEvolutions -= 1;
                }
                previousEvolutions = previousEvolutionsHolder;
            }
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
                        previousEvolutions = 1;
                        while (i - previousEvolutions >= 0 && creatures[i].getCreatureName() == creatures[i - previousEvolutions].getCreatureName())
                        {
                            previousEvolutions += 1;
                        }
                        previousEvolutions -= 1;
                        textBubblePosition = new Vector2(creatures[i].getCollisionBox().X + creatures[i].getWidth() / 2, creatures[i].getCollisionBox().Y - textBubble.Height * scale / 3);
                        if (textBubblePosition.Y < 0)
                        {
                            textBubblePosition.Y = 0;
                        }
                        fontPosition = new Vector2(textBubblePosition.X + 50 * scale, textBubblePosition.Y + 10 * scale);

                        if (textBubblePosition.X > screenWidth * 53 / 100)
                        {
                            flipped = true;
                            textBubblePosition.X = creatures[i].getCollisionBox().X - textBubble.Width * scale;
                            fontPosition = new Vector2(textBubblePosition.X + 15 * scale, textBubblePosition.Y + 10 * scale);
                        }
                        else
                        {
                            flipped = false;
                        }

                        creatureText = "Name: " + creatures[i].getCreatureName() + "\n" +
                            "Type: " + creatures[i].getCreatureType() + "\n" +
                            "Level: " + creatures[i].getCreatureLevel() + "\n" +
                            "Power Ups: " + creatures[i].getPowerUpsRemaining() + "\n" +
                            "Description:\n" + creatures[i].getCreatureDescription();
                        hover = true;
                        currentHoverCreature = i;
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
