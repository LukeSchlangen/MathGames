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

                switch (worldStage)
                {
                    case 0: //0+0, 0+1, 1+0
                        if (i > 7) { factorOne = 1; } else if (i > 3) { factorTwo = 1; }
                        break;
                    case 1: //small numbers + 0
                        if (i > 7) { factorOne = i - 6; } else if (i > 3) { factorTwo = i - 4; } else { factorTwo = i; }
                        break;
                    case 2: //small numbers + 1
                        factorOne = 1;
                        factorTwo = 1;
                        if (i > 7) { factorOne = i - 6; } else if (i > 3) { factorTwo = i - 4; } else { factorTwo = i; }
                        break;
                    case 3: //small numbers + 1 or small numbers + 0
                        if (i / 2 == 1)
                        {
                            factorOne = 1;
                            factorTwo = 1;
                        }
                        if (i > 7) { factorOne = i - 6; } else if (i > 3) { factorTwo = i - 4; } else { factorTwo = i; }
                        break;
                    default:
                        factorOne = i;
                        factorTwo = i;
                        break;
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

            //We now have an array containing 10 different dictionary objects, whose contents are randomized
            return problemsToReturn;
        }

    }
}
