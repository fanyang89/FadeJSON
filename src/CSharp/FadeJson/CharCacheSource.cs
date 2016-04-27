using System.IO;

namespace FadeJson
{
    public class CharCacheSource : ICacheSource<TextReader, char>
    {
        public CharCacheSource(TextReader source) {
            Source = source;
        }

        public char Eof { get; } = '\uffff';

        public TextReader Source { get; }

        public char GetNextResult() {
            return unchecked((char)Source.Read());
        }

        public void ReadBlock(char[] buffer, int from, int to) {
            Source.ReadBlock(buffer, from, to - from);
        }
    }
}