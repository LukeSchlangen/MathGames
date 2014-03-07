using System;
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

namespace ProjectDelta
{
    class Story
    {
        public enum State
        {
            happyCreatures, //from beginning to bad guys landing
            badGuysLanding, //bad guys land
            badGuysCapturingCreatures, //bad guys herd baby creatures into space ship
            badGuysLeaving, //bad guys leave with creatures
            heroChases, //hero runs accross screen to rocket
            heroLeaving, //hero ship takes off
        }

        public State state;

        private int screenX;
        private int screenY;
        private float scale;
        private float speed = .1f;
        private int storyCounter = 1000;
        private float constantlyIncreasingNumber;

        private Texture2D background;

        private Texture2D[] babyCreature = new Texture2D[10];

        private Texture2D[] monster = new Texture2D[2];

        private Texture2D largeEnemyShipEnginesOff;
        private Texture2D largeEnemyShipEnginesOn;

        private Texture2D smallEnemyShipEnginesOff;
        private Texture2D smallEnemyShipEnginesOn;

        private Texture2D[] storyScript = new Texture2D[5];


        private Texture2D heroShipEnginesOn;
        private Texture2D heroShipEnginesOff;

        private Texture2D hero;

        private Texture2D smokeCloud;


        //set baby creatures position
        private Vector2[] babyCreaturePosition = new Vector2[10];

        private Vector2[] monsterPosition = new Vector2[10];

        private Vector2[] smokeCloudPosition = new Vector2[10];

        private Vector2 largeEnemyShipPosition;
        private Vector2 smallEnemyShipOnePosition;
        private Vector2 smallEnemyShipTwoPosition;

        private Vector2 heroShipPosition;

        private Vector2 heroPosition;

        private Vector2 backgroundPosition;
        private Vector2 storyScriptPosition;
        private Animation heroAnimation;

        private SoundEffect soundEffect;

        public Story(int screenX, int screenY, float scale)
        {
            this.screenX = screenX;
            this.screenY = screenY;
            this.scale = scale;
        }

        public void Initialize(float scale, int screenX)
        {
            state = State.happyCreatures;
            this.scale = scale;
        }

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {

            soundEffect = content.Load<SoundEffect>("Story/story_audio");
            soundEffect.Play();

            background = content.Load<Texture2D>("Story/story_background");
            //load baby creatures
            babyCreature[0] = content.Load<Texture2D>("Creatures/wild_creature_0");
            babyCreature[1] = content.Load<Texture2D>("Creatures/wild_creature_18");
            babyCreature[2] = content.Load<Texture2D>("Creatures/wild_creature_46");
            babyCreature[3] = content.Load<Texture2D>("Creatures/wild_creature_64");
            babyCreature[4] = content.Load<Texture2D>("Creatures/wild_creature_75");
            babyCreature[5] = content.Load<Texture2D>("Creatures/wild_creature_98");
            babyCreature[6] = content.Load<Texture2D>("Creatures/wild_creature_110");
            babyCreature[7] = content.Load<Texture2D>("Creatures/wild_creature_116");
            babyCreature[8] = content.Load<Texture2D>("Creatures/wild_creature_140");
            babyCreature[9] = content.Load<Texture2D>("Creatures/wild_creature_145");

            for (int i = 0; i < storyScript.Length; i++)
            {
                storyScript[i] = content.Load<Texture2D>("Story/story_" + (i + 1));
            }

            largeEnemyShipEnginesOff = content.Load<Texture2D>("Story/enemy_ship_engines_off");
            largeEnemyShipEnginesOn = content.Load<Texture2D>("Story/enemy_ship_engines_on");
            smallEnemyShipEnginesOff = content.Load<Texture2D>("Story/vertical_enemy_ship_engines_off");
            smallEnemyShipEnginesOn = content.Load<Texture2D>("Story/vertical_enemy_ship_engines_on");

            heroShipEnginesOff = content.Load<Texture2D>("Story/hero_ship_engines_off");
            heroShipEnginesOn = content.Load<Texture2D>("Story/hero_ship_engines_on");

            monster[0] = content.Load<Texture2D>("General/Monsters/enemy_1");
            monster[1] = content.Load<Texture2D>("General/Monsters/enemy_2");

            smokeCloud = content.Load<Texture2D>("Story/smoke_cloud");




            backgroundPosition = new Vector2((screenX / 2 - background.Width * scale / 2), (screenY / 2 - background.Height * scale / 2));

            //set baby creatures position
            for (int i = 0; i < babyCreaturePosition.Length; i++)
            {

                babyCreaturePosition[i] = new Vector2(((220 + 200 * i) * scale), (800 * scale));
            }

            largeEnemyShipPosition = new Vector2((-600 * scale), (600 * scale));
            smallEnemyShipOnePosition = new Vector2((1000 * scale), (200 * scale));
            smallEnemyShipTwoPosition = new Vector2((1600 * scale), (300 * scale));

            heroShipPosition = new Vector2((800 * scale), (250 * scale));

            for (int i = 0; i < monsterPosition.Length; i++)
            {
                monsterPosition[i] = new Vector2(((1600 + 50 * i) * scale), ((600 + 50 * i) * scale));
            }

            int j = 0;
            for (int i = 0; i < smokeCloudPosition.Length; i++)
            {
                smokeCloudPosition[i] = new Vector2(((-200 + 100 * i) * scale), ((800 + 80 * j) * scale));
                j++;
                if (j > 1) { j = 0; }
            }

            storyScriptPosition = new Vector2((300 * scale), (200 * scale));

            heroPosition = new Vector2((-1250 * scale), (800 * scale));
            hero = content.Load<Texture2D>("General/Hero/running_sprite_sheet_5x5");
            heroAnimation = new Animation(hero, heroPosition, 5, 5, scale, 10f);
        }

