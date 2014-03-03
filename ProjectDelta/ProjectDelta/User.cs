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
    //A User object is a programmatic representation
    //of the player currently playing the game

    [DynamoDBTable("User")]
    public class User
    {
        [DynamoDBHashKey("USERNAME")]
        public string username { get; set; }

        [DynamoDBProperty("PASSWORD")]
        public string password { get; set; }

        [DynamoDBProperty("SKILL")]
        public string skill { get; set; }

        [DynamoDBProperty("WORLD_101")]
        public int world101 { get; set; }

        [DynamoDBProperty("TIME_PLAYED")]
        public int timePlayed { get; set; }

        [DynamoDBProperty("ANSWERS_ATTEMPTED")]
        public int answersAttempted { get; set; }

        [DynamoDBProperty("ANSWERS_CORRECT")]
        public int answersCorrect { get; set; }
    }
}
