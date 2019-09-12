using Ctrl.Core.Core.Auth;
using Ctrl.Core.Core.Http;
using Ctrl.Core.Core.Utils;
using System;

namespace Ctrl.Core.Core.Log
{
    public class SqlLogHandler : BaseHandler<SqlLog>
    {
        public SqlLogHandler(string operateSql,
        DateTime endDateTime,
        double elapsedTime,
        string parameter
        )
        : base("SqlLogToDatabase")
        {
            PrincipalUser principalUser = new PrincipalUser
            {
                Name = "匿名用户",
                UserId = Guid.Empty
            };
            var current = HttpContexts.Current;
            if (current != null)
            {
                principalUser = AuthenticationExtension.Current();
            }
            if (principalUser == null)
            {
                principalUser = new PrincipalUser()
                {
                    Name = "匿名用户",
                    UserId = Guid.Empty
                };
            }
            log = new SqlLog
            {
                SqlLogId = CombUtil.NewComb(),
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateUserId = principalUser.UserId.ToString(),
                CreateUserCode = principalUser.Code,
                CreateUserName = principalUser.Name,
                OperateSql = operateSql,
                ElapsedTime = elapsedTime,
                EndDateTime = endDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Parameter = parameter
            };
        }
    }
}
