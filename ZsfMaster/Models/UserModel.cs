using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using ZsfData;
using System.Web.Security;
using System.Web;

namespace ZsfProject.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage="账号不能为空")]
        [Display(Name = "账号")]
        public string UserName { get; set; }

        [Required(ErrorMessage="密码不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "自动登录")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "账号不能为空")]
        [Display(Name = "账号")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required(ErrorMessage="邮箱不能为空")]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "验证码")]
        public string ValidateCode { get; set; }
    }

    public class ChangePassword
    {
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [Display(Name = "确认新密码")]
        public string NewPasswordConfirm { get; set; }
    }

    public class FindPassword
    {
        [Display(Name = "邮箱")]
        public string UserEmail { get; set; }

        [Display(Name = "验证码")]
        public string ValidateCode { get; set; }
    }


    public class UserModel
    {
        ZsfEntities db = new ZsfEntities();

        public UserInfo GetUserInfo(string userName)
        {
            return db.UserInfo.FirstOrDefault(r => r.Name == userName || r.Email == userName);
        }

        public V_UserInfoDetail GetUserInfoDetail(string userName)
        {
            return db.V_UserInfoDetail.FirstOrDefault(r => r.Name == userName || r.Email == userName);
        }

        public UserMessage GetUserMessage(int id)
        {
            return db.UserMessage.FirstOrDefault(r => r.Id == id);
        }

        public IQueryable<UserMessage> GetUserMessages(string userName)
        {
            return db.UserMessage.Where(r => r.UserInfo.Name == userName).OrderByDescending(r => r.CreateTime);
        }

        public IQueryable<V_UserShopComment> GetUserComments(string userName)
        {
            return db.V_UserShopComment.Where(r => r.UserName == userName).OrderByDescending(r => r.CreateTime);
        }

        public IQueryable<V_UserFavoriteShopForList> GetUserFavoriteShops(int userId)
        {
            return db.V_UserFavoriteShopForList.Where(r => r.UserId == userId).OrderByDescending(r => r.FavoriteCreateTime);
        }

        public static int GerUserUnreadMessagesCount()
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return 0;
            
            ZsfEntities db = new ZsfEntities();
            return db.UserMessage.Where(r => r.UserInfo.Name == HttpContext.Current.User.Identity.Name && r.Readed == false).Count();
        }

        public int GetUserTotalUsablePoint(string userName)
        {
            int pointCount = db.UserPoint.Where(r => r.UserInfo.Name == userName && r.Usable == true).Count();
            if (pointCount == 0)
                return 0;
            return db.UserPoint.Where(r => r.UserInfo.Name == userName && r.Usable == true).Sum(r => r.PointCount);
        }

        public IQueryable<UserPoint> GetUserPoints(string userName, bool usable)
        {
            return db.UserPoint.Where(r => r.UserInfo.Name == userName && r.Usable == usable).OrderByDescending(r => r.CreateTime);
        }

        public UserFavoriteShop GetUserFavoriteShop(int shopId, int userId)
        {
            return db.UserFavoriteShop.Where(r => r.UserId == userId && r.ShopId == shopId).FirstOrDefault();
        }

        public UserInfo CheckUserName(string userName)
        {
            return db.UserInfo.FirstOrDefault(r => r.Name == userName);
        }

        public UserInfo CheckUserEmail(string email)
        {
            return db.UserInfo.FirstOrDefault(r => r.Email == email);
        }

        public UserGradeCategory GetUserGrade(int userId)
        {
            return db.UserInfo.FirstOrDefault(r => r.Id == userId).UserGradeCategory;
        }

        public UserGradeCategory GetUserGrade(string userName)
        {
            return db.UserInfo.FirstOrDefault(r => r.Name == userName).UserGradeCategory;
        }

        public UserDefaultArea GetUserDefaultArea(int userId)
        {
            return db.UserDefaultArea.FirstOrDefault(r => r.UserId == userId);
        }

        public static void GetUserDefaultAreaSetting(ref string areaId, ref string areaName)
        {
            HttpCookie areaCookie = HttpContext.Current.Request.Cookies.Get("udefaultarea");
            if(areaCookie != null)
            {
                areaId = areaCookie.Values["Id"];
                areaName = HttpUtility.UrlDecode(areaCookie.Values["Name"]);
            }
            else if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string d = HttpContext.Current.Request.Url.Host;
                ZsfEntities db = new ZsfEntities();
                var defaultArea = db.V_UserDefaultArea.FirstOrDefault(r => r.UserName == HttpContext.Current.User.Identity.Name);
                if (defaultArea != null)
                {
                    areaId = defaultArea.AreaId.ToString();
                    areaName = defaultArea.AreaValue;

                    areaCookie = new HttpCookie("udefaultarea");
                    areaCookie.Domain = BaseDataModel.CookieDomain;
                    areaCookie.Values["Id"] = areaId;
                    areaCookie.Values["Name"] = HttpUtility.UrlEncode(areaName);
                    areaCookie.Expires = DateTime.Now.AddYears(1);
                    HttpContext.Current.Response.Cookies.Set(areaCookie);
                }
            }

        }

        public void SignIn(UserInfo user, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(user.Name, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public UserInfo ValidateUser(string userName, string password)
        {
            string pwd = PasswordEncrypt(password);
            return db.UserInfo.FirstOrDefault(r => (r.Name == userName || r.Email == userName) && r.Password == pwd);
        }

        public string PasswordEncrypt(string str)
        {
            //先SHA1编码再MD5编码
            string sha = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "SHA1");
            return FormsAuthentication.HashPasswordForStoringInConfigFile(sha, "MD5");
        }

        public void AddUserPoint(int uId, string reason, short point)
        {
            UserPoint uPoint = new UserPoint();
            uPoint.UserId = uId;
            uPoint.Usable = true;
            uPoint.Reason = reason;
            uPoint.CreateTime = DateTime.Now;
            uPoint.PointCount = point;
            Add(uPoint);
        }

        public void AddUserMessage(int uId,string title,string content)
        {
            UserMessage uMessage = new UserMessage();
            uMessage.UserId = uId;
            uMessage.MsgTitle = title;
            uMessage.MsgContent = content;
            uMessage.CreateTime = DateTime.Now;
            uMessage.Readed = false;
            Add(uMessage);
        }

        public void Add(UserInfo userInfo)
        {
            db.UserInfo.AddObject(userInfo);
            Save();
        }

        public void Add(UserMessage userMsg)
        {
            db.UserMessage.AddObject(userMsg);
            Save();
        }

        public void Add(UserDefaultArea area)
        {
            db.UserDefaultArea.AddObject(area);
            Save();
        }

        public void Add(UserPoint point)
        {
            db.UserPoint.AddObject(point);
            Save();
        }

        public void Add(UserFavoriteShop data)
        {
            db.UserFavoriteShop.AddObject(data);
            Save();
        }

        public void Delete(UserFavoriteShop data)
        {
            db.UserFavoriteShop.DeleteObject(data);
            Save();
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }

}