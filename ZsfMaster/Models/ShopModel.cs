using System.Linq;
using ZsfData;
using System.Collections.Generic;

namespace ZsfProject.Models
{
    public class ShopsListModel
    {

        public IQueryable<ShopCategoryWithShopStatistic> ShopCategoriesList { get; private set; }
        public IQueryable<V_ShopForList> ShopsList { get; private set; }
        public List<V_UserFavoriteShopForList> FavoriteShopList { get; set; }
        public ShopsListModel(IQueryable<ShopCategoryWithShopStatistic> shopCategoryies,
            IQueryable<V_ShopForList> shops, List<V_UserFavoriteShopForList> favoriteShops)
        {
            this.ShopCategoriesList = shopCategoryies;
            this.ShopsList = shops;
            this.FavoriteShopList = favoriteShops;
        }
    }
    public class ShopDetailEditorModel
    {
        public Shop ShopInfo { get; set; }
        public List<V_ShopDisheWithCategory> ShopDishesList { get; set; }
        public ShopDetailEditorModel(Shop shopInfo, List<V_ShopDisheWithCategory> disheWithCategory)
        {
            ShopInfo = shopInfo;
            ShopDishesList = disheWithCategory;
        }
    }

    public class ShopDetailModel
    {
        public V_ShopDetail ShopInfo { get; private set; }
        public IQueryable<V_ShopServiceArea> ShopServiceAreasList { get; set; }
        public IQueryable<V_ShopDisheWithCategory> ShopDishesList { get; private set; }
        public IQueryable<V_ShopComment> ShopComments { get; private set; }

        public ShopDetailModel(V_ShopDetail shopInfo, IQueryable<V_ShopServiceArea> shopServiceAreas, 
            IQueryable<V_ShopDisheWithCategory> shopDishesList, IQueryable<V_ShopComment> shopComments)
        {
            this.ShopInfo = shopInfo;
            this.ShopServiceAreasList = shopServiceAreas;
            this.ShopDishesList = shopDishesList;
            this.ShopComments = shopComments;
        }
    }

    public class ShopCategoryWithShopStatistic
    {
        public int CategoryId { get; set; }
        public string CategoryValue { get; set; }
        public int CoverShopsCount { get; set; }
    }

    public class ShopDisheJson
    {
        public int CategoryId { get; set; }
        public DisheJosn[] Dishes { get; set; }
    }

    public class DisheJosn
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public class DisheWithCategoryDetailObject
    {
        public string CategoryName { get; set; }
        public List<ShopDishe> Dishes { get; set; }
    }
    public class ShopWithSAObject : Shop
    {
        public List<int> ServiceAreas { get; set; }
    }
    public class ShopEditorObject
    {
        public ShopWithSAObject ShopBaseInfo { get; set; }
        public List<DisheWithCategoryDetailObject> DisheWithCategoryDetail { get; set; }
    }

