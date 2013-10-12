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
        SpriteFont welcomeFont;
        SpriteFont skillFont;

        Vector2 welcomePosition;
        Vector2 skillPosition;

        float scale;
        
        string welcome = "";
        string skill = "";

        public HomeText(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            welcome = "Welcome back " + Game1.globalUser.username + "!";
            skill = getSkillValue() + "";
            welcomeFont = content.Load<SpriteFont>("small_input_font");
            skillFont = content.Load<SpriteFont>("tiny_input_font");
            welcomePosition = new Vector2(screenWidth / 2 - (welcomeFont.MeasureString(welcome).X) / 2 * scale, 300 * scale);  
            skillPosition = new Vector2(screenWidth / 2 - (skillFont.MeasureString(skill).X) / 2 * scale, 765 * scale);
        }

        public void Update(String whereTo)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(welcomeFont, welcome, welcomePosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(skillFont, skill, skillPosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private int getSkillValue()
        {
            int skillValue = Game1.globalUser.world101;

            return skillValue;
        }
    }
}
