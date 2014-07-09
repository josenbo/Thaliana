namespace BongApiV1.Public
{
    public class BongNoSuchItemException : BongException
    {
        public BongNoSuchItemException() : base("There is no such item") { }
        public BongNoSuchItemException(string message) : base(message) { }
    }
}