        public bool Update(GameTime gameTime)
        {
            if (storyCounter == 1000)
            {
                storyCounter = -28000;
            }

            constantlyIncreasingNumber += speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (state == State.badGuysLanding)
            {
                for (int i = 0; i < smokeCloudPosition.Length; i++)
                {

                    smokeCloudPosition[i].Y += 12;
                    if (i < 5)
                    {
                        smokeCloudPosition[i].X -= 50;

                    }
                    else
                    {
                        smokeCloudPosition[i].X += 50;
                    }
                }
            }
            updateState();
            heroAnimation.animateLoop(gameTime, heroPosition);
            if (state != State.badGuysLanding)
            {
                for (int i = 0; i < babyCreaturePosition.Length; i++)
                {
                    babyCreaturePosition[i] = new Vector2(updateCreatureHorizontal(babyCreaturePosition[i], i), updateCreatureBounce(babyCreaturePosition[i], i));
                }
            }

            for (int i = 0; i < monsterPosition.Length; i++)
            {
                monsterPosition[i] = new Vector2(updateCreatureHorizontal(monsterPosition[i], i), updateCreatureBounce(monsterPosition[i], i));

            }

            if (state == State.badGuysLeaving)
            {
                smallEnemyShipOnePosition.Y -= 5;
                smallEnemyShipTwoPosition.Y -= 8;
                largeEnemyShipPosition.Y -= 3;
                if (storyCounter > -12500)
                {

                    largeEnemyShipPosition.X -= 15;
                    smallEnemyShipOnePosition.Y -= 2;
                    smallEnemyShipTwoPosition.Y -= 2;
                }
            }

            if (state == State.heroChases)
            {
                heroPosition.X += 8;
            }

            if (state == State.heroLeaving)
            {
                heroShipPosition.Y -= 10;
            }

            if (storyCounter < 0)
            {
                storyCounter += gameTime.ElapsedGameTime.Milliseconds;
            }
            if (storyCounter >= 0 || (storyCounter >= -27000 && isKeyDown()))
            {
                return true;
            }

            return false;
        }

