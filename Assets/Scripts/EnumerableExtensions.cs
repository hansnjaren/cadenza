using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Cycle<T>(this IEnumerable<T> source)
    {
        // Cache the sequence in a list to allow looping
        var cache = source.ToList();

        // If the source is empty, we're done
        if (!cache.Any())
        {
            yield break;
        }

        // The infinite loop
        while (true)
        {
            foreach (var item in cache)
            {
                yield return item;
            }
        }
    }
}
