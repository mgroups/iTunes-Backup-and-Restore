
using System.Data.SQLite;

namespace iTunes_Backup_Restore;

class MGroups
{
    public string dbFilePath;
    public string sourceFilesPath;
    public string destinationFilesPath;

    public MGroups()
    {
        Console.Write("Enter the path to the SQLite database file: ");
        this.dbFilePath = Console.ReadLine();

        Console.Write("Enter the source files path: ");
        this.sourceFilesPath = Console.ReadLine();

        Console.Write("Enter the destination files path: ");
        this.destinationFilesPath = Console.ReadLine();
    }

    public void readMgDb()
    {

        string connectionString = $"Data Source={this.dbFilePath};Version=3;";
        string query = "SELECT * FROM Files WHERE domain = 'CameraRollDomain'";

        using (var connection = new SQLiteConnection(connectionString))
        {
            try
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {

                    using (var reader = command.ExecuteReader())
                    {
                        bool hasRows = false;
                        while (reader.Read())
                        {
                            hasRows = true;
                            //Console.WriteLine($"Column1: {reader["domain"]}, Column2: {reader["relativePath"]}");
                            this.getFilePath(reader["fileID"].ToString(), reader["relativePath"].ToString());
                        }
                        if (!hasRows)
                        {
                            Console.WriteLine("No results found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public void getFilePath(String fileName, String mPath)
    {

        Console.WriteLine("file name : " + fileName);
        Console.WriteLine("m path : " + mPath);

        try
        {
            // Search for the file
            string[] files = Directory.GetFiles(this.sourceFilesPath, fileName, SearchOption.AllDirectories);

            // Check if any files were found
            if (files.Length == 0)
            {
                Console.WriteLine("No files found matching the specified name.");
            }
            else
            {
                Console.WriteLine("Found files:");
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                    this.copyMgFile(file, mPath);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while searching for files: {ex.Message}");
        }

    }

    public void copyMgFile(String sourceFilePath, String mPath)
    {


        // Check if the source file exists
        if (!File.Exists(sourceFilePath))
        {
            Console.WriteLine("Source file does not exist. Please check the path.");
            return;
        }

        // Check if the destination folder exists; if not, create it
        if (!Directory.Exists(this.destinationFilesPath))
        {
            Directory.CreateDirectory(this.destinationFilesPath);
        }

        string destinationFilePath = Path.Combine(this.destinationFilesPath, mPath);

        // Get the directory from the destination file path
        string destinationDirectory = Path.GetDirectoryName(destinationFilePath);

        // Check if the destination directory exists; if not, create it
        if (!string.IsNullOrEmpty(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory); // This will create all necessary directories
        }

        try
        {
            // Copy the file to the new location with the new name
            File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
            Console.WriteLine($"File copied successfully to: {destinationFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while copying the file: {ex.Message}");
        }


    }




}
