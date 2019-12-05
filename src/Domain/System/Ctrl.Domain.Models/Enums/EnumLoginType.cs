using System;
using System.Collections.Generic;
using System.Text;

namespace Ctrl.Domain.Models.Enums
{
    /// <summary>
    ///     登录类型
    /// </summary>
    public enum EnumLoginType:byte
    {
        账号密码登录,
        人脸识别登录
    }
}
