using System;
using System.Collections.Generic;
using System.Linq;
using Ctrl.Core.DataAccess;
using Ctrl.Core.PetaPoco;
using Xunit;
using static Ctrl.XUnit.MsSqlQueryTests;

namespace Ctrl.XUnit {
    [Collection ("MssqlTests")]
    public class MsSqlQueryTests {
        IRepository<S_Address> _repository = new PetaPocoRepository<S_Address> ();

        [Fact]
        public async System.Threading.Tasks.Task Query_ForDynamicTypeAsync () {
           

            var sss= await _repository.Insert(new S_Address { Name = "222" });


            var sssd = await _repository.Update("Sys_Article", "ArticleId", new { ArticleId= "3f25155a-694b-4650-b71f-aa49009593ef", Title = "test" },new List<string> { "ArticleId", "Title" });
            //var db=  SqlMapperUtil.CreateDbBase();
            //db.BeginTransaction();
            //var ss= db.Insert(new S_Address { Name="222"});
            //db.CompleteTransaction();
            // var db = new Database("null", "SqlServer");
            //// SqlConnection connection = new SqlConnection("null");
            // //var db = new Database(connection);
            // var sql = "SELECT * FROM S_Address";
            // var result = db.Query<aaa>(sql).ToList();
            Assert.Equal(sssd,1);
        }
        public class S_Address {
            public int Id { get; set; }
            public string Name { get; set; }

        }
    }
}