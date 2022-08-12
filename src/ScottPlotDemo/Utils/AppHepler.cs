using System;
using System.Diagnostics;

namespace ScottPlotDemo.Utils
{
    internal static class AppHepler
    {
        /// <summary>
        /// CPU使用率。
        /// </summary>
        private static readonly PerformanceCounter CpuLoad = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);

        /// <summary>
        /// 記憶體使用率。
        /// </summary>
        private static readonly PerformanceCounter RamLoad = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);

        private const int MB_DIV = 1024 * 1024;

        //private static readonly Random _rand = new Random();

        /// <summary>
        /// CPU使用率。
        /// </summary>
        /// <returns></returns>
        public static float CpuUsage() => CpuLoad.NextValue() / Environment.ProcessorCount;

        /// <summary>
        /// 記憶體使用率。
        /// </summary>
        /// <returns></returns>
        public static float RamUasge() => RamLoad.NextValue() / MB_DIV;

        //public static double GetRandomNumber(double minimum, double maximum) => (_rand.NextDouble() * (maximum - minimum)) + minimum;
    }
}
