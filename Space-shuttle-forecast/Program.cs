using CsvHelper;
using System.Globalization;

namespace Space_shuttle_forecast
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int choice;
            string? location = "", filePath = "";

            while (true)
            {
                Console.WriteLine("Choose a launch location:");
                Console.WriteLine("1. Kourou, French Guyana");
                Console.WriteLine("2. Tanegashima, Japan");
                Console.WriteLine("3. Cape Canaveral, USA");
                Console.WriteLine("4. Mahia, New Zealand");
                Console.WriteLine("5. Kodiak, USA");
                Console.WriteLine("0. Leave");

                Console.Write("Enter your choice (0-5): ");
                choice = Convert.ToInt32(Console.ReadLine());

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

                if (choice == 0)
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }

                Console.WriteLine($"You chose {location}");

                try
                {
                    Console.Write("Enter file path: ");
                    filePath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        throw new ArgumentNullException("File path cannot be empty.");
                    }

                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException("File does not exist.");
                    }

                    ReadCsvFilesFromFileSystem(filePath);
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
                Console.WriteLine($"File path: {filePath}");
            }

        }

        public static List<CsvObject> ReadCsvFilesFromFileSystem(string filePath)
        {
            // Create a list to store the parsed data
            List<CsvObject> parsedData = new List<CsvObject>();

            try
            {
                    using (var reader = new StreamReader(filePath))

                    using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        //using ToList(), the entire file will be read into memory 
                        var records = csvReader.GetRecords<CsvObject>().ToList();
                        parsedData.AddRange(records);
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
