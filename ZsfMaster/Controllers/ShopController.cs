using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using ZsfProject.Models;
using ZsfProject.Tools;
using ZsfData;
using System.Web;
using System.IO;
using System.Drawing;


namespace ZsfProject.Controllers
{
    public class ShopController : Controller
    {
        string[] offsetTime = { "5:00", "5:30", "6:00", "6:30", "7:00", "7:30", "8:00", "8:30", "9:00",
                                      "9:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00",
                                      "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30", "17:00",
                                      "17:30", "18:00", "18:30", "19:00", "19:30", "20:00", "20:30", "21:00",
                                      "21:30", "22:00", "22:30", "23:00", "23:30", "24:00" };
        string shopImagePath = System.Configuration.ConfigurationManager.AppSettings["ShopImagePath"];

        public ActionResult Index(int id = 0, string shopName = "", string s = "default", int p = 1)
        {
            return RedirectToRoute("ShopDetail", new { id = 173 });

            /*
            ViewBag.ShopSearchName = shopName;
            ShopModel shopModel = new ShopModel();

            string serviceAreaId = "0";//默认宁波大学
            string serviceAreaName = "";
            UserModel.GetUserDefaultAreaSetting(ref serviceAreaId, ref serviceAreaName);
            ViewBag.ServiceAreaName = serviceAreaName;

            ViewBag.FavoriteListShow = true;
            HttpCookie cookie = Request.Cookies.Get("favShopsToggle");
            if(cookie != null)
                ViewBag.FavoriteListShow = (cookie.Value == "1" ? true : false);

            IQueryable<V_ShopForList> shops;
            if(String.IsNullOrWhiteSpace(shopName))
                shops = shopModel.GetShopsList(int.Parse(serviceAreaId), ZsfProject.Models.ZsfAuthorizeAttribute.GetRole(User.Identity.Name).Equals("9"));
            else
                shops = shopModel.GetShopsList(shopName, int.Parse(serviceAreaId));

            List<V_UserFavoriteShopForList> favoriteShops = new List<V_UserFavoriteShopForList>();
            ViewBag.UserId = -1;
            if(User.Identity.IsAuthenticated)
            {
                UserModel userModel = new UserModel();
                UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
                favoriteShops = shopModel.GetFavoriteShopsList(userInfo.Id).ToList();
                ViewBag.UserId = userInfo.Id;
            }

            var shopCategories = shopModel.GetShopCategoryStatistic(shops);

            ViewBag.TotalShopsCount = shops.Count();

            if(id != 0)
            {
                shops = shops.Where(r => r.CategoryId == id);
                ViewBag.ShopsCount = shops.Count();
            }
            else
                ViewBag.ShopsCount = ViewBag.TotalShopsCount;

            ViewBag.CategoryId = id;

            switch(s.ToLower())
            {
                case "grade":
                    shops = shops.OrderByDescending(r => r.Stars);
                    break;
                case "price":
                    shops = shops.OrderBy(r => r.UpSendPrice);
                    break;
                case "speed":
                    shops = shops.OrderBy(r => r.DeliveryTime);
                    break;
                case "paymoney":
                    shops = shops.OrderBy(r => r.AveragePayMoney);
                    break;
            }
            ViewBag.SortBy = s;

            int pageSize = MobileCapableRazorViewEngine.IsMobile(Request.UserAgent) ? 10 : 25;
            Paging.ToPaging(p, ViewBag.ShopsCount, this, pageSize);
            shops = shops.Skip((p - 1) * pageSize).Take(pageSize);
            return View(new ShopsListModel(shopCategories, shops, favoriteShops));*/
        }

        [HttpPost]
        public ActionResult QSearch(string shopName)
        {
            return RedirectToRoute("ShopSearch", new { shopName = shopName });
        }

