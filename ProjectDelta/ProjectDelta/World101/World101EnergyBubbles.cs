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
        Vector2[] bubblePosition;
        private Vector2 bubbleStartingPosition;
        private Vector2 bubbleEndingPosition;
        private int energyBubblesWaiting = 0;
        private int energyBubblesInMotion = 0;
        private float scale;

        public World101EnergyBubbles(Vector2 bubbleStartingPosition, Vector2 bubbleCollectorPosition, float scale)
        {
            this.scale = scale;
            this.bubblePosition = new Vector2[1000];
            this.bubbleStartingPosition = bubbleStartingPosition;
            this.bubbleEndingPosition = bubbleCollectorPosition;
            for (int i = 0; i < bubblePosition.Length; i++)
            {
                bubblePosition[i] = bubbleEndingPosition;
            }
        }

        public void LoadContent(ContentManager content)
        {
            energyBubble = content.Load<Texture2D>("General/energy_bubble");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < bubblePosition.Length; i++)
            {
                if (bubblePosition[i].Y > bubbleEndingPosition.Y )
                {
                    spriteBatch.Draw(energyBubble, bubblePosition[i], null, Color.White, 0f, Vector2.Zero, scale / (i%3 +3), SpriteEffects.None, 0f);
                }
            }
        }

        public void Update(GameTime gameTime, Vector2 heroPosition)
        {
            bubbleStartingPosition = new Vector2(heroPosition.X + 200 * scale, heroPosition.Y -150 * scale);
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

            
            if (energyBubblesInMotion < bubblePosition.Length)
            {
                for (int i = 0; i < bubblePosition.Length; i++)
                {
                    if (bubblePosition[i].Y <= bubbleEndingPosition.Y)
                    {
                        if (energyBubblesWaiting > 0)
                        {
                            bubblePosition[i] = bubbleStartingPosition;
                            energyBubblesWaiting -= 1;
                            i = bubblePosition.Length;
                        }
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
        }

        public int energyBubblesInTransit()
        {
            return energyBubblesInMotion + energyBubblesWaiting;
        }
    }
}
