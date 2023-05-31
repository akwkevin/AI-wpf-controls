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
            if (url.Contains("SaveData"))
            {
                await Task.Run(() => {
                    if (url.Contains("Device"))
                    {
                        var data = JsonConvert.DeserializeObject<Device>(json);
                        var olddata = Devices.FirstOrDefault(p => p.Id == data.Id);
                        data.CopyTo(olddata);
                    }
                    result.Success = true;
                });
            }
            else if (url.Contains("SaveDicData"))
            {
                await Task.Run(() => {
                    if (url.Contains("Device"))
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                        var olddata = Devices.FirstOrDefault(p => p.Id == data["Id"]?.ToString());
                        data.CopyTo(olddata);
                    }
                    result.Success = true;
                });
            }
            else if (url.Contains("GetConfig"))
            {
                await Task.Run(() => {
                    if (url.Contains("Device"))
                    {
                        result.Data = JsonConvert.DeserializeObject<T>(deviceconfig);
                    }
                    result.Success = true;
                });
            }
            return result;
        }

        public async Task<PageResult<T>> GetDataList<T>(string url, string json = "{}")
        {
            var result = new PageResult<T>();
            if (url.Contains("GetDataList"))
            {
                await Task.Run(() => {
                    if (url.Contains("Device"))
                    {
                        var pagination = JsonConvert.DeserializeObject<Pagination>(json);
                        var data = GetData(Devices.OfType<T>().ToList(), pagination);
                        result.Data = data.Item2;
                        result.Total = data.Item1;
                    }
                    result.Success = true;
                });
            }
            else if (url.Contains("GetDicDataList"))
            {
                await Task.Run(() => {
                    if (url.Contains("Device"))
                    {
                        var pagination = JsonConvert.DeserializeObject<Pagination>(json);
                        var data = GetDicData(Devices, pagination);
                        result.Data = data.Item2.OfType<T>().ToList();
                        result.Total = data.Item1;
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
                filterdatas = filterdatas.Where(p => p.GetType().GetProperty(dic.Key).GetValue(p) == dic.Value);
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

        public Tuple<int, List<IDictionary<string, object>>> GetDicData<T>(List<T> datas, Pagination pagination)
        {
            var filterdatas = datas.Where(p => 1 == 1);
            foreach (var dic in pagination.Keywords)
            {
                filterdatas = filterdatas.Where(p => p.GetType().GetProperty(dic.Key).GetValue(p) == dic.Value);
            }

            int count = filterdatas.Count();

            if (pagination.SortType.ToLower() == "asc")
            {
                return new Tuple<int, List<IDictionary<string, object>>>(count, filterdatas.OrderBy(p => pagination.SortField)
                    .Skip((pagination.PageIndex - 1) * pagination.PageRows)
                    .Take(pagination.PageRows).Select(p => p.ModelToDic()).ToList());
            }
            else
            {
                return new Tuple<int, List<IDictionary<string, object>>>(count, filterdatas.OrderByDescending(p => pagination.SortField)
                      .Skip((pagination.PageIndex - 1) * pagination.PageRows)
                      .Take(pagination.PageRows).Select(p => p.ModelToDic()).ToList());
            }
        }

        //假设这些数据为配置在数据库中，这里为了简单，使用字符串保存一下。
        readonly string deviceconfig = "{\"Item1\":[{\"DisplayIndex\":2147483647,\"Header\":\"名称\",\"PropertyName\":\"Name\",\"Value\":null,\"Visibility\":0,\"ControlType\":0,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":null,\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"模式1\",\"PropertyName\":\"Mode1\",\"Value\":null,\"Visibility\":2,\"ControlType\":0,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":null,\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"模式2\",\"PropertyName\":\"Mode2\",\"Value\":null,\"Visibility\":2,\"ControlType\":0,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":null,\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"数值1\",\"PropertyName\":\"Value1\",\"Value\":null,\"Visibility\":2,\"ControlType\":8,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"f3\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"时间\",\"PropertyName\":\"DateTime\",\"Value\":null,\"Visibility\":0,\"ControlType\":10,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"yyyy-MM-dd HH:mm:ss\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"查询\",\"PropertyName\":null,\"Value\":null,\"Visibility\":0,\"ControlType\":11,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":null,\"IsReadOnly\":false}],\"Item2\":[{\"DisplayIndex\":2147483647,\"Binding\":\"Mode1\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Mode1\",\"Visibility\":2,\"Header\":\"模式1\",\"StringFormat\":null,\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":1,\"Binding\":\"Mode2\",\"CellStyle\":null,\"CanUserSort\":false,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Mode2\",\"Visibility\":0,\"Header\":\"模式2\",\"StringFormat\":null,\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":0,\"Binding\":\"Id\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Id\",\"Visibility\":0,\"Header\":\"名称\",\"StringFormat\":null,\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"Value1\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Value1\",\"Visibility\":0,\"Header\":\"数值1\",\"StringFormat\":\"f3\",\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"Value2\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Value2\",\"Visibility\":0,\"Header\":\"数值2\",\"StringFormat\":\"f3\",\"Converter\":\"AIStudio.Wpf.Controls.Converter.AdditionConverter\",\"ConverterParameter\":1,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"Value3\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Value3\",\"Visibility\":0,\"Header\":\"数值3\",\"StringFormat\":\"f3\",\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":\";_p0<50_#FF0078D7——;_50<=p0 && p0<100_Red\",\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"Value4\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Value4\",\"Visibility\":0,\"Header\":\"数值4\",\"StringFormat\":\"f3\",\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":\"Value4;Value5_p0<p1_Red\",\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"Value5\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Value5\",\"Visibility\":0,\"Header\":\"Value5\",\"StringFormat\":\"f3\",\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"Value6\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Value6\",\"Visibility\":0,\"Header\":\"数值6\",\"StringFormat\":\"n0\",\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":\";_p0<50_#FF0078D7——;_50<=p0 && p0<100_Red\",\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"Value7\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Value7\",\"Visibility\":0,\"Header\":\"Value7\",\"StringFormat\":\"n0\",\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"Value8\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Value8\",\"Visibility\":0,\"Header\":\"Value8\",\"StringFormat\":\"n0\",\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"Value9\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"Value9\",\"Visibility\":0,\"Header\":\"Value9\",\"StringFormat\":\"n0\",\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0},{\"DisplayIndex\":2147483647,\"Binding\":\"DateTime\",\"CellStyle\":null,\"CanUserSort\":true,\"CanUserResize\":false,\"CanUserReorder\":false,\"Width\":\"Auto\",\"IsFrozen\":false,\"IsReadOnly\":false,\"MaxWidth\":0.0,\"Minwidth\":0.0,\"SortMemberPath\":\"DateTime\",\"Visibility\":0,\"Header\":\"时间\",\"StringFormat\":\"yyyy-MM-dd HH:mm:ss\",\"Converter\":null,\"ConverterParameter\":null,\"ForegroundExpression\":null,\"BackgroundExpression\":null,\"HorizontalAlignment\":0}],\"Item3\":[{\"DisplayIndex\":0,\"Header\":\"名称\",\"PropertyName\":\"Id\",\"Value\":null,\"Visibility\":0,\"ControlType\":0,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":null,\"IsReadOnly\":true},{\"DisplayIndex\":1,\"Header\":\"模式2\",\"PropertyName\":\"Mode2\",\"Value\":null,\"Visibility\":0,\"ControlType\":0,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":null,\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"模式1\",\"PropertyName\":\"Mode1\",\"Value\":null,\"Visibility\":2,\"ControlType\":0,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":null,\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"数值1\",\"PropertyName\":\"Value1\",\"Value\":null,\"Visibility\":0,\"ControlType\":8,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"f3\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"数值2\",\"PropertyName\":\"Value2\",\"Value\":null,\"Visibility\":0,\"ControlType\":8,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"f3\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"数值3\",\"PropertyName\":\"Value3\",\"Value\":null,\"Visibility\":0,\"ControlType\":8,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"f3\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"数值4\",\"PropertyName\":\"Value4\",\"Value\":null,\"Visibility\":0,\"ControlType\":8,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"f3\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"Value5\",\"PropertyName\":\"Value5\",\"Value\":null,\"Visibility\":0,\"ControlType\":8,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"f3\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"数值6\",\"PropertyName\":\"Value6\",\"Value\":null,\"Visibility\":0,\"ControlType\":6,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"n0\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"Value7\",\"PropertyName\":\"Value7\",\"Value\":null,\"Visibility\":0,\"ControlType\":6,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"n0\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"Value8\",\"PropertyName\":\"Value8\",\"Value\":null,\"Visibility\":0,\"ControlType\":6,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"n0\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"Value9\",\"PropertyName\":\"Value9\",\"Value\":null,\"Visibility\":0,\"ControlType\":6,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"n0\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"时间\",\"PropertyName\":\"DateTime\",\"Value\":null,\"Visibility\":0,\"ControlType\":10,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":\"yyyy-MM-dd HH:mm:ss\",\"IsReadOnly\":false},{\"DisplayIndex\":2147483647,\"Header\":\"提交\",\"PropertyName\":null,\"Value\":null,\"Visibility\":0,\"ControlType\":12,\"ItemSource\":null,\"IsRequired\":false,\"StringFormat\":null,\"IsReadOnly\":false}]}";
    }
}
