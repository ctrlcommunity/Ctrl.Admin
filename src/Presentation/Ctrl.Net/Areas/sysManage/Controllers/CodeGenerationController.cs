using Ctrl.Core.Core.Attributes;
using Ctrl.Core.Core.Converts;
using Ctrl.Core.Core.Utils;
using Ctrl.Core.Entities;
using Ctrl.Core.Entities.Dtos;
using Ctrl.Core.Web;
using Ctrl.Domain.Business.Config;
using Ctrl.Domain.Models.Dtos.Config;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ctrl.Net.Areas.sysManage.Controllers
{
    /// <summary>
    /// 代码生成控制器
    /// </summary>
    public class CodeGenerationController : BaseController
    {
        #region 构造函数

        private readonly ISystemDataBaseLogic _dataBaseLogic;

        public CodeGenerationController(ISystemDataBaseLogic dataBaseLogic)
        {
            _dataBaseLogic = dataBaseLogic;
        }

        #endregion

        #region 视图
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region 方法
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("代码生成-方法-列表-获取对应表列信息")]
        public async Task<JsonResult> GetDataBaseTables()
        {
            return Json(await _dataBaseLogic.GetDataBaseTables());
        }
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("代码生成-方法-列表-获取对应表的列信息")]
        public async Task<JsonResult> GetDataBaseColumn(IdInput idInput) {
            return Json(await _dataBaseLogic.GetDataBaseColumn(idInput));
        }
        [HttpPost]
        [CreateBy("冯辉")]
        [Description("代码生成-方法-tree-获取对应的列信息")]
        public async Task<JsonResult> GetDataBaseColumnsTree(string Name) {
            return Json(await _dataBaseLogic.GetDataBaseColumnsTree(Name));
        }
        /// <summary>
        ///     数据库中与C#中的数据类型对照
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string ChangeToCSharpType(string type)
        {
            string reval;
            switch (type.ToLower())
            {
                case "int":
                case "bigint":
                case "smallint":
                    reval = "int";
                    break;
                case "text":
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "varchar":
                    reval = "string";
                    break;
                case "binary":
                case "varbinary":
                case "tinyint":
                case "image":
                    reval = "byte[]";
                    break;
                case "bit":
                    reval = "bool";
                    break;
                case "datetime":
                case "smalldatetime":
                case "timestamp":
                    reval = "DateTime";
                    break;
                case "decimal":
                case "smallmoney":
                case "money":
                case "numeric":
                    reval = "decimal";
                    break;
                case "float":
                    reval = "double";
                    break;
                case "real":
                    reval = "System.Single";
                    break;
                case "uniqueidentifier":
                    reval = "Guid";
                    break;
                case "Variant":
                    reval = "Object";
                    break;
                default:
                    reval = "String";
                    break;
            }
            return reval;
        }
        #endregion

        #region 生成代码
        /// <summary>
        ///     生成代码
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CreateCode(SystemCodeGenerationCreateFileViewModel viewModel)
        {
            var input = new SystemCodeGenerationInput
            {
                Base = viewModel.Base.ToObject<SystemCodeGenerationBaseInput>(),
                List = viewModel.List.ToList<SystemCodeGenerationListForListInput>(),
                Edit = viewModel.Edit.ToList<SystemCodeGenerationEditInput>()
            };
            var output = new SystemCodeGenerationOutput
            {
                Entities = await CreateEntity(input.Base),
                DataAccessInterface = CreateDataAccessInterface(input.Base),
                DataAccess = CreteDataAccess(input.Base),
                BusinessInterface = await CreteLogicInterface(input.Base),
                Business = await CreateLogic(input.Base),
                Controller = await CreateController(input.Base),
                List = CreateList(input.Base, input.List),
                Edit = await CreateEdit(input.Base,input.Edit)
            };
            return Json(output);
        }

        /// <summary>
        ///     生成实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<string> CreateEntity(SystemCodeGenerationBaseInput input)
        {
            //返回值
            var columnContent = new StringBuilder();
            //获取文件内容
            var returnContent = FileUtil.ReadFile(Path.Combine("DataUsers", "Templates", "CodeGeneration", "Entities.txt"));
            //获取对应列
            var columns = (await _dataBaseLogic.GetDataBaseColumn(new IdInput { Id = input.Name.Replace("System", "Sys") })).ToList();
            //1:命名空间
            returnContent = returnContent.Replace("{{NameSpace}}", input.Name.Split('_')[0]);
            //2:表名
            returnContent = returnContent.Replace("{{TableName}}", input.Name.Replace("System","Sys"));
            //3:类名称
            returnContent = returnContent.Replace("{{ClassName}}", input.Name.Replace("_", ""));
            //4:主键名称
            returnContent = returnContent.Replace("{{KeyName}}", input.TableKey);
            //5:动态生成列
            foreach (var column in columns)
            {
                columnContent.Append("        /// <summary>\r\n");
                columnContent.Append(string.Format("        /// {0}\r\n", column.Remarks));
                columnContent.Append("        /// </summary>\r\n");
                columnContent.Append("        public " + ChangeToCSharpType(column.DataType) + " " + column.FieldName + " { get; set; }\r\n\r\n");
            }
            returnContent = returnContent.Replace("{{EntitiesBody}}", columnContent.ToString());
            //7:主键类型
            var key = columns.Where(column => column.FieldName == input.TableKey).FirstOrDefault();
            returnContent = returnContent.Replace("{{GenerationType}}", key != null && key.DataType == "uniqueidentifier" ? "GenerationType.Sequence" : "GenerationType.Indentity");
            //8:描述
            returnContent = returnContent.Replace("{{Description}}", input.Value);
            return returnContent;

        }
        /// <summary>
        ///     生成DataAccess接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string CreateDataAccessInterface(SystemCodeGenerationBaseInput input) {
            //获取文件内容
            var returnContent = FileUtil.ReadFile(Path.Combine("DataUsers", "Templates", "CodeGeneration", "IRepository.txt"));
            //1:命名空间
            returnContent = returnContent.Replace("{{NameSpace}}", input.Name.Split('_')[0]);
            //2:表名
            returnContent = returnContent.Replace("{{ClassName}}", input.Name.Replace("_", ""));
            //3:描述
            returnContent = returnContent.Replace("{{Description}}",  input.Value);
            //4:分页信息
            if (input.IsPaging)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Task<PagedResults<{input.Name.Replace("_", "")}>> Paging{input.Name.Replace("_", "")}(QueryParam queryParam);");
                returnContent = returnContent.Replace("{{PagingAction}}", sb.ToString());
            }
            else {
                returnContent = returnContent.Replace("{{PagingAction}}","");
            }
            return returnContent;
        }
        /// <summary>
        /// 生成DataAccess接口实现
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string CreteDataAccess(SystemCodeGenerationBaseInput input)
        {
            //获取文件内容
            var returnContent = FileUtil.ReadFile(Path.Combine("DataUsers", "Templates", "CodeGeneration", "Repository.txt"));
            //替换操作

            //1:命名空间
            returnContent = returnContent.Replace("{{NameSpace}}", input.Name.Split('_')[0]);
            //2:表名
            returnContent = returnContent.Replace("{{ClassName}}", input.Name.Replace("_", ""));
            //3:描述
            returnContent = returnContent.Replace("{{Description}}", input.Value);
            //4:分页
            if (input.IsPaging)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($" public Task<PagedResults<{input.Name.Replace("_", "")}>> Paging{input.Name.Replace("_", "")}(QueryParam queryParam)");
                sb.AppendLine("{");
                sb.AppendLine($" string sql=\"select * from {input.Name.Replace("System", "Sys")}\";");
                sb.AppendLine($" return SqlMapperUtil.PagingQuery<{input.Name.Replace("_", "")}>(sql, queryParam);");
                sb.AppendLine("}");
                returnContent = returnContent.Replace("{{PagingAction}}", sb.ToString());
            }
            else
            {
                returnContent = returnContent.Replace("{{PagingAction}}", "");
            }
            return returnContent;
        }

        /// <summary>
        /// 生成Logic接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Task<string> CreteLogicInterface(SystemCodeGenerationBaseInput input)
        {
            //获取文件内容
            var returnContent = FileUtil.ReadFile(Path.Combine("DataUsers", "Templates", "CodeGeneration", "ILogic.txt"));
            //替换操作

            //1:命名空间
            returnContent = returnContent.Replace("{{NameSpace}}", input.Name.Split('_')[0]);
            //2:表名
            returnContent = returnContent.Replace("{{ClassName}}", input.Name.Replace("_", ""));
            //3:描述
            returnContent = returnContent.Replace("{{Description}}", input.Value);
            //4:是否分页
            if (input.IsPaging)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Task<PagedResults<{1}>> Paging{0}(QueryParam queryParam);", input.Name.Replace("_", ""), input.Name.Replace("_", ""));
                returnContent = returnContent.Replace("{{PagingAction}}",sb.ToString());
            }
            else
            {
                returnContent = returnContent.Replace("{{PagingAction}}", "");
            }
            //5:保存方法
            StringBuilder sbsa = new StringBuilder();
            sbsa.AppendFormat("    Task<OperateStatus> Save{0} ({1} input);", input.Name.Replace("_", ""), input.Name.Replace("_", ""));
            returnContent = returnContent.Replace("{{SaveAction}}",sbsa.ToString());
            return Task.Factory.StartNew(()=>returnContent);
        }

        /// <summary>
        ///     生成Logic接口实现
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private  Task<string> CreateLogic(SystemCodeGenerationBaseInput input) {
            //获取文件内容
            var returnContent = FileUtil.ReadFile(Path.Combine("DataUsers", "Templates", "CodeGeneration", "Logic.txt"));
            //替换操作
            //1:命名空间
            returnContent = returnContent.Replace("{{NameSpace}}", input.Name.Split('_')[0]);
            //2:表名
            returnContent = returnContent.Replace("{{ClassName}}", input.Name.Replace("_", ""));
            //3:表名转换为小写
            returnContent = returnContent.Replace("{{ClassNameLower}}", input.Name.Replace("_", "").ReplaceFistLower());
            //4:主键
            returnContent = returnContent.Replace("{{KeyName}}", input.TableKey);
            //5:描述
            returnContent = returnContent.Replace("{{Description}}", input.Value);
            //6:分页
            if (input.IsPaging)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($" public Task<PagedResults<{input.Name.Replace("_", "")}>> Paging{input.Name.Replace("_", "")}(QueryParam queryParam)");
                sb.AppendLine("{");
                sb.AppendLine($" return  _{input.Name.Replace("_", "").ReplaceFistLower()}Repository.Paging{input.Name.Replace("_", "")}(queryParam);");
                sb.AppendLine("}");
                returnContent = returnContent.Replace("{{PagingAction}}",sb.ToString());
            }
            else
            {
                returnContent = returnContent.Replace("{{PagingAction}}", "");
            }
            //7:保存方法
            StringBuilder sbsa = new StringBuilder();
            sbsa.AppendLine($" public  Task<OperateStatus> Save{input.Name.Replace("_", "")} ({input.Name.Replace("_", "")} input) ");
            sbsa.AppendLine("{");
            sbsa.AppendLine($" input.{input.TableKey} = Guid.NewGuid().ToString();");
            sbsa.AppendLine(" return InsertAsync(input);");
            sbsa.AppendLine("}");
            returnContent = returnContent.Replace("{{SaveAction}}", sbsa.ToString());
            return Task.Factory.StartNew(()=>returnContent);
        }
        /// <summary>
        /// 生成控制器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private  Task<string> CreateController(SystemCodeGenerationBaseInput input)
        {
            //获取文件内容
            var returnContent = FileUtil.ReadFile(Path.Combine("DataUsers", "Templates", "CodeGeneration", "Controller.txt"));
            //替换操作
            //1:命名空间
            returnContent = returnContent.Replace("{{NameSpace}}", input.Name.Split('_')[0]);
            //2:表名
            returnContent = returnContent.Replace("{{ClassName}}", input.Name.Replace("_", ""));
            //3:表名转换为小写
            returnContent = returnContent.Replace("{{ClassNameLower}}", input.Name.Replace("_", "").ReplaceFistLower());
            //4:控制器名称
            returnContent = returnContent.Replace("{{ControllerName}}", input.Name.Replace(input.Name.Split('_')[0], "").Replace("_", ""));
            //5:分页方法
            if (input.IsPaging)
            {
                StringBuilder strPagingAction=new StringBuilder();
                strPagingAction.AppendLine("[HttpPost]");
                strPagingAction.AppendLine($"[Description(\"应用系统-方法-列表-获取{input.Value}分页信息\")]");
                strPagingAction.AppendLine($" public async Task<JsonResult> GetPaging{input.Name.Replace("_", "")}(QueryParam queryParam) ");
                strPagingAction.AppendLine("{");
                strPagingAction.AppendLine($"  return JsonForGridPaging(await _{input.Name.Replace("_", "").ReplaceFistLower()}Logic.Paging{input.Name.Replace("_", "")}(queryParam));");
                strPagingAction.AppendLine("}");
                returnContent = returnContent.Replace("{{PagingAction}}", strPagingAction.ToString());
            }
            else
            {
                returnContent = returnContent.Replace("{{PagingAction}}", "");
            }
            //6:保存方法
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[HttpPost]");
            sb.AppendLine("   [ValidateAntiForgeryToken]");
            sb.AppendLine($"  [Description(\"应用系统-方法-保存{input.Value}\")]");
            sb.AppendLine($"    public async Task<JsonResult> Save{input.Name.Replace("_", "")}({input.Name.Replace("_", "")} input)");
            sb.AppendLine("{");
            sb.AppendLine($"   return Json(await _{input.Name.Replace("_", "").ReplaceFistLower()}Logic.Save{input.Name.Replace("_", "")}(input));");
            sb.AppendLine("}");
            returnContent = returnContent.Replace("{{SaveAction}}",sb.ToString());
            //描述
            returnContent = returnContent.Replace("{{Description}}", input.Value);
            
            return Task.Factory.StartNew(() => returnContent); ;
        }
        /// <summary>
        ///     生成编辑页面
        /// </summary>
        /// <param name="input"></param>
        /// <param name="editInputs"></param>
        /// <returns></returns>
        private Task<string> CreateEdit(SystemCodeGenerationBaseInput input,IEnumerable<SystemCodeGenerationEditInput> editInputs) {
            var str = "";
            foreach (var item in editInputs)
            {
                if (item.ControlType == Domain.Models.Enums.EnumControlType.文本域)
                {
                    str += $@"  <div class='hr-line-dashed'></div>
                            <div class='form-group'>
                                <label class='col-sm-2 control-label'> {item.ControlName}：</label>
                                <div class='input-group  col-sm-10'>
                                      <textarea  rows='3' name='{item.ControlId}' id='{item.ControlId}' class='form-control'>< /textarea>
                                </div>
                            </div>";
                }
                else {
                    str += $@"  <div class='hr-line-dashed'></div>
                            <div class='form-group'>
                                <label class='col-sm-2 control-label'> {item.ControlName}：</label>
                                <div class='input-group  col-sm-10'>
                                    <input type='text' name='{item.ControlId}' id='{item.ControlId}' class='form-control input-sm' >
                                </div>
                            </div>";
                }
               
            }
            //获取文件内容
            var returnContent = FileUtil.ReadFile(Path.Combine("DataUsers", "Templates", "CodeGeneration", "Edit.txt"));
            //主体
            returnContent = returnContent.Replace("{{Body}}", str);
            return Task.Factory.StartNew(()=>returnContent);
        }

        /// <summary>
        /// 生成列表界面
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string CreateList(SystemCodeGenerationBaseInput input, IEnumerable<SystemCodeGenerationListForListInput> listInput) {
            //获取文件内容
            var returnContent = FileUtil.ReadFile(Path.Combine("DataUsers", "Templates", "CodeGeneration", "List.txt"));
            //替换操作
            //1:命名空间
            returnContent = returnContent.Replace("{{NameSpace}}", input.Name.Split('_')[0].ToLower());
            //2:控制器名称
            returnContent = returnContent.Replace("{{ControllerName}}", input.Name.Replace(input.Name.Split('_')[0], "").Replace("_", "").ToLower());
            //3:描述
            returnContent = returnContent.Replace("{{Description}}", input.Value);
            //4:列表项
            returnContent = returnContent.Replace("{{ColModel}}", GetColModel(listInput.OrderBy(o => o.OrderNo).ToList(), input));
            //5:是否分页
            returnContent = returnContent.Replace("{{IsPaging}}", input.IsPaging.ToString().ToLower());
            return returnContent;
        }
        /// <summary>
        ///     根据用户的字段权限拼接界面显示字段表达式
        /// </summary>
        /// <param name="listInput"></param>
        /// <param name="baseInput"></param>
        /// <returns>拼接后的表达式</returns>
        private string GetColModel(IList<SystemCodeGenerationListForListInput> listInput, SystemCodeGenerationBaseInput baseInput)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[\r\n");
            foreach (SystemCodeGenerationListForListInput input in listInput)
            {
                stringBuilder.Append("            {");

                if (!string.IsNullOrWhiteSpace(input.field))
                {
                    stringBuilder.Append("field:\"" + input.field.ReplaceFistLower() + "\",");
                }
                else {
                    stringBuilder.Append("field:\"" + input.field + "\",");
                }
               
                //是否隐藏
                if (input.Hidden)
                {
                    stringBuilder.Append("title:\"" + input.title + "\",");
                   
                    stringBuilder.Append("width:" + input.Width + ",");
                    //是否排序
                    if (input.Sort)
                    {
                       stringBuilder.Append("sort:true,");
                    }
                    else
                    {
                        stringBuilder.Append("sort:false,");
                    }

                    //对齐格式
                    if (input.Align.IsNullOrEmpty())
                    {
                        input.Align = "center";
                    }

                    //格式化
                    if (!input.Formatter.IsNullOrEmpty())
                    {
                        stringBuilder.Append("align:\"" + input.Align + "\",");
                        stringBuilder.Append("formatter:\"" + input.Formatter + "\"},\r\n");
                    }
                    else
                    {
                        stringBuilder.Append("align:\"" + input.Align + "\"");
                        stringBuilder.Append(input == listInput.Last() ? "}\r\n" : "},\r\n");
                    }
                }
                else
                {
                    stringBuilder.Append("hidden:true},\r\n");
                }
            }
            string returnStr = stringBuilder.ToString().TrimEnd(',');
            returnStr += "        ]";
            return returnStr;
        }
        #endregion

        #region 生成文件
        public JsonResult CreateFile(SystemCodeGenerationCreateFileViewModel model)
        {
            OperateStatus operateStatus = new OperateStatus();
            try
            {
                SystemCodeGenerationBaseInput baseInput = model.Base.ToObject<SystemCodeGenerationBaseInput>();
                //实体
                FileUtil.WriteFile(baseInput.EntityPath + "\\" + baseInput.Entity + ".cs", model.Entities);
                //DataAccess接口
                FileUtil.WriteFile(baseInput.DataAccessInterfacePath + "\\" + baseInput.DataAccessInterface + ".cs", model.DataAccessInterface);
                //DataAccess实现
                FileUtil.WriteFile(baseInput.DataAccessPath + "\\" + baseInput.DataAccess + ".cs", model.DataAccess);
                //Business接口
                FileUtil.WriteFile(baseInput.BusinessInterfacePath + "\\" + baseInput.BusinessInterface + ".cs", model.BusinessInterface);
                //Business实现
                FileUtil.WriteFile(baseInput.BusinessPath + "\\" + baseInput.Business + ".cs", model.Business);
                //控制器
                FileUtil.WriteFile(baseInput.ControllerPath + "\\" + baseInput.Controller + ".cs", model.Controller);
                //控制器
                FileUtil.WriteFile(baseInput.ControllerPath.Replace("Controllers", "Views") + "\\" + baseInput.Controller.Replace("Controller", "") + "\\" + baseInput.List + ".cshtml", model.List);
                //表单
                FileUtil.WriteFile(baseInput.ControllerPath.Replace("Controllers", "Views") + "\\" + baseInput.Controller.Replace("Controller", "") + "\\" + baseInput.Edit + ".cshtml", model.Edit);
            }
            catch (Exception ex)
            {
                operateStatus.Message = ex.Message;
                return Json(operateStatus);
            }
            operateStatus.Message = "生成成功";
            operateStatus.ResultSign = ResultSign.Successful;
            return Json(operateStatus);
        }

        #endregion
    }
}