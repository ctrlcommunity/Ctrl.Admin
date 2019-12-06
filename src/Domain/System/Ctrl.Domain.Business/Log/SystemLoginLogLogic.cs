using Ctrl.Core.Business;
using Ctrl.Core.Entities.Paging;
using Ctrl.Domain.DataAccess.Log;
using Ctrl.Domain.Models.Dtos.Logs;
using Ctrl.Domain.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ctrl.Domain.Business.Log
{
    /// <summary>
    ///     登录日志业务逻辑实现
    /// </summary>
    public class SystemLoginLogLogic : AsyncLogic<SystemLoginLog>, ISystemLoginLogLogic
    {
        #region 构造函数
        private readonly ISystemLoginLogRepository _loginLogRepository;

        public SystemLoginLogLogic(ISystemLoginLogRepository loginLogRepository)
            : base(loginLogRepository)
        {
            _loginLogRepository = loginLogRepository;
        }

        #endregion
        /// <summary>
        ///     登录信息分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<PagedResults<SystemLoginLog>> PagingLoginLogQuery(SystemLoginLogPagingInput logPagingInput)
        {
            return _loginLogRepository.PagingLoginLogQuery(logPagingInput);
        }
        /// <summary>
        ///     登录数据分析图数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<LoginDataOutPut>> GetLoginCountData()
        {
            List<LoginDataOutPut> loginDatas = new List<LoginDataOutPut>();

            string[] provincesText = { "上海", "河北", "山西", "内蒙古", "辽宁", "吉林", "黑龙江", "江苏", "浙江", "安徽", "福建", "江西", "山东", "河南", "湖北", "湖南", "广东", "广西", "海南", "四川", "贵州", "云南", "西藏", "陕西", "甘肃", "青海", "宁夏", "新疆", "北京", "天津", "重庆", "香港", "澳门" };
            foreach (var item in provincesText)
            {
                loginDatas.Add(new LoginDataOutPut
                {
                    LoginCount = await _loginLogRepository.GetLoginCountByAreaName(item),
                    provinceName = item
                });

            }
            return loginDatas;
        }
        /// <summary>
        ///     获取登录日志分析数据
        /// </summary>
        /// <returns></returns>
        public async Task<LoginDataAnalysisOutPut> FindLoginLogAnalysis()
        {
            LoginDataAnalysisOutPut outPut = new LoginDataAnalysisOutPut();
            List<int> ydata = new List<int>();
            outPut.xdata = (await _loginLogRepository.GetDateMonth()).ToList();
            var loges = (await _loginLogRepository.GetLoginLogDateMonth()).ToList();

            foreach (var item in outPut.xdata)
            {
              ydata.Add(loges.Count(d => d.CreateTime.ToString("MM-dd") == item));
            }
            outPut.ydata = ydata;
            return outPut;

        }
    }
}
