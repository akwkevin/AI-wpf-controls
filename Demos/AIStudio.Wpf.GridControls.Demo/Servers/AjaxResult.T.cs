﻿namespace AIStudio.Wpf.GridControls.Demo.Servers
{
    /// <summary>
    /// Ajax请求结果
    /// </summary>
    public class AjaxResult<T> : AjaxResult
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public new T Data { get; set; }

    }
}
