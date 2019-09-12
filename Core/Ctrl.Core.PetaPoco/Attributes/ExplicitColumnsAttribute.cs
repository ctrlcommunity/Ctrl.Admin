using System;
namespace Ctrl.Core.PetaPoco
{
    /// <summary>
    ///     表示修饰POCO类的属性，所有列都必须使用
    ///     <seealso cref="ColumnAttribute"/>or<seealso cref="ResultColumnAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExplicitColumnsAttribute:Attribute
    {
    }
}
