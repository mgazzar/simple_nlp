using System;
using System.IO;
using System.Collections.Generic;
// Need this directive to use our library
using MyLibrary;

namespace MyLibraryClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string sentence = "AI project for Proff Alaa Hamdy ... Made by Maged Mohammed Elgazzar"+
           " sec 2 4th year Computer Engineering ";
            Console.WriteLine(sentence);
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("**************************Result of greedy tokenization*************************");


            // Use the instance methods of the Tokenizer class
            Tokenizer tokenizer = new Tokenizer();

            // use greedy tokenization
            string[] tokens = tokenizer.GreedyTokenize(sentence);
            DisplayTokens(" ", tokens);

            // use improved tokenization, keeping digits
            tokens = tokenizer.Tokenize(sentence, true);
            DisplayTokens("Result of imporved tokenization, keeping digits", tokens);

            // use improved tokenization, throwing away digits
            tokens = tokenizer.Tokenize(sentence, false);
            DisplayTokens("Result of imporved tokenization, throwing away digits", tokens);

            // Use the static methods of the Tool class
            //

            // Get word frequency table
            Dictionary<string, int> wordFreq = Tool.ToStrIntDict(sentence);
            Console.WriteLine("**************************Result of word frequency*****************************");

            DisplayDictionary("", wordFreq);

            // List words by frequency
            wordFreq = Tool.ListWordsByFreq(wordFreq, true);
            Console.WriteLine("**************************List words by frequency******************************");

            DisplayDictionary(" ", wordFreq);

            // Get type token ratio
            Double ttr = Tool.GetTypeTokenRatio(sentence);
            Console.WriteLine("\r\nType token ratio = {0}\r\n", ttr);

            // talking to L
            Console.WriteLine("*******************************************************************************");
            Console.WriteLine("*******************************************************************************");
            Console.WriteLine("*******************************************************************************");

            // begin of while loop
            // 1- The user talk 
            string x = string.Empty;
            string y = string.Empty;
            string lines = (@"C:\Users\Maged\Desktop\UseNlpLibrary\MyLibraryClient\training.txt");
            while (true)
            {
                Console.Write("#Maged :");

                x = Console.ReadLine();
                // L responds to the user
                y = lcheck(x);
                Console.Write("#L :");
                Console.WriteLine(y);
                
                // appending concersation
                using (StreamWriter w = File.AppendText(lines))
                {
                    w.WriteLine();
                    w.WriteLine(x);
                    // if I want to append the answer to new answers or what I said

                    Console.Write("do u have a better answer?");
                   x = Console.ReadLine();
                   if (x == "yes") y = Console.ReadLine();
                   else { }
                    w.WriteLine(y);
                    w.Flush();
                    w.Close();
                }

                // if I want to quit.
                if (x == "q")
                {
                    break;
                }
            }// end of while loop
        }

        // check function 
        private static string lcheck(string z)
        {
            string w = string.Empty;
            Tokenizer tokenizer = new Tokenizer();

            string[] lines = File.ReadAllLines(@"C:\Users\Maged\Desktop\UseNlpLibrary\MyLibraryClient\training.txt");
            string X = string.Empty;
            string Y = string.Empty;
            string keyword = string.Empty;
            // 1- use tokenaizer method to get the key word 
            string[] tokens = tokenizer.GreedyTokenize(z);
            // constants questions
            if (z == "how are u?")
            {
                w = "I'm fine and u?";
                return w;
            }
            else if (z == "who developed u?")
            {
                w = "Maged Elgazzar";
                return w;
            }
            else if (z == "what's today date?")
            {
                w = DateTime.Now.Date.ToShortDateString();
                return w;
            }
            else if (z == "hi")
            {
                w = "hi";
                return w;
            }
            //  Question from files "training.txt"
            
            // 2- search for the keyword in the training file.
            // 2 words tokens I'm happy , I'm sad
            // search for the second keywords
            if (tokens.Length == 2)
            {
                keyword = tokens[1];
            }
            // 3 words tokens 
            // I'm very happy , today Iam happy 
            else if (tokens.Length == 3)
            {
                keyword = tokens[2];
            }
            else if (tokens.Length == 1)
            {
                keyword = tokens[0];
            }
            // 4 tokens
            // I've a new friend 
            // why you have a new frinds (X) not accepatble case
            // here we look in the file for the special keyword token[3]
            else if (tokens.Length == 4)
            {
                keyword = tokens[3];
            }
            // 6 words tokens 
            // Iam not ready(X) to do(Y) that 
            // why u are not ready(X) for that ?
            //else if (tokens.Length == 6)
            //{
            //    tokens = tokenizer.GreedyTokenize(z);
            //    X = tokens[2];
            //    w = "why u are not " + X + " for that?";
            //    return (w);
            //}
            // Exile
            // 5 words tokens 
            // Iam ready(X) to do(Y) that 
            // say something like great ,,, really , sounds good !!
          
            // search in the files
            // tokenaize the line into words 
            // search in the tokenized words if matched the keyword
            // Use a tab to indent each line of the file.
            //Console.WriteLine("\t" + lines[i]);
            int j = 0;
            int k = 0;

            for (k = 0; k < lines.Length; k++)
            {
                w = lines[k];

                tokens = tokenizer.GreedyTokenize(w);

                for (j = 0; j < tokens.Length; j++)
                {
                    if (string.Compare(keyword, tokens[j]) == 0)
                    {
                        return(lines[k + 1]);
                        // the w strings get the nextLine form the text which refer to the answer
                        //return(w);
                    }
                }
            }

            // if not found use this scenario

            // do you like me? 4 tokens
            // search in the file for the whole string not a token ...

            // Non-Question
            return w;
        }

        private static void DisplayDictionary(string title, Dictionary<string, int> dict)
        {
            foreach (KeyValuePair<string, int> kv in dict)
            {
                Console.WriteLine(kv.Key + "\t" + kv.Value);
            }
        }

        private static void DisplayTokens(string title, string[] tokens)
        {
            foreach (string token in tokens)
            {
                Console.Write(token + " | ");
            }
        }
    }
}