    public class OrderDishe
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public float Price { set; get; }
        public short Count { set; get; }
    }

    public class Order
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public List<OrderDishe> Dishes { get; set; }
    }

    public class ShopModel
    {
        ZsfEntities db = new ZsfEntities();

        public IQueryable<V_ShopForList> GetShopsWithServiceArea()
        {
            return db.V_ShopForList.OrderBy(r => r.Name);
        }

        public IQueryable<Shop> GetShops(string filter = "")
        {
            if(string.IsNullOrWhiteSpace(filter))
                return db.Shop;
            else
                return db.Shop.Where(r => r.Name.Contains(filter));
        }

        public IQueryable<V_ShopForList> GetShopsList(int serviceAreaId, bool showHidden = false)
        {
            if(showHidden)
                return db.V_ShopForList.Where(r => r.AreaId == serviceAreaId).OrderBy(r => r.Name);
            else
                return db.V_ShopForList.Where(r => r.AreaId == serviceAreaId && r.Hidden == false).OrderBy(r => r.Name);
        }

        public IQueryable<V_UserFavoriteShopForList> GetFavoriteShopsList(int userId)
        {
            return db.V_UserFavoriteShopForList.Where(r => r.UserId == userId).OrderBy(r => r.FavoriteCreateTime);
        }

        public IQueryable<V_ShopForList> GetShopsList(string shopName, int serviceAreaId)
        {
            return db.V_ShopForList.Where(r => r.Name.Contains(shopName) && r.AreaId == serviceAreaId && r.Hidden == false).OrderBy(r => r.Name);
        }

        public Shop GetShop(int id)
        {
            return db.Shop.FirstOrDefault(r => r.Id == id);
        }

        public V_ShopDetail GetShopDetail(int id)
        {
            return db.V_ShopDetail.FirstOrDefault(r => r.Id == id);
        }

        public ShopRankingAttribute GetShopRankingAttribute(int shopId)
        {
            return db.ShopRankingAttribute.FirstOrDefault(r => r.ShopId == shopId);
        }

        public IQueryable<ShopCategory> GetShopCategories()
        {
            return db.ShopCategory;
        }

        public IQueryable<Location> GetProvinces()
        {
            return db.Location.Where(r => r.ParentId == 0);
        }

        public IQueryable<Location> GetCities(int provinceId)
        {
            return db.Location.Where(r => r.ParentId == provinceId);
        }

        public IQueryable<ServiceArea> GetServiceAreas()
        {
            return db.ServiceArea.OrderBy(r => r.Value);
        }

        public IQueryable<ShopCategoryWithShopStatistic> GetShopCategoryStatistic(IQueryable<V_ShopForList> shopEntity)
        {
            return shopEntity.GroupBy(r => new { r.CategoryId, r.Category })
            .Select(r => new ShopCategoryWithShopStatistic() { CategoryId = r.Key.CategoryId, CategoryValue = r.Key.Category, CoverShopsCount = r.Count() })
            .OrderBy(r => r.CategoryId);
        }

        public IQueryable<V_ShopServiceArea> GetShopServiceAreas(int shopId)
        {
            return db.V_ShopServiceArea.Where(r => r.ShopId == shopId);
        }

        public IQueryable<ShopComment> GetShopComments(int shopId)
        {
            return db.ShopComment.Where(r => r.ShopId == shopId);
        }

        public ShopComment GetShopComment(int shopId)
        {
            return db.ShopComment.FirstOrDefault(r => r.ShopId == shopId);
        }

        public IQueryable<ShopDisheCategory> GetShopDisheCategories(int shopId)
        {
            return db.ShopDisheCategory.Where(r => r.ShopId == shopId);
        }

        public IQueryable<V_ShopDisheWithCategory> GetShopDishesWithCategory(int shopId)
        {
            return db.V_ShopDisheWithCategory.Where(r => r.ShopId == shopId);
        }

        public IQueryable<V_ShopComment> GetShopCommentsWithUserInfo(int shopId)
        {
            return db.V_ShopComment.Where(r => r.ShopId == shopId).OrderByDescending(r => r.CreateTime);
        }

        public IQueryable<ShopServiceArea> GetShopSerivceArea(int shopId)
        {
            return db.ShopServiceArea.Where(r => r.ShopId == shopId);
        }

        public bool CancelAllAvailableOrder()
        {
            string sql = @"Update UserOrder Set Available = 0";

            int result = db.ExecuteStoreCommand(sql, null);

            return (result > 0);
        }

        public int GetAvailableUserOrderId()
        {
            var result = db.UserOrder.Where(r => r.Available);
            if(result.Count() > 0)
                return result.Max(r => r.Id);
            else
                return -1;
        }

        public UserOrderDetail GetUserOrderDetail(int orderId)
        {
            return db.UserOrderDetail.FirstOrDefault(r => r.Id == orderId);
        }


        public void DeleteShopServiceAreas(int shopId)
        {
            db.ExecuteStoreCommand("DELETE ShopServiceArea where ShopId={0}", shopId);
        }

        public void DeleteDisheCategories(int shopId)
        {
            db.ExecuteStoreCommand("DELETE ShopDisheCategory where ShopId={0}", shopId);
        }

        public void DeleteDishes(int shopId)
        {
            db.ExecuteStoreCommand("DELETE ShopDishe where ShopId={0}", shopId);
        }

        public void RemoveUserOrderDetail(UserOrderDetail orderDetail)
        {
            db.UserOrderDetail.DeleteObject(orderDetail);
            Save();
        }

        public void Add(UserOrderDetail orderDetail)
        {
            db.UserOrderDetail.AddObject(orderDetail);
            Save();
        }

        public void Add(ShopComment shopComment)
        {
            db.ShopComment.AddObject(shopComment);
            Save();
        }

        public void Add(ShopDisheCategory disheCategory)
        {
            db.ShopDisheCategory.AddObject(disheCategory);
            Save();
        }

        public void AddWithoutSave(ShopServiceArea shopServiceArea)
        {
            db.ShopServiceArea.AddObject(shopServiceArea);
        }

        public void AddWithoutSave(ShopDisheCategory shopDisheCategory)
        {
            db.ShopDisheCategory.AddObject(shopDisheCategory);
        }

        public void AddWithoutSave(ShopDishe shopDishe)
        {
            db.ShopDishe.AddObject(shopDishe);
        }

        public void Add(UserOrder order)
        {
            db.UserOrder.AddObject(order);
            Save();
        }

        public void Add(Shop shop)
        {
            db.Shop.AddObject(shop);
            Save();
        }

        public void Add(ShopRankingAttribute rankingAttr)
        {
            db.ShopRankingAttribute.AddObject(rankingAttr);
            Save();
        }

        public void Add(ShopModifyLog modifyLog)
        {
            db.ShopModifyLog.AddObject(modifyLog);
            Save();
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}