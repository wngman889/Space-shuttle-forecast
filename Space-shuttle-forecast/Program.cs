using CsvHelper;
using MailKit.Net.Smtp;
using MimeKit;
using System.Globalization;
namespace Space_shuttle_forecast
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? folderPath = "";

            while (true)
            {
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

                    //writing a mail
                    SendEmailViaSMTP();

                    //if everything is okay it leave the console
                    break;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine($"\nError: {e.Message}\n");
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine($"\nError: {e.Message}\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nError: {e.Message}\n");
                }
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
                Console.WriteLine($"\nError reading CSV file: {ex.Message}");
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
                //using linq filter among the criteria to make the best day with smallest wind and humidity
                keyValuePairForBestDay = filteredCriteria.OrderBy(kv => kv.Key.Wind).ThenBy(kv => kv.Key.Humidity).FirstOrDefault();

                if (keyValuePairForBestDay.Key != null)
                    bestDayForLaunch = keyValuePairForBestDay.Key;
                else
                    Console.WriteLine($"\nThere is no suitable day for launch.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError with analysis of the csv files: {ex}");
            }

            //add the object and the string with the location to the writing method
            WriteTheBestDayAndLocationToCsvFile(bestDayForLaunch, keyValuePairForBestDay.Value);

        }

        public static void WriteTheBestDayAndLocationToCsvFile(CsvObject bestDayForLauch, string location)
        {
            bool success = false;

            while (!success)
            {
                Console.Write("\nEnter the file path to save the CSV file: ");
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
                    Console.WriteLine("\nCSV file successfully created.\n");
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError writing CSV file: {ex.Message}");
                }
            }
        }

        public static void SendEmailViaSMTP()
        {
            string? senderMail, senderMailPassword, subjectOfMail, receiverMail, filePathWithWeatherForecast;
            bool success = false;

            while (!success)
            {
                try
                {
                    Console.Write("\nFrom which mail you send: ");
                    //senderp@outlook.com
                    senderMail = Console.ReadLine();

                    Console.Write("\nPassword: ");
                    //Sender123
                    //GetPassword method is for hiding the password with '*'
                    senderMailPassword = GetPassword();

                    Console.Write("\nWrite subject of the mail: ");
                    subjectOfMail = Console.ReadLine();

                    Console.Write("\nWrite which mail should receive it: ");
                    //receiver123456@outlook.com
                    receiverMail = Console.ReadLine();

                    Console.Write("\nWrite file location of the csv file: ");
                    //E:\csv-file-with-best-day-to-lauch\LauchAnalysisReport.csv
                    filePathWithWeatherForecast = Console.ReadLine();

                    var email = new MimeMessage();

                    email.From.Add(new MailboxAddress(subjectOfMail, senderMail));
                    email.To.Add(new MailboxAddress("Receiver", receiverMail));

                    email.Subject = subjectOfMail;

                    var builder = new BodyBuilder();
                    builder.Attachments.Add(filePathWithWeatherForecast);
                    email.Body = builder.ToMessageBody();

                    using (var smtp = new SmtpClient())
                    {
                        smtp.Connect("smtp.outlook.com", 587, false);

                        //authentication via smtp
                        smtp.Authenticate(senderMail, senderMailPassword);

                        smtp.Send(email);
                        smtp.Disconnect(true);
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public static string GetPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Ignore any key if it's not a valid character or enter key
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*"); // Display asterisk (*) instead of the actual character
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b"); // Move cursor back, overwrite character with space, move cursor back again
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password;
        }
    }
}
