using System.Collections.Generic;

namespace FadeJson
{
    public interface ICommonCache<T, out TSourceProvider>
    {
        bool Check(IEnumerable<T> list);
        T Lookahead(int index = 0);
        T Next();
        void Next(int count);
    }
}