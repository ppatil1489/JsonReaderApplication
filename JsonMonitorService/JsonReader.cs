using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JsonMonitorService
{
    /// <summary>
    /// This class used to Read JSON file from file path.
    /// </summary>
    public class JsonReader : IJsonReader, IDisposable
    {
        private readonly int _refreshJSONContentTimeRateInSeconds = 5;
        private readonly Action<List<Book>> _updatedResult;
        private readonly Action<bool> _isProgressStarted;

        private List<Book>? BooksInCache;
        private bool disposedValue;
        private CancellationTokenSource? _cancellationTokenSource;
        private object _lock = new object();

        public string FilePath { get; set; } = string.Empty;

        public JsonReader(Action<List<Book>> updateResult, Action<bool> IsProgressStarted) 
        {
            _updatedResult = updateResult;
            _isProgressStarted = IsProgressStarted;
            FilePath = GetFilePath();
        }

        /// <summary>
        /// This method used to call the json refresh content based on time rate.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateJSONContentOnRefreshTimeRate()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            using (var timer = new PeriodicTimer(TimeSpan.FromSeconds(_refreshJSONContentTimeRateInSeconds)))
            {
                try
                {
                    while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
                    {

                        _isProgressStarted?.Invoke(true);// This Action to visible progress bar on UI
                        await Task.Delay(2000); // To Display progress bar on UI.
                                                // time for reading content from file takes less than second.
                        var result = ReadJsonFileAndUpdateJsonObjects();
                        _updatedResult?.Invoke(result); // This Action to update the results on UI.

                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Monitor service has stopped." + ex);
                }
                finally
                {
                    _cancellationTokenSource.Cancel();
                }
            }
        }

        public void CancelUpdateJSON()
        {
            _cancellationTokenSource?.Cancel();
        }

        private List<Book> ReadJsonFileAndUpdateJsonObjects()
        {
            List<Book> updatedJsonObjects = new List<Book>();
            try
            {

                updatedJsonObjects = ReadJsonFileFromPath();
                // Find updated Json objects from this object- updatedJsonObjects and 
                // set isdataupdated is true to notify the updated objects on UI
                List<Book>? filteredResults = (updatedJsonObjects == null || BooksInCache == null) 
                                             ? null : updatedJsonObjects.Except(BooksInCache, new BookEqualityComparer()).ToList();

                if (filteredResults != null && updatedJsonObjects != null)
                {
                    updatedJsonObjects.ToList().Where(s => filteredResults.Select(s => s.UniqueID).Contains(s.UniqueID))
                                    .ToList().ForEach(s => s.IsDataUpdated = true);
                }
                if (updatedJsonObjects == null)
                {
                    updatedJsonObjects = new List<Book>();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception occured in filtered the updated json objects -" + ex);
                throw;
            }
            return BooksInCache = updatedJsonObjects;
        }

        /// <summary>
        /// Read JSON file from path and Deserialize the content into List of Books.
        /// </summary>
        /// <returns></returns>
        private List<Book> ReadJsonFileFromPath()
        {
            List<Book> JsonFileContents = new List<Book>();
            try
            {
                Book.ResetID(); // UniqueID to find the updated objects on UI.
                if (string.IsNullOrEmpty(FilePath))
                {
                    throw new FileNotFoundException("Json file is not present in this path.");
                }
                string text = File.ReadAllText(FilePath);
                var deserializeJsonObjects = JsonConvert.DeserializeObject<List<Book>>(text);
                if (deserializeJsonObjects != null)
                {
                    JsonFileContents = deserializeJsonObjects;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception occured while parsing the json file from path -" + ex);
                throw;
            }
            return JsonFileContents;
        }

        private static string GetFilePath()
        {
            string filePath = string.Empty;
            try
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(assemblyPath))
                {
                    string? assemblyDirectory = Path.GetDirectoryName(assemblyPath);
                    if (!string.IsNullOrEmpty(assemblyDirectory))
                    {
                        filePath = Path.Combine(assemblyDirectory, "BookInformation", "Book.json");
                    }
                }
            }
            catch (Exception ex)
            {
                filePath = string.Empty;
                Log.Error("Exception occured while retrieving the file path -" + ex);
            }
            return filePath;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }
                BooksInCache = null;

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~JsonReader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
