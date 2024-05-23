namespace TriangelsSystem.CommonGraphics.Helpers;

public static class RandomHelper
{
    private static readonly Random _random = new(DateTime.Now.Millisecond);

    static RandomHelper()
    {
    }

    public static bool Chance(double chance)
    {
        return _random.NextDouble() < chance;
    }

    public static int Next(int min, int max)
    {
        lock (_random)
        {
            return _random.Next(min, max);
        }
    }
    public static double Next(float min, float max)
    {
        lock (_random)
        {
            return _random.NextDouble() * (max - min) + min;
        }
    }

    public static double NextDouble()
    {
        lock (_random)
        {
            return _random.NextDouble();
        }
    }

    public static TElement TakeRandom<TElement>(this IEnumerable<TElement> elements)
    {
        var list = elements.ToList();
        var count = list.Count;
        var number = Next(0, count);
        return list[number];
    }
}
