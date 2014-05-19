using System.Web;
using System.Web.Mvc;
using System.Security.Principal;

namespace ZsfProject.Models
{
    public class ZsfAuthorizeAttribute : AuthorizeAttribute   
    {
        // 只需重载此方法，模拟自定义的角色授权机制   
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            string currentRole = GetRole(httpContext.User.Identity.Name);

            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("(,|^)" + currentRole + "(,|$)");
        
            if (reg.IsMatch(Roles))
                return true;
            
            return base.AuthorizeCore(httpContext);
        }

        // 返回用户对应的角色
        public static string GetRole(string userName)
        {
            if(string.IsNullOrWhiteSpace(userName))
                return "0";
            UserModel accountModel = new UserModel();
            var userGrade = accountModel.GetUserGrade(userName);
            return userGrade.GradeLevel.ToString();
        }
    }
}