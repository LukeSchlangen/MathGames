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
    class CreatureOrganizer
    {
        private bool mostEvolvedCreature(int worldStage, int creature)
        {
            bool mostEvolved;

            switch (creature)
            {
                case 15:
                    mostEvolved = true;
                    break;
                case 24:
                    mostEvolved = true;
                    break;
                case 29:
                    mostEvolved = true;
                    break;
                case 38:
                    mostEvolved = true;
                    break;
                case 44:
                    mostEvolved = true;
                    break;
                case 49:
                    mostEvolved = true;
                    break;
                case 55:
                    mostEvolved = true;
                    break;
                case 62:
                    mostEvolved = true;
                    break;
                case 67:
                    mostEvolved = true;
                    break;
                case 72:
                    mostEvolved = true;
                    break;
                case 78:
                    mostEvolved = true;
                    break;
                case 86:
                    mostEvolved = true;
                    break;
                case 93:
                    mostEvolved = true;
                    break;
                case 101:
                    mostEvolved = true;
                    break;
                case 109:
                    mostEvolved = true;
                    break;
                case 115:
                    mostEvolved = true;
                    break;
                case 123:
                    mostEvolved = true;
                    break;
                case 130:
                    mostEvolved = true;
                    break;
                case 138:
                    mostEvolved = true;
                    break;
                case 144:
                    mostEvolved = true;
                    break;
                case 149:
                    mostEvolved = true;
                    break;
                case 157:
                    mostEvolved = true;
                    break;
                default:
                    mostEvolved = false;
                    break;
            }

            if (creature == (worldStage - 1))
            {
                mostEvolved = true;
            }

            return mostEvolved;
        }

        public bool isCreatureAvailable(int worldStage, int creature)
        {
            //if they are the most evolved state, then return true
            bool available = mostEvolvedCreature(worldStage, creature);

            //if the creature hasn't been collected yet,
            //the creature is not available
            if (creature >= worldStage)
            {
                available = false;
            }

            return available;
        }

        private string creatureName(int creature)
        {
            string creatureName = "";
            if (creature < 16)
            {
                creatureName = "Begino";
            }
            else if (creature < 25)
            {
                creatureName = "Addingo";
            }
            else if (creature < 30)
            {
                creatureName = "Lotolegs";
            }
            else if (creature < 39)
            {
                creatureName = "Toradd";
            }
            else if (creature < 45)
            {
                creatureName = "Diamon";
            }
            else if (creature < 50)
            {
                creatureName = "Torton";
            }
            else if (creature < 56)
            {
                creatureName = "Harehat";
            }
            else if (creature < 63)
            {
                creatureName = "Umbrello";
            }
            else if (creature < 68)
            {
                creatureName = "Springo";
            }
            else if (creature < 73)
            {
                creatureName = "Vamp";
            }
            else if (creature < 79)
            {
                creatureName = "Chargo";
            }
            else if (creature < 87)
            {
                creatureName = "Lanwal";
            }
            else if (creature < 94)
            {
                creatureName = "Slinko";
            }
            else if (creature < 102)
            {
                creatureName = "Hingo";
            }
            else if (creature < 110)
            {
                creatureName = "Downgo";
            }
            else if (creature < 116)
            {
                creatureName = "Slicko";
            }
            else if (creature < 124)
            {
                creatureName = "Fuzzbump";
            }
            else if (creature < 131)
            {
                creatureName = "Arrow";
            }
            else if (creature < 139)
            {
                creatureName = "Flamingren";
            }
            else if (creature < 145)
            {
                creatureName = "Twisto";
            }
            else if (creature < 150)
            {
                creatureName = "Hoverbo";
            }
            else if (creature < 158)
            {
                creatureName = "Wisdo";
            }
            else { creatureName = "Unnamed"; }

            return creatureName;
        }

        private string creatureType(int creature)
        {
            string creatureType = "";
            if (creature < 16)
            {
                creatureType = "Tackler";
            }
            else if (creature < 25)
            {
                creatureType = "Spiker";
            }
            else if (creature < 30)
            {
                creatureType = "Strecher";
            }
            else if (creature < 39)
            {
                creatureType = "Tackler";
            }
            else if (creature < 45)
            {
                creatureType = "Freezer";
            }
            else if (creature < 50)
            {
                creatureType = "Tackler";
            }
            else if (creature < 56)
            {
                creatureType = "Digger";
            }
            else if (creature < 63)
            {
                creatureType = "Stomper";
            }
            else if (creature < 68)
            {
                creatureType = "Spinner";
            }
            else if (creature < 73)
            {
                creatureType = "Scarer";
            }
            else if (creature < 79)
            {
                creatureType = "Zapper";
            }
            else if (creature < 87)
            {
                creatureType = "Stomper";
            }
            else if (creature < 94)
            {
                creatureType = "Stretcher";
            }
            else if (creature < 102)
            {
                creatureType = "Shooter";
            }
            else if (creature < 110)
            {
                creatureType = "Digger";
            }
            else if (creature < 116)
            {
                creatureType = "Zapper";
            }
            else if (creature < 124)
            {
                creatureType = "Stomper";
            }
            else if (creature < 131)
            {
                creatureType = "Tackler";
            }
            else if (creature < 139)
            {
                creatureType = "Zapper";
            }
            else if (creature < 145)
            {
                creatureType = "Spinner";
            }
            else if (creature < 150)
            {
                creatureType = "Shooter";
            }
            else if (creature < 158)
            {
                creatureType = "Freezer";
            }
            else { creatureType = "Tackler"; }

            return creatureType;

        }

        private int creatureLevel(int worldStage, int creature, int numberOfAttemptedProblems, int numberOfCorrectProblems)
        {
            int creatureLevel = worldStage-creature;



            if (creatureLevel > 99)
            {
                creatureLevel = 99;
            }
            return creatureLevel;
        }

        public string getCreatureType(int creature)
        {
            string type = creatureType(creature);
            return type;
        }

        public string getCreatureName(int creature)
        {
            string name = creatureName(creature);
            return name;
        }

        public int getCreatureLevel(int worldStage, int creature, int numberOfAttemptedProblems, int numberOfCorrectProblems)
        {
            int level = creatureLevel(worldStage, creature, numberOfAttemptedProblems, numberOfCorrectProblems);
            return level;
        }
    }
}
