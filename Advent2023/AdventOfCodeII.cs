using CodingChallanges.Advent2023.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace CodingChallanges.Advent2023;

public static partial class AdventOfCode
{
    /// <summary>
    /// --- Day 9: Mirage Maintenance ---
    /// You pull out your handy Oasis And Sand Instability Sensor and analyze your surroundings.
    /// The OASIS produces a report of many values and how they are changing over time (your
    /// puzzle input). Each line in the report contains the history of a single value. For example:
    ///   0 3 6 9 12 15
    ///   1 3 6 10 15 21
    ///   10 13 16 21 30 45
    /// To best protect the oasis, your environmental report should include a prediction of the
    /// next value in each history. To do this, start by making a new sequence from the difference
    /// at each step of your history. If that sequence is not all zeroes, repeat this process,
    /// using the sequence you just generated as the input sequence. Once all of the values in
    /// your latest sequence are zeroes, you can extrapolate what the next value of the original
    /// history should be.
    /// In the above dataset, the first history is 0 3 6 9 12 15. Because the values increase by
    /// 3 each step, the first sequence of differences that you generate will be 3 3 3 3 3. Note
    /// that this sequence has one fewer value than the input sequence because at each step it 
    /// considers two numbers from the input. Since these values aren't all zero, repeat the 
    /// process: the values differ by 0 at each step, so the next sequence is 0 0 0 0. This means
    /// you have enough information to extrapolate the history! Visually, these sequences can be
    /// arranged like this:
    /// 0   3   6   9  12  15
    ///   3   3   3   3   3
    ///     0   0   0   0
    /// To extrapolate, start by adding a new zero to the end of your list of zeroes; because 
    /// the zeroes represent differences between the two values above them, this also means there
    /// is now a placeholder in every sequence above it:
    /// 0   3   6   9  12  15   B
    ///   3   3   3   3   3   A
    ///     0   0   0   0   0
    /// You can then start filling in placeholders from the bottom up. A needs to be the result
    /// of increasing 3 (the value to its left) by 0 (the value below it); this means A must be 3:
    /// 0   3   6   9  12  15   B
    ///   3   3   3   3   3   3
    ///     0   0   0   0   0
    /// Finally, you can fill in B, which needs to be the result of increasing 15 (the value to
    /// its left) by 3 (the value below it), or 18:
    /// 0   3   6   9  12  15  18
    ///   3   3   3   3   3   3
    ///     0   0   0   0   0
    /// So, the next value of the first history is 18.
    /// Finding all-zero differences for the second history requires an additional sequence:
    /// 1   3   6  10  15  21
    ///   2   3   4   5   6
    ///     1   1   1   1
    ///       0   0   0
    /// Then, following the same process as before, work out the next value in each sequence 
    /// from the bottom up:
    /// 1   3   6  10  15  21  28
    ///   2   3   4   5   6   7
    ///     1   1   1   1   1
    ///       0   0   0   0
    /// So, the next value of the second history is 28.
    /// The third history requires even more sequences, but its next value can be found the 
    /// same way:
    /// 10  13  16  21  30  45  68
    ///    3   3   5   9  15  23
    ///      0   2   4   6   8
    ///        2   2   2   2
    ///          0   0   0
    /// So, the next value of the third history is 68.
    /// If you find the next value for each history in this example and add them together, you
    /// get 114. Analyze your OASIS report and extrapolate the next value for each history.
    /// What is the sum of these extrapolated values?
    /// Answer should be 1696140818
    /// </summary>
    public static long Day9Part1()
    {
        var input = File.ReadAllLines("advent2023\\input\\9.txt");
        var sum = 0;

        foreach (var line in input)
        {
            var ordersOfValues = Day9GetOrdersOfValues(line);

            while (ordersOfValues.Count > 1)
            {
                var lastDiff = ordersOfValues.Last().Last();
                var beforeLastOrder = ordersOfValues[^2];
                beforeLastOrder.Add(beforeLastOrder.Last() + lastDiff);
                ordersOfValues.RemoveAt(ordersOfValues.Count - 1);
            }
            sum += ordersOfValues.First().Last();
        }

        return sum;
    }

