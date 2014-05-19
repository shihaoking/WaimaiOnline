using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using ZsfData;
using ZsfProject.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ZsfMaster
{
    public class ShopOrderHub : Hub
    {
        private static readonly ConcurrentDictionary<int, ClientInfo> _ClientInfos = new ConcurrentDictionary<int, ClientInfo>();
        private static List<string> _Managers = new List<string>();

        public void Join(int userId, int shopId)
        {
            List<OrderDetail> orders = new List<OrderDetail>();
            ShopModel sm = new ShopModel();
            UserModel um = new UserModel();

            UserInfo userInfo = um.GetUserInfo(userId);
            short userGradeLevel = userInfo.UserGradeCategory.GradeLevel;

            ClientInfo clientInfo = new ClientInfo() 
            { ConnectionId = Context.ConnectionId, UserId = userId, UserName = userInfo.Name, GradeLevel = userGradeLevel };

            if (_ClientInfos.Any(r => r.Key == userId))
            {
                _ClientInfos[userId] = clientInfo;
            }
            else
            {
                _ClientInfos[userId] = clientInfo;
            }

            if (_Managers.Any(r => r == Context.ConnectionId))
            {
                if (userGradeLevel != 9)
                {
                    _Managers.Remove(Context.ConnectionId);
                }
            }
            else
            {
                if (userGradeLevel == 9)
                {
                    _Managers.Add(Context.ConnectionId);
                }
            }

            orders = sm.GetAvailableUserOrderDetail(shopId)
                .Select(r => new OrderDetail()
                {
                    Id = r.Id,
                    DisheId = r.DisheId,
                    Count = r.OrderCount,
                    Price = r.Price,
                    Available = (r.UserId == userId || userGradeLevel == 9) ? 1 : 0
                }).ToList<OrderDetail>();

            Clients.Caller.broadcastOrderAdd(orders);
        }

        public void AddNewOrderDetail(int userId, OrderDetail orderDetail)
        {
            UserOrderDetail order = new UserOrderDetail();
            ShopModel sm = new ShopModel();
            int orderId = sm.GetAvailableUserOrderId();

            try
            {
                UserModel um = new UserModel();
                UserInfo userInfo = um.GetUserInfo(userId);

                order.OrderId = orderId;
                order.DisheId = orderDetail.DisheId;
                order.ShopId = orderDetail.ShopId;
                order.UserId = userInfo.Id;
                order.Price = orderDetail.Price;
                order.OrderCount = orderDetail.Count;
                order.CreateDate = DateTime.Now;

                sm.Add(order);

                orderDetail.Id = order.Id;

                orderDetail.Available = 1;
            }
            catch (Exception ex)
            {
                
            }

            orderDetail.Available = 1;
            List<string> managers = _ClientInfos.Where(r => r.Value.GradeLevel == 9 || r.Key == userId)
                .Select(r => r.Value.ConnectionId).ToList();

            Clients.Clients(managers).broadcastOrderAdd(new List<OrderDetail> { orderDetail });
            
            orderDetail.Available = 0;
            Clients.AllExcept(managers.ToArray()).broadcastOrderAdd(new List<OrderDetail> { orderDetail });
        }

        public void DeleteOrderDetail(int orderId)
        {
            ShopModel sm = new ShopModel();

            UserOrderDetail orderDetail = sm.GetUserOrderDetail(orderId);
            sm.RemoveUserOrderDetail(orderDetail);

            Clients.All.broadcastOrderDelete(orderId);

        }

        public void ChangeOrderDetail(int orderId, short count)
        {
            ShopModel sm = new ShopModel();
            UserOrderDetail orderDetail = sm.GetUserOrderDetail(orderId);
            orderDetail.OrderCount = count;
            sm.Save();

            Clients.All.broadcastOrderChange(orderId, count);
        }

        public override Task OnDisconnected()
        {
            ClientInfo clientInfo = _ClientInfos.Values.FirstOrDefault(r => r.ConnectionId == Context.ConnectionId);

            if (clientInfo != null)
            {
                ClientInfo removedClient;
                _ClientInfos.TryRemove(clientInfo.UserId, out removedClient);
            }

            if (_Managers.Any(r => r == Context.ConnectionId))
            {
                _Managers.Remove(Context.ConnectionId);
            }

            return null;
        }
    }

    public class OrderDetail
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public int DisheId { get; set; }
        public short Price { get; set; }
        public short Count { get; set; }
        public int Available { get; set; }
    }

    public class ClientInfo
    {
        public string ConnectionId { get; set; }
        public int UserId { get; set; }
        public short GradeLevel { get; set; }
        public string UserName { get; set; }
    }
}