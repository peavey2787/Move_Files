using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoveTorrents
{
    class Program
    {
        private static string Default_Torrent_Directory = @"C:\Users\" + Environment.UserName + @"\Downloads\";
        private static string Domain_User_Directory = @"C:\Users\" + Environment.UserName + "." + Environment.UserDomainName + @"\Downloads\";
        private static string Default_Destination_Directory = @"\\192.168.0.11\Torrents\";

        static void Main(string[] args)
        {
            // Important information
            Console.WriteLine("This program will help you watch a folder for certain file extensions.");
            Console.WriteLine("\nOnce a file is found with the given extension it will be moved to a destination folder.");
            Console.WriteLine("\nBy default the watched folder is the logged in user's Download directory.\n\n)"); 

            // Variables
            string Watched_Folder = "";
            string Destination_Directory = "";
            string Extension_To_Watch = ".torrent";
            char Key_Pressed = ' ';

            #region Get Folder to Watch
            
            // Check if the user's folder has .DOMAINNAME (i.e. a local account already exists with same name as Domain User)
            if (Directory.Exists(Domain_User_Directory))
                Watched_Folder = Domain_User_Directory; 
            // If not check just the username 
            else if (string.IsNullOrEmpty(Watched_Folder) && Directory.Exists(Default_Torrent_Directory)) 
                Watched_Folder = Default_Torrent_Directory;

            // If a folder was found
            if (!string.IsNullOrEmpty(Watched_Folder))
            {
                do
                {
                    // Confirm with the user that this is the folder they want to watch
                    Console.WriteLine("Download Directory found at: " + Watched_Folder);
                    Console.WriteLine("Is this the folder you want to watch? (Y)es (N)o");
                    Key_Pressed = Console.ReadKey().KeyChar;

                    // Validate User's Input
                    if (!Is_Yes_Or_No(Key_Pressed))
                        Console.WriteLine("\nIncorrect input. You must enter 'y' or 'n'.");

                  // Keep asking until the user enters valid input
                } while (!Is_Yes_Or_No(Key_Pressed));

                // If its not then erase it
                if (Key_Pressed == 'N' || Key_Pressed == 'n')
                    Watched_Folder = ""; 
            }

            // If no watched folder is picked then have the user input the folder to watch
            if (string.IsNullOrEmpty(Watched_Folder))
            {
                Console.WriteLine("\nDownload Directory Not found! You will have to enter it manually.");
                Console.WriteLine("\nBelow are all the user's found on this pc.\n");

                // Display all user folders to help
                var User_Folders = Directory.GetDirectories(@"C:\Users\");
                Console.WriteLine(@"Users in C:\Users\ :");
                foreach (string folder in User_Folders)
                    Console.WriteLine(folder);

                // While the user inputs incorrent paths
                while (!Directory.Exists(Watched_Folder))
                {
                    // If the user entered a path let them know it was wrong
                    if (!string.IsNullOrEmpty(Watched_Folder))
                        Console.WriteLine("\nThe Path you entered doesn't exist!");

                    // Get the path from the user
                    Console.WriteLine("\nPlease enter the path to the watched folder: ");
                    Watched_Folder = Console.ReadLine();
                }
            }
            #endregion

            #region Get the File Extension to watch
            do
            {
                // Get the file extension to move
                Console.WriteLine("\nWhat type of file do you want to move?");
                Console.WriteLine("\nIs the default file extension to move '.torrent' ok? (Y)es (N)o");
                Key_Pressed = Console.ReadKey().KeyChar;
            
                // While the user inputs incorrent input
            } while (!Is_Yes_Or_No(Key_Pressed));
            
            // If its not then ask the user to input the correct one
            if (Key_Pressed == 'N' || Key_Pressed == 'n')
            {
                bool Correct = false;
                do
                {
                    Console.WriteLine("\nWhat file extension do you want to move? (Include the .  i.e. '.jpg')");
                    Extension_To_Watch = Console.ReadLine();

                    do
                    {
                        Console.WriteLine("\n Confirm you want to watch files with this extension: " + Extension_To_Watch + "   (Y)es or (N)o");
                        Key_Pressed = Console.ReadKey().KeyChar;

                        // If it is set Correct to true and exit the loop
                        if (Key_Pressed == 'Y' || Key_Pressed == 'y')
                            Correct = true;

                        // While the user inputs incorrect input
                    } while (!Is_Yes_Or_No(Key_Pressed));

                // While the user says "no this is not the file extension I want"
                } while (!Correct);
            }
            #endregion

            #region Get the Destination Folder
            // Confirm destination location
            Console.WriteLine("\nIs the destination folder located at : " + Default_Destination_Directory + " ? (Y)es or (N)o");
            Key_Pressed = Console.ReadKey().KeyChar;

            // If destination is not in the default location
            if (Key_Pressed == 'N' || Key_Pressed == 'n')
            {
                // While the user inputs incorrent paths
                while (!Directory.Exists(Destination_Directory))
                {
                    // If the user entered a path let them know it was wrong
                    if (!string.IsNullOrEmpty(Destination_Directory))
                        Console.WriteLine("\nThe Path you entered doesn't exist!");

                    // Get the path from the user
                    Console.WriteLine("\nPlease enter the destination path: ");
                    Destination_Directory = Console.ReadLine();
                }
            }
            else
                Destination_Directory = Default_Destination_Directory;

            // Inform user of settings
            Console.WriteLine("\nStarting Up...\n");
            Console.WriteLine("Watching Folder: " + Watched_Folder);
            Console.WriteLine("For File Extensions: " + Extension_To_Watch);
            Console.WriteLine("With The Destination Folder: " + Destination_Directory);

            #endregion

            #region Run Forever Moving Files With Given Extension From Watched Folder to Destination Folder
            // Run forever
            while (true)
            {
                Console.WriteLine("Searching... [ " + Watched_Folder + " ]");
                
                // Get all files with extension of .torrent
                var Files = Directory.GetFiles(Watched_Folder, "*" + Extension_To_Watch, SearchOption.TopDirectoryOnly); 
                
                // Foreach file move it
                foreach (string file in Files)
                {
                    try
                    {
                        File.Move(file, Destination_Directory + Path.GetFileName(file));
                        Console.WriteLine("Moved File [ " + file + " ]");
                        Console.WriteLine("To: [ " + Destination_Directory + Path.GetFileName(file) + " ]"); 
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to move [ " + file + " ] to [ " + Destination_Directory + Path.GetFileName(file) + " ]");
                        Console.WriteLine("Because : " + e.Message);
                    }
                }

                // Take it easy on the cpu
                Thread.Sleep(3000);
            }
            #endregion
        }

        public static bool Is_Yes_Or_No(char c)
        {
            if (c == 'Y' || c == 'y' || c == 'N' || c == 'n')
                return true;
            return false;
        }
    }
}
