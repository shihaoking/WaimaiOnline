using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZsfData;

namespace ZsfProject.Models
{
    public class JobModel
    {
        ZsfEntities db = new ZsfEntities();
        public IQueryable<V_ShopJobAd> GetShopJobAds(int? shopId, int serviceAreaId, string userName,string jobName = "")
        {
            IQueryable<V_ShopJobAd> result;
            if(shopId != null)
                result = db.V_ShopJobAd.Where(r => r.ShopId == shopId.Value);
            else
            {
                if(!String.IsNullOrWhiteSpace(userName))
                {
                    UserModel userModel = new UserModel();
                    V_UserInfoDetail userInfo = userModel.GetUserInfoDetail(userName);
                    if(userInfo.GradeLevel >= 9)
                        result = db.V_ShopJobAd.Where(r => (r.PublishedByUserId == userInfo.Id || r.LastModifyByUserId == userInfo.Id));
                    else
                        result = db.V_ShopJobAd;
                }
                else
                    result = db.V_ShopJobAd;
            }
            return result.Where(r=> r.JobTitle.Contains(jobName) && r.AreaId == serviceAreaId).OrderByDescending(r => r.PublishedAt).ThenByDescending(r => r.OrderIndex);
        }

        public ShopJobAd GetShopJobAd(int id)
        {
            return db.ShopJobAd.FirstOrDefault(r => r.Id == id);
        }

        public void Add(ShopJobAd jobAds)
        {
            db.ShopJobAd.AddObject(jobAds);
            Save();
        }

        public void Add(ShopJobAdModifyLog modifyLog)
        {
            db.ShopJobAdModifyLog.AddObject(modifyLog);
            Save();
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}