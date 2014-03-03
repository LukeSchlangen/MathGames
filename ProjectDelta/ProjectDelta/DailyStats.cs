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
    class DailyStats
    {
        private DynamoDBContext context;

        public DailyStats(DynamoDBContext context)
        {
            this.context = context;
        }

        public void resetDailyStats()
        {
            try
            {
                if (Game1.globalUser.lastDatePlayed != DateTime.Today)
                {
                    Game1.globalUser.answersAttemptedToday = 0;
                    Game1.globalUser.answersCorrectToday = 0;
                    Game1.globalUser.timePlayedToday = 0;
                    Game1.globalUser.lastDatePlayed = DateTime.Today;
                    context.Save<User>(Game1.globalUser);
                }
            }
            catch
            {
                //for some reason, Game1.globalUser.lastDatePlayed keeps equaling null... this should fix it
            }
        }
    }
}
