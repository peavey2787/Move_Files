#Move_Files
Watch a given folder for a specific extension and move those files to a specified directory.

MoveFilesEngine mfe = new MoveFilesEngine();

// Have user pick which folder to watch, what extension to look for, and where to move the files to.

mfe.Setup();

// Start watching the folder and moving files until the program is closed by the user

mfe.Start();
