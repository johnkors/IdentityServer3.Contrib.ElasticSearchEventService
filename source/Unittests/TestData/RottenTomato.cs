using System;

namespace Unittests.TestData
{
    public class RottenTomato : Exception
    {
        public RottenTomato(string message) : base(message)
        {
        }
    }
}