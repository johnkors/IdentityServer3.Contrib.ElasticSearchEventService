using System.Collections.Generic;

namespace Unittests.TestData
{
    public class InnerObject
    {
        public string Value { get; set; }
        public IList<int> Numbers { get; set; }

        public InnerObject()
        {
            Numbers = new List<int>();
        }
    }
}