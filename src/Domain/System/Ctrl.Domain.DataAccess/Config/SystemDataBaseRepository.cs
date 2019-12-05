using System.Collections.Generic;
using System.Threading.Tasks;
using Ctrl.Core.DataAccess;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Tree;
using Ctrl.Core.PetaPoco;
using Ctrl.Domain.Models.Dtos.Config;

namespace Ctrl.Domain.DataAccess.Config
{
    public class SystemDataBaseRepository : PetaPocoRepository<SystemDataBaseTableOutput>, ISystemDataBaseRepository
    {
        /// <summary>
        ///     获取所有表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SystemDataBaseTableOutput>> GetDataBaseTables()
        {
            var sql = @"SELECT
                        A.name ,C.value,A.create_date  CreateDate
                        FROM sys.tables A

                        LEFT JOIN sys.extended_properties C ON C.major_id = A.object_id
                        WHERE C.minor_id=0 
                        ";
            return SqlMapperUtil.Query<SystemDataBaseTableOutput>(sql);
        }
        /// <summary>
        ///     根据表名获取所有列
        /// </summary>
        /// <param name="idInput"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemDataBaseColumnOutput>> GetDataBaseColumn(IdInput idInput)
        {
            var sql = @"SELECT 
                        b.name DataType,
                        a.name FieldName,
                        isnull(g.[value], ' ') AS Remarks
                        FROM  syscolumns a 
                        left join systypes b on a.xtype=b.xusertype  
                        inner join sysobjects d on a.id=d.id and d.xtype='U' and d.name<>'dtproperties' 
                        left join sys.extended_properties g on a.id=g.major_id AND a.colid=g.minor_id
                        where b.name is not null and d.name=@name
                        order by a.id,a.colorder";
            return SqlMapperUtil.Query<SystemDataBaseColumnOutput>(sql,new{name=idInput.Id});
        }

        public Task<IEnumerable<TreeEntity>> GetDataBaseColumnsTree(string Name)
        {
            var sql = @"SELECT 
                         a.name id,
                       cast(a.name as varchar)+cast(isnull(g.[value], ' ') as varchar) as name,
                    isnull(g.[value], ' ') as code
                        FROM  syscolumns a 
                        left join systypes b on a.xtype=b.xusertype  
                        inner join sysobjects d on a.id=d.id and d.xtype='U' and d.name<>'dtproperties' 
                        left join sys.extended_properties g on a.id=g.major_id AND a.colid=g.minor_id
                        where b.name is not null and d.name=@name
                        order by a.id,a.colorder";
            return SqlMapperUtil.Query<TreeEntity>(sql, new { name = Name });
        }
    }
}
