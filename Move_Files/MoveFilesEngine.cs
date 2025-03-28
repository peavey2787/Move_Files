using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Move_Files
{
    class MoveFilesEngine
    {
        #region Variables
        private const string directoryNotFoundError = "Directory not found!";
        private const string invalidDirectoryError = "Invalid directory!";
        private const string invalidInputError = "\nInvalid input! Please only press Y or N to answer the question.";
        private const string invalidExtensionError = "\nInvalid extension! Please make sure you begin with a period.";
        private static string defaultDownloadsDir = @"C:\Users\" + Environment.UserName + @"\Downloads\";
        private static string defaultDomainDownloadsDir = @"C:\Users\" + Environment.UserName + "." + Environment.UserDomainName + @"\Downloads\";
        private static string defaultDomanDocumentsDir = @"C:\Users\" + Environment.UserName + "." + Environment.UserDomainName + @"\Documents\";
        private static string defaultDocumentsDir = @"C:\Users\" + Environment.UserName + @"\Documents\";
        private static string defaultDestinationDirectory = @"\\MyWinServer\Torrents\";
        private string watchedFolder;
        private string destinationFolder;
        #endregion

        #region Public methods
        public void MoveFiles()
        {
            InitializeDefaults();
            ShowUserInstructions();
        }
        public void Setup()
        {
            WatchedFolder = PickFolder("watched", WatchedFolder);
            DestinationFolder = PickFolder("destination", DestinationFolder);
            SetExtensionToWatch();
        }
        public void Start()
        {
            // TODO: Allow user to stop and change options.
            //       Allow users to watch multiple file extensions with different destinations.
            // Confirm the paths/extension are correct or re-enter them.
            if (!OptionsAreCorrect())
            {
                Setup();
                Start();
            }

            // Run until the user closes the program.
            while (true)
            {
                Console.WriteLine("\nSearching... [ " + WatchedFolder + " ]");

                // Get all files with the given extension in the watched folder. 
                var files = Directory.GetFiles(WatchedFolder, "*" + ExtensionToWatch, SearchOption.TopDirectoryOnly);

                // Foreach matching file attempt to move it to the given destination.
                foreach (string file in files)
                {
                    bool tryAgain = false;
                    do
                    {
                        try
                        {
                            File.Move(file, DestinationFolder + Path.GetFileName(file));
                            Console.WriteLine("Moved File [ " + file + " ]");
                            Console.WriteLine("To: [ " + DestinationFolder + Path.GetFileName(file) + " ]");
                            tryAgain = false;
                        }
                        catch (Exception e)
                        {
                            // TODO: Add options for the user to decide wether or not to overwrite.
                            // If the file exists already in the destination then replace it with the newer one.
                            // Otherwise inform the user of the problem.
                            if (File.Exists(DestinationFolder + Path.GetFileName(file)))
                            {
                                File.Delete(DestinationFolder + Path.GetFileName(file));
                                tryAgain = true;
                            }
                            else
                            {
                                // TODO: Create a log file to save any errors/files not moved.
                                Console.WriteLine("Failed to move [ " + file + " ] to [ " + DestinationFolder + Path.GetFileName(file) + " ]");
                                Console.WriteLine("Because : " + e.Message);
                                tryAgain = false;
                            }
                        }
                    } while (tryAgain);
                }

                // TODO: Allow user to set how often to check for new files to move.
                // Take it easy on the cpu and user's eyes.
                Thread.Sleep(3000);
            }
        }
        #endregion

        #region Private methods
        private void InitializeDefaults()
        {
            ExtensionToWatch = ".torrent";

            // Check if the user's folder has .DOMAINNAME 
            // I.e. a local account already exists with same name as domain user
            // If not check just the username 
            if (Directory.Exists(defaultDestinationDirectory))
                DestinationFolder = defaultDestinationDirectory;
            else if (Directory.Exists(defaultDomanDocumentsDir))
                DestinationFolder = defaultDomanDocumentsDir;
            else if (Directory.Exists(defaultDocumentsDir))
                DestinationFolder = defaultDocumentsDir;
                
            if (Directory.Exists(defaultDomainDownloadsDir))
                watchedFolder = defaultDomainDownloadsDir;
            else if (string.IsNullOrEmpty(watchedFolder) && Directory.Exists(defaultDownloadsDir))
                watchedFolder = defaultDownloadsDir;
        }
        private void ShowUserInstructions()
        {
            // Instructions on how this program works
            Console.WriteLine("***** This program will allow you to watch a folder for certain file extensions.              *****");
            Console.WriteLine("***** Once a file is found with the given extension it will be moved to a destination folder. *****");
            Console.WriteLine("***** Please note that if a file already exists it will be overwritten!                       *****");
        }
        private string PickFolder(string folderName, string folderPath)
        {
            var question = String.Format("\n\nCurrent {0} directory is {1} , do you want to use this folder? (Y)es or (N)o", folderName, folderPath);
            var answer = AskYesOrNo(question);

            // User selected default directory, end of function.
            if (answer == 'y')
                return folderPath;

            // User is manually entering the path.
            // Ensure the user inputs valid paths.
            bool isValidDir = false;
            do
            {
                Console.WriteLine("\n Please enter the path to the {0} directory: ", folderName);
                folderPath = Console.ReadLine();

                if (!Directory.Exists(folderPath))
                    Console.WriteLine("\n {0}. Please double check the path entered.", directoryNotFoundError);
                else
                {
                    isValidDir = true;
                    // Make sure there is a trailling '\' to successfully move files.
                    if(folderPath.LastIndexOf("\\") != folderPath.Length)
                        folderPath += "\\";
                }
            } while (!isValidDir);

            return folderPath;
        }
        private void SetExtensionToWatch()
        {
            var question = String.Format("\nCurrent extension is {0}, do you want to use this extension? (Y)es or (N)o", ExtensionToWatch);
            var answer = AskYesOrNo(question);

            // User selected default extension, end of function.
            if (answer == 'y')
                return;

            // Since anyone can create their own file extension.
            // Just ensure the input from user starts with a period
            // and doesn't include illegal characters.
            bool isValidExtension = false;
            string extension = "";
            do
            {
                Console.WriteLine("\n Enter the extension to watch beginning with a period. (.exe, .jpg, .png, etc.)");
                extension = Console.ReadLine();

                if (extension.IndexOf('.') != 0 && !HasInvalidPathCharacters(extension))
                    Console.WriteLine(invalidExtensionError);
                else
                    isValidExtension = true;
            } while (!isValidExtension);

            ExtensionToWatch = extension;
        }
        private bool OptionsAreCorrect()
        {
            var question = String.Format("\nWatching folder {0}", WatchedFolder);
            question += String.Format("\nFor files with the {0} extension", ExtensionToWatch);
            question += String.Format("\nTo move them to the destination directory located at {0}", DestinationFolder);
            question += String.Format("\n\n Is this correct? (Y)es or (N)o");

            var answer = AskYesOrNo(question);

            if (answer == 'y')
                return true;

            return false;
        }
        #endregion

        #region Private validation methods
        private char AskYesOrNo(string question)
        {
            // Ask the user a yes/no question and validate their input.
            char keyPressed = ' ';
            bool isValidInput = false;
            do
            {
                Console.WriteLine(question);
                keyPressed = Console.ReadKey().KeyChar;

                if (!IsValidInput(keyPressed))
                    Console.WriteLine(invalidInputError);
                else
                    isValidInput = true;
            } while (!isValidInput);

            return char.ToLower(keyPressed);
        }
        private static bool IsValidInput(char c)
        {
            if (char.ToLower(c) == 'y' || char.ToLower(c) == 'n')
                return true;
            return false;
        }
        private static bool HasInvalidPathCharacters(string path)
        {
            for (var i = 0; i < path.Length; i++)
            {
                int c = path[i];
                if (c == '\"' || c == '<' || c == '>' || c == '|' || c == '*' || c == '?' || c < 32)
                    return true;
            }

            return false;
        }
        #endregion

        #region Properties
        public string ExtensionToWatch { get; set; }
        public string WatchedFolder
        {
            get => watchedFolder;
            set
            {
                if (Directory.Exists(value))
                    watchedFolder = value;
                else
                    watchedFolder = invalidDirectoryError;
            }
        }
        public string DestinationFolder
        {
            get => destinationFolder;
            set
            {
                if (Directory.Exists(value))
                    destinationFolder = value;
                else
                    destinationFolder = invalidDirectoryError;
            }
        }
        #endregion
    }
}
