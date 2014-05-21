var orderList = new Array();
var totalOrderPrice = 0;

window.onscroll = function () {
    MoveLayout();
}

function MoveLayout() {
    if (document.documentElement.scrollTop >= 200) {
        if (navigator.userAgent.indexOf("MSIE 6") > 0) {
            $('.lf .order').css({ 'top': (document.documentElement.scrollTop - 135) + 'px', 'position': 'absolute' });
        }
        else
            $('.lf .order').css('position', 'fixed');
    }
    else if (document.body.scrollTop >= 145) {
        $('.lf .order').css({ 'position': 'fixed' });
    }
    else
        $('.lf .order').css({ 'position': 'relative',top:0 });

    if (document.body.scrollTop >= 584)
        $('.show_lb').css({ 'position': 'fixed', 'width': $('.show_lb').width() + 'px' }).addClass('bg');
    else if (document.documentElement.scrollTop >= 584) {
        if (navigator.userAgent.indexOf("MSIE 6") > 0)
            $('.show_lb').css({ 'position': 'absolute', 'top': document.documentElement.scrollTop - 135 + 'px', 'width': $('.show_lb').width() + 'px' }).addClass('bg');
        else
            $('.show_lb').css({ 'position': 'fixed', 'width': $('.show_lb').width() + 'px' }).addClass('bg');
    }
    else
        $('.show_lb').css({ 'position': 'relative', 'top': 0 }).removeClass('bg');
}

function AddFavoriteShop(e, shopId) {
    $.post('/user/AddFavoriteShop', { id: shopId }, function (result) {
        if (result.type == 1) {
            $(e).removeClass('fav_off').addClass('fav_on').text('已收藏')
            .bind('hover', function () { $(this).text('取消收藏'); });
        }
        else if (result.type == 0)
            alert("添加失败，请稍后重试！");
        else if (result.type == 2) {
            ShowUserLoginDialog();
        }
    });
}

function CancelFavoriteShop() {
    $.post('/user/CancelFavoriteShop', { id: shopId }, function (result) {
        if (result.type == 1)
            $(e).removeClass('fav_on').addClass('fav_off').text('添加收藏').unbind('hover');
    });
}

function ShareShop() {
    document.cookie = "shopviewcount=0";
    var dayDate = new Date();
    dayDate.setDate(dayDate.getDate() + 1);
    //sharedshop表示今天已经分享过店铺
    document.cookie = "sharedshop=true;expires=" + dayDate.toLocaleString();
}

function OrderList(id, price) {
    this.id = id;
    this.price = price;
}

function DeleteOrder(e) {
    var $parent = $(e).parents('li');
    var id = orderList[$parent.attr('index')].id;
    $buyOrder = $('<a href="javascript:void(0)" class="buy"></a>').click(function () { AddOrder(this); })
    $('#' + id + ' span.check_mark').removeClass('check_mark').before($buyOrder);
    $parent.remove();
}

function CheckInputForNumber() {
    if ((event.keyCode > 47 && event.keyCode < 58) || (event.keyCode > 95 && event.keyCode < 106) || event.keyCode == 8) {
        event.returnValue = true;
    } else
        event.returnValue = false;
}

function CreateNewOrder(orderIndex, id, name, price) {
    var $newOrder = $('<li index="' + orderIndex + '"><span class="n">'
            + name + '</span><div class="dsh_dt"><span class="l" title="减一份"></span><span class="c">1</span>'
            + '<span class="m" title="加一份"></span><span>份</span><span class="d" title="删除"></span><span class="p">'
            + price + '元</span><div class="clear"></div></div><div class="clear"></div></li>');

    $newOrder.find('.l').click(function () {
        var $parent = $(this).parents('li');
        var count = $(this).next().html();
        var price = orderList[$parent.attr('index')].price;

        if (count == 1) {
            DeleteOrder(this);
        }
        else {
            $(this).next().html(--count);

            $parent.find('.p').html(price * count + '元');
        }

        totalOrderPrice -= parseInt(price);
        OrderPriceChange(totalOrderPrice);
    });

    $newOrder.find('.m').click(function () {
        var $parent = $(this).parents('li');
        var count = $(this).prev().html();
        $(this).prev().html(++count);
        var price = orderList[$parent.attr('index')].price;
        $parent.find('.p').html(price * count + '元');

        totalOrderPrice += parseInt(price);
        OrderPriceChange(totalOrderPrice);
    });

    $newOrder.find('.d').click(function () {
        var $parent = $(this).parents('li');
        var count = $parent.find('.c').html();
        var price = orderList[$parent.attr('index')].price;
        DeleteOrder(this);

        totalOrderPrice -= parseInt(count * price);
        OrderPriceChange(totalOrderPrice);
    });

    return $newOrder;
}

