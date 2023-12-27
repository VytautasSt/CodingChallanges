using System.Collections.Generic;
using System.Linq;

namespace CodingChallanges
{
    public static class Katas
    {
        /// <summary>
        /// Row, Col, Possible values
        /// </summary>
        private static List<int>[][] grid = new List<int>[][]
        {
            new List<int>[]{ new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4} },
            new List<int>[]{ new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4} },
            new List<int>[]{ new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4} },
            new List<int>[]{ new List<int> { 1, 2, 3, 4}, new List<int> { 1, 2, 3, 4 }, new List<int> { 1, 2, 3, 4 }, new List<int> { 1, 2, 3, 4 } }
        };

        private static bool finished => grid.All(r => r.All(l => l.Count == 1));
        private static int[] clues;

        public static int[][] Sky(int[] clue)
        {
            clues = clue;
            // place 100 guaranteed rows/cols
            CluesOf4();
            CluesOf1();
            // remove perma invalid ones
            CluesOf2();

            while (!finished)
            {
                RuntimeCluesOf2();
                //CluesOf3();
                RemoveUnnecessary();
                RemoveUnnecessary2();
                CheckPobbabilityOf2WithClues();
            }

            return new int[][] { }; //todo implement;
        }

        /// <summary>
        /// lock value and remove neighboor options
        /// </summary>
        private static void Lock(int row, int col, int val)
        {
            grid[row][col] = new List<int> { val };
            RemoveValInRowNeigbhors(row, col, val);
            RemoveValInColNeigbhors(row, col, val);
        }

        private static void RemoveValInRowNeigbhors(int r, int cHost, int val)
        {
            for (int c = 0; c < 4; c++)
            {
                if (c != cHost)
                    Remove(r, c, val);
            }
        }

        private static void RemoveValInColNeigbhors(int rHost, int c, int val)
        {
            for (int r = 0; r < 4; r++)
            {
                if (r != rHost)
                    Remove(r, c, val);
            }
        }

        private static void Remove(int r, int c, int val) => grid[r][c].Remove(val);

        /// <summary>
        /// TODO make general method - one left in cell, remove neighboors
        /// </summary>
        private static void RemoveUnnecessary()
        {

        }

        /// <summary>
        /// TODO make general method - if one left in row/col, lock it
        /// </summary>
        private static void RemoveUnnecessary2()
        {
            for (int r = 0; r < 4; r++)
            {
                for (int v = 1; v <= 4; v++)
                {
                    int id = 0;
                    int count = 0;

                    for (int c = 0; c < 4; c++)
                    {
                        if (grid[r][c].Contains(v))
                        {
                            count++;
                            id = c;
                        }
                    }

                    if (count == 1)
                        Lock(r, id, v);
                }
            }

            for (int c = 0; c < 4; c++)
            {
                for (int v = 1; v <= 4; v++)
                {
                    int id = 0;
                    int count = 0;

                    for (int r = 0; r < 4; r++)
                    {
                        if (grid[r][c].Contains(v))
                        {
                            count++;
                            id = r;
                        }
                    }

                    if (count == 1)
                        Lock(id, c, v);
                }
            }
        }

        /// <summary>
        /// TODO make general method - if two items with two options, try both options, if clue restricts remove;
        /// </summary>
        private static void CheckPobbabilityOf2WithClues()
        {

        }

        private static void RuntimeCluesOf2()
        {
            for (int c = 0; c < 16; c++)
            {
                if (clues[c] != 2)
                    continue;

                // if 4 is locked last, lock 3 at first item
                if (c < 4 && grid[3][c][0] == 4)
                    Lock(0, c, 3);
                else if (c < 8 && c >= 4 && grid[c - 4][0][0] == 4)
                    Lock(c - 4, 3, 3);
                else if (c < 12 && c >= 8 && grid[0][3 - (c % 8)][0] == 4)
                    Lock(3, 3 - (c % 8), 3);
                else if (c >= 12 && grid[3 - (c % 12)][3][0] == 4)
                    Lock(3 - (c % 12), 0, 3);
                else
                {
                    // if 1 locked first remove 2 second
                    if (c < 4 && grid[0][c].All(i => i == 1))
                        Remove(1, c, 2);
                    else if (c < 8 && c >= 4 && grid[c - 4][3].All(i => i == 1))
                        Remove(c - 4, 2, 2);
                    else if (c < 12 && c >= 8 && grid[3][3 - (c % 8)].All(i => i == 1))
                        Remove(2, 3 - (c % 8), 2);
                    else if (c >= 12 && grid[3 - (c % 12)][0].All(i => i == 1))
                        Remove(3 - (c % 12), 1, 2);
                    else
                    {
                        // if 2 locked first remove 3 second TODO
                        if (c < 4 && grid[0][c].All(i => i == 2))
                            Remove(1, c, 3);
                        else if (c < 8 && c >= 4 && grid[c - 4][3].All(i => i == 2))
                            Remove(c - 4, 2, 3);
                        else if (c < 12 && c >= 8 && grid[3][3 - (c % 8)].All(i => i == 2))
                            Remove(2, 3 - (c % 8), 3);
                        else if (c >= 12 && grid[3 - (c % 12)][0].All(i => i == 2))
                            Remove(3 - (c % 12), 1, 3);
                    }
                }
            }
        }

        private static void CluesOf1()
        {
            for (int c = 0; c < 16; c++)
            {
                if (clues[c] != 1)
                    continue;

                if (c < 4)
                    Lock(0, c, 4);
                else if (c < 8)
                    Lock(c - 4, 3, 4);
                else if (c < 12)
                    Lock(3, 3 - (c % 8), 4);
                else
                    Lock(3 - (c % 12), 0, 4);
            }
        }

        private static void CluesOf2()
        {
            for (int c = 0; c < 16; c++)
            {
                if (clues[c] != 2)
                    continue;

                if (c < 4)
                    Remove(0, c, 4);
                else if (c < 8)
                    Remove(c - 4, 3, 4);
                else if (c < 12)
                    Remove(3, 3 - (c % 8), 4);
                else
                    Remove(3 - (c % 12), 0, 4);
            }
        }

        private static void CluesOf4()
        {
            for (int c = 0; c < 16; c++)
            {
                if (clues[c] != 4)
                    continue;

                if (c < 4)
                {
                    Lock(0, c, 1);
                    Lock(1, c, 2);
                    Lock(2, c, 3);
                    Lock(3, c, 4);
                }
                else if (c < 8)
                {
                    Lock(c - 4, 3, 1);
                    Lock(c - 4, 2, 2);
                    Lock(c - 4, 1, 3);
                    Lock(c - 4, 0, 4);
                }
                else if (c < 12)
                {
                    Lock(3, 3 - (c % 8), 1);
                    Lock(2, 3 - (c % 8), 2);
                    Lock(1, 3 - (c % 8), 3);
                    Lock(0, 3 - (c % 8), 4);
                }
                else
                {
                    Lock(3 - (c % 12), 0, 1);
                    Lock(3 - (c % 12), 1, 2);
                    Lock(3 - (c % 12), 2, 3);
                    Lock(3 - (c % 12), 3, 4);
                }
            }
        }
    }
}
