using System.Collections.Generic;
using System.Linq;

namespace Unittests.TestData
{
    public class InnerTestDetails
    {
        public string String { get; set; }
        public int Number { get; set; }
        public IEnumerable<object> Objects { get; set; }

        public string ThrowsException
        {
            get
            {
                throw new RottenTomato("Booo!");
            }
        }

        public InnerTestDetails()
        {
            Objects = Enumerable.Empty<object>();
        }
    }
}