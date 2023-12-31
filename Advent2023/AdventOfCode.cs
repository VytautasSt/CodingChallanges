﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CodingChallanges.Advent2023.Utils;

namespace CodingChallanges.Advent2023
{
    /// <summary>
    /// https://adventofcode.com/2023/
    /// </summary>
    public static partial class AdventOfCode
    {
        private static long Day1MakeNumberFromDigits(IEnumerable<char> list)
            => int.Parse(list.First().ToString()) * 10 + int.Parse(list.Last().ToString());

        /// <summary>
        /// --- Day 1: Trebuchet?! ---
        /// The newly-improved calibration document consists of lines of text; 
        /// each line originally contained a specific calibration value that the Elves now need to recover.
        /// On each line, the calibration value can be found by combining the first digit
        /// and the last digit(in that order) to form a single two-digit number.
        /// For example:
        ///   1abc2
        ///   pqr3stu8vwx
        ///   a1b2c3d4e5f
        ///   treb7uchet
        /// In this example, the calibration values of these four lines are 12, 38, 15, and 77. 
        /// Adding these together produces 142.
        /// What is the sum of all of the calibration values?
        /// Answer shoud be 55971.
        /// </summary>
        public static long Day1Part1()
        {
            IEnumerable<char> Day1ExtractDigits(string line)
               => line.Where(c => char.IsDigit(c));

            var input = File.ReadAllLines("advent2023\\input\\1.txt");

            return input.Sum(line => Day1MakeNumberFromDigits(Day1ExtractDigits(line)));
        }

        /// <summary>
        /// Your calculation isn't quite right. It looks like some of the digits are actually spelled 
        /// out with letters: one, two, three, four, five, six, seven, eight, and nine also count as valid "digits".
        /// Equipped with this new information, you now need to find the real first and last digit on each line.For example:
        ///   two1nine
        ///   eightwothree
        ///   abcone2threexyz
        ///   xtwone3four
        ///   4nineeightseven2
        ///   zoneight234
        ///   7pqrstsixteen
        /// In this example, the calibration values are 29, 83, 13, 24, 42, 14, and 76. Adding these together produces 281.
        /// What is the sum of all of the calibration values?
        /// Answer should be 54719.
        /// </summary>
        public static long Day1Part2()
        {
            var input = File.ReadAllLines("advent2023\\input\\1.txt");
            var digitsInWords = new Dictionary<string, char>
            {
                { "one", '1' },
                { "two", '2' },
                { "three", '3' },
                { "four", '4' },
                { "five", '5' },
                { "six", '6' },
                { "seven", '7' },
                { "eight", '8' },
                { "nine", '9' }
            };
            var digitKeys = digitsInWords.Keys.ToArray();
            List<char> ExtractDigitsFromLine(string line)
            {
                var digits = new List<char>();

                for (int i = 0; i < line.Length; i++)
                {
                    for (int j = i; j < line.Length; j++)
                    {
                        if (char.IsDigit(line[j]))
                        {
                            digits.Add(line[j]);
                            i = j;
                            break;
                        }
                        var sub = line.Substring(i, j - i + 1);
                        if (digitKeys.Any(k => sub.Contains(k)))
                        {
                            var key = digitKeys.First(k => sub.Contains(k));
                            digits.Add(digitsInWords[key]);
                            i = j - 2;
                            break;
                        }
                    }
                }
                return digits;
            }
            return input.Sum(line => Day1MakeNumberFromDigits(ExtractDigitsFromLine(line)));
        }

        /// <summary>
        /// --- Day 2: Cube Conundrum ---
        /// As you walk, the Elf shows you a small bag and some cubes which are either red, green, or blue.
        /// Each time you play this game, he will hide a secret number of cubes of each color in the bag, 
        /// and your goal is to figure out information about the number of cubes.
        /// To get information, once a bag has been loaded with cubes, the Elf will reach into the bag, 
        /// grab a handful of random cubes, show them to you, and then put them back in the bag. 
        /// He'll do this a few times per game.
        /// You play several games and record the information from each game (your puzzle input). 
        /// Each game is listed with its ID number(like the 11 in Game 11: ...) followed by a semicolon-separated
        /// list of subsets of cubes that were revealed from the bag(like 3 red, 5 green, 4 blue).
        /// For example, the record of a few games might look like this:
        ///   Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
        ///   Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
        ///   Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
        ///   Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
        ///   Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
        /// In game 1, three sets of cubes are revealed from the bag(and then put back again). 
        /// The first set is 3 blue cubes and 4 red cubes; the second set is 1 red cube, 
        /// 2 green cubes, and 6 blue cubes; the third set is only 2 green cubes.
        /// The Elf would first like to know which games would have been possible 
        /// if the bag contained only 12 red cubes, 13 green cubes, and 14 blue cubes?
        /// In the example above, games 1, 2, and 5 would have been possible if the bag 
        /// had been loaded with that configuration. However, game 3 would have been impossible
        /// because at one point the Elf showed you 20 red cubes at once; similarly, game 4 would
        /// also have been impossible because the Elf showed you 15 blue cubes at once. 
        /// If you add up the IDs of the games that would have been possible, you get 8.
        /// Determine which games would have been possible if the bag had been loaded with 
        /// only 12 red cubes, 13 green cubes, and 14 blue cubes. 
        /// What is the sum of the IDs of those games?
        /// Answer should be 2283
        /// </summary>
        public static long Day2Part1()
        {
            var input = File.ReadAllLines("advent2023\\input\\2.txt");
            int redAmount = 12;
            var red = "red";
            var green = "green";
            var blue = "blue";
            int greenAmount = 13;
            int blueAmount = 14;
            var gameIdSum = 0;

            int ParseSubPart(string subPart) => int.Parse(Regex.Match(subPart, @"\d+").Value);

            for (int i = 0; i < input.Length; i++)
            {
                var parts = input[i].Split(new char[] { ':', ';' });
                var gameId = i + 1;
                var validGame = true;

                for (int j = 1; j < parts.Length; j++)
                {
                    var subParts = parts[j].Split(',');
                    var redCount = 0;
                    var greenCount = 0;
                    var blueCount = 0;

                    foreach (var subPart in subParts)
                    {
                        if (subPart.Contains(red))
                            redCount = ParseSubPart(subPart);
                        if (subPart.Contains(green))
                            greenCount = ParseSubPart(subPart);
                        if (subPart.Contains(blue))
                            blueCount = ParseSubPart(subPart);
                    }

                    if (redCount > redAmount || greenCount > greenAmount || blueCount > blueAmount)
                        validGame = false;
                }

                if (validGame)
                    gameIdSum += gameId;
            }

            return gameIdSum;
        }

