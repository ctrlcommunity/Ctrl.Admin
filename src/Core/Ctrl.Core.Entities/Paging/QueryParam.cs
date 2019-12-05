namespace Ctrl.Core.Entities.Paging
{
    /// <summary>
    ///     说 明：分页基础参数
    ///     备 注：用于分页实体继承,必须继承该实体才可进行分页
    /// </summary>
    public class QueryParam
    {
        /// <summary>
        ///     无参构造函数,提供默认值
        /// </summary>
        public QueryParam()
        {
            Pagerow = 10;
            Pageindex = 1;
            Sord = "asc";
        }
        /// <summary>
        ///     页码,如:1
        /// </summary>
        public long Pageindex { get; set; }
        /// <summary>
        ///     每页显示数量如:100
        /// </summary>
        public long Pagerow { get; set; }
        /// <summary>
        ///     总记录数
        /// </summary>
        public long RecordCount { get; set; }
        /// <summary>
        ///     排序字段(可多个)如:title
        /// </summary>
        public string Sidx { get; set; }
        /// <summary>
        /// 默认排序方式,如:asc
        /// </summary>
        public string Sord { get; set; }

        /// <summary>
        /// 过滤
        /// </summary>
        public string _filters;
        public string Filters
        {
            get
            {
                return string.IsNullOrWhiteSpace(_filters) ? string.Empty : SearchFilterUtil.ConvertFilters(_filters);
            }
            set { _filters = value; }
        }


    }
}
