namespace FadeJson
{
    public class TokenCacheSource : ICacheSource<Tokenizer, JsonValue>
    {
        public TokenCacheSource(Tokenizer source) {
            Source = source;
        }

        public JsonValue Eof { get; } = JsonValue.Eof;

        public Tokenizer Source { get; }

        public JsonValue GetNextResult() {
            return Source.GetNextToken();
        }

        public void ReadBlock(JsonValue[] buffer, int from, int to) {
            for (var i = from; i < to; i++) {
                buffer[i] = Source.GetNextToken();
                if (!buffer[i].Equals(JsonValue.Eof)) {
                    continue;
                }
                break;
            }
        }
    }
}