    /// <summary>
    /// For each history, repeat the process of finding differences until the sequence of
    /// differences is entirely zero. Then, rather than adding a zero to the end and filling
    /// in the next values of each previous sequence, you should instead add a zero to the
    /// beginning of your sequence of zeroes, then fill in new first values for each previous
    /// sequence. In particular, here is what the third example history looks like when
    /// extrapolating back in time:
    /// 5  10  13  16  21  30  45
    ///   5   3   3   5   9  15
    ///    -2   0   2   4   6
    ///       2   2   2   2
    ///         0   0   0
    /// Adding the new values on the left side of each sequence from bottom to top eventually
    /// reveals the new left-most history value: 5. Doing this for the remaining example data
    /// above results in previous values of -3 for the first history and 0 for the second history.
    /// Adding all three new values together produces 2. Analyze your OASIS report again, this 
    /// time extrapolating the previous value for each history. What is the sum of these extrapolated values?
    /// Answer should be 1152
    /// </summary>
    public static long Day9Part2()
    {
        var input = File.ReadAllLines("advent2023\\input\\9.txt");
        var sum = 0;

        foreach (var line in input)
        {
            var ordersOfValues = Day9GetOrdersOfValues(line);

            while (ordersOfValues.Count > 1)
            {
                var lastDiff = ordersOfValues.Last().First();
                var beforeLastOrder = ordersOfValues[^2];
                beforeLastOrder.Insert(0, beforeLastOrder.First() - lastDiff);//add in first space
                ordersOfValues.RemoveAt(ordersOfValues.Count - 1);
            }
            sum += ordersOfValues.First().First();
        }

        return sum;
    }

    private static List<List<int>> Day9GetOrdersOfValues(string line)
    {
        var ordersOfValues = new List<List<int>>();
        ordersOfValues.Add(line.Split(' ').Select(p => int.Parse(p)).ToList());

        while (ordersOfValues.Last().Any(x => x != 0))
        {
            var lastOrder = ordersOfValues.Last();
            var newOrder = new List<int>();
            for (int i = 1; i < lastOrder.Count; i++)
                newOrder.Add(lastOrder[i] - lastOrder[i - 1]);

            ordersOfValues.Add(newOrder);
        }

        return ordersOfValues;
    }

    /// <summary>
    /// --- Day 10: Pipe Maze ---
    /// The pipes are arranged in a two-dimensional grid of tiles:
    /// | is a vertical pipe connecting north and south.
    /// - is a horizontal pipe connecting east and west.
    /// L is a 90-degree bend connecting north and east.
    /// J is a 90-degree bend connecting north and west.
    /// 7 is a 90-degree bend connecting south and west.
    /// F is a 90-degree bend connecting south and east.
    /// . is ground; there is no pipe in this tile.
    /// S is the starting position of the animal; there is a pipe on this tile,
    /// but your sketch doesn't show what shape the pipe has.
    /// Based on the acoustics of the animal's scurrying, you're confident the pipe that contains 
    /// the animal is one large, continuous loop.
    /// For example, here is a square loop of pipe:
    ///   .....
    ///   .F-7.
    ///   .|.|.
    ///   .L-J.
    ///   .....
    /// If the animal had entered this loop in the northwest corner, 
    /// the sketch would instead look like this:
    ///   .....
    ///   .S-7.
    ///   .|.|.
    ///   .L-J.
    ///   .....
    /// In the above diagram, the S tile is still a 90-degree F bend: you can tell because of how 
    /// the adjacent pipes connect to it.
    /// Unfortunately, there are also many pipes that aren't connected to the loop! This sketch 
    /// shows the same loop as above:
    ///   -L|F7
    ///   7S-7|
    ///   L|7||
    ///   -L-J|
    ///   L|-JF
    /// In the above diagram, you can still figure out which pipes form the main loop: they're 
    /// the ones connected to S, pipes those pipes connect to, pipes those pipes connect to, and
    /// so on. Every pipe in the main loop connects to its two neighbors (including S, which will
    /// have exactly two pipes connecting to it, and which is assumed to connect back to those two pipes).
    /// Here is a sketch that contains a slightly more complex main loop:
    ///   ..F7.
    ///   .FJ|.
    ///   SJ.L7
    ///   |F--J
    ///   LJ...
    /// Here's the same example sketch with the extra, non-main-loop pipe tiles also shown:
    ///   7-F7-
    ///   .FJ|7
    ///   SJLL7
    ///   |F--J
    ///   LJ.LJ
    /// If you want to get out ahead of the animal, you should find the tile in the loop that 
    /// is farthest from the starting position.Because the animal is in the pipe, it doesn't 
    /// make sense to measure this by direct distance. Instead, you need to find the tile that
    /// would take the longest number of steps along the loop to reach from the starting point
    /// - regardless of which way around the loop the animal went.
    /// In the first example with the square loop:
    ///   .....
    ///   .S-7.
    ///   .|.|.
    ///   .L-J.
    ///   .....
    /// You can count the distance each tile in the loop is from the starting point like this:
    ///   .....
    ///   .012.
    ///   .1.3.
    ///   .234.
    ///   .....
    /// In this example, the farthest point from the start is 4 steps away.
    /// Here's the more complex loop again:
    ///   ..F7.
    ///   .FJ|.
    ///   SJ.L7
    ///   |F--J
    ///   LJ...
    /// Here are the distances for each tile on that loop:
    ///   ..45.
    ///   .236.
    ///   01.78
    ///   14567
    ///   23...
    /// Find the single giant loop starting at S. How many steps along the loop does it take to
    /// get from the starting position to the point farthest from the starting position?
    /// Answer should be 7005
    /// </summary>
    public static long Day10Part1()
    {
        var input = File.ReadAllLines("advent2023\\input\\10.txt");
        var map = input.Select(l => l.ToArray()).ToArray();
        (int R, int C) s = Day10FindS(input);

        // try go up
        var seq = Day10GetPipeSeq(map, s, (s.R - 1, s.C));
        // try go right
        seq ??= Day10GetPipeSeq(map, s, (s.R, s.C + 1));
        // try go down
        seq ??= Day10GetPipeSeq(map, s, (s.R + 1, s.C));

        return seq.Count / 2;
    }

