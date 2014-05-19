using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZsfData;

namespace ZsfProject.Models
{
    public class BaseDataModel
    {
        ZsfEntities db = new ZsfEntities();

        public static readonly string CookieDomain = System.Configuration.ConfigurationManager.AppSettings["CookieDomain"]; 

        public IQueryable<ServiceArea> GetServiceAreas()
        {
            return db.ServiceArea;
        }

        public ServiceArea GetServiceArea(int areaId)
        {
            return db.ServiceArea.FirstOrDefault(r => r.Id == areaId);
        }

        public IQueryable<Location> GetLocationsByParentId(int parentId)
        {
            return db.Location.Where(r => r.ParentId == parentId).OrderBy(r => r.Value);
        }

        public Location GetLocationById(int id)
        {
            return db.Location.FirstOrDefault(r => r.Id == id);
        }

        public List<Location> GetServiceAreasWithCircels(int areaId)
        {
            var allAreas = db.ServiceArea.Where(r => r.ParentId == areaId).ToList();
            var areas = allAreas.Where(r => r.AreaType == "U").Select(r => new Location() { Id = r.Id, ParentId = r.ParentId, Value = r.Value }).ToList();
            var businessCircles = allAreas.Where(r => r.AreaType == "B").Select(r => new Location() { Id = r.Id, ParentId = r.ParentId, Value = r.Value });
            var offices = (from a in businessCircles
                           join b in db.ServiceArea
                           on a.Id equals b.ParentId
                           select new Location() { Id = b.Id, ParentId = b.ParentId, Value = b.Value + " (" + a.Value + ")" }).ToList();
            areas.AddRange(offices);
            return areas;
        }
    }
}