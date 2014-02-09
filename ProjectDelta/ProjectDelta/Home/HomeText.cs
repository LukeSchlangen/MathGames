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
        Vector2 world201MenuPosition;
        Vector2 world101SkillPosition;
        Vector2 world201SkillPosition;

        float scale;
        
        string welcome = "";
        string world101Skill = "";
        string world201Skill = "";
        string world101 = "";
        string world201 = "";

        public HomeText(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager content, int screenHeight, int screenWidth)
        {
            welcome = "Welcome back " + Game1.globalUser.username + "!";
            world101 = "World 1: Addition";
            world201 = "World 2: Subtraction";
            setSkillValues();
            menuFont = content.Load<SpriteFont>("small_input_font");
            skillFont = content.Load<SpriteFont>("tiny_input_font");
            world101MenuPosition = new Vector2(screenWidth / 4 - (menuFont.MeasureString(welcome).X) / 2 * scale, 300 * scale);
            world201MenuPosition = new Vector2(3*screenWidth / 4 - (menuFont.MeasureString(welcome).X) / 2 * scale, 300 * scale);
            world101SkillPosition = new Vector2(screenWidth / 4 - (skillFont.MeasureString(world101Skill).X) / 2 * scale, 765 * scale);
            world201SkillPosition = new Vector2(3*screenWidth / 4 - (skillFont.MeasureString(world201Skill).X) / 2 * scale, 765 * scale);
        }

        public void Update(String whereTo)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(menuFont, world101, world101MenuPosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(menuFont, world201, world201MenuPosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(skillFont, world101Skill, world101SkillPosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(skillFont, world201Skill, world201SkillPosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void setSkillValues()
        {
            world101Skill = Game1.globalUser.world101 + "";
            world201Skill = Game1.globalUser.world201 + "";
        }
    }
}