    /// <summary>
    /// Calculating the area within the loop. How many tiles are enclosed by the loop?
    /// Answer should be 417
    /// </summary>
    public static long Day10Part2()
    {
        var input = File.ReadAllLines("advent2023\\input\\10.txt");
        var map = input.Select(l => l.ToArray()).ToArray();
        (int R, int C) s = Day10FindS(input);

        // try go up
        var seq = Day10GetPipeSeq(map, s, (s.R - 1, s.C));
        // if null try go right
        seq ??= Day10GetPipeSeq(map, s, (s.R, s.C + 1));
        // if null try go down
        seq ??= Day10GetPipeSeq(map, s, (s.R + 1, s.C));

        // fix thingy with S
        var prevTile = seq[^2];
        var nextTile = seq[1];
        Day10ReplaceSWithPipe(map, prevTile, s, nextTile);

        var startRow = seq.Min(tile => tile.r);
        var endRow = seq.Max(tile => tile.r);
        var startCol = seq.Min(tile => tile.c);
        var endCol = seq.Max(tile => tile.c);
        var innerTiles = 0;

        for (int r = startRow + 1; r < endRow; r++)
        {
            var counting = false;
            var horiCorner = '.';// horizontal beggining corner
            
            for (var c = startCol; c < endCol; c++)
            {
                if (seq.Contains((r, c)) && (map[r][c] == '|' || map[r][c] == 'J' || map[r][c] == '7'))
                {
                    if ((map[r][c] == 'J' && horiCorner == 'L') || (map[r][c] == '7' && horiCorner == 'F'))
                        continue;

                    counting = !counting;
                }
                else if (seq.Contains((r, c)) && (map[r][c] == 'L' || map[r][c] == 'F'))
                {
                    horiCorner = map[r][c];
                }
                else if (!seq.Contains((r, c)) && counting)
                {
                    innerTiles += 1;
                } 
            }
        }
        return innerTiles;
    }

    private static void Day10ReplaceSWithPipe(
        char[][] map, (int r, int c) prevTile, (int r, int c) sTile, (int r, int c) nextTile)
    {
        (int r, int c) delta = (nextTile.r - prevTile.r, nextTile.c - prevTile.c);
        var nextChar = map[nextTile.r][nextTile.c];
        var prevChar = map[prevTile.r][prevTile.c];

        if (delta.r == 0)
        {
            map[sTile.r][sTile.c] = '-';
        }
        else if (delta.c == 0)
        {
            map[sTile.r][sTile.c] = '|';
        }
        else if (((delta.r == 1 && delta.c == 1) && (prevChar == '|' || prevChar == '7' || prevChar == 'F')) ||
               ((delta.r == -1 && delta.c == -1) && (nextChar == '|' || nextChar == '7' || nextChar == 'F')))
        {
            map[sTile.r][sTile.c] = 'L';
        }
        else if (((delta.r == 1 && delta.c == 1) && (nextChar == '|' || nextChar == 'L' || nextChar == 'J')) ||
               ((delta.r == -1 && delta.c == -1) && (prevChar == '|' || prevChar == 'L' || prevChar == 'J')))
        {
            map[sTile.r][sTile.c] = '7';
        }
        else if (((delta.r == 1 && delta.c == -1) && (prevChar == '|' || prevChar == '7' || prevChar == 'F')) ||
                 ((delta.r == -1 && delta.c == 1) && (nextChar == '|' || nextChar == '7' || nextChar == 'F')))
        {
            map[sTile.r][sTile.c] = 'J';
        }
        else if (((delta.r == 1 && delta.c == -1) && (nextChar == '|' || nextChar == 'L' || nextChar == 'J')) ||
                 ((delta.r == -1 && delta.c == 1) && (prevChar == '|' || prevChar == 'L' || prevChar == 'J')))
        {
            map[sTile.r][sTile.c] = 'F';
        }
    }