function AddOrder(e) {
    var $parent = $(e).parent();
    var id = $parent.attr('id');
    var name = $parent.attr('name');
    var price = $parent.attr('price');


    var newOrder = new OrderList(id, price);
    orderList.push(newOrder);

    $newOrder = new CreateNewOrder(orderList.length - 1, id, name, price);
    $('.order_ls').append($newOrder);

    totalOrderPrice += parseInt(price);
    OrderPriceChange(totalOrderPrice);
    
    $(e).next().addClass('check_mark');
    $(e).remove();
}

function OrderPriceChange(p) {
    $('.order_tprice span.c').html('<strong>' + p + '</strong>元');
}

function ToggleMenuContent(e) {
    if ($(e).hasClass('now'))
        return false;


    var target = $(e).attr('target');
    $('.sh_m').children('div.' + target).prevAll().hide();
    $('.sh_m').children('div.' + target).show().nextAll().show();

    $('a.menu.now').removeClass('now');
    $(e).addClass('now');
    MoveLayout();
    return false;
}

$(document).ready(function () {//$jquery ready1
    var nowUrl = window.location.href;
    var $d_more_info = $('<div class="d_more_info"></div>');
    if(nowUrl.indexOf('#show_lb') > 0) {
        ToggleMenuContent('.menu[target="sh_cmt"]');
    }

    $('a.menu').click(function () {
        return ToggleMenuContent(this);
    });

    $('.dishe_cat').click(function () {
        if($(this).hasClass('now'))
            return false;

        $('.dishe_cat.now').removeClass('now');
        $(this).addClass('now');

        var cid = $(this).attr('cid');

        if(cid == -1)
            $('ul.dishe_ls').show();
        else {
            $('ul.dishe_ls[cid=' + cid + ']').show();
            $('ul.dishe_ls[cid!=' + cid + ']').hide();
        }
        return false;
    });

    $('a.buy').click(function () {
        AddOrder(this);
        return false;
    });

    $('li.dishe_i').hover(function () {
        if($(this).find('.buy').length != 0)
            $(this).addClass('dh_hover').find('.buy').html('来一份');

        var $more_info = $d_more_info.clone();
        var leaveBottom = 0;
        if($(this).find('.d_gift').length != 0) {
            $more_info.append('<div class="gift">' + $(this).attr('gift').trim() + '</div>');
            leaveBottom += 36;
        }
        if($(this).find('.d_remark').length != 0) {
            $more_info.append('<div class="remark">' + $(this).attr('remark').trim() + '</div>');
            leaveBottom += 36;
        }
        $more_info.css({ top: '-' + leaveBottom + 'px' }).appendTo(this);

    }, function () {
        $(this).removeClass('dh_hover').find('.buy').html('');
        $(this).find('.d_more_info').remove();
    });

    $('#cmt_star').rate({
        onClick: function (stars) { $('#CommentStar').val(stars); }
    });

    $('#cmtForm').submit(function () {
        if($('#CommentStar').val() == '' || $('#CommentStar').val() == 0) {
            alert('请 "评分"');
            return false;
        }

        if($('#PayMoney').val() == '') {
            alert('请填写 "人均消费"');
            return false;
        }

        if($('#DeliveryTime').val() == '') {
            alert('请填写 "送餐速度"');
            return false;
        }

        if($('#CommentContent').val() == '') {
            alert('请填写 "评论内容"');
            return false;
        }
    });
});