namespace FadeJSON.Serialization
{
    internal interface ISerialize<in T>
    {
        string Serialize(T obj);
    }
}