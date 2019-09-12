using System;

namespace Ctrl.Core.PetaPoco
{
    /// <summary>
    ///     表示一个属性,该属性应用于POCO类时,指定他映射到的DB表名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute:Attribute
    {
        /// <summary>
        ///     该实体映射到的数据库的表名
        /// </summary>
        public string Value { get;}
        /// <summary>
        ///     该实体映射到数据库的表NANE
        /// </summary>
        /// <param name="tableName"></param>
        public TableNameAttribute(string tableName) => Value = tableName;
    }
}
