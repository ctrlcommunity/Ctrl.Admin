using Ctrl.Core.Business;
using Ctrl.Core.Core.Resource;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Entities.Tree;
using Ctrl.Domain.DataAccess.Identity;
using Ctrl.Domain.Models.Dtos.Permission;
using Ctrl.Domain.Models.Entities;
using Ctrl.Domain.Models.Enums;
using Ctrl.System.DataAccess;
using Ctrl.System.Models.Entities;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ctrl.System.Business
{
    /// <summary>
    ///     ///权限记录表业务逻辑接口实现
    /// </summary>
    public class SystemPermissionLogic : AsyncLogic<SystemPermission>, ISystemPermissionLogic
    {
        #region 构造函数
        private readonly ISystemPermissionRepository _systemPermissionRepository;
        private readonly ISystemMenuRepository _menuRepository;
        private readonly ISystemUserRepository _userRepository;
        private readonly ISystemPermissionUserRepository _permissionUserRepository;
        private readonly ISystemMenuButtonRepository _buttonRepository;
        private readonly IMemoryCache _cache;
        public const string USER_MENU_CACHE_KEY = "_USERMENU_";

        public SystemPermissionLogic(
            ISystemPermissionRepository systemPermissionRepository,
            ISystemMenuRepository menuRepository,
            ISystemUserRepository userRepository, ISystemPermissionUserRepository permissionUserRepository, ISystemMenuButtonRepository buttonRepository,IMemoryCache cache) : base(systemPermissionRepository)
        {
            _systemPermissionRepository = systemPermissionRepository;
            this._menuRepository = menuRepository;
            this._userRepository = userRepository;
            this._permissionUserRepository = permissionUserRepository;
            this._buttonRepository = buttonRepository;
            this._cache = cache;
        }

        #endregion

        #region 方法
        /// <summary>
        ///     根据用户id获取具有权限的菜单
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TreeEntity>> GetSystemPermissionMenuByUserId(Guid userId)
        {
            IList<TreeEntity> treeEntities = new List<TreeEntity>();
            string cacheKey = USER_MENU_CACHE_KEY + userId;
            treeEntities = this._cache.Get<List<TreeEntity>>(cacheKey);
            if (treeEntities==null)
            {
                var userInfo = await _userRepository.GetById(userId);
                //判断当前用户是否是超级管理员:若是超级管理员则显示所有菜单
                if (userInfo != null)
                {
                    //如果是超级管理员
                    if (userInfo.IsAdmin)
                    {
                        treeEntities = (await _menuRepository.GetAllMenu(true, true)).ToList();
                        return treeEntities;
                    }
                    treeEntities = (await _systemPermissionRepository.GetSystemPermissionMenuByUserId(userInfo.UserId)).ToList();
                }
            }
            return treeEntities;

        }
        /// <summary>
        ///     根据权限id获取权限信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TreeEntity>> GetPermissionByCheckedPrivilegeMasterValue(GetPermissionByPrivilegeMasterValueInput input)
        {
            try
            {
                //获取所有菜单
                var GetMenuAll = (await _menuRepository.GetAllMenu()).ToList();
                IEnumerable<SystemPermission> GetPermissionByMaster = (await _systemPermissionRepository.GetPermissionByPrivilegeMasterValue(input)).ToList();
                List<TreeEntity> TreeList = new List<TreeEntity>();
                foreach (TreeEntity tree in GetMenuAll)
                {
                    tree.Checked = GetPermissionByMaster.Count(m => m.PrivilegeAccessValue.ToString() == tree.id.ToString()) == 0 ? false : true;
                    tree.isParent = GetMenuAll.Select(m1 => m1.pId).Contains(tree.id);
                    TreeList.Add(tree);
                }
                return TreeList;
            }
            catch
            {

                return null;
            }

        }
        /// <summary>
        ///     保存权限信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OperateStatus> SavePermission(SavePermissionInput input)
        {
            var operateStatus = new OperateStatus();
            try
            {
                IList<SystemPermission> systemPermissions = input.Permissiones.Select(per => new SystemPermission
                {
                    PrivilegeAccess = (byte)input.PrivilegeAccess,
                    PrivilegeAccessValue = per,
                    PrivilegeMasterValue = input.PrivilegeMasterValue,
                    PrivilegeMaster = (byte)input.PrivilegeMaster,
                    PrivilegeMenuId = input.PrivilegeMenuId
                }).ToList();
                //删除该角色的权限信息
                await _systemPermissionRepository.DeletePermissionByPrivilegeMasterValue(input.PrivilegeAccess, input.PrivilegeMasterValue, input.PrivilegeMenuId);
                if (input.PrivilegeMaster == EnumPrivilegeMaster.人员)
                {
                    //删除对应人员数据
                    await _permissionUserRepository.DeletePermissionUser(input.PrivilegeMaster, input.PrivilegeMasterValue);
                    //判断是否具有权限
                    if (!systemPermissions.Any())
                    {
                        operateStatus.ResultSign = ResultSign.Successful;
                        operateStatus.Message = Chs.Successful;
                        return operateStatus;
                    }
                    //插入权限人员数据
                    var permissionUser = new SystemPermissionUser
                    {
                        PrivilegeMaster = (byte)input.PrivilegeMaster,
                        PrivilegeMasterUserId = input.PrivilegeMasterValue.ToString(),
                        PrivilegeMasterValue = input.PrivilegeMasterValue.ToString()
                    };
                    await _permissionUserRepository.Insert(permissionUser);
                }
                //是否具有权限数据
                if (!systemPermissions.Any())
                {
                    operateStatus.ResultSign = ResultSign.Successful;
                    operateStatus.Message = Chs.Successful;
                    return operateStatus;
                }
                await _systemPermissionRepository.InsertMultiplePetaPocoAsync(systemPermissions);
                operateStatus.ResultSign = ResultSign.Successful;
                operateStatus.Message = Chs.Successful;
                return operateStatus;

            }
            catch
            {
                return operateStatus;
            }
        }

        public Task<IEnumerable<SystemMenuButtonOutput>> GetMenuButtonByMenuId(IdInput input)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///     根据登录人员获取对应的路由信息
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AuthMenuButtonOutput>> GetRoutersByUserId(string UserId)
        {
            try
            {
                //判断当前人员是否为超级管理员若是超级管理员则具有最大权限
                var userInfo = await _userRepository.GetById(UserId);
                var result = await _buttonRepository.GetMenuButtonByUserId(UserId, userInfo.IsAdmin);
                return result;
            }
            catch
            {

                return null;
            }
        }
        /// <summary>
        ///     根据特权ID获取当前拥有的权限
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TreeEntity>> GetMenuHavePermissionByPrivilegeMasterValue(GetMenuHavePermissionByPrivilegeMasterValueInput input)
        {
            var TreeList = await _systemPermissionRepository.GetMenuHavePermissionByPrivilegeMasterValue(input);
            return TreeList;
        }
        /// <summary>
        ///     根据权限ID查询权限信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<IEnumerable<SystemPermission>> GetPermissionByPrivilegeMasterValue(GetPermissionByPrivilegeMasterValueInput input)
        {
            return _systemPermissionRepository.GetPermissionByPrivilegeMasterValue(input);
        }
        #endregion
    }
}