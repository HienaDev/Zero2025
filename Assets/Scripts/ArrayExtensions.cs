using System;

public static class ArrayExtensions
{
    private static Random rng = new Random();

    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (array[k], array[n]) = (array[n], array[k]); // tuple swap
        }
    }
}
