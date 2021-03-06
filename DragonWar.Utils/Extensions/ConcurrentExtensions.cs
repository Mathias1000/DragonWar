﻿using System.Collections.Concurrent;
public static class ConcurrentExtensions
{

    public static void Fill(this ConcurrentQueue<int> Queue, int Start = int.MinValue, int End = int.MaxValue, bool Reset = true)
    {
        var isReset = false;

        for (int i = End; i >= Start; i--)
        {
            if (Reset
                && i == Start)
            {
                if (isReset)
                    break;
                else
                    isReset = true;

            }


            Queue.Enqueue(i);
        }
    }
    public static void Fill(this ConcurrentQueue<ushort> Queue, ushort Start = ushort.MinValue, ushort End = ushort.MaxValue, bool Reset = true)
    {
        var isReset = false;

        for (ushort i = End; i >= Start; i--)
        {
            if (Reset
                && i == Start)
            {
                if (isReset)
                    break;
                else
                    isReset = true;

            }


            Queue.Enqueue(i);
        }
    }
}