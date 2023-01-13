using JsonMonitorService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JsonReaderApplication.Model
{
    public class JsonReaderModel
    {
        private JsonMonitorService.IJsonReader _jsonReader;

        public JsonReaderModel(Action<List<Book>> updateResult, Action<bool> IsProgressStarted) 
        { 
            _jsonReader = new JsonMonitorService.JsonReader(updateResult, IsProgressStarted);

        }

        public async void RefreshJSONContent()
        {
            await Task.Run(() =>
            {
                _jsonReader.UpdateJSONContentOnRefreshTimeRate();
            });
        }

        public void CancelJSONUpdateService()
        {
            _jsonReader.CancelUpdateJSON();
        }

        public void Dispose()
        {
            _jsonReader.Dispose();
        }

       
    }
}
