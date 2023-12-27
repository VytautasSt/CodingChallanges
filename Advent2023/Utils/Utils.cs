using System.Collections.Generic;
using System.Linq;

namespace CodingChallanges.Advent2023.Utils;

public class Utils
{
    public static long GCF(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static long LCM(long a, long b)
    {
        return (a / GCF(a, b)) * b;
    }

    public static Hand[] QuicksortArray(Hand[] array, int leftIndex, int rightIndex)
    {
        var i = leftIndex;
        var j = rightIndex;
        var pivot = array[leftIndex];
        while (i <= j)
        {
            while (array[i] < pivot)
            {
                i++;
            }

            while (array[j] > pivot)
            {
                j--;
            }

            if (i <= j)
            {
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
                i++;
                j--;
            }
        }

        if (leftIndex < j)
            QuicksortArray(array, leftIndex, j);
        if (i < rightIndex)
            QuicksortArray(array, i, rightIndex);
        return array;
    }
}

internal class LeftRight
{
    private string left;
    private string right;
    public LeftRight(string left, string right)
    {
        this.left = left;
        this.right = right;
    }
    public string this[char c] => c == 'R' ? right : left;
}

public class Scope
{
    public bool Modified { get; set; }
    public long Start { get; private set; }
    public long Range { get; private set; }
    public long End { get; private set; }
    public Scope(long start, long range)
    {
        Start = start;
        Range = range;
        End = Start + Range;
    }
    /// <summary>
    /// modifies scope if needed and returns new scopes which need to be added
    /// </summary>
    public List<Scope> TryModifyScope(long destStr, long sourceStr, long range)
    {
        var sourceEnd = sourceStr + range;
        var diff = destStr - sourceStr;

        if (sourceStr <= Start && sourceEnd >= End)
        {
            ShiftScope(diff);
            return null;
        }
        if (sourceStr > Start && sourceEnd < End)
        {
            var scopeStr = CropScopeStart(sourceStr);
            var scopeEnd = CropScopeEnd(sourceEnd);
            ShiftScope(diff);
            return new List<Scope> { scopeStr, scopeEnd };
        }
        if (sourceStr > Start && sourceStr < End)
        {
            var scopeStr = CropScopeStart(sourceStr);
            ShiftScope(diff);
            return new List<Scope> { scopeStr };
        }
        if (sourceEnd > Start && sourceEnd < End)
        {
            var scopeEnd = CropScopeEnd(sourceEnd + 1);
            ShiftScope(diff);
            return new List<Scope> { scopeEnd };
        }
        return null;
    }
    private void ShiftScope(long diff)
    {
        Modified = true;
        Start += diff;
        End += diff;
    }
    /// <summary>
    /// returns croped scope
    /// </summary>
    private Scope CropScopeStart(long newStart)
    {
        var range = newStart - Start;
        var scopeCropped = new Scope(Start, range);

        Range -= range;
        Start = newStart;
        return scopeCropped;
    }
    /// <summary>
    /// returns cropped end
    /// </summary>
    private Scope CropScopeEnd(long newEnd)
    {
        var range = End - newEnd;
        var scopeCropped = new Scope(newEnd, range);

        Range -= range;
        End = newEnd;
        return scopeCropped;
    }
}

public class Map
{
    private List<(long Destination, long Source, long Range)> AsymPairs { get; } = new List<(long, long, long)>();
    public void AddAsymPairs(string destString, string sourceString, string rangeStr)
    {
        var range = long.Parse(rangeStr);
        var destStr = long.Parse(destString);
        var sourceStr = long.Parse(sourceString);
        AsymPairs.Add((destStr, sourceStr, range));
    }
    public long GetDestination(long source)
    {
        foreach (var pair in AsymPairs)
        {
            if (source >= pair.Source && source <= pair.Source + pair.Range)
                return source + (pair.Destination - pair.Source);
        }
        return source;
    }
}

public class Hand
{
    public string Cards { get; }
    public int Type { get; }
    public int Bet { get; }
    public Hand(string line, bool JIsWildcard = false)
    {
        if (JIsWildcard)
            values = "J23456789TQKA";
        else
            values = "23456789TJQKA";

        var parts = line.Split(' ');
        Cards = parts[0];
        Bet = int.Parse(parts[1]);
        Dictionary<char, int> kinds = new Dictionary<char, int>();
        int wildCards = 0;
        for (int i = 0; i < Cards.Length; i++)
        {
            if (Cards[i] == 'J' && JIsWildcard)
                wildCards++;
            else if (kinds.ContainsKey(Cards[i]))
                kinds[Cards[i]]++;
            else
                kinds.Add(Cards[i], 1);
        }
        if (wildCards > 0)
        {
            if (wildCards == 5)
            {
                kinds.Add('J', 5);
            }
            else
            {
                // find max kinds.Value and plus it
                var val = kinds.Max(x => x.Value);
                var maxKind = kinds.First(x => x.Value == val);
                kinds[maxKind.Key] = maxKind.Value + wildCards;
            }
        }
        switch (kinds.Count)
        {
            case 5:
                Type = 0;
                break;
            case 4:
                Type = 1;
                break;
            case 3:// two-pairs or three-of-a-kind
                if (kinds.Any(k => k.Value == 2))
                    Type = 2;
                else
                    Type = 3;
                break;
            case 2:// full-house or four-of-a-kind
                if (kinds.Any(k => k.Value == 3))
                    Type = 4;
                else
                    Type = 5;
                break;
            case 1:
                Type = 6;
                break;
        }
    }
    public static bool operator >(Hand a, Hand b)
    {
        if (a.Type != b.Type) return a.Type > b.Type;
        for (int i = 0; i < 5; i++)
        {
            if (a.Cards[i] != b.Cards[i])
                return a.GetValueOfACard(i) > b.GetValueOfACard(i);
        }
        return false;
    }
    public static bool operator <(Hand a, Hand b)
    {
        if (a.Type != b.Type) return a.Type < b.Type;
        for (int i = 0; i < 5; i++)
        {
            if (a.Cards[i] != b.Cards[i])
                return a.GetValueOfACard(i) < b.GetValueOfACard(i);
        }
        return false;
    }
    private readonly string values = "J23456789TQKA";
    public int GetValueOfACard(int cardId) => values.IndexOf(Cards[cardId]);
}

/// <summary>
/// For advent day 3
/// </summary>
public class SpecNumber
{
    public int Number { get; }
    public List<Symbol> Symbols { get; }
    public SpecNumber(int nr)
    {
        Symbols = new List<Symbol>();
        Number = nr;
    }
}

public class Symbol
{
    public static bool IsSymbol(char c) => !char.IsDigit(c) && c != '.';
    public char Value { get; }
    public int Row { get; }
    public int Column { get; }
    public Symbol(char symbol, int row, int col)
    {
        Value = symbol;
        Row = row;
        Column = col;
    }
}
