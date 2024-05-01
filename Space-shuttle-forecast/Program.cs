using CsvHelper;
using System.Formats.Asn1;
using System.Globalization;

namespace Space_shuttle_forecast
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        public static async Task ReadCsvFilesFromFileSystem(string[] filePaths)
        {
            // Create a list to store the parsed data
            List<MyDataClass> parsedData = new List<MyDataClass>();

            try
            {
                foreach (var filePath in filePaths)
                {
                    using (var reader = new StreamReader(filePath))

                    using (var csvReader = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                    {
                        //usuing ToList(), the entire file will be read into memory 
                        var records = csvReader.GetRecords<MyDataClass>().ToList();
                        parsedData.AddRange(records);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }

            // Now parsedData contains the combined CSV data from all files
            // You can analyze or further process this data as needed
        }
    }
}
