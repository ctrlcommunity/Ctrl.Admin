using System.Diagnostics;

namespace Ctrl.Core.Core.Utils
{
    /// <summary>
    ///  常用工具类
    /// </summary>
    public class CommonHelper
    {
        #region Stopwatch计时器
        /// <summary>
        ///     计时器开始
        /// </summary>
        /// <returns></returns>
        public static Stopwatch TimerStart()
        {
            Stopwatch watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            return watch;
        }
        /// <summary>
        ///     计时器结束
        /// </summary>
        /// <returns></returns>
        public static double TimerEnd(Stopwatch watch)
        {
            watch.Stop();
            double costtime = watch.Elapsed.TotalSeconds;
            return costtime;
        }
        #endregion
    }
}