        private void updateState()
        {
            if (storyCounter < -20000)
            {
                state = State.happyCreatures;
            }
            else if (storyCounter < -19500)
            {
                state = State.badGuysLanding;
            }
            else if (storyCounter < -14500)
            {
                state = State.badGuysCapturingCreatures;
            }
            else if (storyCounter < -10000)
            {
                state = State.badGuysLeaving;
            }
            else if (storyCounter < -5000)
            {
                state = State.heroChases;
            }
            else if (storyCounter < -2000)
            {
                state = State.heroLeaving;
            }
        }

        private bool isKeyDown()
        {
            if (Keyboard.GetState().GetPressedKeys().Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private float updateCreatureBounce(Vector2 position, int i)
        {
            if (i % 2 == 0)
            {
                position.Y += 6 * (float)Math.Sin(constantlyIncreasingNumber / 8) * scale;
            }
            else
            {
                position.Y -= 6 * (float)Math.Sin(constantlyIncreasingNumber / 8) * scale;
            }
            return position.Y;
        }
        private float updateCreatureHorizontal(Vector2 position, int i)
        {
            if (state == State.happyCreatures)
            {
                if (i % 3 == 0)
                {
                    position.X += 10 * (float)Math.Sin(constantlyIncreasingNumber / 15) * scale;
                }
                else
                {
                    position.X -= 5 * (float)Math.Sin(constantlyIncreasingNumber / 15) * scale;
                }
                return position.X;
            }
            if (state == State.badGuysCapturingCreatures)
            {
                return position.X - 10;
                //moving into the big ship
            }
            if (state == State.heroChases)
            {
                return position.X;
                //hero running
                //hero bouncing
            }
            return position.X;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, backgroundPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            if (state == State.heroChases)
            {
                heroAnimation.Draw(spriteBatch, heroPosition);
                spriteBatch.Draw(storyScript[3], storyScriptPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            if (state == State.badGuysLanding || state == State.badGuysCapturingCreatures)
            {
                spriteBatch.Draw(smallEnemyShipEnginesOff, smallEnemyShipOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(smallEnemyShipEnginesOff, smallEnemyShipTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(storyScript[2], storyScriptPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            }

            if (state == State.badGuysLeaving)
            {
                spriteBatch.Draw(largeEnemyShipEnginesOn, largeEnemyShipPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(smallEnemyShipEnginesOn, smallEnemyShipOnePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(smallEnemyShipEnginesOn, smallEnemyShipTwoPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(storyScript[2], storyScriptPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            }

            if (state == State.heroLeaving)
            {
                spriteBatch.Draw(heroShipEnginesOn, heroShipPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(storyScript[4], storyScriptPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            }
            else
            {
                spriteBatch.Draw(heroShipEnginesOff, heroShipPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }



            if (state == State.happyCreatures || state == State.badGuysLanding || state == State.badGuysCapturingCreatures)
            {
                for (int i = 0; i < babyCreaturePosition.Length; i++)
                {

                    spriteBatch.Draw(babyCreature[i], babyCreaturePosition[i], null, Color.White, 0f, Vector2.Zero, scale / 2, SpriteEffects.None, 0f);

                    if (storyCounter < -23000)
                    {
                        spriteBatch.Draw(storyScript[0], storyScriptPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        spriteBatch.Draw(storyScript[1], storyScriptPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                }
            }

            if (state == State.badGuysCapturingCreatures)
            {
                int j = 0;
                for (int i = 0; i < monsterPosition.Length; i++)
                {

                    spriteBatch.Draw(monster[j], monsterPosition[i], null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    j++;
                    if (j > 1) { j = 0; }
                }
                spriteBatch.Draw(storyScript[2], storyScriptPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            }
            if (state == State.badGuysLanding || state == State.badGuysCapturingCreatures)
            {
                spriteBatch.Draw(largeEnemyShipEnginesOff, largeEnemyShipPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(storyScript[2], storyScriptPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                for (int i = 0; i < smokeCloudPosition.Length; i++)
                {
                    spriteBatch.Draw(smokeCloud, smokeCloudPosition[i], null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                }
            }
            if (state == State.badGuysLeaving)
            {
                spriteBatch.Draw(largeEnemyShipEnginesOn, largeEnemyShipPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }




        }
    }
}
