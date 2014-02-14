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
    class HomeText
    {
        SpriteFont menuFont;
        SpriteFont skillFont;

        Vector2 world101MenuPosition;
        Vector2 world101SkillPosition;

        float scale;
        
        string welcome = "";
        string world101Skill = "";

        public HomeText(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            welcome = "Welcome back " + Game1.globalUser.username + "!";
            setSkillValues();
            menuFont = content.Load<SpriteFont>("small_input_font");
            skillFont = content.Load<SpriteFont>("tiny_input_font");
            world101MenuPosition = new Vector2(screenWidth / 2 - (menuFont.MeasureString(welcome).X) / 2 * scale, 300 * scale);
            world101SkillPosition = new Vector2(screenWidth / 2 - (skillFont.MeasureString(world101Skill).X) / 2 * scale, 765 * scale);
        }

        public void Update(String whereTo)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(menuFont, welcome, world101MenuPosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(skillFont, world101Skill, world101SkillPosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void setSkillValues()
        {
            world101Skill = Game1.globalUser.world101 + "";
        }
    }
}
