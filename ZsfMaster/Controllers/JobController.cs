using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZsfData;
using ZsfProject.Models;
using ZsfProject.Tools;

namespace ZsfProject.Controllers
{
    public class JobController : Controller
    {
        //
        // GET: /Jobs/

        public ActionResult Index(int? id, int p = 1, int mine = 0, string jobTitle = "")
        {
            ViewBag.JobSearchName = jobTitle;
            JobModel jobModel = new JobModel();

            string serviceAreaId = "0";
            string serviceAreaName = "";
            UserModel.GetUserDefaultAreaSetting(ref serviceAreaId, ref serviceAreaName);

            var jobAds = jobModel.GetShopJobAds(id, int.Parse(serviceAreaId), mine == 0 ? "" : User.Identity.Name, jobTitle);

            if(id != null)
                ViewBag.ShopName = jobAds.Select(r => r.ShopName).FirstOrDefault();
            ViewBag.JobsCount = jobAds.Count();
            int pageSize = Request.Browser.IsMobileDevice ? 15 : 20;
            Paging.ToPaging(p, ViewBag.JobsCount, this, pageSize);
            jobAds = jobAds.Skip((p - 1) * pageSize).Take(pageSize);
            return View(jobAds);
        }

        [ZsfAuthorize(Roles = "6,9")]
        public ActionResult Create()
        {
            BaseDataModel baseDataModel = new BaseDataModel();
            var cityDistricts = baseDataModel.GetLocationsByParentId(214);
            ViewBag.CityDistrictSelectList = (new SelectList(cityDistricts, "Id", "Value")).ToList();
            ViewBag.ShopsSelectList = new List<SelectListItem>();
            return View("edit");
        }

        [HttpPost]
        [ZsfAuthorize(Roles = "6,9")]
        public ActionResult Create(ShopJobAd jobAd)
        {
            jobAd.PublishedAt = DateTime.Now;
            UserModel userModel = new UserModel();
            jobAd.PublishedBy = userModel.GetUserInfo(User.Identity.Name).Id;
            jobAd.LastModifyBy = jobAd.PublishedBy;
            jobAd.LastModifyAt = jobAd.PublishedAt;
            jobAd.DateDue = jobAd.PublishedAt.AddMonths(1);
            JobModel jobModel = new JobModel();
            jobModel.Add(jobAd);
            return RedirectToRoute("JobListOfShopOfMine", new { mine = 1 });
        }

        [ZsfAuthorize(Roles = "6,9")]
        public ActionResult Edit(int id)
        {
            JobModel jobModel = new JobModel();
            BaseDataModel baseDataModel = new BaseDataModel();

            ShopJobAd jobAd = jobModel.GetShopJobAd(id);

            var cityDistricts = baseDataModel.GetLocationsByParentId(214);
            ViewBag.CityDistrictSelectList = (new SelectList(cityDistricts, "Id", "Value")).ToList();

            List<SelectListItem> shopList = new List<SelectListItem>();
            shopList.Add(new SelectListItem { Value = jobAd.ShopId.ToString(), Text = jobAd.Shop.Name, Selected = true });
            ViewBag.ShopsSelectList = shopList;

            return View(jobAd);
        }

        [HttpPost]
        [ZsfAuthorize(Roles = "6,9")]
        public ActionResult Edit(ShopJobAd jobAd)
        {
            UserModel userModel = new UserModel();
            int userId = userModel.GetUserInfo(User.Identity.Name).Id;

            JobModel jobModel = new JobModel();
            ShopJobAd shopJobAd = jobModel.GetShopJobAd(jobAd.Id);
            shopJobAd.JobTitle = jobAd.JobTitle;
            shopJobAd.ShopId = jobAd.ShopId;
            shopJobAd.LastModifyAt = DateTime.Now;
            shopJobAd.LastModifyBy = userId;
            shopJobAd.Description = jobAd.Description;
            shopJobAd.ContactPhone = jobAd.ContactPhone;
            shopJobAd.OrderIndex = jobAd.OrderIndex;
            shopJobAd.WorkPlace = jobAd.WorkPlace;
            shopJobAd.SalaryMin = jobAd.SalaryMin;
            shopJobAd.SalaryMax = jobAd.SalaryMin;
            shopJobAd.JobType = jobAd.JobType;
            jobModel.Save();

            ShopJobAdModifyLog modifyLog = new ShopJobAdModifyLog();
            modifyLog.JobId = jobAd.Id;
            modifyLog.ModifyAt = shopJobAd.LastModifyAt;
            modifyLog.ModifyBy = shopJobAd.LastModifyBy;
            jobModel.Add(modifyLog);
            return RedirectToRoute("JobListOfShopOfMine", new { mine = 1 });
        }

        [HttpPost]
        public ActionResult GetShopNamesByServiceArea(int id)
        {
            ShopModel shopModel = new ShopModel();
            var shops = shopModel.GetShopsList(id, true).Select(r => new { r.Id, r.Name }).ToList();
            return Json(shops);
        }
    }
}