        public ActionResult Search(string shopName, int categoryId = 0, int p = 1, string s = "default")
        {
            ViewBag.ShopSearchName = shopName;
            ShopModel shopModel = new ShopModel();
            string serviceAreaId = "0";//默认宁波大学
            string serviceAreaName = "";
            UserModel.GetUserDefaultAreaSetting(ref serviceAreaId, ref serviceAreaName);

            var shops = shopModel.GetShopsList(shopName, int.Parse(serviceAreaId));
            List<V_UserFavoriteShopForList> favoriteShops = new List<V_UserFavoriteShopForList>();
            if(User.Identity.IsAuthenticated)
            {
                UserModel userModel = new UserModel();
                UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
                favoriteShops = shopModel.GetFavoriteShopsList(userInfo.Id).ToList();
            }

            var shopCategories = shopModel.GetShopCategoryStatistic(shops);

            ViewBag.TotalShopsCount = shops.Count();

            if(categoryId != 0)
            {
                shops = shops.Where(r => r.CategoryId == categoryId);
                ViewBag.ShopsCount = shops.Count();
            }
            else
                ViewBag.ShopsCount = ViewBag.TotalShopsCount;

            ViewBag.CategoryId = categoryId;

            switch(s.ToLower())
            {
                case "grade":
                    shops = shops.OrderByDescending(r => r.Stars);
                    break;
                case "price":
                    shops = shops.OrderBy(r => r.UpSendPrice);
                    break;
                case "speed":
                    shops = shops.OrderBy(r => r.DeliveryTime);
                    break;
                case "paymoney":
                    shops = shops.OrderBy(r => r.AveragePayMoney);
                    break;
            }
            ViewBag.SortBy = s;

            int pageSize = Request.Browser.IsMobileDevice ? 15 : 25;
            Paging.ToPaging(p, ViewBag.ShopsCount, this, pageSize);
            shops = shops.Skip((p - 1) * pageSize).Take(pageSize);
            return View(new ShopsListModel(shopCategories, shops, favoriteShops));
        }

        public ActionResult Detail(int id, int p = 1)
        {
            id = 173;
            ShopModel shopModel = new ShopModel();
            V_ShopDetail shopInfo = shopModel.GetShopDetail(id);

            if (shopInfo == null)
                return RedirectToAction("HttpError404", "Error");

            /*ShopRankingAttribute rankAttr = shopModel.GetShopRankingAttribute(id);
            rankAttr.Visits += 1;
            shopModel.Save();*/

            ViewData["ShowShopPhone"] = true;

            var shopComments = shopModel.GetShopCommentsWithUserInfo(id);
            var shopServiceAreas = shopModel.GetShopServiceAreas(id);
            var shopDishes = shopModel.GetShopDishesWithCategory(id);

            ViewBag.UserFavoriteThisShop = false;
            ViewBag.UserId = -1;
            if(User.Identity.IsAuthenticated)
            {
                UserModel userModel = new UserModel();
                UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
                ViewBag.UserId = userInfo.Id;
                ViewBag.UserName = userInfo.Name;
                UserFavoriteShop userFavoriteShop = userModel.GetUserFavoriteShop(id, userInfo.Id);
                if(userFavoriteShop != null)
                    ViewBag.UserFavoriteThisShop = true;
            }

            ViewBag.Id = id;
            Session["ShopDetailPhone"] = shopInfo.PhoneNumber;
            
            int pageSize = 5;
            int shopCommentsCount = shopComments.Count();
            ViewBag.ShopCommentsCount = shopCommentsCount;
            Paging.ToPaging(p, shopCommentsCount, this, pageSize);

            return View(new ShopDetailModel(shopInfo, shopServiceAreas, shopDishes, shopComments.Skip((p - 1) * pageSize).Take(pageSize)));
        }

        public ActionResult CheckOut()
        {
            HttpCookie orderCookie = Request.Cookies.Get("u_order");
            var jser = new System.Web.Script.Serialization.JavaScriptSerializer();
            Order order = jser.Deserialize<Order>(HttpUtility.UrlDecode(orderCookie.Value));

            List<string> takeoutTimes = new List<string>();
            DateTime timeStart = DateTime.Parse(DateTime.Now.ToString("HH:mm"));
            int minuteNow = timeStart.Minute;
            if(minuteNow < 15)
                timeStart = timeStart.AddMinutes(15 - minuteNow);
            else if(minuteNow < 30)
                timeStart = timeStart.AddMinutes(30 - minuteNow);
            else if(minuteNow < 45)
                timeStart = timeStart.AddMinutes(45 - minuteNow);
            else
                timeStart = timeStart.AddMinutes(60 - minuteNow);

            ShopModel shopModel = new ShopModel();
            Shop shopInfo = shopModel.GetShop(order.ShopId);
            if(TimeSpan.Compare(shopInfo.OfficeTimeBegin, timeStart.TimeOfDay) > 0)
                timeStart = DateTime.Parse(shopInfo.OfficeTimeBegin.ToString("HH:00"));

            while(timeStart.TimeOfDay <= shopInfo.OfficeTimeEnd && timeStart.Day <= DateTime.Now.Day)
            {
                takeoutTimes.Add(timeStart.ToString("HH:mm"));
                timeStart = timeStart.AddMinutes(15);
            }
            ViewBag.TakeoutTime = takeoutTimes;

            return View(order);
        }

