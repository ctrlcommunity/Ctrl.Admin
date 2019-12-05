using System;

namespace Ctrl.Core.PetaPoco
{
    /// <summary>
    ///     是一个属性,当应用到PoCO类时,他指定主键列,另外,指定是否该列是自动递增的,
    ///     并且是Oracle序列列的可选序列名称。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute:Attribute
    {
        /// <summary>
        ///     列名
        /// </summary>
        public string Value { get; }
        /// <summary>
        ///     序列名
        /// </summary>
        public string SequenceName { get; set; }
        /// <summary>
        ///     指定主键是否自动递增的标志
        ///     如果主键是自动递增的，则为true；否则，false
        /// </summary>
        public bool AutoIncrement { get; set; } = false;
        /// <summary>
        ///     构造一个新的<see cref="PrimaryKeyAttribute">
        /// </summary>
        /// <param name="primaryKey">主键列的名称</param>
        public PrimaryKeyAttribute(string primaryKey) => Value = primaryKey;

        public PrimaryKeyAttribute(string primaryKey, bool AutoIncrement) {
            Value = primaryKey;
            this.AutoIncrement = AutoIncrement;
        }
    }
}
