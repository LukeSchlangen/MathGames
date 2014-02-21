﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectDelta
{
    //Note: we want this to be a static class so that
    //we can make static references to it from our other
    //classes
    static class Problems
    {


        public static Dictionary<string, int>[] determineProblems(int worldStage, int numberOfProblems)
        {
            int randomSpot; //randomSpot will later be used to decide the order of the problem as presented to player
            numberOfProblems += 2; //two problems will not be used, but must be generated because after the last two monsters die, they will still be looking for problems to present

            //This is the array of dictionary objects that we will return to the game loop
            Dictionary<string, int>[] problemsToReturn = new Dictionary<string, int>[numberOfProblems];

            Random random = new Random(); //A random is used to pick the order of the problems and 2+3 vs 3+2 later in the code

            bool[] spotTaken = new bool[numberOfProblems]; //array to keep track of which spots already have problems in them

            //make all of the spots "open" meaning they can have a new problem placed there
            for (int i = 0; i < numberOfProblems; i++)
            {
                spotTaken[i] = false;
            }

            //This is where the magic happens... operation, factorOne, and factorTwo are all set based on level

            for (int i = 0; i < numberOfProblems; i++)
            {
                //make an instance of the dictionary to be added to the array

                Dictionary<string, int> problemsDictionary = new Dictionary<string, int>();
                int operation = 0;
                int factorOne = 0;
                int factorTwo = 0;

                //based on worldStage, determine the operation
                if (worldStage < 40) { operation = 0; } else if (worldStage < 117) { operation = 1; } else if (worldStage < 157) { operation = 2; } else { operation = 3; }

                //based on worldStage, this switch case determines which factors to show
                //factorOneGenerator and factorTwoGenerator pick one of the 2,3, or 4 options and return them
                //all possible combinations are created and used so student will never randomly
                // get lucky and skip a problem they are supposed to learn on that level

                switch (worldStage)
                {
                    case 0: //0 through 1 + 0
                        factorOne = factorOneGenerator(0, 1, i);
                        factorTwo = 0;
                        break;
                    case 1: //0 through 2 + 0
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = 0;
                        break;
                    case 2: //0 through 2 + 1
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = 1;
                        break;
                    case 3: //0 through 2 + 0 through 1
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(0, 1, i, numberOfProblems);
                        break;
                    case 4: //0 through 2 + 1 through 2
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(1, 2, i, numberOfProblems);
                        break;
                    case 5: //0 through 2 + 2 through 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(2, 3, i, numberOfProblems);
                        break;
                    case 6: //1 through 3 + 2 through 3
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(2, 3, i, numberOfProblems);
                        break;
                    case 7: //0 through 2 + 3 through 4
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 8: //1 through 3 + 3 through 4
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 9: //2 through 4 + 3 through 4
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 10: //0 through 2 + 4 through 5
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 11: //1 through 3 + 4 through 5
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 12: //2 through 4 + 4 through 5
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 13: //3 through 5 + 4 through 5
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 14: //0 through 2 + 5 through 6
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 15: //1 through 2 + 5 through 6
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 16: //2 through 4 + 5 through 6
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 17: //3 through 5 + 5 through 6
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 18: //0 through 2 + 6 through 7
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 19: //1 through 3 + 6 through 7
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 20: //0 through 2 + 7 through 8
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(8, 7, i, numberOfProblems);
                        break;
                    case 21: //4 through 5 + 5 through 6
                        factorOne = factorOneGenerator(4, 5, 6, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 22: //the remainder of these continue in this way factorOne and factorTwo
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 23:
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 24:
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 25:
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 26:
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 27:
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 28:
                        factorOne = factorOneGenerator(4, 5, 6, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 29:
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 30:
                        factorOne = factorOneGenerator(5, 6, 7, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 31:
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 32:
                        factorOne = factorOneGenerator(5, 4, 6, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 33:
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 34:
                        factorOne = factorOneGenerator(5, 6, 7, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 35:
                        factorOne = factorOneGenerator(5, 6, 4, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 36:
                        factorOne = factorOneGenerator(8, 6, 7, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 37:
                        factorOne = factorOneGenerator(5, 6, 7, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 38:
                        factorOne = factorOneGenerator(8, 6, 7, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 39: //last level of addition: 7 through 9 + 8 through 9
                        factorOne = factorOneGenerator(8, 9, 7, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;

                    //end of addition, begin subtraction
                    case 40: //1 minus 0 through 1
                        factorOne = 1;
                        factorTwo = factorTwoGenerator(0, 1, i, numberOfProblems);
                        break;
                    case 41:// 1 through 2 minus 0 through 1
                        factorOne = factorOneGenerator(1, 2, i);
                        factorTwo = factorTwoGenerator(0, 1, i, numberOfProblems);
                        break;
                    case 42:// remainder of subtraction continue this way: factorOne minus factorTwo
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(0, 1, i, numberOfProblems);
                        break;
                    case 43:
                        factorOne = factorOneGenerator(3, 2, i);
                        factorTwo = factorTwoGenerator(0, 1, 2, i, numberOfProblems);
                        break;
                    case 44:
                        factorOne = factorOneGenerator(3, 4, i);
                        factorTwo = factorTwoGenerator(0, 1, 2, i, numberOfProblems);
                        break;
                    case 45:
                        factorOne = factorOneGenerator(3, 4, i);
                        factorTwo = factorTwoGenerator(2, 3, 1, i, numberOfProblems);
                        break;
                    case 46:
                        factorOne = factorOneGenerator(4, 5, i);
                        factorTwo = factorTwoGenerator(0, 1, 2, i, numberOfProblems);
                        break;
                    case 47:
                        factorOne = factorOneGenerator(5, 6, i);
                        factorTwo = factorTwoGenerator(0, 1, 2, i, numberOfProblems);
                        break;
                    case 48:
                        factorOne = factorOneGenerator(5, 4, i);
                        factorTwo = factorTwoGenerator(2, 3, 1, i, numberOfProblems);
                        break;
                    case 49:
                        factorOne = factorOneGenerator(5, 6, i);
                        factorTwo = factorTwoGenerator(2, 3, 1, i, numberOfProblems);
                        break;
                    case 50:
                        factorOne = factorOneGenerator(6, 7, i);
                        factorTwo = factorTwoGenerator(0, 1, 2, i, numberOfProblems);
                        break;
                    case 51:
                        factorOne = factorOneGenerator(6, 7, i);
                        factorTwo = factorTwoGenerator(2, 3, 1, i, numberOfProblems);
                        break;
                    case 52:
                        factorOne = factorOneGenerator(4, 5, i);
                        factorTwo = factorTwoGenerator(2, 3, 4, i, numberOfProblems);
                        break;
                    case 53:
                        factorOne = factorOneGenerator(7, 8, i);
                        factorTwo = factorTwoGenerator(0, 1, 2, i, numberOfProblems);
                        break;
                    case 54:
                        factorOne = factorOneGenerator(5, 6, i);
                        factorTwo = factorTwoGenerator(2, 3, 4, i, numberOfProblems);
                        break;
                    case 55:
                        factorOne = factorOneGenerator(5, 6, i);
                        factorTwo = factorTwoGenerator(3, 4, 5, i, numberOfProblems);
                        break;
                    case 56:
                        factorOne = factorOneGenerator(8, 9, i);
                        factorTwo = factorTwoGenerator(0, 1, 2, i, numberOfProblems);
                        break;
                    case 57:
                        factorOne = factorOneGenerator(6, 7, i);
                        factorTwo = factorTwoGenerator(2, 3, 4, i, numberOfProblems);
                        break;
                    case 58:
                        factorOne = factorOneGenerator(8, 9, i);
                        factorTwo = factorTwoGenerator(1, 2, 3, i, numberOfProblems);
                        break;
                    case 59:
                        factorOne = factorOneGenerator(7, 8, i);
                        factorTwo = factorTwoGenerator(2, 3, 4, i, numberOfProblems);
                        break;
                    case 60:
                        factorOne = factorOneGenerator(6, 7, i);
                        factorTwo = factorTwoGenerator(3, 4, 5, i, numberOfProblems);
                        break;
                    case 61:
                        factorOne = factorOneGenerator(9, 10, i);
                        factorTwo = factorTwoGenerator(0, 1, 2, i, numberOfProblems);
                        break;
                    case 62:
                        factorOne = factorOneGenerator(6, 7, i);
                        factorTwo = factorTwoGenerator(5, 4, 6, i, numberOfProblems);
                        break;
                    case 63:
                        factorOne = factorOneGenerator(4, 5, 6, i);
                        factorTwo = factorTwoGenerator(1, 2, 3, i, numberOfProblems);
                        break;
                    case 64:
                        factorOne = factorOneGenerator(7, 8, i);
                        factorTwo = factorTwoGenerator(5, 4, 6, i, numberOfProblems);
                        break;
                    case 65:
                        factorOne = factorOneGenerator(7, 8, i);
                        factorTwo = factorTwoGenerator(5, 6, 7, i, numberOfProblems);
                        break;
                    case 66:
                        factorOne = factorOneGenerator(9, 10, i);
                        factorTwo = factorTwoGenerator(1, 2, 3, i, numberOfProblems);
                        break;
                    case 67:
                        factorOne = factorOneGenerator(8, 9, i);
                        factorTwo = factorTwoGenerator(2, 3, 4, i, numberOfProblems);
                        break;
                    case 68:
                        factorOne = factorOneGenerator(8, 9, i);
                        factorTwo = factorTwoGenerator(3, 4, 5, i, numberOfProblems);
                        break;
                    case 69:
                        factorOne = factorOneGenerator(9, 10, i);
                        factorTwo = factorTwoGenerator(3, 4, 5, i, numberOfProblems);
                        break;
                    case 70:
                        factorOne = factorOneGenerator(8, 9, i);
                        factorTwo = factorTwoGenerator(4, 5, 6, i, numberOfProblems);
                        break;
                    case 71:
                        factorOne = factorOneGenerator(9, 10, i);
                        factorTwo = factorTwoGenerator(3, 4, 5, i, numberOfProblems);
                        break;
                    case 72:
                        factorOne = factorOneGenerator(10, 11, i);
                        factorTwo = factorTwoGenerator(2, 3, 4, i, numberOfProblems);
                        break;
                    case 73:
                        factorOne = factorOneGenerator(8, 9, i);
                        factorTwo = factorTwoGenerator(4, 5, 6, i, numberOfProblems);
                        break;
                    case 74:
                        factorOne = factorOneGenerator(10, 11, i);
                        factorTwo = factorTwoGenerator(3, 4, 5, i, numberOfProblems);
                        break;
                    case 75:
                        factorOne = factorOneGenerator(10, 11, 12, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 76:
                        factorOne = factorOneGenerator(8, 9, i);
                        factorTwo = factorTwoGenerator(6, 7, 8, i, numberOfProblems);
                        break;
                    case 77:
                        factorOne = factorOneGenerator(10, 11, 12, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 78:
                        factorOne = factorOneGenerator(8, 9, 10, i);
                        factorTwo = factorTwoGenerator(5, 6, i, numberOfProblems);
                        break;
                    case 79:
                        factorOne = factorOneGenerator(10, 11, i);
                        factorTwo = factorTwoGenerator(4, 5, 6, i, numberOfProblems);
                        break;
                    case 80:
                        factorOne = factorOneGenerator(9, 10, i);
                        factorTwo = factorTwoGenerator(5, 6, 7, i, numberOfProblems);
                        break;
                    case 81:
                        factorOne = factorOneGenerator(8, 9, 10, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 82:
                        factorOne = factorOneGenerator(11, 12, i);
                        factorTwo = factorTwoGenerator(4, 5, 6, i, numberOfProblems);
                        break;
                    case 83:
                        factorOne = factorOneGenerator(9, 10, i);
                        factorTwo = factorTwoGenerator(7, 8, 9, i, numberOfProblems);
                        break;
                    case 84:
                        factorOne = factorOneGenerator(7, 8, 9, i);
                        factorTwo = factorTwoGenerator(2, 3, 4, i, numberOfProblems);
                        break;
                    case 85:
                        factorOne = factorOneGenerator(6, 7, 8, i);
                        factorTwo = factorTwoGenerator(4, 5, 6, i, numberOfProblems);
                        break;
                    case 86:
                        factorOne = factorOneGenerator(10, 11, i);
                        factorTwo = factorTwoGenerator(5, 6, 7, i, numberOfProblems);
                        break;
                    case 87:
                        factorOne = factorOneGenerator(10, 11, 12, i);
                        factorTwo = factorTwoGenerator(2, 3, 4, i, numberOfProblems);
                        break;
                    case 88:
                        factorOne = factorOneGenerator(10, 11, i);
                        factorTwo = factorTwoGenerator(6, 7, 8, i, numberOfProblems);
                        break;
                    case 89:
                        factorOne = factorOneGenerator(11, 12, 13, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 90:
                        factorOne = factorOneGenerator(9, 10, 11, i);
                        factorTwo = factorTwoGenerator(4, 5, 6, i, numberOfProblems);
                        break;
                    case 91:
                        factorOne = factorOneGenerator(10, 11, i);
                        factorTwo = factorTwoGenerator(7, 8, 9, i, numberOfProblems);
                        break;
                    case 92:
                        factorOne = factorOneGenerator(6, 7, 8, i);
                        factorTwo = factorTwoGenerator(3, 4, 5, i, numberOfProblems);
                        break;
                    case 93:
                        factorOne = factorOneGenerator(12, 13, i);
                        factorTwo = factorTwoGenerator(4, 5, 6, i, numberOfProblems);
                        break;
                    case 94:
                        factorOne = factorOneGenerator(11, 12, i);
                        factorTwo = factorTwoGenerator(5, 6, 7, i, numberOfProblems);
                        break;
                    case 95:
                        factorOne = factorOneGenerator(12, 13, 14, i);
                        factorTwo = factorTwoGenerator(5, 6, i, numberOfProblems);
                        break;
                    case 96:
                        factorOne = factorOneGenerator(11, 12, i);
                        factorTwo = factorTwoGenerator(6, 7, 8, i, numberOfProblems);
                        break;
                    case 97:
                        factorOne = factorOneGenerator(9, 10, 11, i);
                        factorTwo = factorTwoGenerator(8, 9, i, numberOfProblems);
                        break;
                    case 98:
                        factorOne = factorOneGenerator(12, 13, i);
                        factorTwo = factorTwoGenerator(5, 6, 7, i, numberOfProblems);
                        break;
                    case 99:
                        factorOne = factorOneGenerator(11, 12, i);
                        factorTwo = factorTwoGenerator(7, 8, 9, i, numberOfProblems);
                        break;
                    case 100:
                        factorOne = factorOneGenerator(13, 14, i);
                        factorTwo = factorTwoGenerator(5, 6, 7, i, numberOfProblems);
                        break;
                    case 101:
                        factorOne = factorOneGenerator(11, 12, 13, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 102:
                        factorOne = factorOneGenerator(13, 14, 15, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 103:
                        factorOne = factorOneGenerator(12, 13, 14, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 104:
                        factorOne = factorOneGenerator(9, 10, 11, i);
                        factorTwo = factorTwoGenerator(7, 8, 9, numberOfProblems);
                        break;
                    case 105:
                        factorOne = factorOneGenerator(6, 7, 8, 9, i);
                        factorTwo = factorTwoGenerator(2, 3, 4, 5, i, numberOfProblems);
                        break;
                    case 106:
                        factorOne = factorOneGenerator(11, 12, 13, i);
                        factorTwo = factorTwoGenerator(7, 8, 9, i, numberOfProblems);
                        break;
                    case 107:
                        factorOne = factorOneGenerator(14, 15, i);
                        factorTwo = factorTwoGenerator(6, 7, 8, i, numberOfProblems);
                        break;
                    case 108:
                        factorOne = factorOneGenerator(12, 13, 14, i);
                        factorTwo = factorTwoGenerator(8, 9, i, numberOfProblems);
                        break;
                    case 109:
                        factorOne = factorOneGenerator(15, 16, i);
                        factorTwo = factorTwoGenerator(7, 8, 9, i, numberOfProblems);
                        break;
                    case 110:
                        factorOne = factorOneGenerator(13, 14, 15, i);
                        factorTwo = factorTwoGenerator(8, 9, i, numberOfProblems);
                        break;
                    case 111:
                        factorOne = factorOneGenerator(15, 16, 17, i);
                        factorTwo = factorTwoGenerator(8, 7, i, numberOfProblems);
                        break;
                    case 112:
                        factorOne = factorOneGenerator(14, 15, i);
                        factorTwo = factorTwoGenerator(6, 7, 8, 9, i, numberOfProblems);
                        break;
                    case 113:
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(1, 2, 3, i, numberOfProblems);
                        break;
                    case 114:
                        factorOne = factorOneGenerator(15, 16, 17, i);
                        factorTwo = factorTwoGenerator(8, 9, i, numberOfProblems);
                        break;
                    case 115:
                        factorOne = factorOneGenerator(11, 12, 13, 14, i);
                        factorTwo = 9;
                        break;
                    case 116:// last level of subtraction: 15 through 18 minus 9
                        factorOne = factorOneGenerator(15, 16, 17, 18, i);
                        factorTwo = 9;
                        break;

                    //end of subtraction begin multiplication
                    case 117: //these factors are the same as addition: begins with 0 through 1 * 0
                        factorOne = factorOneGenerator(0, 1, i);
                        factorTwo = 0;
                        break;
                    case 118:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = 0;
                        break;
                    case 119:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = 1;
                        break;
                    case 120:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(0, 1, i, numberOfProblems);
                        break;
                    case 121:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(1, 2, i, numberOfProblems);
                        break;
                    case 122:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(2, 3, i, numberOfProblems);
                        break;
                    case 123:
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(2, 3, i, numberOfProblems);
                        break;
                    case 124:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 125:
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 126:
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 127:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 128:
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 129:
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 130:
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 131:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 132:
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 133:
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 134:
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 135:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 136:
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 137:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(8, 7, i, numberOfProblems);
                        break;
                    case 138:
                        factorOne = factorOneGenerator(4, 5, 6, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 139:
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 140:
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 141:
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 142:
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 143:
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 144:
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 145:
                        factorOne = factorOneGenerator(4, 5, 6, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 146:
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 147:
                        factorOne = factorOneGenerator(5, 6, 7, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 148:
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 149:
                        factorOne = factorOneGenerator(5, 4, 6, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 150:
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 151:
                        factorOne = factorOneGenerator(5, 6, 7, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 152:
                        factorOne = factorOneGenerator(5, 6, 4, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 153:
                        factorOne = factorOneGenerator(8, 6, 7, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 154:
                        factorOne = factorOneGenerator(5, 6, 7, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 155:
                        factorOne = factorOneGenerator(8, 6, 7, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 156:
                        factorOne = factorOneGenerator(8, 9, 7, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;

                    //end of multiplication, begin division
                    //addition, subtraction, and multiplication were the same... this is tricky... so listen up
                    //unlike addition, subtraction, and multiplication where you can put any two numbers together willy-nilly
                    //so... we have to get creative

                    //factorOneGenerator given 4 inputs (and i) has a period of 4
                    //factorOneGenerator given 2 inputs (and i) has a period of 2
                    //because of this, let's say we want 16/4, 25/5, 12/4, and 15/5
                    // we can write this as
                    //factorOne = factorOneGenerator(16,25,12,15,i);
                    //factorTwo = factorOneGenerator(4,5,i); 

                    case 157:
                        factorOne = factorOneGenerator(0, 2, 1, 0, i);
                        factorTwo = factorOneGenerator(1, 2, i);
                        break;
                    case 158:
                        factorOne = factorOneGenerator(0, 7, 6, 0, i);
                        factorTwo = factorOneGenerator(6, 7, i);
                        break;
                    case 159:
                        factorOne = factorOneGenerator(5, 8, 1, 0, i);
                        factorTwo = factorOneGenerator(1, 8, i);
                        break;
                    case 160:
                        factorOne = factorOneGenerator(4, 4, 0, 9, i);
                            factorTwo = factorOneGenerator(4,1,i);
                            break;
                    case 161:
                            factorOne = factorOneGenerator(3, 3, 0, 2);
                            factorTwo = factorOneGenerator(3,1,i);
                        break;

                    //end of division, if a student is still going, give them some tough addition and subtraction problems
                    default:
                        operation = random.Next(0, 2);
                        factorOne = random.Next(worldStage * 3 / 2, worldStage * 2);
                        factorTwo = random.Next(worldStage / 4, worldStage * 3 / 2);
                        break;
                }

                //mix up the order of factorOne and factorTwo i.e. 2+3 and 3+2 for addition and multiplication
                if (operation == 0 || operation == 2)
                {
                    if (random.Next(0, 2) == 0)
                    {
                        int temporaryIntegerFactor = factorOne;
                        factorOne = factorTwo;
                        factorTwo = temporaryIntegerFactor;
                    }
                }


                //set the values of the factors
                problemsDictionary.Add("operation", operation);
                problemsDictionary.Add("factorOne", factorOne);
                problemsDictionary.Add("factorTwo", factorTwo);

                //randomize order and determine order of problem (find a spot that isn't taken)
                randomSpot = random.Next(0, numberOfProblems);
                while (spotTaken[randomSpot] == true)
                {
                    randomSpot = random.Next(0, numberOfProblems);
                }

                //mark which spot was taken so that it won't be overwritten
                spotTaken[randomSpot] = true;

                //place problem in return array
                problemsToReturn[randomSpot] = problemsDictionary;

            }

            //We now have an array containing 10 different dictionary objects, whose order is randomized
            return problemsToReturn;


        }

        //factorOneGenerator and factorTwoGenerator differ in order to allow them to be used to generate all possibilities
        //ex. factorOneGenerator(1,2,3,4, i) and factorOneGenerator(1,2,i) would never present 1+2 because they would be periodic
        //this periodic nature of only presenting 4 of the 8 possible problems could be useful for division

        private static int factorOneGenerator(int integerOne, int integerTwo, int timeThroughLoop)
        {
            //alternate number based on which pass i through the dictionary loop
            int factor;
            if (timeThroughLoop % 2 == 0) { factor = integerOne; }
            else { factor = integerTwo; }
            return factor;
        }
        private static int factorOneGenerator(int integerOne, int integerTwo, int integerThree, int timeThroughLoop)
        {
            //alternate number returned every third number is the same
            int factor;
            if (timeThroughLoop % 3 == 0) { factor = integerOne; }
            else if (timeThroughLoop % 3 == 1) { factor = integerTwo; }
            else { factor = integerThree; }
            return factor;
        }
        private static int factorOneGenerator(int integerOne, int integerTwo, int integerThree, int integerFour, int timeThroughLoop)
        {
            //alternate number returned, every fourth number is the same
            int factor;
            if (timeThroughLoop % 4 == 0) { factor = integerOne; }
            else if (timeThroughLoop % 4 == 1) { factor = integerTwo; }
            else if (timeThroughLoop % 4 == 2) { factor = integerThree; }
            else { factor = integerFour; }
            return factor;
        }

        private static int factorTwoGenerator(int integerOne, int integerTwo, int timeThroughLoop, int numberOfProblems)
        {
            //first half of times through the loop return the same number, second half return the other number
            int factor;
            if (timeThroughLoop < numberOfProblems / 2) { factor = integerOne; }
            else { factor = integerTwo; }
            return factor;
        }
        private static int factorTwoGenerator(int integerOne, int integerTwo, int integerThree, int timeThroughLoop, int numberOfProblems)
        {
            //first third of times through the loop return the first number, second third return the second number, final third return the third number
            int factor;
            if (timeThroughLoop < numberOfProblems / 3) { factor = integerOne; }
            else if (timeThroughLoop < numberOfProblems * 2 / 3) { factor = integerTwo; }
            else { factor = integerThree; }
            return factor;
        }
        private static int factorTwoGenerator(int integerOne, int integerTwo, int integerThree, int integerFour, int timeThroughLoop, int numberOfProblems)
        {
            //first fourth of times through the loop return the first number, second fourth return the second number
            //thrid foruth return the thrid number, final fourth return the fourth number
            int factor;
            if (timeThroughLoop < numberOfProblems / 4) { factor = integerOne; }
            else if (timeThroughLoop < numberOfProblems * 2 / 4) { factor = integerTwo; }
            else if (timeThroughLoop < numberOfProblems * 3 / 4) { factor = integerThree; }
            else { factor = integerFour; }
            return factor;
        }
    }

}
