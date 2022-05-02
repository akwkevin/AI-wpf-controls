using System;
using System.Diagnostics;
using System.Threading;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 用于性能测试的时间计数器类
    /// </summary>
    public class ProcessTime : IDisposable
    {
        private Stopwatch sw;

        private TimeSpan tsStart;

        /// <summary>
        /// 本次测试的主题
        /// </summary>
		public string Title
        {
            get; set;
        }

        /// <summary>
        /// CPU使用（占用）时间(ms)
        /// 只包括本进程的耗时
        /// </summary>
        public long CpuTime
        {
            get; private set;
        }

        /// <summary>
        /// 实际耗时(ms)
        /// 包括操作系统中其他线程/进程的切换、耗时等
        /// </summary>
        public long RealTime
        {
            get; private set;
        }

        /// <summary>
        /// 计时器开始计时的时间
        /// 用于在多线程计时中辅助分析耗时情况
        /// </summary>
        private DateTime StartTime = DateTime.Now;

        /// <summary>
        /// 计时器结束计时的时间
        /// 用于在多线程计时中辅助分析耗时情况
        /// </summary>
        private DateTime StopTime = DateTime.Now;

        /// <summary>
        /// 当前计时器是否被忽略
        /// </summary>
        private bool IsIgnored = false;

        /// <summary>
        /// 最大允许耗时毫秒阀值
        /// 耗时超过此数的，将输出调试信息；耗时小于此数的，将自动忽略输出（ignore=true）
        /// </summary>
        private int ThresholdMilliseconds = 0;

        /// <summary>
        /// 当前进程对象
        /// </summary>
        private Process Process = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
		/// <param name="ignore">是否取消计时器</param>
        /// <param name="thresholdMilliseconds">设定的最大耗时阀值，超过此耗时的将输出信息。单位：毫秒(ms)</param>
		public ProcessTime(string title = "", bool ignore = false, int thresholdMilliseconds = 0)
        {
#if TRACE
            this.Title = title;
            IsIgnored = ignore;
            this.ThresholdMilliseconds = thresholdMilliseconds;
            Process = Process.GetCurrentProcess();

            sw = new Stopwatch();

            if (!IsIgnored)
                Reset();
#endif
        }

        ~ProcessTime()
        {
#if TRACE
            if (sw != null && sw.IsRunning)
                sw.Stop();
#endif
        }

        public void Dispose()
        {
#if TRACE
            Stop();
#endif
        }


        /// <summary>
        /// 重置计时器
        /// </summary>
        public void Reset()
        {
#if TRACE
            if (IsIgnored)
                return;

            if (sw != null && sw.IsRunning)
                sw.Stop();

            sw = Stopwatch.StartNew();
            tsStart = Process.TotalProcessorTime;
            StartTime = DateTime.Now;
#endif
        }

        /// <summary>
        /// 重置计数器（包括主题）
        /// </summary>
        /// <param name="title"></param>
        public void Reset(string title)
        {
            this.Title = title;
            Reset();
        }

        /// <summary>
        /// 计时器停止
        /// </summary>
        public void Stop(bool output = true)
        {
#if TRACE
            if (IsIgnored || sw == null || !sw.IsRunning)
                return;

            sw.Stop();

            CpuTime = (long)Math.Round(Process.TotalProcessorTime.Subtract(tsStart).TotalMilliseconds);
            RealTime = sw.ElapsedMilliseconds;
            StopTime = DateTime.Now;

            if (output && RealTime >= ThresholdMilliseconds)
            {
#if DEBUG
                Trace.WriteLine(this.ToString());
                Trace.Flush();
#endif
            }
#endif
        }

        /// <summary>
        /// 在不停止计时器的情况下，获取当前的耗时数据
        /// </summary>
        public void Snapshot()
        {
            if (IsIgnored || sw == null || !sw.IsRunning)
                return;

            CpuTime = (long)Math.Round(Process.TotalProcessorTime.Subtract(tsStart).TotalMilliseconds);
            RealTime = sw.ElapsedMilliseconds;
            StopTime = DateTime.Now;
        }

        public override string ToString()
        {
#if TRACE
            if (IsIgnored || sw == null)
                return string.Empty;

            Snapshot();

            //判断是否是处于主线程内
            string threadName = "";
            if (!string.IsNullOrEmpty(Thread.CurrentThread.Name))
                threadName = Thread.CurrentThread.Name;
            else if (Thread.CurrentThread.IsThreadPoolThread)
                threadName = "线程池线程";
            else if (Thread.CurrentThread.IsBackground || Thread.CurrentThread.Name != "主线程")
                threadName = "背景线程";
            else
                threadName = "主线程";

            string sStartTime = StartTime.ToString("HH:mm:ss.fff");
            string sEndTime = StopTime.ToString("HH:mm:ss.fff");

            //如果实际耗时远大于CPU耗时，说明有优化的余地，应该提醒
            string warning = string.Empty;
            if ((CpuTime > 0 && (RealTime - CpuTime) / CpuTime > 0.5 && RealTime > 100)
                || (CpuTime == 0 && RealTime > 50))
                warning = "【警告：请注意优化！】";

            return string.Format("[ {3} ] {2} ---- CPU耗时：{0} ms, 实际耗时：{1} ms. [计时：{5} - {6}] {4}", CpuTime, RealTime, Title, threadName, warning, sStartTime, sEndTime);
#else
			return string.Empty;
#endif
        }

    }
}
