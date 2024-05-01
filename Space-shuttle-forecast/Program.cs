using CsvHelper;
using System.Globalization;

namespace Space_shuttle_forecast
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        public static List<CsvObject> ReadCsvFilesFromFileSystem(string[] filePaths)
        {
            // Create a list to store the parsed data
            List<CsvObject> parsedData = new List<CsvObject>();

            try
            {
                foreach (var filePath in filePaths)
                {
                    using (var reader = new StreamReader(filePath))

                    using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        //using ToList(), the entire file will be read into memory 
                        var records = csvReader.GetRecords<CsvObject>().ToList();
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
