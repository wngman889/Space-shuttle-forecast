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

        public static void ReadCsvFilesFromFileSystem(string folderPath)
        {
            string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");
            var locations = new List<string>();

            // Create a list to store the parsed data
            List<List<CsvObject>> parsedData = new List<List<CsvObject>>();

            try
            {
                foreach (var csvFile in csvFiles)
                {
                    using (var reader = new StreamReader(csvFile))

                    using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        //Skip the location
                        csvReader.Read();

                        //Read the first field of the first record
                        if (csvReader.TryGetField(0, out string field) && field.StartsWith("Location:"))
                        {
                            // Extract the location information
                            locations.Add(field.Replace("Location:", "").Trim());
                        }
                        else
                            Console.WriteLine("\nProblem while extracting the location\n");

                        //using ToList(), the entire file will be read into memory 
                        var records = csvReader.GetRecords<CsvObject>().ToList();
                        parsedData.Add(records);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }

            AnalysisOfCsvFilesArray(locations, parsedData);
        }

        public static void AnalysisOfCsvFilesArray(List<string> locations, List<List<CsvObject>> parsedData)
        {
            Dictionary<CsvObject, string> filteredCriteria = new Dictionary<CsvObject, string>();
            var bestDayForLaunch = new CsvObject();
            var keyValuePairForBestDay = new KeyValuePair<CsvObject, string>();

            try
            {
                for (int i = 0; i < parsedData.Count; i++)
                {
                    foreach (var data in parsedData[i])
                    {
                        if (data.Temperature >= 1 && data.Temperature <= 32 &&
                            data.Wind <= 11 &&
                            data.Humidity <= 55 &&
                            data.Precipitation == 0 &&
                            data.Lightning == "No" &&
                            data.Clouds != "Cumulus" && data.Clouds != "Nimbus")
                        {
                            filteredCriteria.Add(data, locations[i]);
                        }
                    }
                }
                //using linq filter among the criteria the best day with smallest wind and humidity
                keyValuePairForBestDay = filteredCriteria.OrderBy(kv => kv.Key.Wind).ThenBy(kv => kv.Key.Humidity).FirstOrDefault();

                if (keyValuePairForBestDay.Key != null)
                    bestDayForLaunch = keyValuePairForBestDay.Key;
                else
                    Console.WriteLine($"There is no suitable day for launch.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with analysis of the csv files: {ex}");
            }

            //add the object and the string with the location to the writing method
            WriteTheBestDayAndLocationToCsvFile(bestDayForLaunch, keyValuePairForBestDay.Value);

        }

        public static void WriteTheBestDayAndLocationToCsvFile(CsvObject bestDayForLauch, string location)
        {
            bool success = false;

            while (!success)
            {
                Console.Write("Enter the file path to save the CSV file: ");
                string? filePath = Console.ReadLine();

                try
                {
                    using (var writer = new StreamWriter(filePath))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        // Write comment
                        writer.WriteLine($"Location: {location}");

                        csv.WriteHeader<CsvObject>();
                        csv.NextRecord();

                        csv.WriteRecord(bestDayForLauch);
                    }
                    Console.WriteLine("CSV file successfully created.");
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing CSV file: {ex.Message}");
                }
            }
        }
    }
}