        [HttpPost]
        public ActionResult CheckOut(FormCollection formValues)
        {
            string address = formValues["address"];
            string phone = formValues["phone"];
            string verifyCode = formValues["verifyCode"];
            string takeoutTime = formValues["takeoutTime"];

            if(verifyCode != Session["OrderValidateCode"])
                return Json(new { });
            return View("checkoutdone");
        }

        public ActionResult CheckOutDone()
        {
            return View();
        }

        [HttpGet]
        public FileResult GetOrderImageValidate()
        {
            string validateStr = ImageValidate.CreateCode(4);
            Session["OrderValidateCode"] = validateStr;

            Response.ContentType = "image/jpeg";
            return new FileStreamResult(ImageValidate.CreateImage(validateStr), Response.ContentType);
        }

        [ZsfAuthorize(Roles = "9")]
        public ActionResult UserOderManage()
        {
            return View();
        }

        [ZsfAuthorize]
        [HttpPost]
        public ActionResult AddComment(FormCollection formCollection)
        {
            int shopId = int.Parse(formCollection.Get("ShopId"));
            string star = formCollection.Get("CommentStar");
            string payMoney = formCollection.Get("PayMoney");
            string deliveryTime = formCollection.Get("DeliveryTime");
            string content = formCollection.Get("CommentContent");
            string requestUrl = formCollection.Get("RequestUrl");


            UserModel userModel = new UserModel();
            ShopModel shopModel = new ShopModel();

            UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);

            ShopComment shopComment = new ShopComment();
            shopComment.ShopId = shopId;
            shopComment.UserId = userInfo.Id;
            shopComment.Stars = String.IsNullOrWhiteSpace(star) ? (short)0 : short.Parse(star);
            shopComment.PayMoney = String.IsNullOrWhiteSpace(payMoney) ? (short)0 : short.Parse(payMoney);
            shopComment.DeliveryTime = String.IsNullOrWhiteSpace(deliveryTime) ? (short)0 : short.Parse(deliveryTime);
            shopComment.Comment = content;
            shopComment.CreateTime = DateTime.Now;
            shopModel.Add(shopComment);

            short shopAvgStars = (short)Math.Round(shopModel.GetShopComments(shopComment.ShopId).Average(r => r.Stars));
            short shopAvgPayMoney = (short)Math.Round(shopModel.GetShopComments(shopComment.ShopId).Average(r => r.PayMoney));
            short shopAvgDeliveryTime = (short)Math.Round(shopModel.GetShopComments(shopComment.ShopId).Average(r => r.DeliveryTime));

            ShopRankingAttribute shopRankingAttr = shopModel.GetShopRankingAttribute(shopId);
            shopRankingAttr.Stars = shopAvgStars;
            shopRankingAttr.AveragePayMoney = shopAvgPayMoney;
            shopRankingAttr.DeliveryTime = shopAvgDeliveryTime;
            shopModel.Save();

            Shop shopInfo = shopModel.GetShop(shopId);
            userModel.AddUserPoint(userInfo.Id, "对店铺 <a href=\"/shop/detail/" + shopId + "\">" + shopInfo.Name + "</a> 发表评论", 2);
            userModel.AddUserMessage(userInfo.Id, "积分动态", "您因对店铺 <a href=\"/shop/detail/" + shopId + "\">" + shopInfo.Name + "</a>进行了评论而获得了" + 2 + "点积分。");

            return Redirect(requestUrl);
        }

        [HttpGet]
        public FileResult GetShopPhoneNumberPic()
        {
            string phoneNumber = "";
            if (Session["ShopDetailPhone"] != null)
                phoneNumber = Session["ShopDetailPhone"].ToString();

            Response.ContentType = "image/jpeg";
            return new FileStreamResult(ZsfProject.Tools.PhoneToImage.DrawImage(phoneNumber), Response.ContentType);
        }

