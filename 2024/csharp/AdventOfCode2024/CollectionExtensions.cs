using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024;

public static class CollectionExtensions
{
    public static bool IsEmpty<TItem>(this IEnumerable<TItem> source)
    {
        return !source.Any();
    }

    public static int MultiplyAll(this IEnumerable<int> source)
    {
        return source.Aggregate((product, next) => product * next);
    }

    public static IEnumerable<(TFirst, TSecond)> CrossProduct<TFirst, TSecond>(this ICollection<TFirst> first,
        ICollection<TSecond> second)
    {
        return
            from firstItem in first
            from secondItem in second
            select (firstItem, secondItem);
    }

    public static TItem Middle<TItem>(this IList<TItem> collection)
    {
        var middleIndex = collection.Count / 2;
        return collection[middleIndex];
    }
}