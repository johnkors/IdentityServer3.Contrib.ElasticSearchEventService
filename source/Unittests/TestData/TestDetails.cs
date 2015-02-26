namespace Unittests.TestData
{
    public class TestDetails
    {
        public string StringField;
        public string String { get; set; }
        public InnerTestDetails Inner { get; set; }
        public string ThrowsException { get { throw new RottenTomato("Booo!"); } }

        public TestDetails()
        {
            Inner = new InnerTestDetails();
        }
    }
}