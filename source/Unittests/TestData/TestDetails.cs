using System;

namespace Unittests.TestData
{
    public class TestDetails
    {
        public string StringField;
        public string String { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public int Number { get; set; }
        public double? NullableDouble { get; set; }
        public TestEnum TestEnum { get; set; }
        public TestStruct TestStruct { get; set; }
        public InnerTestDetails Inner { get; set; }
        public string ThrowsException { get { throw new RottenTomato("Booo!"); } }

        public TestDetails()
        {
            Inner = new InnerTestDetails();
        }
    }
}