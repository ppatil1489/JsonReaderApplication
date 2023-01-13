using JsonMonitorService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace JsonMonitorServiceTest
{
    public class JsonFileCreatorHelper
    {
        private readonly string _fileName = "DataReader.json";

        public string FilePath = string.Empty;

        public JsonFileCreatorHelper()
        {
            FilePath = GetFilePath();
        }


        public void WriteContentToFile(List<Book> books)
        {

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(books));
        }

        public List<Book> ReadContentFromFile()
        {
            List<Book> JsonFileContents = new List<Book>();
            string text = File.ReadAllText(FilePath);
            var deserializeJsonObjects = JsonConvert.DeserializeObject<List<Book>>(text);
            if (deserializeJsonObjects != null)
            {
                JsonFileContents = deserializeJsonObjects;
            }
            return JsonFileContents;
        }

        public void DeleteTemporaryFile()
        {
            File.Delete(FilePath);
        }

        private string GetFilePath()
        {
            string filePath = string.Empty;
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                string? assemblyDirectory = Path.GetDirectoryName(assemblyPath);
                if (!string.IsNullOrEmpty(assemblyDirectory))
                {
                    filePath = Path.Combine(assemblyDirectory, _fileName);
                }
            }
            return filePath;
        }

    }
}
