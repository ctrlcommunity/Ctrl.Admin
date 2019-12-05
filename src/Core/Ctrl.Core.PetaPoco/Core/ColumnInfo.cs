using System.Reflection;

namespace Ctrl.Core.PetaPoco
{
    /// <summary>
    ///     保存有关数据库中的列的信息。
    /// </summary>
    /// <remarks>
    /// 通常，CopyNoFo从POCO对象的属性及其属性自动填充。它可以
    ///但是，也可以从IMAPER接口返回，以便在DB和POCoS之间提供自己的绑定。
    /// </remarks>
    public class ColumnInfo
    {
        /// <summary>
        ///     列的SQL名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        ///     如果此列返回数据库中的计算值，则不应用于插入和更新
        //操作。
        /// </summary>
        public bool ResultColumn { get; set; }
        /// <summary>
        /// 如果这是结果列，则应该包含在自动选择查询中.
        /// </summary>
        public bool AutoSelectedResultColumn { get; set; }

        /// <summary>
        ///        如果通过该列返回的时间和日期值应强制UTC DATETMETHOYE，则为true。（没有转换是
        ///应用-日期时间属性的类型
        ///简单地设置为DateTimeKind。UTC而不是DATETMETHONK。未知。
        /// </summary>
        public bool ForceToUtc { get; set; }

        /// <summary>
        ///     The insert template. If not null, this template is used for generating the insert section instead of the deafult
        ///     string.Format("{0}{1}", paramPrefix, index"). Setting this allows DB related interactions, such as "CAST({0}{1} AS
        ///     json)"
        /// </summary>
        public string InsertTemplate { get; set; }

        /// <summary>
        ///     The update template. If not null, this template is used for generating the update section instead of the deafult
        ///     string.Format("{0} = {1}{2}", colName, paramPrefix, index"). Setting this allows DB related interactions, such as
        ///     "{0} = CAST({1}{2} AS
        ///     json)"
        /// </summary>
        public string UpdateTemplate { get; set; }

        /// <summary>
        ///     Creates and populates a ColumnInfo from the attributes of a POCO property.
        /// </summary>
        /// <param name="propertyInfo">The property whose column info is required</param>
        /// <returns>A ColumnInfo instance</returns>
        public static ColumnInfo FromProperty(PropertyInfo propertyInfo)
        {
            // Check if declaring poco has [Explicit] attribute
            var explicitColumns =
                propertyInfo.DeclaringType.GetCustomAttributes(typeof(ExplicitColumnsAttribute), true).Length > 0;

            // Check for [Column]/[Ignore] Attributes
            var colAttrs = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
            if (explicitColumns)
            {
                if (colAttrs.Length == 0)
                    return null;
            }
            else
            {
                if (propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), true).Length != 0)
                    return null;
            }

            var ci = new ColumnInfo();

            // Read attribute
            if (colAttrs.Length > 0)
            {
                var colattr = (ColumnAttribute)colAttrs[0];
                ci.InsertTemplate = colattr.InsertTemplate;
                ci.UpdateTemplate = colattr.UpdateTemplate;
                ci.ColumnName = colattr.Name ?? propertyInfo.Name;
                ci.ForceToUtc = colattr.ForceToUtc;
                if ((colattr as ResultColumnAttribute) != null)
                    ci.ResultColumn = true;
            }
            else
            {
                ci.ColumnName = propertyInfo.Name;
                ci.ForceToUtc = false;
                ci.ResultColumn = false;
            }

            return ci;
        }
    }
}
