using System;
using System.Collections.Generic;
using System.Text;

namespace AIStudio.Wpf.GridControls.Demo.Servers
{
    /// <summary>
    /// 分页返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResult<T> : AjaxResult<List<T>>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total
        {
            get; set;
        }
    }
}
