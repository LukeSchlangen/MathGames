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
        public static Dictionary<string, int>[] determineProblems()
        {
            Dictionary<string, int>[] problemsToReturn = new Dictionary<string, int>[10];

            //Adding some examples to show how to use a dictionary.

            //To add a key/value pair to the array, simply specify which
            //element you wish to access (problemsToReturn[i]) and then use
            //the Add function to specify your key/value pair

            Dictionary<string, int> d = new Dictionary<string,int>();

            d.Add("factorOne", 1);
            d.Add("factorTwo", 2);
            d.Add("answer", 3);

            for (int i = 0; i < 10; i++)
            {
                problemsToReturn[i] = d;
            }

            //When you want to get the contents from a specific Dictionary
            //Choose once again the element you wish to access (problemsToReturn[i]) and then
            //use a pair of [] to specify a key for which value you want

            int factorOne = problemsToReturn[0]["factorOne"];
            int factorTwo = problemsToReturn[0]["factorTwo"];
            int answer = problemsToReturn[0]["answer"];

            //You probably won't need to pull the data from them in this class,
            //but in the game loop you'll need to access the info that you set.
            //Note: You'll throw an exception if you try to pull a value with an
            //invalid key, and their won't be a compilation error, so be careful

            //Also realize that because the type of the values all have to be ints
            //You'll need to parse the operators. 
            //ex. 0 -> +

            


            //This is where the magic happens...
            //TODO: Your algorithm here.

            return problemsToReturn;
        }
    }
}
