using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace WordsFilterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set the console output to UTF-8
            Console.OutputEncoding = Encoding.UTF8;

            string filePath = "words.txt";
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Το αρχείο των λέξεων δεν βρέθηκε!");
                Console.ResetColor();
                return;
            }

            var words = File.ReadAllLines(filePath).ToList();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Εισάγετε το μήκος της λέξης: ");
            Console.ResetColor();
            int wordLength;
            while (!int.TryParse(Console.ReadLine(), out wordLength) || wordLength <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Παρακαλώ εισάγετε έναν έγκυρο θετικό αριθμό για το μήκος της λέξης: ");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Εισάγετε το αρχικό γράμμα: ");
            Console.ResetColor();
            char startLetter = Console.ReadKey().KeyChar;
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Εισάγετε το τελικό γράμμα: ");
            Console.ResetColor();
            char endLetter = Console.ReadKey().KeyChar;
            Console.WriteLine();

            string normalizedStartLetter = RemoveDiacritics(startLetter.ToString());
            string normalizedEndLetter = RemoveDiacritics(endLetter.ToString());

            var filteredWords = words
                .Where(word => word.Length == wordLength)
                .Where(word => RemoveDiacritics(word).StartsWith(normalizedStartLetter, StringComparison.OrdinalIgnoreCase))
                .Where(word => RemoveDiacritics(word).EndsWith(normalizedEndLetter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            char[] displayWord = new char[wordLength];
            for (int i = 0; i < wordLength; i++)
            {
                displayWord[i] = '_';
            }

            displayWord[0] = startLetter;
            displayWord[wordLength - 1] = endLetter;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nΤρέχουσα λέξη: " + new string(displayWord));
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Φιλτραρισμένες λέξεις:");
                Console.ResetColor();

                PrintWordsInTable(filteredWords);

                if (filteredWords.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Δεν υπάρχουν λέξεις που να ταιριάζουν με τα κριτήρια.");
                    Console.ResetColor();
                    break;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Εισάγετε τον δείκτη του γράμματος για αντικατάσταση (1-based): ");
                Console.ResetColor();
                int index;
                while (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > wordLength)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"Παρακαλώ εισάγετε έναν έγκυρο δείκτη μεταξύ 1 και {wordLength}: ");
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Εισάγετε το γράμμα για να τοποθετηθεί σε αυτόν τον δείκτη: ");
                Console.ResetColor();
                char newChar = Console.ReadKey().KeyChar;
                Console.WriteLine();

                string normalizedNewChar = RemoveDiacritics(newChar.ToString());
                displayWord[index - 1] = newChar;

                filteredWords = filteredWords
                    .Where(word => RemoveDiacritics(word)[index - 1] == normalizedNewChar[0])
                    .ToList();
            }
        }

        static void PrintWordsInTable(List<string> words)
        {
            int columns = 5;
            int rows = (int)Math.Ceiling(words.Count / (double)columns);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int wordIndex = row + col * rows;
                    if (wordIndex < words.Count)
                    {
                        Console.Write(words[wordIndex].PadRight(20));
                    }
                }
                Console.WriteLine();
            }
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
