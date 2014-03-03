using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectDelta
{
    class QuestionFormat
    {
        public string question(int operationValue, int factorOne, int factorTwo)
        {
            string question = "";
            if (operationValue == 0)
            {
                question = factorOne + " + " + factorTwo;
            }
            else if (operationValue == 1)
            {
                question = factorOne + " - " + factorTwo;
            }
            else if (operationValue == 2)
            {
                question = factorOne + " * " + factorTwo;
            }
            else if (operationValue == 3)
            {
                question = factorOne + " / " + factorTwo;
            }
            return question;
        }

        public int getExpectedAnswer(int operationValue, int factorOne, int factorTwo)
        {
            switch (operationValue)
            {
                case 0:
                    return factorOne + factorTwo;
                case 1:
                    return factorOne - factorTwo;
                case 2:
                    return factorOne * factorTwo;
                case 3:
                    return factorOne / factorTwo;
                default:
                    return 0;
            }
        }

        public string questionAndCorrectAnswer(int operationValue, int factorOne, int factorTwo)
        {
            string questionAndAnswer = question(operationValue, factorOne, factorTwo);
            questionAndAnswer += " = " + getExpectedAnswer(operationValue, factorOne, factorTwo);
            return questionAndAnswer;
        }
    }
}
