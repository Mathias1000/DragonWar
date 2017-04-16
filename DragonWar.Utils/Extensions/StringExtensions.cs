using System;
using System.Text;
using System.IO;
using DragonWar.Utils;

public static class StringExtensions
{
    public static string ToEscapedString(this string Input, string EscapeSign = "\\")
    {
        return (Input + EscapeSign).Replace(EscapeSign + EscapeSign, EscapeSign);
    }
    public static string ToAbsolutePath(this string Input, string BaseDirectory = null)
    {
        var path = Input;

        if (!Path.IsPathRooted(path))
        {
            path = Path.GetFullPath(Path.Combine((BaseDirectory ?? AppDomain.CurrentDomain.BaseDirectory), Input));
        }

        return path;
    }
    public static string ToTrimedLine(this string Input)
    {
        var output = Input.Replace("\"", "").Replace(",", "").Trim();

        while (output.Contains("		"))
        {
            output = output.Replace("		", "	");
        }

        return output;
    }

    public static string ToString(this string[] Input)
    {
        //
        // Concatenate all the elements into a StringBuilder.
        //
        StringBuilder builder = new StringBuilder();
        foreach (string value in Input)
        {
            builder.Append(value);
            builder.Append(' ');
        }
        return builder.ToString();
    }

    public static string ToFiestaString(this string Input, params Pair<string, string>[] Replacers)
    {
        var outSring = Input;

        for (int i = 0; i < Replacers.Length; i++)
        {
            var rep = Replacers[i];
            var inx = outSring.IndexOf(rep.First);

            if (inx >= 0)
            {
                outSring = outSring.Remove(inx, rep.First.Length);
                outSring = outSring.Insert(inx, rep.Second);
            }
        }

        return outSring;
    }
}