        [ZsfAuthorize(Roles = "4,9")]
        public ActionResult Create(int i = 1)
        {
            ShopModel shopModel = new ShopModel();
            var shopCategories = shopModel.GetShopCategories();

            SelectList shopCategoryList = new SelectList(shopCategories, "Id", "Value");
            List<SelectListItem> shopCategorySelectList = shopCategoryList.ToList();
            ViewBag.ShopCategorySelectList = shopCategorySelectList;

            BaseDataModel baseDataModel = new BaseDataModel();
            var cityDistricts = baseDataModel.GetLocationsByParentId(214);
            ViewBag.CityDistrictSelectList = (new SelectList(cityDistricts, "Id", "Value")).ToList();

            ViewBag.ServiceAreaSelectList = new List<ServiceArea>();
            ViewBag.ShopDisheCategoryList = new List<SelectListItem>();

            ViewBag.ShopLogo = Url.Content("/Contents/ShopImages/shop_default_icon.png");
            ViewBag.OffsetTime = offsetTime;

            return View("Edit");
        }

        [HttpPost]
        [ZsfAuthorize(Roles = "4,9")]
        public ActionResult Create(string shopData)
        {
            ShopModel shopModel = new ShopModel();
            var jser = new System.Web.Script.Serialization.JavaScriptSerializer();
            ShopEditorObject shopDataObj = jser.Deserialize<ShopEditorObject>(shopData);
            ShopWithSAObject newShopInfo = shopDataObj.ShopBaseInfo;
            UserModel userModel = new UserModel();
            UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);

            Shop shopInfo = new Shop();
            shopInfo.Name = newShopInfo.Name;
            shopInfo.Address = newShopInfo.Address;
            shopInfo.PhoneNumber = newShopInfo.PhoneNumber;
            shopInfo.CategoryId = newShopInfo.CategoryId;
            shopInfo.OfficeTimeBegin = newShopInfo.OfficeTimeBegin;
            shopInfo.OfficeTimeEnd = newShopInfo.OfficeTimeEnd;
            shopInfo.UpSendPrice = newShopInfo.UpSendPrice;
            shopInfo.Remark = newShopInfo.Remark;
            shopInfo.Latitude = newShopInfo.Latitude;
            shopInfo.Longitude = newShopInfo.Longitude;
            shopInfo.CreateTime = DateTime.Now;
            shopInfo.LastModifyAt = shopInfo.CreateTime;
            shopInfo.CreateBy = userInfo.Id;
            shopInfo.LastModifyBy = shopInfo.CreateBy;
            shopInfo.Hidden = true;
            if (!String.IsNullOrWhiteSpace(newShopInfo.Logo))
            {
                string sourcePath = Server.MapPath(newShopInfo.Logo);
                string fileName = Path.GetFileName(sourcePath);
                if (!System.IO.File.Exists(Server.MapPath("~/Contents/ShopImages/") + fileName))
                    System.IO.File.Move(sourcePath, Server.MapPath("~/Contents/ShopImages/") + fileName);
                newShopInfo.Logo = fileName;
            }
            else
                newShopInfo.Logo = new Random().Next(1, 7) + ".jpg";
            shopInfo.Logo = newShopInfo.Logo;
            shopModel.Add(shopInfo);
            shopModel.Save();

            int shopId = shopInfo.Id;