        /// <summary>
        /// In each game you played, what is the fewest number of cubes of each color that could 
        /// have been in the bag to make the game possible?
        /// Again consider the example games from earlier:
        ///   Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
        ///   Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
        ///   Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
        ///   Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
        ///   Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
        /// In game 1, the game could have been played with as few as 4 red, 2 green, and 6 blue cubes.
        /// If any color had even one fewer cube, the game would have been impossible.
        /// Game 2 could have been played with a minimum of 1 red, 3 green, and 4 blue cubes.
        /// Game 3 must have been played with at least 20 red, 13 green, and 6 blue cubes.
        /// Game 4 required at least 14 red, 3 green, and 15 blue cubes.
        /// Game 5 needed no fewer than 6 red, 3 green, and 2 blue cubes in the bag.
        /// The power of a set of cubes is equal to the numbers of red, green, and blue cubes multiplied together.
        /// The power of the minimum set of cubes in game 1 is 48. In games 2-5 it was 12, 1560, 630, and 36, respectively.
        /// Adding up these five powers produces the sum 2286.
        /// For each game, find the minimum set of cubes that must have been present.
        /// What is the sum of the power of these sets?
        /// Answer should be 78669.
        /// </summary>
        public static long Day2Part2()
        {
            var input = File.ReadAllLines("advent2023\\input\\2.txt");
            var red = "red";
            var green = "green";
            var blue = "blue";
            var powerSum = 0;

            int ParseSubPart(string subPart) => int.Parse(Regex.Match(subPart, @"\d+").Value);

            for (int i = 0; i < input.Length; i++)
            {
                var parts = input[i].Split(new char[] { ':', ';' });
                var minRed = 0;
                var minGreen = 0;
                var minBlue = 0;

                for (int j = 1; j < parts.Length; j++)
                {
                    var subParts = parts[j].Split(',');
                    var redCount = 0;
                    var greenCount = 0;
                    var blueCount = 0;

                    foreach (var subPart in subParts)
                    {
                        if (subPart.Contains(red))
                            redCount = ParseSubPart(subPart);
                        if (subPart.Contains(green))
                            greenCount = ParseSubPart(subPart);
                        if (subPart.Contains(blue))
                            blueCount = ParseSubPart(subPart);
                    }

                    if (redCount > minRed)
                        minRed = redCount;
                    if (greenCount > minGreen)
                        minGreen = greenCount;
                    if (blueCount > minBlue)
                        minBlue = blueCount;
                }

                powerSum += minRed * minGreen * minBlue;
            }
            return powerSum;
        }

