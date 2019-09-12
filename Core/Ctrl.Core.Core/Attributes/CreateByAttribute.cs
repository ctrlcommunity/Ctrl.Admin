using System;

namespace Ctrl.Core.Core.Attributes
{
    /// <summary>
    ///     标识一个特性,在该特性中指明标定的类或方法的作者。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = false)]
    public class CreateByAttribute:Attribute
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="name">开发人员编码</param>
        public CreateByAttribute(string name) {
            this.Name = name;
        }

        #region 属性
        /// <summary>
        ///     开发人员编码
        /// </summary>
        public string Name { get; set; }
        #endregion
    }
}
