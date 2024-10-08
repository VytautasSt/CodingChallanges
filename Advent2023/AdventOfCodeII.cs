﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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


    /// <summary>
    /// --- Day 11: Cosmic Expansion ---
    /// The researcher has collected a bunch of data and compiled the data into a single giant image. 
    /// The image includes empty space (.) and galaxies (#). For example:
    ///   ...#......
    ///   .......#..
    ///   #.........
    ///   ..........
    ///   ......#...
    ///   .#........
    ///   .........#
    ///   ..........
    ///   .......#..
    ///   #...#.....
    /// The researcher is trying to figure out the sum of the lengths of the shortest path between
    /// every pair of galaxies. However, there's a catch: the universe expanded in the time it took
    /// the light from those galaxies to reach the observatory.
    /// Due to something involving gravitational effects, only some space expands. In fact, the 
    /// result is that any rows or columns that contain no galaxies should all actually be twice as big.
    /// In the above example, three columns and two rows contain no galaxies:
    ///      v  v  v
    ///    ...#......
    ///    .......#..
    ///    #.........
    ///   >..........<
    ///    ......#...
    ///    .#........
    ///    .........#
    ///   >..........<
    ///    .......#..
    ///    #...#.....
    ///      ^  ^  ^
    /// These rows and columns need to be twice as big; the result of cosmic expansion therefore
    /// looks like this:
    ///   ....#........
    ///   .........#...
    ///   #............
    ///   .............
    ///   .............
    ///   ........#....
    ///   .#...........
    ///   ............#
    ///   .............
    ///   .............
    ///   .........#...
    ///   #....#.......
    /// Equipped with this expanded universe, the shortest path between every pair of galaxies 
    /// can be found. It can help to assign every galaxy a unique number:
    ///   ....1........
    ///   .........2...
    ///   3............
    ///   .............
    ///   .............
    ///   ........4....
    ///   .5...........
    ///   ............6
    ///   .............
    ///   .............
    ///   .........7...
    ///   8....9.......
    /// In these 9 galaxies, there are 36 pairs. Only count each pair once; order within the pair
    /// doesn't matter. For each pair, find any shortest path between the two galaxies using only 
    /// steps that move up, down, left, or right exactly one . or # at a time. (The shortest path 
    /// between two galaxies is allowed to pass through another galaxy.)
    /// For example, here is one of the shortest paths between galaxies 5 and 9:
    ///   ....1........
    ///   .........2...
    ///   3............
    ///   .............
    ///   .............
    ///   ........4....
    ///   .5...........
    ///   .##.........6
    ///   ..##.........
    ///   ...##........
    ///   ....##...7...
    ///   8....9.......
    /// This path has length 9 because it takes a minimum of nine steps to get from galaxy 5 to
    /// galaxy 9 (the eight locations marked # plus the step onto galaxy 9 itself). Here are some 
    /// other example shortest path lengths:
    /// ]Between galaxy 1 and galaxy 7: 15
    /// Between galaxy 3 and galaxy 6: 17
    /// Between galaxy 8 and galaxy 9: 5
    /// In this example, after expanding the universe, the sum of the shortest path between all
    /// 36 pairs of galaxies is 374.
    /// Expand the universe, then find the length of the shortest path between every pair of galaxies. 
    /// What is the sum of these lengths?
    /// Answer should be 9177603
    /// </summary>+
    public static long Day11Part1()
    {
        var input = File.ReadAllLines("advent2023\\input\\11.txt");
        var universe = Day11ExpandUniverse(input);
        var galaxies = Day11GetGalaxies(universe);
        var pairs = new List<(int id1, int id2)>();
        
        for (int i = 0; i < galaxies.Length - 1; i++)
        {
            for (int j = i + 1; j < galaxies.Length; j++)
            {
                pairs.Add((i,j));
            }
        }

        var distances = new List<int>();

        foreach (var pair in pairs)
        {
            var galaxyStart = galaxies[pair.id1];
            var galaxyEnd = galaxies[pair.id2];
            distances.Add(Math.Abs(galaxyEnd.r - galaxyStart.r) + Math.Abs(galaxyEnd.c - galaxyStart.c));
        }

        return distances.Sum();
    }

    /// <summary>
    /// Now, instead of the expansion you did before, make each empty row or column one million
    /// times larger. That is, each empty row should be replaced with 1000000 empty rows, and 
    /// each empty column should be replaced with 1000000 empty columns.
    /// Expand the universe according to these new rules, then find the length of the shortest
    /// path between every pair of galaxies.What is the sum of these lengths?
    /// Answer should be 632003913611
    /// </summary>
    public static long Day11Part2()
    {
        var input = File.ReadAllLines("advent2023\\input\\11.txt");
        var universe = input.Select(r => r.ToArray()).ToArray();

        var galaxies = Day11GetGalaxies(universe);
        var pairs = new List<(int id1, int id2)>();

        for (int i = 0; i < galaxies.Length - 1; i++)
        {
            for (int j = i + 1; j < galaxies.Length; j++)
            {
                pairs.Add((i, j));
            }
        }

        long theDistance = 0;

        foreach (var pair in pairs)
        {
            var galaxyStart = galaxies[pair.id1];
            var galaxyEnd = galaxies[pair.id2];
            var rStr = galaxyStart.r;
            var cStr = galaxyStart.c;
            var rEnd = galaxyEnd.r;
            var cEnd = galaxyEnd.c;
            if (cStr > cEnd)
            {
                var tmpr = cStr;
                cStr = cEnd;
                cEnd = tmpr;
            }
            var rExpand = universe[rStr..rEnd].Where(row => row.All(c => c == '.'));
            var cExpand = new List<int>();
            for (int i = cStr; i < cEnd; i++)
            {
                if (universe.All(row => row[i] == '.'))
                    cExpand.Add(i);
            }
            var distance = Math.Abs(galaxyEnd.r - galaxyStart.r) + Math.Abs(galaxyEnd.c - galaxyStart.c);
            distance += 999_999 * rExpand.Count() + 999_999 * cExpand.Count();
            theDistance += distance;
        }

        return theDistance;
    }

    private static (int r, int c)[] Day11GetGalaxies(char[][] universe)
    {
        var galaxies = new List<(int r, int c)>();
        for (int r = 0; r < universe.Length; r++)
        {
            for (int c = 0; c < universe[r].Length; c++)
            {
                if (universe[r][c] == '#')
                    galaxies.Add((r, c));
            }
        }
        return galaxies.ToArray();
    }

    private static char[][] Day11ExpandUniverse(string[] input)
    {
        var universe = input.Select(r => r.ToList()).ToList();

        for (int r = 0; r < universe.Count; r++)
        {
            if (universe[r].All(c => c == '.'))
            {
                var insertableRow = universe[r].ToList();
                universe.Insert(r, insertableRow);
                r++;
            }
        }
        for (int c = 0; c < universe.First().Count; c++)
        {
            if (universe.All(r => r[c] == '.'))
            {
                universe.ForEach(r => r.Insert(c, '.'));
                c++;
            }
        }

        return universe.Select(l => l.ToArray()).ToArray();
    }

    /// <summary>
    /// In the giant field just outside, the springs are arranged into rows. For each row,
    /// the condition records show every spring and whether it is operational (.) or damaged (#).
    /// This is the part of the condition records that is itself damaged; for some springs, it is
    /// simply unknown (?) whether the spring is operational or damaged.
    /// However, the engineer that produced the condition records also duplicated some of this
    /// information in a different format! After the list of springs for a given row, the size
    /// of each contiguous group of damaged springs is listed in the order those groups appear
    /// in the row. This list always accounts for every damaged spring, and each number is the
    /// entire size of its contiguous group (that is, groups are always separated by at least one
    /// operational spring: #### would always be 4, never 2,2).
    /// So, condition records with no unknown spring conditions might look like this:
    ///   #.#.### 1,1,3
    ///   .#...#....###. 1,1,3
    ///   .#.###.#.###### 1,3,1,6
    ///   ####.#...#... 4,1,1
    ///   #....######..#####. 1,6,5
    ///   .###.##....# 3,2,1
    /// However, the condition records are partially damaged; some of the springs' conditions
    /// are actually unknown (?). For example:
    ///   ???.### 1,1,3
    ///   .??..??...?##. 1,1,3
    ///   ?#?#?#?#?#?#?#? 1,3,1,6
    ///   ????.#...#... 4,1,1
    ///   ????.######..#####. 1,6,5
    ///   ?###???????? 3,2,1
    /// Equipped with this information, it is your job to figure out how many different 
    /// arrangements of operational and broken springs fit the given criteria in each row.
    /// In the first line (???.### 1,1,3), there is exactly one way separate groups of one, one,
    /// and three broken springs (in that order) can appear in that row: the first three unknown
    /// springs must be broken, then operational, then broken (#.#), making the whole row #.#.###.
    /// The second line is more interesting: .??..??...?##. 1,1,3 could be a total of four 
    /// different arrangements. The last ? must always be broken (to satisfy the final contiguous
    /// group of three broken springs), and each ?? must hide exactly one of the two broken
    /// springs. (Neither ?? could be both broken springs or they would form a single contiguous
    /// group of two; if that were true, the numbers afterward would have been 2,3 instead.) Since
    /// each ?? can either be #. or .#, there are four possible arrangements of springs.
    /// The last line is actually consistent with ten different arrangements! Because the first 
    /// number is 3, the first and second ? must both be . (if either were #, the first number
    /// would have to be 4 or higher). However, the remaining run of unknown spring conditions
    /// have many different ways they could hold groups of two and one broken springs:
    ///   ?###???????? 3,2,1
    ///   .###.##.#...
    ///   .###.##..#..
    ///   .###.##...#.
    ///   .###.##....#
    ///   .###..##.#..
    ///   .###..##..#.
    ///   .###..##...#
    ///   .###...##.#.
    ///   .###...##..#
    ///   .###....##.#
    /// In this example, the number of possible arrangements for each row is:
    ///   ???.### 1,1,3 - 1 arrangement
    ///   .??..??...?##. 1,1,3 - 4 arrangements
    ///   ?#?#?#?#?#?#?#? 1,3,1,6 - 1 arrangement
    ///   ????.#...#... 4,1,1 - 1 arrangement
    ///   ????.######..#####. 1,6,5 - 4 arrangements
    ///   ?###???????? 3,2,1 - 10 arrangements
    /// Adding all of the possible arrangement counts together produces a total of 21 arrangements.
    /// For each row, count all of the different arrangements of operational and broken springs
    /// that meet the given criteria. What is the sum of those counts?
    /// 
    /// </summary>
    public static long Day12Part1()
    {
        var input = File.ReadAllLines("advent2023\\input\\12.txt");
        input = new string[]
        {
           "???.### 1,1,3",//1
           ".??..??...?##. 1,1,3",//4
           "?#?#?#?#?#?#?#? 1,3,1,6",//1
           "????.#...#... 4,1,1",//1
           "????.######..#####. 1,6,5",//4
           "?###???????? 3,2,1",//10
        };

        for (int i = 0; i < input.Length; i++)
        {
            var wildcards = input[i].Count(c => c == '?');
            var numberOfChoices = (int)Math.Pow(2, wildcards);
            
        }
        
        return 0;
    }
}
