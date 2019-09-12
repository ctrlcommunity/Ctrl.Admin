using System.Collections.Generic;

namespace Ctrl.Core.Entities.Paging
{
    /// <summary>
    ///     说明：分页信息
    ///     备注：继承QueryParam,由于在界面上只需传入基础参数,页码,记录总数无须传入,所以此次使用继承来使用继承原始
    /// </summary>
    public class PagerInfo:QueryParam
    {
        /// <summary>
        ///     页码总数
        /// </summary>
        public long PageCount { get; set; }
    }
    /// <summary>
    ///     说明：分页信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResults<T>
    {
        /// <summary>
        ///     分页信息
        /// </summary>
        public PagerInfo pagerInfo { get; set; }
        /// <summary>
        ///     查询出来数据
        /// </summary>
        public IList<T> Data { get; set; }
    }
}
