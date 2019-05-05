
using System.Collections.Generic;
using System.Linq;
namespace Pure.Data 
{
    public class BatchOptions
    {
        public BatchOptions()
        {
            BatchSize = 10000;
            StatementSeperator = ";";
        }

        public int BatchSize { get; set; }
        public string StatementSeperator { get; set; }
    }

    public static class BatchingExtensions
    {
        public static IEnumerable<T[]> Chunkify<T>(this IEnumerable<T> items, int chunkSize)
        {
            var enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return Take(enumerator, chunkSize).ToArray();
            }
        }

        private static IEnumerable<T> Take<T>(IEnumerator<T> enumerator, int num)
        {
            do
            {
                yield return enumerator.Current;
            } while (--num > 0 && enumerator.MoveNext());
        }
    }
}
