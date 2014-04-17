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

namespace ProjectDelta
{
    class World101EnergyBubbles
    {
        private Texture2D energyBubble;
        private Vector2[] bubblePosition;
        private Vector2[] gunBubblesPosition;
        private Vector2[] bubblesInStorageTankPosition;
        private Vector2 bubbleStartingPosition;
        private Vector2 bubbleEndingPosition;
        private int energyBubblesWaiting = 0;
        private int energyBubblesInMotion = 0;
        private float scale;

        public World101EnergyBubbles(Vector2 bubbleStartingPosition, Vector2 bubbleCollectorPosition, float scale)
        {
            this.scale = scale;
            this.bubblePosition = new Vector2[250];
            this.gunBubblesPosition = new Vector2[80];
            this.bubblesInStorageTankPosition = new Vector2[200];
            this.bubbleStartingPosition = bubbleStartingPosition;
            this.bubbleEndingPosition = bubbleCollectorPosition;
            for (int i = 0; i < bubblePosition.Length; i++)
            {
                bubblePosition[i] = bubbleEndingPosition;
            }
            for (int i = 0; i < gunBubblesPosition.Length; i++)
            {
                gunBubblesPosition[i] = new Vector2(1000 * scale + (float)i / (float)gunBubblesPosition.Length * (1000 + 2000) / 2, bubbleStartingPosition.Y);
            }

            for (int i = 0; i < bubblesInStorageTankPosition.Length; i++)
            {
                bubblesInStorageTankPosition[i] = bubbleEndingPosition;

                bubblesInStorageTankPosition[i].X -= (float)i * scale;  
                bubblesInStorageTankPosition[i].Y += 200 * (float)Math.Sin((float)i / 15) * scale;
            }

        }

        public void LoadContent(ContentManager content)
        {
            energyBubble = content.Load<Texture2D>("General/energy_bubble");
        }

        public void Draw(SpriteBatch spriteBatch, int energyBubblesInStorageTank)
        {
            for (int i = 0; i < bubblePosition.Length; i++)
            {
                if (bubblePosition[i].Y > bubbleEndingPosition.Y)
                {
                    spriteBatch.Draw(energyBubble, bubblePosition[i], null, Color.White, 0f, Vector2.Zero, scale / (i % 3 + 3), SpriteEffects.None, 0f);
                }
            }

            DrawBubblesInStorageTank(spriteBatch, energyBubblesInStorageTank);
        }

        public void DrawBubbleGun(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < gunBubblesPosition.Length; i++)
            {
                if (gunBubblesPosition[i].X < 2000 * scale)
                {
                    spriteBatch.Draw(energyBubble, gunBubblesPosition[i], null, Color.White, 0f, Vector2.Zero, scale / (i % 3 + 3), SpriteEffects.None, 0f);
                }
            }
        }

        public void DrawBubblesInStorageTank(SpriteBatch spriteBatch, int numberOfBubbles)
        {
            for (int i = 0; i < numberOfBubbles/5; i++)
            {
                if (bubblesInStorageTankPosition[i].Y > bubbleEndingPosition.Y)
                {
                    spriteBatch.Draw(energyBubble, bubblesInStorageTankPosition[i], null, Color.White, 0f, Vector2.Zero, scale / (i % 3 + 3), SpriteEffects.None, 0f);
                }
            }
        }

        public void Update(GameTime gameTime, Vector2 heroPosition)
        {
            bubbleStartingPosition = new Vector2(heroPosition.X + 200 * scale, heroPosition.Y - 150 * scale);
            energyBubblesInMotion = 0;

            for (int i = 0; i < bubblePosition.Length; i++)
            {
                if (bubblePosition[i].Y > bubbleEndingPosition.Y)
                {
                    bubblePosition[i].X += (bubbleEndingPosition.X - bubbleStartingPosition.X) / 100;
                    bubblePosition[i].Y += (bubbleEndingPosition.Y - bubbleStartingPosition.Y) / 100;
                    bubblePosition[i].Y += 10 * (float)Math.Sin(bubblePosition[i].X / 15) * scale;
                    energyBubblesInMotion += 1;
                }
            }

            for (int i = 0; i < bubblesInStorageTankPosition.Length; i++)
            {
                if (bubblesInStorageTankPosition[i].Y > 300 * scale || bubblesInStorageTankPosition[i].X < 500 * scale || bubblesInStorageTankPosition[i].X > bubbleEndingPosition.X)
                {
                    bubblesInStorageTankPosition[i] = bubbleEndingPosition;
                }
                else
                {
                    float xDirection = (float)(i - 100) / 100;
                    float yDirection = (float)Math.Sqrt(1 - xDirection * xDirection);
                    bubblesInStorageTankPosition[i].Y += yDirection;
                    bubblesInStorageTankPosition[i].X += (i%9+1)*xDirection -2 * scale;
                }
            }

            for (int i = 0; i < gunBubblesPosition.Length; i++)
            {
                if (gunBubblesPosition[i].X < 2000 * scale)
                {
                    gunBubblesPosition[i].X += (float)(i + gunBubblesPosition.Length) / (float)gunBubblesPosition.Length * (2000 * scale - bubbleStartingPosition.X) / 100;
                    if (i % 3 == 0)
                    {
                        gunBubblesPosition[i].Y = 900 * scale + 10 * (float)Math.Sin(gunBubblesPosition[i].X / 90 + 10) * scale;
                    }
                    else if (i % 3 == 1)
                    {
                        gunBubblesPosition[i].Y = 900 * scale - 20 * (float)Math.Sin(gunBubblesPosition[i].X / 70 + 20) * scale;
                    }
                    else
                    {
                        gunBubblesPosition[i].Y = 900 * scale - 30 * (float)Math.Sin(gunBubblesPosition[i].X / 50) * scale;
                    }
                }
                else
                {
                    gunBubblesPosition[i] = new Vector2(bubbleStartingPosition.X + 125 * scale, bubbleStartingPosition.Y + 30 * scale);
                }
            }




            if (energyBubblesInMotion < bubblePosition.Length)
            {
                for (int i = 0; i < bubblePosition.Length; i++)
                {
                    if (bubblePosition[i].Y <= bubbleEndingPosition.Y && energyBubblesWaiting > 0)
                    {
                        bubblePosition[i] = bubbleStartingPosition;
                        energyBubblesWaiting -= 1;
                        i = bubblePosition.Length;
                    }
                }
            }
        }

        public void newBubbles(int newEnergyBubbles)
        {
            energyBubblesWaiting += newEnergyBubbles;
        }

        public void reset()
        {
            energyBubblesWaiting = 0;
            for (int i = 0; i < bubblePosition.Length; i++)
            {
                bubblePosition[i] = bubbleEndingPosition;
            }
            for (int i = 0; i < gunBubblesPosition.Length; i++)
            {
                gunBubblesPosition[i] = new Vector2(1000 * scale + (float)i / (float)gunBubblesPosition.Length * (1000 + 2000) / 2, bubbleStartingPosition.Y);
            }
        }

        public int energyBubblesInTransit()
        {
            return energyBubblesInMotion + energyBubblesWaiting;
        }

    }
}
