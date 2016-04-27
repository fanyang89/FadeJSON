namespace FadeJson
{
    public interface ICacheSource<out TSource, TResult>
    {
        TSource Source { get; }
        TResult Eof { get; }
        TResult GetNextResult();
        void ReadBlock(TResult[] buffer, int from, int to);
    }
}