        private static List<SpecNumber> Day3GetSpecNumbers(string[] input)
        {
            var cols = input[0].Length;
            var rows = input.Length;
            List<SpecNumber> numbers = new List<SpecNumber>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (!char.IsDigit(input[r][c]))
                        continue;

                    int cEnd = c + 1;
                    while (cEnd < cols && char.IsDigit(input[r][cEnd]))
                        cEnd++;

                    var nr = new SpecNumber(int.Parse(input[r].Substring(c, cEnd - c)));

                    // look for adjacent symbol
                    var checkColStart = c - 1;
                    var checkColEnd = cEnd + 1;

                    for (int i = checkColStart; i < checkColEnd; i++)
                    {
                        if (i < 0 || i == cols)
                            continue;

                        if (r > 0 && Symbol.IsSymbol(input[r - 1][i]))
                        {
                            nr.Symbols.Add(new Symbol(input[r - 1][i], r - 1, i));
                        }
                        if (r + 1 < rows && Symbol.IsSymbol(input[r + 1][i]))
                        {
                            nr.Symbols.Add(new Symbol(input[r + 1][i], r + 1, i));
                        }
                        if ((i == checkColStart || i == cEnd) && Symbol.IsSymbol(input[r][i]))
                        {
                            nr.Symbols.Add(new Symbol(input[r][i], r, i));
                        }
                    }

                    numbers.Add(nr);
                    c = cEnd;
                }
            }
            return numbers;
        }

        /// <summary>
        /// --- Day 3: Gear Ratios ---
        /// The engine schematic (your puzzle input) consists of a visual representation of the engine.
        /// There are lots of numbers and symbols you don't really understand, but apparently any number
        /// adjacent to a symbol, even diagonally, is a "part number" and should be included in your sum. 
        /// (Periods (.) do not count as a symbol.) Here is an example engine schematic:
        ///   467..114..
        ///   ...*......
        ///   ..35..633.
        ///   ......#...
        ///   617*......
        ///   .....+.58.
        ///   ..592.....
        ///   ......755.
        ///   ...$.*....
        ///   .664.598..
        /// In this schematic, two numbers are not part numbers because they are not adjacent 
        /// to a symbol: 114 (top right) and 58 (middle right). Every other number is adjacent
        /// to a symbol and so is a part number; their sum is 4361.
        /// Of course, the actual engine schematic is much larger.
        /// What is the sum of all of the part numbers in the engine schematic?
        /// Answer should be 509115.
        /// </summary>
        public static long Day3Part1()
        {
            var input = File.ReadAllLines("advent2023\\input\\3.txt");
            var numbers = Day3GetSpecNumbers(input);
            return numbers.Where(n => n.Symbols.Any()).Sum(n => n.Number);
        }

        /// <summary>
        /// The missing part wasn't the only issue - one of the gears in the engine is wrong. 
        /// A gear is any * symbol that is adjacent to exactly two part numbers. 
        /// Its gear ratio is the result of multiplying those two numbers together.
        /// This time, you need to find the gear ratio of every gear and add them all up so
        /// that the engineer can figure out which gear needs to be replaced.
        /// Consider the same engine schematic again:
        ///   467..114..
        ///   ...*......
        ///   ..35..633.
        ///   ......#...
        ///   617*......
        ///   .....+.58.
        ///   ..592.....
        ///   ......755.
        ///   ...$.*....
        ///   .664.598..
        /// In this schematic, there are two gears. The first is in the top left; it has part
        /// numbers 467 and 35, so its gear ratio is 16345. The second gear is in the lower right;
        /// its gear ratio is 451490. Adding up all of the gear ratios produces 467835.
        /// What is the sum of all of the gear ratios in your engine schematic?
        /// Answer should be 75220503.
        /// </summary>
        public static long Day3Part2()
        {
            var input = File.ReadAllLines("advent2023\\input\\3.txt");
            var numbers = Day3GetSpecNumbers(input);
            var gears = new List<int>();

            for (int r = 0; r < input.Length; r++)
            {
                for (int c = 0; c < input.Length; c++)
                {
                    if (input[r][c] != '*')
                        continue;

                    var numbersWithStar = numbers.Where(n => n.Symbols.Any(s => s.Row == r && s.Column == c)).ToArray();
                    if (numbersWithStar.Length == 2)
                        gears.Add(numbersWithStar[0].Number * numbersWithStar[1].Number);
                }
            }
            return gears.Sum();
        }

        private static new List<IEnumerable<int>> Day4GetMatches(string[] input)
        {
            IEnumerable<int> ParseNrStr(string str)
                => str.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s));

            var gameMatches = new List<IEnumerable<int>>();

            for (int r = 0; r < input.Length; r++)
            {
                var parts = input[r].Split(new char[] { ':', '|' });
                var winningNrs = ParseNrStr(parts[1]);
                var yourNrs = ParseNrStr(parts[2]);

                gameMatches.Add(yourNrs.Where(myNr => winningNrs.Contains(myNr)));
            }

            return gameMatches;
        }

        /// <summary>
        /// --- Day 4: Scratchcards ---
        /// The Elf leads you over to the pile of colorful cards.There, you discover dozens of scratchcards, 
        /// all with their opaque covering already scratched off. Picking one up, it looks like each card
        /// has two lists of numbers separated by a vertical bar (|): a list of winning numbers and then 
        /// a list of numbers you have.You organize the information into a table(your puzzle input).
        /// As far as the Elf has been able to figure out, you have to figure out which of the numbers
        /// you have appear in the list of winning numbers.The first match makes the card worth one point
        /// and each match after the first doubles the point value of that card.
        /// For example:
        /// Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
        /// Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
        /// Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
        /// Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
        /// Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
        /// Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
        /// In the above example, card 1 has five winning numbers (41, 48, 83, 86, and 17) and 
        /// eight numbers you have(83, 86, 6, 31, 17, 9, 48, and 53). Of the numbers you have,
        /// four of them(48, 83, 17, and 86) are winning numbers! That means card 1 is worth 8 points
        /// (1 for the first match, then doubled three times for each of the three matches after the first).
        /// Card 2 has two winning numbers(32 and 61), so it is worth 2 points.
        /// Card 3 has two winning numbers(1 and 21), so it is worth 2 points.
        /// Card 4 has one winning number(84), so it is worth 1 point.
        /// Card 5 has no winning numbers, so it is worth no points.
        /// Card 6 has no winning numbers, so it is worth no points.
        /// So, in this example, the Elf's pile of scratchcards is worth 13 points.
        /// Take a seat in the large pile of colorful cards.
        /// How many points are they worth in total?
        /// Answer should be 20117.
        /// </summary>
        public static long Day4Part1()
        {
            var input = File.ReadAllLines("advent2023\\input\\4.txt");
            var gameMatches = Day4GetMatches(input);
            return gameMatches
                .Where(g => g.Any())
                .Select(g => (int)Math.Pow(2, g.Count() - 1))
                .Sum();
        }

        /// <summary>
        /// There's no such thing as "points". Instead, scratchcards only cause you to win more 
        /// scratchcards equal to the number of winning numbers you have.
        /// Specifically, you win copies of the scratchcards below the winning card equal to the
        /// number of matches.So, if card 10 were to have 5 matching numbers, you would win one 
        /// copy each of cards 11, 12, 13, 14, and 15.
        /// Copies of scratchcards are scored like normal scratchcards and have the same card number
        /// as the card they copied. So, if you win a copy of card 10 and it has 5 matching numbers,
        /// it would then win a copy of the same cards that the original card 10 won: cards 11, 12,
        /// 13, 14, and 15. This process repeats until none of the copies cause you to win any more
        /// cards. (Cards will never make you copy a card past the end of the table.)
        /// This time, the above example goes differently:
        ///   Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
        ///   Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
        ///   Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
        ///   Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
        ///   Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
        ///   Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
        /// Card 1 has four matching numbers, so you win one copy each of the next four cards: cards 2, 3, 4, and 5.
        /// Your original card 2 has two matching numbers, so you win one copy each of cards 3 and 4.
        /// Your copy of card 2 also wins one copy each of cards 3 and 4.
        /// Your four instances of card 3 (one original and three copies) have two matching numbers,
        /// so you win four copies each of cards 4 and 5.
        /// Your eight instances of card 4 (one original and seven copies) have one matching number,
        /// so you win eight copies of card 5.
        /// Your fourteen instances of card 5 (one original and thirteen copies) have no matching 
        /// numbers and win no more cards.
        /// Your one instance of card 6 (one original) has no matching numbers and wins no more cards.
        /// Once all of the originals and copies have been processed, you end up with 1 instance of
        /// card 1, 2 instances of card 2, 4 instances of card 3, 8 instances of card 4, 14 instances
        /// of card 5, and 1 instance of card 6. In total, this example pile of scratchcards causes 
        /// you to ultimately have 30 scratchcards!
        /// Process all of the original and copied scratchcards until no more scratchcards are won.
        /// Including the original set of scratchcards, how many total scratchcards do you end up with?
        /// Answer should be 13768818.
        /// </summary>
        public static long Day4Part2()
        {
            var input = File.ReadAllLines("advent2023\\input\\4.txt");
            var gameMatches = Day4GetMatches(input);
            var cards = 0;
            int[] setOfCopies = new int[input.Length];

            for (int g = 0; g < gameMatches.Count; g++)
            {
                cards += setOfCopies[g] + 1; // +1 because you always add OG card

                for (int i = g + 1; i <= g + gameMatches[g].Count(); i++)
                {
                    if (i == input.Length)
                        break;
                    setOfCopies[i] += 1 + setOfCopies[g];
                }
            }

            return cards;
        }

        /// <summary>
        /// --- Day 5: If You Give A Seed A Fertilizer ---
        /// The almanac (your puzzle input) lists all of the seeds that need to be planted. 
        /// It also lists what type of soil to use with each kind of seed, what type of fertilizer
        /// to use with each kind of soil, what type of water to use with each kind of fertilizer,
        /// and so on. Every type of seed, soil, fertilizer and so on is identified with a number,
        /// but numbers are reused by each category - that is, soil 123 and fertilizer 123 aren't 
        /// necessarily related to each other. For example:
        ///   seeds: 79 14 55 13
        ///   
        ///   seed-to-soil map:
        ///   50 98 2
        ///   52 50 48
        ///   
        ///   soil-to-fertilizer map:
        ///   0 15 37
        ///   37 52 2
        ///   39 0 15
        ///   
        ///   fertilizer-to-water map:
        ///   49 53 8
        ///   0 11 42
        ///   42 0 7
        ///   57 7 4
        ///   
        ///   water-to-light map:
        ///   88 18 7
        ///   18 25 70
        ///   
        ///   light-to-temperature map:
        ///   45 77 23
        ///   81 45 19
        ///   68 64 13
        ///   
        ///   temperature-to-humidity map:
        ///   0 69 1
        ///   1 0 69
        ///   
        ///   humidity-to-location map:
        ///   60 56 37
        ///   56 93 4
        /// The almanac starts by listing which seeds need to be planted: seeds 79, 14, 55, and 13.
        /// The rest of the almanac contains a list of maps which describe how to convert numbers 
        /// from a source category into numbers in a destination category. That is, the section 
        /// that starts with seed-to-soil map: describes how to convert a seed number (the source)
        /// to a soil number(the destination). This lets the gardener and his team know which soil
        /// to use with which seeds, which water to use with which fertilizer, and so on.
        /// Rather than list every source number and its corresponding destination number one by one,
        /// the maps describe entire ranges of numbers that can be converted.Each line within a map
        /// contains three numbers: the destination range start, the source range start, and the range length.
        /// Consider again the example seed-to-soil map. The first line has a destination range start of 50,
        /// a source range start of 98, and a range length of 2. This line means that the source range
        /// starts at 98 and contains two values: 98 and 99. The destination range is the same length,
        /// but it starts at 50, so its two values are 50 and 51. With this information, you know that 
        /// seed number 98 corresponds to soil number 50 and that seed number 99 corresponds to soil number 51.
        /// The second line means that the source range starts at 50 and contains 48 values: 50,
        /// 51, ..., 96, 97. This corresponds to a destination range starting at 52 and also containing 48
        /// values: 52, 53, ..., 98, 99. So, seed number 53 corresponds to soil number 55.
        /// Any source numbers that aren't mapped correspond to the same destination number.
        /// So, seed number 10 corresponds to soil number 10.
        /// With this map, you can look up the soil number required for each initial seed number:
        /// Seed number 79 corresponds to soil number 81.
        /// Seed number 14 corresponds to soil number 14.
        /// Seed number 55 corresponds to soil number 57.
        /// Seed number 13 corresponds to soil number 13.
        /// The gardener and his team want to get started as soon as possible, so they'd like to know the 
        /// closest location that needs a seed. Using these maps, find the lowest location number that 
        /// corresponds to any of the initial seeds. To do this, you'll need to convert each seed number
        /// through other categories until you can find its corresponding location number. In this example,
        /// the corresponding types are:
        /// Seed 79, soil 81, fertilizer 81, water 81, light 74, temperature 78, humidity 78, location 82.
        /// Seed 14, soil 14, fertilizer 53, water 49, light 42, temperature 42, humidity 43, location 43.
        /// Seed 55, soil 57, fertilizer 57, water 53, light 46, temperature 82, humidity 82, location 86.
        /// Seed 13, soil 13, fertilizer 52, water 41, light 34, temperature 34, humidity 35, location 35.
        /// So, the lowest location number in this example is 35.
        /// What is the lowest location number that corresponds to any of the initial seed numbers?
        /// Answer should be 403695602.
        /// </summary>
        public static long Day5Part1()
        {
            var input = File.ReadAllLines("advent2023\\input\\5.txt");
            var map = new Map();
            var mappedSeed = input[0].Split(':')[1].Split(' ', 
                StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToArray();
            
            for (int i = 2; i <= input.Length; i++)
            {
                if (i == input.Length || string.IsNullOrEmpty(input[i]))
                {
                    for (int j = 0; j < mappedSeed.Length; j++)
                    {
                        mappedSeed[j] = map.GetDestination(mappedSeed[j]);
                    }
                    continue;
                }

                if (input[i].Contains("map"))
                {
                    map = new Map();
                    continue;
                }

                var pairParts = input[i].Split(' ');
                map.AddAsymPairs(pairParts[0], pairParts[1], pairParts[2]);
            }
            return mappedSeed.Min();
        }

        /// <summary>
        /// Re-reading the almanac, it looks like the seeds: line actually describes ranges of seed numbers.
        /// The values on the initial seeds: line come in pairs.Within each pair, the first value is
        /// the start of the range and the second value is the length of the range.So, in the first 
        /// line of the example above: seeds: 79 14 55 13
        /// This line describes two ranges of seed numbers to be planted in the garden.The first range
        /// starts with seed number 79 and contains 14 values: 79, 80, ..., 91, 92. The second range 
        /// starts with seed number 55 and contains 13 values: 55, 56, ..., 66, 67.
        /// Now, rather than considering four seed numbers, you need to consider a total of 27 seed numbers.
        /// In the above example, the lowest location number can be obtained from seed number 82, which 
        /// corresponds to soil 84, fertilizer 84, water 84, light 77, temperature 45, humidity 46, and
        /// location 46. So, the lowest location number is 46.
        /// Consider all of the initial seed numbers listed in the ranges on the first line of the almanac.
        /// What is the lowest location number that corresponds to any of the initial seed numbers?
        /// Answer should be 219529182.
        /// </summary>
        public static long Day5Part2()
        {
            var input = File.ReadAllLines("advent2023\\input\\5.txt");
            var seeds = input[0].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToArray();
            var scopes = new List<Scope>();

            for (int i = 0; i < seeds.Length; i += 2)
            {
                scopes.Add(new Scope(seeds[i], seeds[i + 1]));
            }

            for (int i = 2; i <= input.Length; i++)
            {
                if (i == input.Length || string.IsNullOrEmpty(input[i]))
                {
                    scopes.ForEach(s => s.Modified = false);
                    continue;
                }

                if (input[i].Contains("map"))
                    continue;

                var pairParts = input[i].Split(' ');
                var range = long.Parse(pairParts[2]);
                var destStr = long.Parse(pairParts[0]);
                var sourceStr = long.Parse(pairParts[1]);
                var k = -1;

                while (k < scopes.Count - 1)
                {
                    k++;
                    if (scopes[k].Modified)
                        continue;

                    var rez = scopes[k].TryModifyScope(destStr, sourceStr, range);
                    if (rez != null)
                        scopes.AddRange(rez);
                }
            }

            return scopes.Min(s => s.Start);
        }

        /// <summary>
        /// --- Day 6: Wait For It ---
        /// As part of signing up, you get a sheet of paper (your puzzle input) that lists the time
        /// allowed for each race and also the best distance ever recorded in that race.To guarantee
        /// you win the grand prize, you need to make sure you go farther in each race than the current record holder.
        /// The organizer brings you over to the area where the boat races are held. The boats are 
        /// much smaller than you expected - they're actually toy boats, each with a big button on top.
        /// Holding down the button charges the boat, and releasing the button allows the boat to move.
        /// Boats move faster if their button was held longer, but time spent holding the button counts
        /// against the total race time. You can only hold the button at the start of the race, and
        /// boats don't move until the button is released. For example:
        /// Time:      7  15   30
        /// Distance:  9  40  200
        /// This document describes three races:
        /// The first race lasts 7 milliseconds.The record distance in this race is 9 millimeters.
        /// The second race lasts 15 milliseconds.The record distance in this race is 40 millimeters.
        /// The third race lasts 30 milliseconds.The record distance in this race is 200 millimeters.
        /// Your toy boat has a starting speed of zero millimeters per millisecond. For each whole 
        /// millisecond you spend at the beginning of the race holding down the button, the boat's 
        /// speed increases by one millimeter per millisecond.
        /// So, because the first race lasts 7 milliseconds, you only have a few options:
        /// Don't hold the button at all (that is, hold it for 0 milliseconds) at the start of the race.
        /// The boat won't move; it will have traveled 0 millimeters by the end of the race.
        /// Hold the button for 1 millisecond at the start of the race.Then, the boat will travel
        /// at a speed of 1 millimeter per millisecond for 6 milliseconds, reaching a total 
        /// distance traveled of 6 millimeters.
        /// Hold the button for 2 milliseconds, giving the boat a speed of 2 millimeters per 
        /// millisecond.It will then get 5 milliseconds to move, reaching a total distance of 
        /// 10 millimeters.
        /// Hold the button for 3 milliseconds.After its remaining 4 milliseconds of travel time,
        /// the boat will have gone 12 millimeters.
        /// Hold the button for 4 milliseconds.After its remaining 3 milliseconds of travel time,
        /// the boat will have gone 12 millimeters.
        /// Hold the button for 5 milliseconds, causing the boat to travel a total of 10 millimeters.
        /// Hold the button for 6 milliseconds, causing the boat to travel a total of 6 millimeters.
        /// Hold the button for 7 milliseconds.That's the entire duration of the race. You never 
        /// let go of the button. The boat can't move until you let go of the button.Please make 
        /// sure you let go of the button so the boat gets to move. 0 millimeters.
        /// Since the current record for this race is 9 millimeters, there are actually 4 different
        /// ways you could win: you could hold the button for 2, 3, 4, or 5 milliseconds at the
        /// start of the race.
        /// In the second race, you could hold the button for at least 4 milliseconds and at most
        /// 11 milliseconds and beat the record, a total of 8 different ways to win.
        /// In the third race, you could hold the button for at least 11 milliseconds and no more
        /// than 19 milliseconds and still beat the record, a total of 9 ways you could win.
        /// To see how much margin of error you have, determine the number of ways you can beat 
        /// the record in each race; in this example, if you multiply these values together,
        /// you get 288 (4 * 8 * 9).
        /// Determine the number of ways you could beat the record in each race.
        /// What do you get if you multiply these numbers together?
        /// Answer should be 2756160.
        /// </summary>
        public static long Day6Part1()
        {
            var input = File.ReadAllLines("advent2023\\input\\6.txt");
            var gameTimes = input[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(x => int.Parse(x)).ToList();
            var records = input[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(x => int.Parse(x)).ToList();
            List<int>[] distances = new List<int>[gameTimes.Count];

            for (var gameId = 0; gameId < gameTimes.Count; gameId++)
            {
                distances[gameId] = new List<int>();

                for (var holdTime = 1; holdTime < gameTimes[gameId]; holdTime++)
                {
                    var vel = holdTime;
                    var distancio = (gameTimes[gameId] - holdTime) * vel;
                    if (distancio > records[gameId])
                        distances[gameId].Add(distancio);
                }
            }

            var betterGameCount = distances.Select(x => x.Count);
            return betterGameCount.Aggregate((a, x) => a * x);
        }

        /// <summary>
        /// Ignore the spaces between the numbers on each line.
        /// So, the example from before:
        /// Time:      7  15   30
        /// Distance:  9  40  200
        /// ...now instead means this:
        /// Time:      71530
        /// Distance:  940200
        /// Now, you have to figure out how many ways there are to win this single race. 
        /// In this example, the race lasts for 71530 milliseconds and the record distance 
        /// you need to beat is 940200 millimeters.You could hold the button anywhere from 
        /// 14 to 71516 milliseconds and beat the record, a total of 71503 ways!
        /// How many ways can you beat the record in this one much longer race?
        /// Answer should be 34788142.
        /// </summary>
        public static long Day6Part2()
        {
            var input = File.ReadAllLines("advent2023\\input\\6.txt");
            var time = long.Parse(input[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Aggregate((a, x) => a + x));
            var dist = long.Parse(input[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Aggregate((a, x) => a + x));
            int wins = 0;

            for (long holdTime = 1; holdTime < time; holdTime++)
            {
                if ((time - holdTime) * holdTime > dist)
                    wins++;
            }

            return wins;
        }

        /// <summary>
        /// --- Day 7: Camel Cards ---
        /// In Camel Cards, you get a list of hands, and your goal is to order them based on the
        /// strength of each hand. A hand consists of five cards labeled one of A, K, Q, J, T, 9,
        /// 8, 7, 6, 5, 4, 3, or 2. The relative strength of each card follows this order, where A
        /// is the highest and 2 is the lowest. Every hand is exactly one type.
        /// From strongest to weakest, they are:
        /// -Five of a kind
        /// -Four of a kind
        /// -Full house
        /// -Three of a kind
        /// -Two pair
        /// -One pair
        /// -High card, where all cards' labels are distinct: 23456
        /// Hands are primarily ordered based on type; for example, every full house is stronger than 
        /// any three of a kind. If two hands have the same type, a second ordering rule takes effect.
        /// Start by comparing the first card in each hand. If these cards are different, the hand
        /// with the stronger first card is considered stronger. If the first card in each hand have
        /// the same label, however, then move on to considering the second card in each hand. If 
        /// they differ, the hand with the higher second card wins; otherwise, continue with the 
        /// third card in each hand, then the fourth, then the fifth.
        /// So, 33332 and 2AAAA are both four of a kind hands, but 33332 is stronger because its first 
        /// card is stronger.Similarly, 77888 and 77788 are both a full house, but 77888 is stronger
        /// because its third card is stronger (and both hands have the same first and second card).
        /// To play Camel Cards, you are given a list of hands and their corresponding bid. For example:
        ///   32T3K 765
        ///   T55J5 684
        ///   KK677 28
        ///   KTJJT 220
        ///   QQQJA 483
        /// This example shows five hands; each hand is followed by its bid amount.
        /// Each hand wins an amount equal to its bid multiplied by its rank, where the weakest hand
        /// gets rank 1, the second-weakest hand gets rank 2, and so on up to the strongest hand.
        /// Because there are five hands in this example, the strongest hand will have rank 5 and 
        /// its bid will be multiplied by 5.
        /// So, the first step is to put the hands in order of strength:
        /// 32T3K is the only one pair and the other hands are all a stronger type, so it gets rank 1.
        /// KK677 and KTJJT are both two pair.Their first cards both have the same label, but the 
        /// second card of KK677 is stronger(K vs T), so KTJJT gets rank 2 and KK677 gets rank 3.
        /// T55J5 and QQQJA are both three of a kind.QQQJA has a stronger first card, so it gets 
        /// rank 5 and T55J5 gets rank 4. Now, you can determine the total winnings of this set 
        /// of hands by adding up the result of multiplying each hand's bid with its rank 
        /// (765 * 1 + 220 * 2 + 28 * 3 + 684 * 4 + 483 * 5). So the total winnings in this example are 6440.
        /// Find the rank of every hand in your set. What are the total winnings?
        /// Answer should be 253910319.
        /// </summary>
        public static long Day7Par1()
        {
            var input = File.ReadAllLines("advent2023\\input\\7.txt");
            var hands = input.Select(l => new Hand(l)).ToArray();
            var sortedHands = Utils.Utils.QuicksortArray(hands, 0, hands.Length - 1);
            var sum = 0;
            for (int i = 0; i < sortedHands.Length; i++)
                sum += (i + 1) * sortedHands[i].Bet;
            return sum;
        }

        /// <summary>
        /// The Elf introduces one additional rule. Now, J cards are jokers - wildcards that can
        /// act like whatever card would make the hand the strongest type possible.
        /// To balance this, J cards are now the weakest individual cards, weaker even than 2.
        /// The other cards stay in the same order: A, K, Q, T, 9, 8, 7, 6, 5, 4, 3, 2, J.
        /// J cards can pretend to be whatever card is best for the purpose of determining hand type;
        /// for example, QJJQ2 is now considered four of a kind.However, for the purpose of breaking
        /// ties between two hands of the same type, J is always treated as J, not the card it's
        /// pretending to be: JKKK2 is weaker than QQQQ2 because J is weaker than Q.
        /// Now, the above example goes very differently:
        /// 32T3K 765
        /// T55J5 684
        /// KK677 28
        /// KTJJT 220
        /// QQQJA 483
        /// 32T3K is still the only one pair; it doesn't contain any jokers, so its strength doesn't increase.
        /// KK677 is now the only two pair, making it the second-weakest hand.
        /// T55J5, KTJJT, and QQQJA are now all four of a kind! T55J5 gets rank 3, QQQJA gets rank 4,
        /// and KTJJT gets rank 5. With the new joker rule, the total winnings in this example are 5905.
        /// Using the new joker rule, find the rank of every hand in your set.What are the new total winnings?
        /// Answer should be 254083736.
        /// </summary>
        public static long Day7Par2()
        {
            var input = File.ReadAllLines("advent2023\\input\\7.txt");
            var hands = input.Select(l => new Hand(l, JIsWildcard: true)).ToArray();
            var sortedHands = Utils.Utils.QuicksortArray(hands, 0, hands.Length - 1);
            var sum = 0;
            for (int i = 0; i < sortedHands.Length; i++)
                sum += (i + 1) * sortedHands[i].Bet;
            return sum;
        }

        /// <summary>
        /// --- Day 8: Haunted Wasteland ---
        /// It seems like you're meant to use the left/right instructions to navigate the network.
        /// Perhaps if you have the camel follow the same instructions, you can escape the haunted wasteland!
        /// After examining the maps for a bit, two nodes stick out: AAA and ZZZ.You feel like AAA
        /// is where you are now, and you have to follow the left/right instructions until you reach ZZZ.
        /// This format defines each node of the network individually. For example:
        ///   RL
        ///   AAA = (BBB, CCC)
        ///   BBB = (DDD, EEE)
        ///   CCC = (ZZZ, GGG)
        ///   DDD = (DDD, DDD)
        ///   EEE = (EEE, EEE)
        ///   GGG = (GGG, GGG)
        ///   ZZZ = (ZZZ, ZZZ)
        /// Starting with AAA, you need to look up the next element based on the next left/right 
        /// instruction in your input. In this example, start with AAA and go right (R) by choosing
        /// the right element of AAA, CCC.Then, L means to choose the left element of CCC, ZZZ.
        /// By following the left/right instructions, you reach ZZZ in 2 steps.
        /// Of course, you might not find ZZZ right away.If you run out of left/right instructions,
        /// repeat the whole sequence of instructions as necessary: RL really means RLRLRLRLRLRLRLRL
        /// and so on. For example, here is a situation that takes 6 steps to reach ZZZ:
        /// LLR
        /// AAA = (BBB, BBB)
        /// BBB = (AAA, ZZZ)
        /// ZZZ = (ZZZ, ZZZ)
        /// Starting at AAA, follow the left/right instructions. How many steps are required to reach ZZZ?
        /// Answer should be 15517.
        /// </summary>
        public static long Day8Part1()
        {
            var input = File.ReadAllLines("advent2023\\input\\8.txt");
            var instructions = input[0];
            Dictionary<string, LeftRight> options = new Dictionary<string, LeftRight>();
            var position = "AAA";
            var turns = 0;

            for (int i = 2; i < input.Length; i++)
            {
                var key = input[i].Substring(0, 3);
                var left = input[i].Substring(7, 3);
                var right = input[i].Substring(12, 3);
                options.Add(key, new LeftRight(left, right));
            }

            while (position != "ZZZ")
            {
                var instruction = instructions[turns % instructions.Length];
                position = options[position][instruction];
                turns++;
            }
            return turns;
        }

        /// <summary>
        /// After examining the maps a bit longer, your attention is drawn to a curious fact: 
        /// the number of nodes with names ending in A is equal to the number ending in Z! 
        /// If you were a ghost, you'd probably just start at every node that ends with A and follow
        /// all of the paths at the same time until they all simultaneously end up at nodes that end with Z.
        /// For example:
        ///   LR
        ///   
        ///   11A = (11B, XXX)
        ///   11B = (XXX, 11Z)
        ///   11Z = (11B, XXX)
        ///   22A = (22B, XXX)
        ///   22B = (22C, 22C)
        ///   22C = (22Z, 22Z)
        ///   22Z = (22B, 22B)
        ///   XXX = (XXX, XXX)
        /// Here, there are two starting nodes, 11A and 22A(because they both end with A). 
        /// As you follow each left/right instruction, use that instruction to simultaneously 
        /// navigate away from both nodes you're currently on. Repeat this process until all of 
        /// the nodes you're currently on end with Z. (If only some of the nodes you're on end with Z,
        /// they act like any other node and you continue as normal.) In this example, you would proceed as follows:
        /// Step 0: You are at 11A and 22A.
        /// Step 1: You choose all of the left paths, leading you to 11B and 22B.
        /// Step 2: You choose all of the right paths, leading you to 11Z and 22C.
        /// Step 3: You choose all of the left paths, leading you to 11B and 22Z.
        /// Step 4: You choose all of the right paths, leading you to 11Z and 22B.
        /// Step 5: You choose all of the left paths, leading you to 11B and 22C.
        /// Step 6: You choose all of the right paths, leading you to 11Z and 22Z.
        /// So, in this example, you end up entirely on nodes that end in Z after 6 steps.
        /// Simultaneously start on every node that ends with A.How many steps does it take
        /// before you're only on nodes that end with Z?
        /// Answer should be 14935034899483
        /// </summary>
        public static long Day8Part2()
        {
            var input = File.ReadAllLines("advent2023\\input\\8.txt");
            var instructions = input[0];
            var options = new Dictionary<string, LeftRight>();

            for (int i = 2; i < input.Length; i++)
            {
                var key = input[i].Substring(0, 3);
                var left = input[i].Substring(7, 3);
                var right = input[i].Substring(12, 3);
                options.Add(key, new LeftRight(left, right));
            }

            var positions = options.Keys.Where(k => k[2] == 'A').ToArray();
            var turns = new long[positions.Length];
            var turn = 0;

            while (positions.Any(p => p[2] != 'Z'))
            {
                var instruction = instructions[turn % instructions.Length];
                turn++;

                for (int i = 0; i < positions.Length; i++)
                {
                    if (turns[i] > 0)
                        continue;

                    positions[i] = options[positions[i]][instruction];

                    if (positions[i][2] == 'Z')
                        turns[i] = turn;
                }
            }

            // now find least common multiple
            return turns.Aggregate((a,b) => Utils.Utils.LCM(a,b));
        }
    }
}
