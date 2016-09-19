namespace FadeJSON.Serialization
{
    public static class SerializationUtils
    {
        public static string Serialize(this object obj) {
            return Json.SerializeDynamic(obj);
        }
    }
}