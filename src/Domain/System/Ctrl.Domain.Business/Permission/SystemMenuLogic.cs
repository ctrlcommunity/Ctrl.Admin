using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ctrl.Core.Business;
using Ctrl.Core.Core.Converts;
using Ctrl.Core.Core.Resource;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.System.DataAccess;
using Ctrl.System.Models.Entities;

namespace Ctrl.System.Business {
    /// <summary>
    ///     系统菜单业务逻辑接口实现
    /// /// </summary>
    public class SystemMenuLogic :AsyncLogic<SystemMenu>, ISystemMenuLogic {
        #region 构造函数
        private readonly ISystemMenuRepository _systemMenuRepository;

        public SystemMenuLogic(ISystemMenuRepository systemMenuRepository){
            this._systemMenuRepository = systemMenuRepository;
        }
        #endregion

        #region 方法
        /// <summary>
        ///     保存菜单
        /// </summary>
        /// <param name="systemMenu"></param>
        /// <returns></returns>
        public  Task<OperateStatus> SaveMenu (SystemMenu systemMenu) {
            if (systemMenu.MenuId.IsEmptyGuid())
            {
                systemMenu.MenuId = Guid.NewGuid();
                return InsertAsync(systemMenu);
            }
            else
            {
                return UpdateAsync(systemMenu);
            }
        }
        /// <summary>
        ///     根据状态为True的菜单信息
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TreeEntity>> GetAllMenu () {
            var MenuAllList = await _systemMenuRepository.GetAllMenu ();
            MenuAllList = MenuAllList.Select (m => {
                m.url = "";
                m.pId = (Guid.Parse (m.pId.ToString()).IsEmptyGuid ()) ? null : m.pId;
                m.isParent = MenuAllList.Select(m1=>m1.pId).Contains(m.id);
                return m;
            }).ToList ();


            return MenuAllList;
        }
        /// <summary>
        ///     根据父级获取下级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public  Task<IEnumerable<SystemMenuOutput>> GetMenuByPid(IdInput input)
        {
            return   _systemMenuRepository.GetMenuByPid(input);
        }
        /// <summary>
        ///     删除菜单
        /// </summary>
        /// <param name="input"></param>
        public async Task<OperateStatus>DeleteMenu(IdInput input){
               var OperateStatus=new OperateStatus();
            var Menu = await GetById(input.Id);
            //判断是否存在
            if (Menu == default(SystemMenu))
            {
                OperateStatus.ResultSign = ResultSign.Error;
                OperateStatus.Message = Chs.HaveDelete;
                goto Ending;
            }
            //是否可以删除
            if (!Menu.CanbeDelete)
            {
                OperateStatus.ResultSign = ResultSign.Error;
                OperateStatus.Message = Chs.CanotDelete;
                goto Ending;
            }
            //是否存在下级菜单 
            if ((await GetMenuByPid(input)).Any())
            {
                OperateStatus.ResultSign = ResultSign.Error;
                OperateStatus.Message = string.Format(Chs.Error, ResourceSystem.具有下级项);
                goto Ending;
            }
            OperateStatus = await DeleteById(input.Id);

            Ending:
                return OperateStatus;
        }
        #endregion
    }
}