            shopModel.DeleteShopServiceAreas(shopId);
            if (newShopInfo.ServiceAreas != null)
            {
                foreach (int saItem in newShopInfo.ServiceAreas)
                {
                    ShopServiceArea shopServiceArea = new ShopServiceArea();
                    shopServiceArea.ShopId = shopId;
                    shopServiceArea.AreaId = saItem;
                    shopModel.AddWithoutSave(shopServiceArea);
                }
                shopModel.Save();
            }
            try
            {
                shopModel.DeleteDisheCategories(shopId);
                shopModel.DeleteDishes(shopId);
                double totalPrice = 0;
                List<DisheWithCategoryDetailObject> disheWithCats = shopDataObj.DisheWithCategoryDetail;
                foreach(DisheWithCategoryDetailObject dwcItem in disheWithCats)
                {
                    ShopDisheCategory disheCategory = new ShopDisheCategory();
                    disheCategory.ShopId = shopId;
                    disheCategory.Value = dwcItem.CategoryName;
                    shopModel.Add(disheCategory);

                    foreach(ShopDishe disheItem in dwcItem.Dishes)
                    {
                        ShopDishe dishe = disheItem;
                        dishe.ShopId = shopId;
                        dishe.CategoryId = disheCategory.Id;
                        shopModel.AddWithoutSave(dishe);
                        totalPrice += dishe.Price;
                    }
                    shopModel.Save();
                }

                ShopRankingAttribute rankingAttribute = new ShopRankingAttribute();
                rankingAttribute.ShopId = shopId;
                rankingAttribute.Stars = 0;
                rankingAttribute.AveragePayMoney = (short)(totalPrice / disheWithCats.Select(r => r.Dishes.Count).Sum(r => r));
                rankingAttribute.DeliveryTime = 30;
                shopModel.Add(rankingAttribute);
            }
            catch(Exception ex)
            {
                return Json(new { status = 0, data = ex.InnerException }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = 1, url = "/shop/detail/" + shopId }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ZsfAuthorize(Roles = "4,9")]
        public ActionResult Edit(int id)
        {
            ShopModel shopModel = new ShopModel();
            Shop shopBaseInfo = shopModel.GetShop(id);
            UserModel userModel = new UserModel();
            UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
            if(userInfo.UserGradeCategory.GradeLevel != 9 && shopBaseInfo.CreateBy != userInfo.Id)
                return Redirect("/");
            List<V_ShopDisheWithCategory> shopDisheWithCategory = shopModel.GetShopDishesWithCategory(id).OrderByDescending(r => r.DisheCategoryOrder).ToList();

            var shopCategories = shopModel.GetShopCategories();

            SelectList shopCategoryList = new SelectList(shopCategories, "Id", "Value");
            List<SelectListItem> shopCategorySelectList = shopCategoryList.ToList();

            for(int i = 0; i < shopCategorySelectList.Count; i++)
                if(shopCategorySelectList[i].Value == shopBaseInfo.CategoryId.ToString())
                {
                    shopCategorySelectList[i].Selected = true;
                    break;
                }
            ViewBag.ShopCategorySelectList = shopCategorySelectList;


            BaseDataModel baseDataModel = new BaseDataModel();
            var cityDistricts = baseDataModel.GetLocationsByParentId(214);
            ViewBag.CityDistrictSelectList = (new SelectList(cityDistricts, "Id", "Value")).ToList();

            List<ServiceArea> shopServiceAraes = shopModel.GetShopServiceAreas(id).ToList()
                .Select(r => new ServiceArea() { Id = r.AreaId, ParentId = r.ParentId, Value = r.ServiceArea, AreaType = r.AreaType }).ToList();
            ViewBag.ServiceAreaSelectList = shopServiceAraes;

            ViewBag.ShopDisheCategoryList = shopDisheWithCategory.GroupBy(r => new { r.CategoryId, r.CategoryValue })
                .Select(r => new SelectListItem() { Value = r.Key.CategoryId.ToString(), Text = r.Key.CategoryValue }).ToList();

            ViewBag.ShopLogo = Url.Content(shopImagePath + shopBaseInfo.Logo);
            ViewBag.OffsetTime = offsetTime;
            return View(new ShopDetailEditorModel(shopBaseInfo, shopDisheWithCategory));
        }

