/// <reference path="../../Scripts/jquery-1.6.4.js" />
/// <reference path="../../Scripts/jQuery.tmpl.js" />
/// <reference path="../../Scripts/jquery.cookie.js" />

$(function () {
    var chat = $.connection.chat;

    function clearMessages() {
        $('#messages').html('');
    }

    function refreshMessages() { refreshList($('#messages')); }

    function clearUsers() {
        $('#users').html('');
    }

    function refreshUsers() { refreshList($('#users')); }

    function refreshList(list) {
        if (list.is('.ui-listview')) {
            list.listview('refresh');
        }
    }

    function addMessage(content, type) {
        var e = $('<li/>').html(content).appendTo($('#messages'));
        refreshMessages();

        if (type) {
            e.addClass(type);
        }
        updateUnread();
        e[0].scrollIntoView();
        return e;
    }

    chat.client.refreshRoom = function (room) {
        clearMessages();
        clearUsers();

        chat.server.getUsers()
            .done(function (users) {
                $.each(users, function () {
                    chat.client.addUser(this, true);
                });
                refreshUsers();

                $('#new-message').focus();
            });
    };

    chat.client.showRooms = function (rooms) {
        if (!rooms.length) {
            addMessage('没有该房间！', 'notification');
        }
        else {
            $.each(rooms, function () {
                addMessage(this.Name + ' (' + this.Count + ')');
            });
        }
    };

    chat.client.addMessageContent = function (id, content) {
        var e = $('#m-' + id).append(content);
        refreshMessages();
        updateUnread();
        e[0].scrollIntoView();
    };

    chat.client.addMessage = function (id, uId, name, message) {
        var data = {
            name: (uId == chat.state.id ? '我' : name),
            message: message,
            id: id
        };

        var e = $('#new-message-template').tmpl(data)
                                          .appendTo($('#messages'));
        refreshMessages();
        updateUnread();
        e[0].scrollIntoView();
    };

    chat.client.addUser = function (user, exists) {

        var id = 'u-' + user.Name;

        if (document.getElementById(id)) {
            return;
        }

        var data = {
            name: user.Name,
            hash: user.Hash
        };

        var tmp = $('#new-user-template').tmpl(data);
        if (user.Name == chat.state.name)
            tmp.css('color', 'red');
        var e = tmp.appendTo($('#users'));

        if (!exists && this.state.name != user.Name) {
            addMessage(user.Name + ' 进入了聊天室.', 'notification');
            e.hide().fadeIn('slow');
        }
        updateCookie();
    };

    chat.client.changeUserName = function (oldUser, newUser) {
        $('#u-' + oldUser.Name).replaceWith(
                $('#new-user-template').tmpl({
                    name: newUser.Name,
                    hash: newUser.Hash
                })
        );
        refreshUsers();

        if (newUser.Name === this.state.name) {
            addMessage('你的名字是 ' + newUser.Name, 'notification');
            updateCookie();
        }
        else {
            addMessage(oldUser.Name + '\'s 名字更换成 ' + newUser.Name, 'notification');
        }
    };

    /*chat.client.sendMeMessage = function (name, message) {
    addMessage('*' + name + '* ' + message, 'notification');
    };

    chat.client.sendPrivateMessage = function (from, to, message) {
    addMessage('<emp>*' + from + '*</emp> ' + message, 'pm');
    };*/

    chat.client.leave = function (user) {
        if (this.state.id != user.Id) {
            $('#u-' + user.Name).fadeOut('slow', function () {
                $(this).remove();
            });

            addMessage(user.Name + ' 离开了。', 'notification');
        }
    };

    $('#send-message').submit(function () {
        var command = $('#new-message').val();

        chat.server.send(command)
            .fail(function (e) {
                addMessage(e, 'error');
            });

        $('#new-message').val('');
        $('#new-message').focus();

        return false;
    });

    $(window).blur(function () {
        chat.state.focus = false;
    });

    $(window).focus(function () {
        chat.state.focus = true;
        chat.state.unread = 0;
        document.title = 'SignalR Chat';
    });

    function updateUnread() {
        if (!chat.state.focus) {
            if (!chat.state.unread) {
                chat.state.unread = 0;
            }
            chat.state.unread++;
        }
        updateTitle();
    }

    function updateTitle() {
        if (chat.state.unread == 0) {
            document.title = '聊天室';
        }
        else {
            document.title = '聊天室 (' + chat.state.unread + ')';
        }
    }

    function updateCookie() {
        $.cookie('userid', chat.state.id, { path: '/', expires: 1 });
        //$.cookie('username', chat.state.name, { path: '/', expires: 1 });
    }

    addMessage('欢迎来到聊天室，开放时间为 11:00~13:00', 'notification');

    $('#new-message').val('');
    $('#new-message').focus();
    var initNameTick = 0;
    //$.connection.hub.logging = true;
    $.connection.hub.error(function (msg) { console.error(msg); });
    $.connection.hub.start({ transport: activeTransport }, function () {
        chat.server.join()
            .done(function (success) {
                if (success === false) {
                    $.cookie('userid', '');
                }
                InitName();
            });

    });

    function InitName() {
        initNameTick++;
        var numReg = /(\d{5}$)/g;
        var redNum = '00000' + Math.round(Math.random() * 100000);

        chat.server.send('/name ZSF-' + redNum.match(numReg)[0])
                .fail(function (e) {
                    console.error(e);
                    if (e == 'nameused' && initNameTick < 5)
                        InitName();
                }).done(function () { JoinRoom('宁波大学'); });

    }

    function JoinRoom(room) {
        chat.server.send('/join ' + room)
        .fail(function (e) {
            console.error(e);
        });
    }
});