using CsvHelper;
using System.Globalization;

namespace Space_shuttle_forecast
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int choice;
            string? location = "", folderPath = "";

            while (true)
            {
                Console.WriteLine("Choose a launch location:");
                Console.WriteLine("1. Kourou, French Guyana");
                Console.WriteLine("2. Tanegashima, Japan");
                Console.WriteLine("3. Cape Canaveral, USA");
                Console.WriteLine("4. Mahia, New Zealand");
                Console.WriteLine("5. Kodiak, USA");
                Console.WriteLine("0. Leave");

                Console.Write("Enter location for launch (0-5): ");
                choice = Convert.ToInt32(Console.ReadLine());

                if (choice == 0)
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }
                else if (choice < 1 || choice > 5)
                {
                    Console.WriteLine("\nWrite number from the menu.\n");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        location = "Kourou, French Guyana";
                        break;
                    case 2:
                        location = "Tanegashima, Japan";
                        break;
                    case 3:
                        location = "Cape Canaveral, USA";
                        break;
                    case 4:
                        location = "Mahia, New Zealand";
                        break;
                    case 5:
                        location = "Kodiak, USA";
                        break;
                    case 0:
                        break;
                    default:
                        location = "";
                        break;
                }

                Console.WriteLine($"You chose {location}");

                try
                {
                    Console.Write("Enter folder path: ");
                    folderPath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(folderPath))
                    {
                        throw new ArgumentNullException("Folder path cannot be empty.");
                    }

                    if (!Directory.Exists(folderPath))
                    {
                        throw new FileNotFoundException("Folder does not exist.");
                    }
                    //Reading the csv file
                    ReadCsvFilesFromFileSystem(folderPath);
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }

                Console.WriteLine($"Launch location: {location}");
                Console.WriteLine($"File path: {folderPath}");
            }

        }

        public static List<CsvObject> ReadCsvFilesFromFileSystem(string folderPath)
        {
            string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");

            // Create a list to store the parsed data
            List<CsvObject> parsedData = new List<CsvObject>();

            try
            {
                foreach (var csvFile in csvFiles)
                {
                    using (var reader = new StreamReader(csvFile))

                    using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        //using ToList(), the entire file will be read into memory 
                        var records = csvReader.GetRecords<CsvObject>();
                        parsedData.AddRange(records);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }

            return parsedData;
            // Now parsedData contains the combined CSV data from all files
        }
    }
}
