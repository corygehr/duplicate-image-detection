using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace DuplicateImageDetection
{
    class Program
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args">
        /// [0] Photo directory
        /// </param>
        static void Main(string[] args)
        {
            string photosPath = String.Empty;
            int threshold = 25;

            if(args.Length != 2)
            {
                Console.WriteLine("Usage: {pathToPhotos} {threshold}");
                return;
            }
            else
            {
                photosPath = args[0];
                threshold = Int32.Parse(args[1]);
            }

            // Prepare for storing hashes
            Dictionary<string, string> hashes = new Dictionary<string, string>();

            // Create reference to directory
            string[] photos = Directory.GetFiles(photosPath);

            // Process hashes for each file
            foreach(string photoPath in photos)
            {
                string currentFile = Path.GetFileName(photoPath);

                if(currentFile != "desktop.ini")
                {
                    string hash = String.Empty;

                    try
                    {
                        // Get hash
                        hash = _getImageHash(photoPath);

                        // Check Levenschtein distance to existing keys
                        foreach (string existingHash in hashes.Keys)
                        {
                            // Compare levenschtein distance
                            int score = _getDamerauLevenshteinDistance(hash, existingHash);

                            if(score <= threshold)
                            {
                                string matchedFile = hashes[existingHash];
                                Console.WriteLine("[INFO] {0} matches {1} (score: {2})", currentFile, Path.GetFileName(hashes[existingHash]), score);
                                break;
                            }
                        }

                        // Store hash in Dictionary for other lookups
                        if(!hashes.ContainsKey(hash))
                        {
                            hashes[hash] = currentFile;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[ERROR] Failed to process {0}: {1}.", photoPath, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Generates a hash of an image
        /// </summary>
        /// <param name="filePath">Path to an image file</param>
        /// <returns>Bit hash</returns>
        private static string _getImageHash(string filePath)
        {
            // Get reference to input image
            Bitmap image = new Bitmap(filePath);
            // Resize to begin calculating hash
            Bitmap resized = new Bitmap(image, new Size(32, 32));
            // Hash output
            string hash = String.Empty;

            // Resize image
            for(int y=0; y<resized.Height; y++)
            {
                for(int x=0; x<resized.Width; x++)
                {
                    // Reduce to true/false
                    if(resized.GetPixel(x, y).GetBrightness() < 0.5f)
                    {
                        hash += "0";
                    }
                    else
                    {
                        hash += "1";
                    }
                }
            }

            return hash;
        }

        private static int _getDamerauLevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException(s, "String Cannot Be Null Or Empty");
            }

            if (string.IsNullOrEmpty(t))
            {
                throw new ArgumentNullException(t, "String Cannot Be Null Or Empty");
            }

            int n = s.Length; // length of s
            int m = t.Length; // length of t

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            int[] p = new int[n + 1]; //'previous' cost array, horizontally
            int[] d = new int[n + 1]; // cost array, horizontally

            // indexes into strings s and t
            int i; // iterates through s
            int j; // iterates through t

            for (i = 0; i <= n; i++)
            {
                p[i] = i;
            }

            for (j = 1; j <= m; j++)
            {
                char tJ = t[j - 1]; // jth character of t
                d[0] = j;

                for (i = 1; i <= n; i++)
                {
                    int cost = s[i - 1] == tJ ? 0 : 1; // cost
                                                       // minimum of cell to the left+1, to the top+1, diagonally left and up +cost                
                    d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
                }

                // copy current distance counts to 'previous row' distance counts
                int[] dPlaceholder = p; //placeholder to assist in swapping p and d
                p = d;
                d = dPlaceholder;
            }

            // our last action in the above loop was to switch d and p, so p now 
            // actually has the most recent cost counts
            return p[n];
        }
    }
}
