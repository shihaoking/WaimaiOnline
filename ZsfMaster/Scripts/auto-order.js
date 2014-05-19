var orderHub = $.connection.shopOrderHub;

orderHub.client.broadcastOrderAdd = function (orders) {
    $.each(orders, function (i, order) {
        if (order) {
            AddOrderLayout(order.ShopId, order.DisheId, order.Price, order.Count, order.Id, order.Available);
        }
    });
}

orderHub.client.broadcastOrderDelete = function (orderId) {
    //移除已删除的单子
    var order;
    $.each(orderList.dishes, function (i, d) {
        if (d && d.orderDetailId == orderId) {
            order = d;
            return false;
        }
    });

    DeleteOrder($('.order_ls li[orderid=' + order.orderDetailId + '] span'), order.count, order.price);
}

orderHub.client.broadcastOrderChange = function (orderId, disheCount) {
    $.each(orderList.dishes, function (i, d) {
        if (d && d.orderDetailId == orderId) {

            var dCount = disheCount - d.count;

            if (dCount != 0) {
                var $target = $('.order_ls li[orderid=' + d.orderDetailId + ']');
                $target.find('span.c').html(disheCount);
                var price = orderList.dishes[$target.attr('index')].price;
                $target.find('.p').html(price * disheCount + '元');

                orderList.dishes[$target.attr('index')].count = disheCount;
                totalOrderPrice += dCount * price;
                OrderPriceChange(totalOrderPrice);
            }

            return false;
        }
    });
}

$.connection.hub.start({ transport: ['webSockets', 'longPolling'] }).done(function () {
    var name = $('#UserId').val();
    var shopId = $('#ShopId').val();
    orderHub.server.join(name, shopId);
});