    private static (int R, int C) Day10FindS(string[] input)
    {
        for (int r = 0; r < input.Length; r++)
        {
            if (input[r].Contains('S'))
            {
                var c = input[r].IndexOf('S');
                return (r, c);
            }
        }
        return (0, 0);
    }

    private static bool Day10AreTilesConnected(char[][] map, (int r, int c) prevTile, (int r, int c) tile)
    {
        if (tile.r < 0 ||
            tile.r >= map.Length ||
            tile.c < 0 ||
            tile.c >= map[0].Length ||
            map[tile.r][tile.c] == '.')
            return false;

        var symbol = map[tile.r][tile.c];
        (int r, int c) delta = (tile.r - prevTile.r, tile.c - prevTile.c);

        //check left-to-right connection
        if (delta.c == 1 && (symbol == '|'|| symbol == 'L' || symbol == 'F'))
                return false;
        //check right-to-left connection
        if (delta.c == -1 && (symbol == '|' || symbol == 'J' || symbol == '7'))
            return false;
        //check down-to-up connection
        if (delta.r == -1 && (symbol == '-' || symbol == 'J' || symbol == 'L'))
            return false;
        //check up-to-down connection
        if (delta.r == 1 && (symbol == '-' || symbol == 'F' || symbol == '7'))
            return false;
        
        return true;
    }

    private static List<(int r, int c)> Day10GetPipeSeq(
        char[][] map, 
        (int r, int c) prevTile, 
        (int r, int c) nextTile)
    {
        var seq = new List<(int r, int c)>();
        var chars = new List<char>();
        seq.Add(prevTile);
        chars.Add(map[prevTile.r][prevTile.c]);

        while (Day10AreTilesConnected(map, seq[^1], nextTile))
        {
            var currentTile = nextTile;
            seq.Add(currentTile);
            chars.Add(map[currentTile.r][currentTile.c]);
            var currentTileSymbol = map[currentTile.r][currentTile.c];
            if (currentTileSymbol == 'S')
                return seq;

            prevTile = seq[^2];
            var prevDelta = (currentTile.r - prevTile.r, currentTile.c - prevTile.c);

            switch (prevDelta)
            {
                case (-1, 0): // came from down
                    if (currentTileSymbol == '|')
                        nextTile = (currentTile.r - 1, currentTile.c);
                    else if (currentTileSymbol == 'F')
                        nextTile = (currentTile.r, currentTile.c + 1);
                    else if (currentTileSymbol == '7')
                        nextTile = (currentTile.r, currentTile.c - 1);
                    break;
                case (0, 1): // came from left
                    if (currentTileSymbol == '-')
                        nextTile = (currentTile.r, currentTile.c + 1);
                    else if (currentTileSymbol == 'J')
                        nextTile = (currentTile.r - 1, currentTile.c);
                    else if (currentTileSymbol == '7')
                        nextTile = (currentTile.r + 1, currentTile.c);
                    break;
                case (1, 0): // came from up
                    if (currentTileSymbol == '|')
                        nextTile = (currentTile.r + 1, currentTile.c);
                    else if (currentTileSymbol == 'J')
                        nextTile = (currentTile.r, currentTile.c - 1);
                    else if (currentTileSymbol == 'L')
                        nextTile = (currentTile.r, currentTile.c + 1);
                    break;
                case (0, -1): // came from right
                    if (currentTileSymbol == '-')
                        nextTile = (currentTile.r, currentTile.c - 1);
                    else if (currentTileSymbol == 'F')
                        nextTile = (currentTile.r + 1, currentTile.c);
                    else if (currentTileSymbol == 'L')
                        nextTile = (currentTile.r - 1, currentTile.c);
                    break;
            }
        }

        if (seq.Count == 1 || chars.Last() != 'S')
            return null;
        return seq;
    }

    public static long Day11Part1()
    {
        return 0;
    }
}
