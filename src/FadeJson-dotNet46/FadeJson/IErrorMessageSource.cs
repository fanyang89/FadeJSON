namespace FadeJson
{
    public interface IErrorMessageSource
    {
        int LineNumber { get; }
        int LinePosition { get; }
    }
}