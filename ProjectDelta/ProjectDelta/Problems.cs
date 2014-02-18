using System;
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
            //A *better* example
            int randomSpot;
            numberOfProblems += 2;
            //This is the array of dictionary objects that we will return to the game loop
            Dictionary<string, int>[] problemsToReturn = new Dictionary<string, int>[numberOfProblems];

            //Let's just use a random number for the time being as our factors
            Random random = new Random();

            bool[] spotTaken = new bool[numberOfProblems];

            for (int i = 0; i < numberOfProblems; i++)
            {
                spotTaken[i] = false;
            }

            //This is where the magic happens...
            //TODO: Your algorithm here.


            for (int i = 0; i < numberOfProblems; i++)
            {
                //make an instance of the dictionary to be added to the array

                Dictionary<string, int> problemsDictionary = new Dictionary<string, int>();
                int operation = 0;
                int factorOne = 0;
                int factorTwo = 0;

                if (worldStage < 40) { operation = 0; } else if (worldStage < 117) { operation = 1; } else { operation = 2; }

                switch (worldStage)
                {
                    case 0: //0+0, 0+1, 1+0
                        //if (i > 7) { factorOne = 1; } else if (i > 3) { factorTwo = 1; }
                        factorOne = factorOneGenerator(0, 1, i);
                        factorTwo = 0;
                        break;
                    case 1: //small numbers + 0
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = 0;
                        break;
                    case 2: //small numbers + 1
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = 1;
                        break;
                    case 3: //small numbers + 1 or small numbers + 0
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(0, 1, i, numberOfProblems);
                        break;
                    case 4: //small numbers + 1 or small numbers + 2
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(1, 2, i, numberOfProblems);
                        break;
                    case 5: //small number + 2 or small number + 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(2, 3, i, numberOfProblems);
                        break;
                    case 6: //1 to 4 + 2 or 3
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(2, 3, i, numberOfProblems);
                        break;
                    case 7: // 2 to 5 + 2 or 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 8: // 2 to 5 + 4 or 3
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 9: // 4 to 7 + 1 or 2
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 10: // 3 to 6 + 4 or 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 11: // 3 to 6 + 5 or 4
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 12: // 4 to 7 + 2 or 3
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 13: // 4 to 7 + 4 or 3
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 14: // 4 to 7 + 5 or 4
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 15: // 5 to 8 + 1 or 2
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 16:
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 17: // 5 to 8 + 2 or 3
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 18: // 5 to 8 + 4 or 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 19: // 5 to 8 + 4 or 5
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 20: // 6 to 9 + 2 or 1
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(8, 7, i, numberOfProblems);
                        break;
                    case 21: // 6 to 9 + 2 or 3
                        factorOne = factorOneGenerator(4, 5, 6, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 22: // 6 to 9 + 4 or 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 23: // 6 to 9 + 4 or 5
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 24: // 3 to 6 + 6 or 5
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 25: // 4 to 7 + 6 or 5
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 26: // 4 to 7 + 6 or 7
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 27: // 5 to 8 + 6 or 7
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 28: // 5 to 8 + 8 or 7
                        factorOne = factorOneGenerator(4, 5, 6, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 29: // 6 to 9 + 5 or 6
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 30: // 6 to 9 + 7 or 6
                        factorOne = factorOneGenerator(5, 6, 7, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 31: // 6 to 9 + 7 or 8
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 32: // 6 to 9 + 9 or 8
                        factorOne = factorOneGenerator(5, 4, 6, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 33: // 6 to 9 + 9 or 10
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
                    case 39:
                        factorOne = factorOneGenerator(8, 9, 7, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;

                        //end of addition, begin subtraction
                    case 40:
                        factorOne = 1;
                        factorTwo = factorTwoGenerator(0,1, i, numberOfProblems);
                        break;
                    case 41:
                        factorOne = factorOneGenerator(1,2,i);
                        factorTwo = factorTwoGenerator(0, 1, i, numberOfProblems);
                        break;
                    case 42:
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
                        factorTwo = factorTwoGenerator(0, 1,2, i, numberOfProblems);
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
                        factorOne = factorOneGenerator(10,11, 12, i);
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
                    case 116:
                        factorOne = factorOneGenerator(15, 16, 17, 18, i);
                        factorTwo = 9;
                        break;

                       //end of subtraction begin multiplication
                    case 117: //0+0, 0+1, 1+0
                        //if (i > 7) { factorOne = 1; } else if (i > 3) { factorTwo = 1; }
                        factorOne = factorOneGenerator(0, 1, i);
                        factorTwo = 0;
                        break;
                    case 118: //small numbers + 0
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = 0;
                        break;
                    case 119: //small numbers + 1
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = 1;
                        break;
                    case 120: //small numbers + 1 or small numbers + 0
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(0, 1, i, numberOfProblems);
                        break;
                    case 121: //small numbers + 1 or small numbers + 2
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(1, 2, i, numberOfProblems);
                        break;
                    case 122: //small number + 2 or small number + 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(2, 3, i, numberOfProblems);
                        break;
                    case 123: //1 to 4 + 2 or 3
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(2, 3, i, numberOfProblems);
                        break;
                    case 124: // 2 to 5 + 2 or 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 125: // 2 to 5 + 4 or 3
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 126: // 4 to 7 + 1 or 2
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(3, 4, i, numberOfProblems);
                        break;
                    case 127: // 3 to 6 + 4 or 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 128: // 3 to 6 + 5 or 4
                        factorOne = factorOneGenerator(1, 2, 3, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 129: // 4 to 7 + 2 or 3
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 130: // 4 to 7 + 4 or 3
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(4, 5, i, numberOfProblems);
                        break;
                    case 131: // 4 to 7 + 5 or 4
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 132: // 5 to 8 + 1 or 2
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 133:
                        factorOne = factorOneGenerator(2, 3, 4, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 134: // 5 to 8 + 2 or 3
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 135: // 5 to 8 + 4 or 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 136: // 5 to 8 + 4 or 5
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 137: // 6 to 9 + 2 or 1
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(8, 7, i, numberOfProblems);
                        break;
                    case 138: // 6 to 9 + 2 or 3
                        factorOne = factorOneGenerator(4, 5, 6, i);
                        factorTwo = factorTwoGenerator(6, 5, i, numberOfProblems);
                        break;
                    case 139: // 6 to 9 + 4 or 3
                        factorOne = factorOneGenerator(0, 1, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 140: // 6 to 9 + 4 or 5
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 141: // 3 to 6 + 6 or 5
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 142: // 4 to 7 + 6 or 5
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 143: // 4 to 7 + 6 or 7
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 144: // 5 to 8 + 6 or 7
                        factorOne = factorOneGenerator(3, 1, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 145: // 5 to 8 + 8 or 7
                        factorOne = factorOneGenerator(4, 5, 6, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 146: // 6 to 9 + 5 or 6
                        factorOne = factorOneGenerator(3, 4, 2, i);
                        factorTwo = factorTwoGenerator(9, 8, i, numberOfProblems);
                        break;
                    case 147: // 6 to 9 + 7 or 6
                        factorOne = factorOneGenerator(5, 6, 7, i);
                        factorTwo = factorTwoGenerator(6, 7, i, numberOfProblems);
                        break;
                    case 148: // 6 to 9 + 7 or 8
                        factorOne = factorOneGenerator(3, 4, 5, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 149: // 6 to 9 + 9 or 8
                        factorOne = factorOneGenerator(5, 4, 6, i);
                        factorTwo = factorTwoGenerator(7, 8, i, numberOfProblems);
                        break;
                    case 150: // 6 to 9 + 9 or 10
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

                    default:
                        operation = 0;
                        factorOne = i;
                        factorTwo = i;
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

                //figure out the answer based on our factors, and set it accordingly
                //note: this assumes all questions are addition, more work will need to be
                //done for subtraction, multiplication, etc.

                //randomize order and determine order of problem
                randomSpot = random.Next(0, numberOfProblems);
                while (spotTaken[randomSpot] == true)
                {
                    randomSpot = random.Next(0, numberOfProblems);
                }

                spotTaken[randomSpot] = true;

                //place problem in return array
                problemsToReturn[randomSpot] = problemsDictionary;

            }

            //We now have an array containing 10 different dictionary objects, whose order is randomized
            return problemsToReturn;


        }

        private static int factorOneGenerator(int integerOne, int integerTwo, int timeThroughLoop)
        {
            int factor;
            if (timeThroughLoop % 2 == 0) { factor = integerOne; }
            else { factor = integerTwo; }
            return factor;
        }
        private static int factorOneGenerator(int integerOne, int integerTwo, int integerThree, int timeThroughLoop)
        {
            int factor;
            if (timeThroughLoop % 3 == 0) { factor = integerOne; }
            else if (timeThroughLoop % 3 == 1) { factor = integerTwo; }
            else { factor = integerThree; }
            return factor;
        }
        private static int factorOneGenerator(int integerOne, int integerTwo, int integerThree, int integerFour, int timeThroughLoop)
        {
            int factor;
            if (timeThroughLoop % 4 == 0) { factor = integerOne; }
            else if (timeThroughLoop % 4 == 1) { factor = integerTwo; }
            else if (timeThroughLoop % 4 == 2) { factor = integerThree; }
            else { factor = integerFour; }
            return factor;
        }

        private static int factorTwoGenerator(int integerOne, int integerTwo, int timeThroughLoop, int numberOfProblems)
        {
            int factor;
            if (timeThroughLoop < numberOfProblems / 2) { factor = integerOne; }
            else { factor = integerTwo; }
            return factor;
        }
        private static int factorTwoGenerator(int integerOne, int integerTwo, int integerThree, int timeThroughLoop, int numberOfProblems)
        {
            int factor;
            if (timeThroughLoop < numberOfProblems / 3) { factor = integerOne; }
            else if (timeThroughLoop < numberOfProblems * 2 / 3) { factor = integerTwo; }
            else { factor = integerThree; }
            return factor;
        }
        private static int factorTwoGenerator(int integerOne, int integerTwo, int integerThree, int integerFour, int timeThroughLoop, int numberOfProblems)
        {
            int factor;
            if (timeThroughLoop < numberOfProblems / 4) { factor = integerOne; }
            else if (timeThroughLoop < numberOfProblems * 2 / 4) { factor = integerTwo; }
            else if (timeThroughLoop < numberOfProblems * 3 / 4) { factor = integerThree; }
            else { factor = integerFour; }
            return factor;
        }
    }

}
