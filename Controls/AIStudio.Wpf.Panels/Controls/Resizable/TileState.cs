namespace AIStudio.Wpf.Panels
{

    /// <summary>
    /// 表示单个瓦片的状态。
    /// </summary>
    public enum TileState
    {
        /// <summary>
        /// 只要没有瓦片处于“最大化”状态，所有的瓦片都将返回“正常”。相反，如果任何瓦片被“最大化”，那么其它瓦片返回“正常”的瓦片。 
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 如果任何瓦片处于“最大化”状态，那么所有其他瓦片只能是“最正常”或“最小化”。
        /// </summary>
        Maximized = 1,

        /// <summary>
        /// 如果“最大化”状态中至少有一个瓦片，则只返回“最小化”。在“最小化”状态下的平铺通常只显示其标题而不是其内容。
        /// </summary>
        Minimized = 2,

        /// <summary>
        /// 如果“最大化”状态中至少有一个瓦片，则只返回“最小化”。在“MimeSimeDePad”状态下的一个瓦片通常会显示它的页眉及其内容，但是瓦片将与所有其他最小化的瓦片分组，折叠和展开。
        /// </summary>
        //MinimizedExpanded = 4,
    }
}
