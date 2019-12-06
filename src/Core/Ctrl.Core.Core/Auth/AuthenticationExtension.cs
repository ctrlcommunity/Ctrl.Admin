using Ctrl.Core.Core.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ctrl.Core.Core.Auth
{
    /// <summary>
    ///     用户登录写入Cookie和取出Cookie信息
    /// </summary>
    public  class AuthenticationExtension
    {
        private static  IHostingEnvironment environment { get; set; }

        public AuthenticationExtension(IHostingEnvironment hosting) {
            environment = hosting;
        }
     
        //Cookie默认保存时间1天
        private static int _cookieSaveDays = 1;
        /// <summary>
        ///     赋值Cookie并加密
        /// </summary>
        /// <param name="user">用户信息</param>
        public static void SetAuthCookie(PrincipalUser userinfo)
        {
            try
            {
                //下面的变量claims是Claim类型的数组，Claim是string类型的键值对，所以claims数组中可以存储任意个和用户有关的信息，
                //不过要注意这些信息都是加密后存储在客户端浏览器cookie中的，所以最好不要存储太多特别敏感的信息，这里我们只存储了用户名到claims数组,
                //表示当前登录的用户是谁
                var claims = new[] { new Claim(ClaimValueTypes.String,JsonConvert.SerializeObject(userinfo)) };
                var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal user = new ClaimsPrincipal(claimsIdentity);
                Task.Run(async () =>
                {
            
                    //可以使用HttpContext.SignInAsync方法的重载来定义持久化cookie存储用户认证信息，例如下面的代码就定义了用户登录后60分钟内cookie都会保留在客户端计算机硬盘上，
                    //即便用户关闭了浏览器，60分钟内再次访问站点仍然是处于登录状态，除非调用Logout方法注销登录。
                    //注意其中的AllowRefresh属性，如果AllowRefresh为true，表示如果用户登录后在超过50%的ExpiresUtc时间间隔内又访问了站点，就延长用户的登录时间（其实就是延长cookie在客户端计算机硬盘上的保留时间），
                    //例如本例中我们下面设置了ExpiresUtc属性为60分钟后，那么当用户登录后在大于30分钟且小于60分钟内访问了站点，那么就将用户登录状态再延长到当前时间后的60分钟。但是用户在登录后的30分钟内访问站点是不会延长登录时间的，
                    //因为ASP.NET Core有个硬性要求，是用户在超过50%的ExpiresUtc时间间隔内又访问了站点，才延长用户的登录时间。
                    //如果AllowRefresh为false，表示用户登录后60分钟内不管有没有访问站点，只要60分钟到了，立马就处于非登录状态（不延长cookie在客户端计算机硬盘上的保留时间，60分钟到了客户端计算机就自动删除cookie）
                    await HttpContexts.Current.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    user, new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(_cookieSaveDays),
                        AllowRefresh = false
                    });

                }).Wait();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        ///     根据请求获取当前登录人员信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static PrincipalUser Current()
        {
            try
            {
                // 如果HttpContext.User.Identity.IsAuthenticated为true，
                //或者HttpContext.User.Claims.Count()大于0表示用户已经登录
                if(HttpContexts.Current.User.Claims.Count()>0)
                {
                    //这里通过 HttpContext.User.Claims 可以将我们在Login这个Action中存储到cookie中的所有
                    //claims键值对都读出来
                    return JsonConvert.DeserializeObject<PrincipalUser>(HttpContexts.Current.User.Claims.First().Value);
                }
                return null;
            }
            catch
            {
                /* 有异常也不要抛出，防止攻击者试探。 */
                return null;
            }
        }
        /// <summary>
        ///     清除授权Cookie信息
        /// </summary>
        public static void SignOut()
        {
            Task.Run(async () =>
            {
                //注销登录的用户，相当于ASP.NET中的FormsAuthentication.SignOut  
                await HttpContexts.Current.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }).Wait();

        }
    }
}

