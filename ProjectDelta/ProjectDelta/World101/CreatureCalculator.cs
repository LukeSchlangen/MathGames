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
    class CreatureCalculator
    {
        public static int levelCalculator(int worldStageMultiplier, int correctAnswersMultiplier, int timeMultiplier, int worldStage, int numberOfCorrectAnswers, int totalTimePlayed)
        {
            int creatureLevel = 0;
            creatureLevel += (int)(worldStageMultiplier * (float)worldStage/200);
            creatureLevel += (int)(correctAnswersMultiplier * (float)numberOfCorrectAnswers / 10000);
            creatureLevel += (int)(timeMultiplier * (float)totalTimePlayed / 900);
            return creatureLevel;
        }
    }
}
