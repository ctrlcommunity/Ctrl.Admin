using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Converts;
using Ctrl.Core.Web;
using Ctrl.System.Business;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ctrl.Net.Areas.sysManage.Controllers
{
    public class UserControlController : BaseController
    {
        #region 构造函数
        private readonly ISystemMenuLogic _menuLogic;
        private readonly ISystemRoleLogic _roleLogic;
        private readonly IWebHostEnvironment _environment;
        public UserControlController(ISystemMenuLogic menuLogic, ISystemRoleLogic roleLogic, IWebHostEnvironment environment)
        {
            this._menuLogic = menuLogic;
            this._roleLogic = roleLogic;
            this._environment = environment;
        }
        #endregion

        #region 选择菜单
        /// <summary>
        ///     读取树结构:排除下级
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("用户控件-方法-读取菜单树结构:排除下级")]
        public async Task<ContentResult> GetMenuRemoveChildren()
        {
            return Content((await _menuLogic.GetAllMenu()).ToJson());
        }
        /// <summary>
        ///     读取角色树结构
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("用户控件-方法-读取角色树结构")]
        public async Task<JsonResult> GetRoleTree()
        {
            return Json(await _roleLogic.GetAllRoleTree());
        }
        #endregion

        #region 文件上传
        [HttpPost]
        public async Task<IActionResult> FileSave()
        {
            string filepath = "";
            var date = Request;
            var files = Request.Form.Files;
            long size = files.Sum(f => f.Length);
            string webRootPath = _environment.WebRootPath;
            string contentRootPath = _environment.ContentRootPath;
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string fileExt = Path.GetExtension(formFile.FileName); 
                    long fileSize = formFile.Length; //获得文件大小，以字节为单位
                    string newFileName = Guid.NewGuid().ToString()+ fileExt; //随机生成新的文件名
                    var FileUrl = 
               $"/Files/{DateTime.Now.ToString("yyyy")}" +
               $"/{DateTime.Now.ToString("MM")}/{DateTime.Now.ToString("MM-dd")}";
                    if (!Directory.Exists(webRootPath+FileUrl))
                    {
                        Directory.CreateDirectory(webRootPath + FileUrl);
                    }
                    filepath = FileUrl +"/"+ newFileName;
                    var finalPath = Path.Combine(webRootPath + FileUrl, newFileName);
                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {

                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return Ok(new { count = files.Count, size,filepath= filepath, url=filepath, uploaded=1 });
        }

        #endregion
    }
}