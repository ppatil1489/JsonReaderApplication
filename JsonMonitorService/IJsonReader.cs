using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonMonitorService
{
    public interface IJsonReader: IDisposable
    {
        Task UpdateJSONContentOnRefreshTimeRate();


        void CancelUpdateJSON();
    }
}
