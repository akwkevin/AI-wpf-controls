using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIStudio.Wpf.GridControls.Demo.Servers
{
    public interface IDataProvider
    {
        Task<AjaxResult> GetToken(string url, string userName, string password, int headMode, TimeSpan timeout);

        //[LogHandler]
        Task<AjaxResult<T>> GetData<T>(string url, Dictionary<string, string> data);

        //[LogHandler]
        Task<AjaxResult<T>> GetData<T>(string url, string json = "{}");
    }
}
