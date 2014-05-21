using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZsfProject.Models;
using ZsfProject.Tools;
using ZsfData;
using System.Collections;
using System.Drawing;
using System.IO;

namespace ZsfProject.Controllers
{

    public class UserController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            UserModel userModel = new UserModel();
            var userInfo = userModel.GetUserInfoDetail(User.Identity.Name);
            ViewBag.PageName = "home";
            return View(userInfo);
        }

        [ZsfAuthorize]
        public ActionResult Points(int p = 1, bool u = true)
        {
            UserModel userModel = new UserModel();
            var userPoints = userModel.GetUserPoints(User.Identity.Name, u);
            int pointCount = userPoints.Count();
            ViewBag.UsablePointsCount = userModel.GetUserTotalUsablePoint(User.Identity.Name);
            ViewBag.PointUsable = u;

            int pageSize = 15;
            Paging.ToPaging(p, pointCount, this, pageSize);
            userPoints = userPoints.Skip((p - 1) * pageSize).Take(pageSize);
            ViewBag.PageName = "point";

            return View(userPoints);
        }

        [ZsfAuthorize]
        public ActionResult Messages(int p = 1)
        {
            UserModel userModel = new UserModel();
            var userMails = userModel.GetUserMessages(User.Identity.Name);
            int msgCount = userMails.Count();
            ViewBag.MessagesCount = msgCount;

            int pageSize = 15;
            Paging.ToPaging(p, msgCount, this, pageSize);
            userMails = userMails.Skip((p - 1) * pageSize).Take(pageSize);
            ViewBag.PageName = "msg";

            return View(userMails);
        }

        [ZsfAuthorize]
        public ActionResult Comments(int p = 1)
        {
            UserModel userModel = new UserModel();
            var userComments = userModel.GetUserComments(User.Identity.Name);
            int cmtCount = userComments.Count();
            ViewBag.CommentsCount = cmtCount;

            int pageSize = 15;
            Paging.ToPaging(p, cmtCount, this, pageSize);
            userComments = userComments.Skip((p - 1) * pageSize).Take(pageSize);
            ViewBag.PageName = "cmt";

            return View(userComments);
        }

        [ZsfAuthorize]
        public ActionResult FavoriteShops(int p = 1)
        {
            UserModel userModel = new UserModel();
            UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
            var favoriteShops = userModel.GetUserFavoriteShops(userInfo.Id);
            int favCount = favoriteShops.Count();
            ViewBag.FavoriteShopsCount = favCount;

            int pageSize = 16;
            Paging.ToPaging(p, favCount, this, pageSize);
            favoriteShops = favoriteShops.Skip((p - 1) * pageSize).Take(pageSize);
            ViewBag.PageName = "favshop";

            return View(favoriteShops);
        }

        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("index");

            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel registerInfo, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("index");

            if (ModelState.IsValid)
            {
                if (Session["ValidateCode"] == null || registerInfo.ValidateCode.ToLower() != Session["ValidateCode"].ToString().ToLower())
                {
                    ModelState.AddModelError("ValidateCode", "验证码错误");
                    return View(registerInfo);
                }

                UserModel userModel = new UserModel();
                UserInfo userInfo = new UserInfo();

                userInfo.Name = registerInfo.UserName;
                userInfo.Password = userModel.PasswordEncrypt(registerInfo.Password);
                userInfo.Email = registerInfo.Email;
                userInfo.GradeId = 1;
                userInfo.CreateTime = DateTime.Now;
                userInfo.Photo = "/Contents/Images/default_user_photo.png";

                userModel.Add(userInfo);

                UserMessage userMsg = new UserMessage();
                userMsg.UserId = userInfo.Id;
                userMsg.MsgTitle = "系统通知";
                userMsg.MsgContent = userInfo.Name + "欢迎您来到宅食府，希望能为您提供优质的服务。";
                userMsg.CreateTime = DateTime.Now;
                userMsg.Readed = false;
                userModel.Add(userMsg);

                userModel.SignIn(userInfo, false);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Shop");
        }

        [ZsfAuthorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ZsfAuthorize]
        public ActionResult ChangePassword(ChangePassword pwdForm)
        {
            UserModel userModel = new UserModel();
            UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
            
            userInfo.Password = userModel.PasswordEncrypt(pwdForm.NewPassword);
            userModel.Save();

            userModel.SignOut();

            return RedirectToAction("Login");
        }

        public ActionResult FindPassword(int step = 1)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("index");

            if (step == 2)
                return View("FindPasswordStep2");

            return View();
        }

        [HttpPost]
        public ActionResult FindPassword(FindPassword formValues)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("index");

            if (ModelState.IsValid)
            {
                var emailReg = new System.Text.RegularExpressions.Regex("^(\\w)+(\\.\\w+)*@[\\w\\d]+((\\.\\w+)+)$");
                if (formValues.UserEmail == null || !emailReg.IsMatch(formValues.UserEmail))
                {
                    ModelState.AddModelError("UserEmail", "邮箱格式不正确");
                    return View(formValues);
                }
                else if (Session["ValidateCode"] == null
                   || formValues.ValidateCode == null
                   || formValues.ValidateCode.ToLower() != Session["ValidateCode"].ToString().ToLower())
                {
                    ModelState.AddModelError("ValidateCode", "验证码错误");
                    return View(formValues);
                }

                UserModel userModel = new UserModel();
                UserInfo userInfo = userModel.GetUserInfo(formValues.UserEmail);
                string newPwd = ImageValidate.CreateCode(6);//生生6位数的随机密码
                userInfo.Password = userModel.PasswordEncrypt(newPwd);
                userModel.Save();
                //发送新密码
                ZsfProject.Tools.SendEmail.ResetPassword(formValues.UserEmail, newPwd);
            }

            return RedirectToAction("FindPassword", new { step = 2 });

        }

        public ActionResult Login()
        {
            //如果用户已经被验证,但是进入到某些页面后由于权限不足,被系统引导到登陆界面.则强制将用户引导到主页面
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Shop");

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("index");

            UserModel accountModel = new UserModel();
            if (ModelState.IsValid)
            {
                var userInfo = accountModel.ValidateUser(model.UserName, model.Password);
                if (userInfo != null)
                {
                    //设置验证用户的cookie信息
                    accountModel.SignIn(userInfo, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl))
                        return Json(new { key = 1, value = returnUrl }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { key = 1, value = "/" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { key = 0, value = "账号或密码错误" }, JsonRequestBehavior.AllowGet);
                }
            }

            return View(model);
        }

        public ActionResult LogOut(string returnUrl)
        {
            UserModel accountModel = new UserModel();
            accountModel.SignOut();

            if (!string.IsNullOrWhiteSpace(returnUrl) && !returnUrl.ToLower().Contains("/user"))
                return Redirect(returnUrl);

            return Redirect("/");;
        }

        [HttpPost]
        public ActionResult GetMsgContent(int id)
        {
            UserModel userModel = new UserModel();
            UserMessage msg = userModel.GetUserMessage(id);
            if (msg == null)
                return Json(new { key = 0, value = "该信息不存在！" }, JsonRequestBehavior.AllowGet);

            //如果该条信息没有被读过，则此时将其标记为已读
            if (!msg.Readed)
            {
                msg.Readed = true;
                userModel.Save();
            }
            return Json(new { key = 1, title = msg.MsgTitle, createTime = msg.CreateTime.ToString("yyyy/M/d HH:mm:ss"), content = msg.MsgContent }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int ChangeUserServiceArea(int areaId)
        {
            string areaName = "";
            
            if (User.Identity.IsAuthenticated)
            {
                UserModel userModel = new UserModel();
                UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
                UserDefaultArea defaultArea;
                if (userInfo.UserDefaultArea.Count == 0)
                {
                    defaultArea = new UserDefaultArea();
                    defaultArea.UserId = userInfo.Id;
                    defaultArea.AreaId = areaId;
                    userModel.Add(defaultArea);
                }
                else
                {
                    defaultArea = userModel.GetUserDefaultArea(userInfo.Id);
                    defaultArea.AreaId = areaId;
                    userModel.Save();
                }
                areaName = defaultArea.ServiceArea.Value;
            }
            else
            {
                BaseDataModel baseModel = new BaseDataModel();
                areaName =  baseModel.GetServiceArea(areaId).Value;
            }

            System.Web.HttpCookie areaCookie = new System.Web.HttpCookie("udefaultarea");
            areaCookie.Domain = BaseDataModel.CookieDomain;
            areaCookie.Values["Id"] = areaId.ToString();
            areaCookie.Values["Name"] = HttpUtility.UrlEncode(areaName);
            areaCookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Set(areaCookie);
            return 1;
        }

        [HttpPost]
        public ActionResult CheckUserInfo(string type, string value)
        {
            UserModel userModel = new UserModel();
            int returnKey = 0;
            string returnValue = "";

            if (type == "name" && userModel.CheckUserName(value) != null)
                returnValue = "用户名已存在";
            else if (type == "email" && userModel.CheckUserEmail(value) != null)
                returnValue = "邮箱已存在";
            else
                returnKey = 1;

            return Json(new { key = returnKey, value = returnValue }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ZsfAuthorize]
        public ActionResult ChangeUserInfo(bool gender, string birthDay)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Json(new { key = 0, value = "用户不合法！" }, JsonRequestBehavior.AllowGet);

                UserModel userModel = new UserModel();
                var userInfo = userModel.GetUserInfo(User.Identity.Name);
                userInfo.Gender = gender;
                userInfo.Birthday = DateTime.Parse(birthDay);
                userModel.Save();
                return Json(new { key = 1, value = "修改成功!" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { key = 0, value = "网站内部错误!请刷新页面重试。" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult AddFavoriteShop(int shopId)
        {
            if(!User.Identity.IsAuthenticated)
                return Json(new { type = 2 }, JsonRequestBehavior.AllowGet);

            try
            {
                ShopModel shopModel = new ShopModel();
                UserModel userModel = new UserModel();
                UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);

                if(userModel.GetUserFavoriteShop(shopId, userInfo.Id) != null)
                    return Json(new { type = 1 }, JsonRequestBehavior.AllowGet);

                UserFavoriteShop favShop = new UserFavoriteShop();
                favShop.UserId = userInfo.Id;
                favShop.ShopId = shopId;
                favShop.CreateTime = DateTime.Now;
                userModel.Add(favShop);
            }
            catch(Exception ex)
            {
                return Json(new { type = 0, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { type = 1 }, JsonRequestBehavior.AllowGet);
        }

        [ZsfAuthorize]
        [HttpPost]
        public ActionResult CancelFavoriteShop(int shopId)
        {
            try
            {
                UserModel userModel = new UserModel();
                UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
                UserFavoriteShop favShop = userModel.GetUserFavoriteShop(shopId, userInfo.Id);
                userModel.Delete(favShop);
            }
            catch(Exception ex)
            {
                return Json(new { type = 0, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { type = 1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult GetImageValidate()
        {
            string validateStr = ImageValidate.CreateCode(4);
            Session["ValidateCode"] = validateStr;

            Response.ContentType = "image/jpeg";
            return new FileStreamResult(ImageValidate.CreateImage(validateStr), Response.ContentType);
        }

        [HttpPost]
        public ActionResult UploadUserPhotoPreview(HttpPostedFileBase fileData)
        {
            if (fileData != null)
            {
                try
                {
                    // 文件上传后的保存路径
                    string filePath = Server.MapPath("~/Contents/Uploads/");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string fileName = Path.GetFileName(fileData.FileName);// 原始文件名称
                    string fileExtension = Path.GetExtension(fileName); // 文件扩展名
                    string saveName = Guid.NewGuid().ToString() + fileExtension; // 保存文件名称

                    fileData.SaveAs(filePath + saveName);

                    Image imgFile = Image.FromStream(fileData.InputStream);

                    return Json(new { Success = true, FileName = saveName, FilePath = "/Contents/Uploads/", ImageWidth = imgFile.Width, ImageHeight = imgFile.Height });
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                return Json(new { Success = false, Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CropUserPhoto(string imgPath, float zoomRatio, float cropX, float cropY, int cropW, int cropH)
        {
            string fileName = "";
            string filePath = "";
            using (Image logo = Image.FromFile(Server.MapPath(imgPath)))
            {
                //根据缩放比例，剪切原图中对应的区域
                RectangleF cropRec = new RectangleF(cropX / zoomRatio, cropY / zoomRatio, cropW / zoomRatio, cropH / zoomRatio);
                //将剪切后的图片填充到该矩形内
                Rectangle fillRec = new Rectangle(0, 0, cropW, cropH);

                using (Bitmap logoBit = new Bitmap(cropW, cropH))
                {
                    using (Graphics g = Graphics.FromImage(logoBit))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.DrawImage(logo, fillRec, cropRec, GraphicsUnit.Pixel);

                        UserModel userModel = new UserModel();
                        UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
                        string fileExtension = Path.GetExtension(imgPath);

                        filePath = Path.GetDirectoryName(Server.MapPath("~/Contents/UserPhotos/"));
                        fileName = (10200 + userInfo.Id) + fileExtension;
                        string fileFullPath = filePath + "\\" + fileName;

                        if (System.IO.File.Exists(fileFullPath))
                        {
                            try
                            {
                                System.IO.File.Delete(fileFullPath);
                            }
                            catch
                            {//如果删除不成功，就以随机的GUID来命名文件
                                fileName = Guid.NewGuid().ToString() + fileExtension;
                                fileFullPath = filePath + "\\" + fileName;
                            }
                        }
                        userInfo.Photo = "/Contents/UserPhotos/" + fileName;
                        userModel.Save();

                        logoBit.Save(fileFullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }

            return Json(new { key = 1, value = "/Contents/UserPhotos/" + fileName }, JsonRequestBehavior.AllowGet);
        }
    }
}
