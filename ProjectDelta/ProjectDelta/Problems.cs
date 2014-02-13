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
        public static Dictionary<string, int>[] determineProblems(int worldStage)
        {
            //A *better* example

            //This is the array of dictionary objects that we will return to the game loop
            Dictionary<string, int>[] problemsToReturn = new Dictionary<string, int>[12];

            //Let's just use a random number for the time being as our factors
            Random random = new Random();

            //This is where the magic happens...
            //TODO: Your algorithm here.

            for (int i = 0; i < 12; i++)
            {
                //make an instance of the dictionary to be added to the array
                //this is where I was concerned with scope. If this dictionary
                //object is outside the for-loop, you end up adding a bunch of the
                //same keys with different values (i.e. "factorOne"/1, "factorOne"/5)
                //to the same dictionary

                Dictionary<string, int> problemsDictionary = new Dictionary<string, int>();

                //set the values of the factors to be some random number between 1 and 9
                problemsDictionary.Add("operation", i/3);
                problemsDictionary.Add("factorOne", i + worldStage);
                problemsDictionary.Add("factorTwo", i + worldStage);

                //figure out the answer based on our factors, and set it accordingly
                //note: this assumes all questions are addition, more work will need to be
                //done for subtraction, multiplication, etc.

                problemsDictionary.Add("answer", problemsDictionary["factorOne"] + problemsDictionary["factorTwo"]);
                problemsToReturn[i] = problemsDictionary;
            }

            //We now have an array containing 10 different dictionary objects, whose contents are randomized
            //in the above for-loop. I wouldn't worry about randomizing the objects here, but rather take care
            //of that in the actually game loop with another random

            return problemsToReturn;
        }
    }
}
