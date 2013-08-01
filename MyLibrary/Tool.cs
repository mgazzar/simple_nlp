using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MyLibrary
{
    public static class Tool
    {
        private static Tokenizer tokenizer = new Tokenizer();

        /// <summary>
        /// Splits an n-letter word into a string array. The array's size will be (n-1) * 2. 
        /// For example, 'carried', a 7-letter word, will be split into a string array
        /// containing the following (7-1) * 2 = 12 elements:
        /// {c, arried, ca, rried, car, ried, carr, ied, carri, ed, carri, d}.
        /// </summary>
        /// <param name="word">the word to split</param>
        /// <returns>all pairs of strings which the word can be split into</returns>
        public static string[] SplitWord(string word)
        {
            //The word must have at least 2 letters
            if (word.Length < 2)
                return null;

            int arrSize = (word.Length - 1) * 2;
            string[] strArr = new string[arrSize];

            int index = 0;

            for (int cutPosition = 1; cutPosition < word.Length; cutPosition++)
            {
                strArr[index] = word.Substring(0, cutPosition);
                strArr[index + 1] = word.Substring(cutPosition, word.Length - cutPosition);

                //Since we collect two elements per time, we need to increase the index by 2.
                index += 2;
            }
            return strArr;
        }

        /// <summary>
        /// Create a string-integer dictionary out of a linked list of tokens.
        /// </summary>
        /// <param name="tokens"> the tokens to create the frequency table. For this
        /// method, there is no difference between List and LinkedList types. </param>
        private static Dictionary<string, int> BuildFreqTable(LinkedList<string> tokens)
        {
            Dictionary<string, int> token_freq_table = new Dictionary<string, int>();
            foreach (string token in tokens)
            {
                if (token_freq_table.ContainsKey(token))
                    token_freq_table[token]++;
                else
                    token_freq_table.Add(token, 1);
            }

            return token_freq_table;
        }

        /// <summary>
        /// Make a string-integer dictionary out of an array of words.
        /// </summary>
        /// <param name="words">the words out of which to make the dictionary</param>
        /// <returns>a string-integer dictionary</returns>
        public static Dictionary<string, int> ToStrIntDict(string text)
        {
            if (text == null)
                return null;

            string[] words = tokenizer.Tokenize(text, false);
            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (string word in words)
            {
                // if the word is in the dictionary, increment its freq.
                if (dict.ContainsKey(word))
                {
                    dict[word]++;
                }
                // if not, add it to the dictionary and set its freq = 1
                else
                {
                    dict.Add(word, 1);
                }
            }

            return dict;
        }

        /// <summary>
        /// Sort a string-int dictionary by its entries' values.
        /// </summary>
        /// <param name="strIntDict">a string-int dictionary to sort</param>
        /// <param name="sortOrder">one of the two enumerations: Ascening and Descending</param>
        /// <returns>a string-integer dictionary sorted by integer values</returns>
        public static Dictionary<string, int> ListWordsByFreq(Dictionary<string, int> strIntDict, bool isDescending)
        {
            // Copy keys and values to two arrays
            string[] words = new string[strIntDict.Keys.Count];
            strIntDict.Keys.CopyTo(words, 0);

            int[] freqs = new int[strIntDict.Values.Count];
            strIntDict.Values.CopyTo(freqs, 0);

            //Sort by freqs: it sorts the freqs array, but it also rearranges
            //the words array's elements accordingly (not sorting)
            Array.Sort(freqs, words);

            // If sort order is descending, reverse the sorted arrays.
            if (isDescending)
            {
                //reverse both arrays
                Array.Reverse(freqs);
                Array.Reverse(words);
            }

            //Copy freqs and words to a new Dictionary<string, int>
            Dictionary<string, int> dictByFreq = new Dictionary<string, int>();

            for (int i = 0; i < freqs.Length; i++)
            {
                dictByFreq.Add(words[i], freqs[i]);
            }

            return dictByFreq;
        }

        public static double GetTypeTokenRatio(string text)
        {
            double typeTokenRatio = 0.0;

            // Tokenize text into tokens, no digits or punctuations.
            string[] tokens = tokenizer.Tokenize(text, false);

            // dump array of words into a HashSet of string. 
            HashSet<string> types = new HashSet<string>();

            // HashSet ignores duplicated elements which ensures for us that duplicated words be counted only once.
            foreach (string token in tokens)
            {
                types.Add(token);
            }

            // A sanity check: if types set is empty, set typeTokenRatio = double.NaN, i.e. Not a Number. 
            // Otherwise, we'll get a "divided by 0" Exception.

            if (types.Count == 0)
            {
                typeTokenRatio = double.NaN;
            }
            else
            {
                // Be very aware that you need to cast either types.Count or tokens.Length into 
                // double type; otherwise you'll always get 0 as the result.
                typeTokenRatio = (double)types.Count / tokens.Length;
            }

            return typeTokenRatio;
        }

        public static double GetFileTypeTokenRatio(string filePath)
        {
            return GetTypeTokenRatio(File.ReadAllText(filePath));
        }

        /// <summary>
        /// Computes the average type token ratio of an array of tokens, based on the window size.
        /// Algorithm:
        /// If windowSize &gt;= tokens.Length, average type token ratio is the general type token
        /// ratio of all tokens.
        /// </summary>
        /// <param name="tokens">the array of the tokens to calculate the average type token ratio</param>
        /// <param name="windowSize">the number of the tokens per window</param>
        private static double GetAverageTypeTokenRatio(string filePath, int windowSize)
        {
            LinkedList<string> movingWindow = new LinkedList<string>();
            string[] tokens = tokenizer.TokenizeFile(filePath, false);

            int index = 0;
            while (index < windowSize)
            {
                movingWindow.AddLast(tokens[index]);
                index++;
            }

            // Build frequency table of this window of tokens
            Dictionary<string, int>
            movingFreqTable = BuildFreqTable(movingWindow);

            // This type token ratio keeps changing
            double finalTTR = (double)movingFreqTable.Count / movingWindow.Count;

            int windowCount = 1;

            // Now index stops at windowSize position of the tokens.
            while (index < tokens.Length)
            {
                // Check the first token of the moving window of tokens and remove it from the moving window.
                string firstToken = movingWindow.First.Value;
                movingWindow.RemoveFirst();

                // Check its frequency in the frequency table. If it is 1, it means that this token
                // occurs in the moving window only once, so we can safely remove it from the moving
                // window; otherwise, it appears more than once, so we cannot delete it but we can
                // reduce its frequency by 1.
                if (movingFreqTable[firstToken] == 1)
                    movingFreqTable.Remove(firstToken);
                else
                    movingFreqTable[firstToken]--;

                // Find the next available token. If it is in the moving frequency table, increase its
                // frequency value by 1; otherwise, add it as a new entry and set its frequency to 1.
                string newToken = tokens[index];

                if (movingFreqTable.ContainsKey(newToken))
                    movingFreqTable[newToken]++;
                else
                    movingFreqTable.Add(newToken, 1);

                // Add this word to the moving window so that the window always has the same number of tokens.
                movingWindow.AddLast(newToken);

                // Re-compute the type token ratio of this changed window.
                double thisTTR = (double)movingFreqTable.Count / windowSize;

                // Add this new type token ratio to the final type token ratio.
                finalTTR += thisTTR;

                // Update index position and window counters
                index++;
                windowCount++;
            }

            // We need to divided the final type token ratio by the number of windows
            finalTTR = finalTTR / windowCount;

            return finalTTR;
        }
    }
}
