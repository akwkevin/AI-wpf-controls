using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AIStudio.Wpf.Wall3D.Demo.Models
{
    /// <summary>
    /// 媒体文件类
    /// 用于定义新的媒体
    /// </summary>
    public class WallItemData
    {
        /// <summary>
        /// 资源地址
        /// </summary>
        public string Source
        {
            get
            {
                return Directory.GetCurrentDirectory() + "\\Resources\\" + Thumb;
            }
        }
        /// <summary>
        /// 缩略图地址
        /// </summary>
        public string Thumb
        {
            get; set;
        }
    }
}
