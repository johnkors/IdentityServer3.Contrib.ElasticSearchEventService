namespace Unittests.TestData
{
    public struct TestStruct
    {
        public string Value { get; private set; }

        public TestStruct(string value) : this()
        {
            Value = value;
        }
    }
}