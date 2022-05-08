using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AIStudio.Wpf.GridControls.Demo.Commons;
using AIStudio.Wpf.GridControls.Demo.Models;
using AIStudio.Wpf.GridControls.Demo.Extensions;
using Newtonsoft.Json;

namespace AIStudio.Wpf.GridControls.Demo.Servers
{
    public class DataProvider : IDataProvider
    {
        List<Device> Devices = new List<Device>();

        public DataProvider()
        {
            Random rd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var d1 = new Device()
                {
                    Id = "MX33333333333333333333333331_" + i,
                    Mode1 = "M303" + i,
                    Mode2 = "M303" + i,
                    Value1 = i,
                    Value2 = i,
                    Value3 = i,
                    Value4 = i + rd.NextDouble(),
                    Value5 = i + rd.NextDouble(),
                    Value6 = i,
                    Value7 = i,
                    Value8 = i,
                    Value9 = i,
                    Value10 = i,
                    DateTime = DateTime.Now.AddMinutes(0 - i),
                };
                Devices.Add(d1);
            }
        }

        public Task<AjaxResult<T>> GetData<T>(string url, Dictionary<string, string> data)
        {
            throw new NotImplementedException();
        }

        public async Task<AjaxResult<T>> GetData<T>(string url, string json = "{}")
        {
            var result = new AjaxResult<T>();
            if (url.Contains("GetDataList"))
            {
                await Task.Run(() => {
                    if (url.Contains("Device"))
                    {
                        var pagination = JsonConvert.DeserializeObject<Pagination>(json);
                        var data = GetData(Devices, pagination);
                        result.Data = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data.Item2));
                        result.Total = data.Item1;
                    }
                });
            }
            else if (url.Contains("SaveData"))
            {
                await Task.Run(() => {
                    if (url.Contains("Device"))
                    {
                        var data = JsonConvert.DeserializeObject<Device>(json);
                        var olddata = Devices.FirstOrDefault(p => p.Id == data.Id);
                        ObjectCopy.CopyTo(data, olddata);
                    }
                    result.Success = true;
                });
            }
            return result;
        }

        public Task<AjaxResult> GetToken(string url, string userName, string password, int headMode, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Tuple<int, List<T>> GetData<T>(List<T> datas, Pagination pagination)
        {
            var filterdatas = datas.Where(p => 1 == 1);
            foreach (var dic in pagination.Keywords)
            {
                filterdatas = filterdatas.Where(p => p.GetType().GetProperty(dic.Key).GetValue(p)?.ToString() == dic.Value);
            }

            int count = filterdatas.Count();

            if (pagination.SortType.ToLower() == "asc")
            {
                return new Tuple<int, List<T>>(count, filterdatas.OrderBy(p => pagination.SortField)
                    .Skip((pagination.PageIndex - 1) * pagination.PageRows)
                    .Take(pagination.PageRows).ToList());
            }
            else
            {
                return new Tuple<int, List<T>>(count, filterdatas.OrderByDescending(p => pagination.SortField)
                   .Skip((pagination.PageIndex - 1) * pagination.PageRows)
                   .Take(pagination.PageRows).ToList());
            }
        }

        public class ObjectCopy
{
    public static PropertyInfo[] GetPropertyInfos(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }
    /// <summary>
    /// 实体属性反射
    /// </summary>
    /// <typeparam name="S">赋值对象</typeparam>
    /// <typeparam name="T">被赋值对象</typeparam>
    /// <param name="s"></param>
    /// <param name="t"></param>
    public static void CopyTo<S, T>(S s, T t)
    {
        PropertyInfo[] pps = GetPropertyInfos(s.GetType());
        Type target = t.GetType();

        foreach (var pp in pps)
        {
            PropertyInfo targetPP = target.GetProperty(pp.Name);
            object value = pp.GetValue(s, null);

            if (targetPP != null && value != null)
            {
                targetPP.SetValue(t, value, null);
            }
        }
    }

}
    }
}
