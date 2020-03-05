using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TryCatchCop
{
    /// <summary>
    /// An application to find missing try catch blocks in project's methods.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Takes the path of the directory
            List<string> FilesToProcess = new List<string>();
            Console.WriteLine("Please insert the path to the file directory:");
            string folderPath = Console.ReadLine();
            if (Directory.Exists(folderPath))
            {
                FilesToProcess = FilesToWorkOn(folderPath);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", folderPath);
            }

            // Processes the whole given folder
            foreach (var path in FilesToProcess)
            {
                LookForMissingTryCatchBlocks(path);
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Checks for missing try catch blocks in each method
        /// </summary>
        /// <param name="pathToFile">the path to the current file we are looking at</param>
        private static void LookForMissingTryCatchBlocks(string pathToFile)
        {
            // Read each line of the file into a string array. Each element
            string[] lines = System.IO.File.ReadAllLines(@"" + pathToFile);

            List<ClassObject> classesFound = new List<ClassObject>();
            int lineCounter = 0;
            int braceCounter = 0;
            int methodID = 0;
            int limit = 1;
            for (int i = 0; i < lines.Count(); i++)
            {
                // Has starting brace
                if (HasOpenBrace(lines[i]))
                {
                    int previousBraceCounter = braceCounter;
                    braceCounter++;
                    if (braceCounter == limit && previousBraceCounter == limit - 1)
                    {
                        methodID++;
                    }
                }

                // Has ending brace
                if (HasClosingBrace(lines[i]))
                {
                    braceCounter--;
                    if (braceCounter < -1)
                    {
                        braceCounter = 0;
                    }
                }

                if (classesFound.Count() == 0 && methodID == 1)
                {
                    if (lines[i - 1] != null)
                    {
                        string lineToSave = DetermineWhichLineToKeep(lines[i], lines[i - 1]);
                        classesFound.Add(new ClassObject(methodID, lineToSave, false, lineCounter));

                        if (classesFound.Count > 0 && (classesFound[classesFound.Count - 1].ClassName.Contains("namespace") || classesFound[classesFound.Count - 1].ClassName.Contains("partial")))
                        {
                            limit++;
                        }
                    }
                    else
                    {
                        classesFound.Add(new ClassObject(methodID, lines[i], false, lineCounter));

                        if (classesFound.Count > 0 && (classesFound[classesFound.Count - 1].ClassName.Contains("namespace") || classesFound[classesFound.Count - 1].ClassName.Contains("partial")))
                        {
                            limit++;
                        }
                    }
                }

                if (classesFound.Count > 0)
                {
                    if (methodID > classesFound[classesFound.Count - 1].MethodID)
                    {
                        string lineToSave = DetermineWhichLineToKeep(lines[i], lines[i - 1]);
                        classesFound.Add(new ClassObject(methodID, lineToSave, false, lineCounter));

                        if (classesFound.Count > 0 && (classesFound[classesFound.Count - 1].ClassName.Contains("namespace") || classesFound[classesFound.Count - 1].ClassName.Contains("partial")))
                        {
                            limit++;
                        }
                    }
                }

                if (lines[i].Contains("try"))
                {
                    classesFound[classesFound.Count - 1].HasTryCatchBlock = true;
                }

                lineCounter++;
            }

            Console.WriteLine();
            Console.WriteLine("Processing file: " + Path.GetFileName(pathToFile) + "...");
            foreach (var cl in classesFound)
            {
                if (cl.HasTryCatchBlock == false && !cl.ClassName.Contains("namespace") && !cl.ClassName.Contains("partial") && !cl.ClassName.Contains("get; set;"))
                {
                    Console.WriteLine(cl.ClassName + " DOES NOT CONTAIN TRY CATCH. LineNumber: " + cl.LineNumber);
                }
            }
        }

        /// <summary>
        /// Determines which line to keep(the method name) to display to the user in the console
        /// </summary>
        /// <param name="thisLine">The current line we are looking at</param>
        /// <param name="previousLine">The previous line of the code</param>
        /// <returns></returns>
        private static string DetermineWhichLineToKeep(string thisLine, string previousLine)
        {
            if (previousLine.Contains("void") || previousLine.Contains("()") || previousLine.Contains("public") || previousLine.Contains("private")
                || previousLine.Contains("internal") || previousLine.Contains("protected") || previousLine.Contains("static") || previousLine.Contains("namespace")
                || previousLine.Contains("partial") || previousLine.Contains("class"))
            {
                return previousLine;
            }
            else return thisLine;
        }

        /// <summary>
        /// Checks if it's the start of a method
        /// </summary>
        /// <param name="line">line to check</param>
        /// <returns>Whether this line contains a method</returns>
        private static bool HasOpenBrace(string line)
        {
            if (line.Contains("{"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if it's the end of a method
        /// </summary>
        /// <param name="line">line to check</param>
        /// <returns>Whether this line contains a method</returns>
        private static bool HasClosingBrace(string line)
        {
            if (line.Contains("}"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Process all files in the directory passed in, recurse on any directories that are found, and process the files they contain
        /// </summary>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        public static List<string> FilesToWorkOn(string targetDirectory)
        {
            List<string> AllClassesFound = new List<string>();

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (fileName.EndsWith(".cs") && !fileName.Contains("designer"))
                {
                    AllClassesFound.Add(fileName);
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                if (subdirectory.EndsWith(".cs") && !subdirectory.Contains("designer"))
                {
                    AllClassesFound.Add(subdirectory);
                }
            }

            return AllClassesFound;
        }
    }
}