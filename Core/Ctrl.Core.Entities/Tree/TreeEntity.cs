namespace Ctrl.Core.Entities.Tree {
    public class TreeEntity {
        /// <summary>
        ///     主键
        /// </summary>
        public object id { get; set; }
        /// <summary>
        ///     父级
        /// </summary>
        public object pId { get; set; }
        /// <summary>
        ///     名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        ///     打开的地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        ///     表示是否显示单选或多选框
        /// </summary>
        public bool nocheck { get; set; }

        /// <summary>
        ///     图标样式
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        ///     是否是父级
        /// </summary>
        public bool isParent { get; set; }

        /// <summary>
        ///     是否默认打开
        /// </summary>
        public bool open { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        ///     是否勾选
        /// </summary>
        public bool Checked { get; set; }
    }
}