        [HttpPost]
        [ZsfAuthorize(Roles = "4,9")]
        public ActionResult Edit(string shopData)
        {
            UserModel userModel = new UserModel();
            UserInfo userInfo = userModel.GetUserInfo(User.Identity.Name);
            ShopModel shopModel = new ShopModel();
            var jser = new System.Web.Script.Serialization.JavaScriptSerializer();
            ShopEditorObject shopDataObj = jser.Deserialize<ShopEditorObject>(shopData);
            ShopWithSAObject newShopInfo = shopDataObj.ShopBaseInfo;
            int shopId = newShopInfo.Id;

            Shop shopInfo = shopModel.GetShop(shopId);
            if(userInfo.UserGradeCategory.GradeLevel != 9 && userInfo.Id != shopInfo.CreateBy)
                return Redirect("/");
            shopInfo.Name = newShopInfo.Name;
            shopInfo.Address = newShopInfo.Address;
            shopInfo.PhoneNumber = newShopInfo.PhoneNumber;
            shopInfo.CategoryId = newShopInfo.CategoryId;
            shopInfo.OfficeTimeBegin = newShopInfo.OfficeTimeBegin;
            shopInfo.OfficeTimeEnd = newShopInfo.OfficeTimeEnd;
            shopInfo.UpSendPrice = newShopInfo.UpSendPrice;
            shopInfo.Remark = newShopInfo.Remark;
            shopInfo.Latitude = newShopInfo.Latitude;
            shopInfo.Longitude = newShopInfo.Longitude;
            shopInfo.LastModifyAt = DateTime.Now;
            shopInfo.LastModifyBy = userInfo.Id;

            if (!String.IsNullOrWhiteSpace(newShopInfo.Logo))
            {
                string sourcePath = Server.MapPath(newShopInfo.Logo);
                string fileName = Path.GetFileName(sourcePath);
                if (!System.IO.File.Exists(Server.MapPath("~/Contents/ShopImages/") + fileName))
                    System.IO.File.Move(sourcePath, Server.MapPath("~/Contents/ShopImages/") + fileName);
                newShopInfo.Logo = fileName;
            }
            else
                newShopInfo.Logo = new Random().Next(1, 7) + ".jpg";
            shopInfo.Logo = newShopInfo.Logo;
            shopModel.Save();

            ShopModifyLog editLog = new ShopModifyLog();
            editLog.ShopId = shopId;
            editLog.ModifyBy = userInfo.Id;
            editLog.ModifyAt = DateTime.Now;
            shopModel.Add(editLog);

            shopModel.DeleteShopServiceAreas(shopId);
            if (newShopInfo.ServiceAreas != null)
            {
                foreach (int saItem in newShopInfo.ServiceAreas)
                {
                    ShopServiceArea shopServiceArea = new ShopServiceArea();
                    shopServiceArea.ShopId = shopId;
                    shopServiceArea.AreaId = saItem;
                    shopModel.AddWithoutSave(shopServiceArea);
                }
                shopModel.Save();
            }

            shopModel.DeleteDisheCategories(shopId);
            shopModel.DeleteDishes(shopId);
            List<DisheWithCategoryDetailObject> disheWithCats = shopDataObj.DisheWithCategoryDetail;
            foreach (DisheWithCategoryDetailObject dwcItem in disheWithCats)
            {
                ShopDisheCategory disheCategory = new ShopDisheCategory();
                disheCategory.ShopId = shopId;
                disheCategory.Value = dwcItem.CategoryName;
                shopModel.Add(disheCategory);

                foreach (ShopDishe disheItem in dwcItem.Dishes)
                {
                    ShopDishe dishe = disheItem;
                    dishe.ShopId = shopId;
                    dishe.CategoryId = disheCategory.Id;
                    shopModel.AddWithoutSave(dishe);
                }
                shopModel.Save();
            }

            return Json(new { status = 1, url = "/shop/detail/" + shopId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ZsfAuthorize(Roles = "9")]
        public ActionResult CancelAllUserOrders()
        {
            ShopModel sm = new ShopModel();

            try
            {
                bool result = sm.CancelAllAvailableOrder();
            }
            catch(Exception ex)
            {
                return Json(new { status = 0, msg = "取消订单失败." + ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = 1, msg = "取消订单成功" }, JsonRequestBehavior.AllowGet);
        }

        [ZsfAuthorize(Roles = "9")]
        public ActionResult CreateNewUserOrder()
        {
            ShopModel sm = new ShopModel();
            UserModel um = new UserModel();

            int availableOrderId = sm.GetAvailableUserOrderId();

            if(availableOrderId != -1)
                return Json(new { status = 0, msg = availableOrderId }, JsonRequestBehavior.AllowGet);

            UserOrder order = new UserOrder();

            UserInfo userInfo = um.GetUserInfo(User.Identity.Name);
            order.Available = true;
            order.CreateBy = userInfo.Id;
            order.CreateTime = DateTime.Now;
            try
            {
                sm.Add(order);
            }
            catch(Exception ex)
            {
                return Json(new { status = 0, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = 1, msg = order.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadShopLogoPreview(HttpPostedFileBase fileData)
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
        public ActionResult CropShopLogo(string imgPath, float zoomRatio, float cropX, float cropY, int cropW, int cropH)
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
                        fileName = Guid.NewGuid().ToString() + Path.GetExtension(imgPath);
                        filePath = Path.GetDirectoryName(Server.MapPath(imgPath));
                        logoBit.Save(filePath + "\\" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }

            return Json(new { key = 1, value = "/Contents/Uploads/" + fileName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocations(int parentId)
        {
            BaseDataModel baseDataModel = new BaseDataModel();
            var result = baseDataModel.GetLocationsByParentId(parentId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetServiceAreas(int id)
        {
            BaseDataModel baseDataModel = new BaseDataModel();
            var result = baseDataModel.GetServiceAreasWithCircels(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int ToggleShopHidden(int shopId)
        {
            bool hidden = false;
            try
            {
                ShopModel shopModel = new ShopModel();
                Shop shop = shopModel.GetShop(shopId);
                hidden = shop.Hidden = shop.Hidden ? false : true;
                shopModel.Save();
            }
            catch
            {
                return 0;
            }
            return hidden ? 1 : 0;
        }
    }
}
