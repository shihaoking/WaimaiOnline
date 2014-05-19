var emailReg = /^(\w)+(\.\w+)*@[\w\d]+((\.\w+)+)$/;
var pwdReg = /^\d+$|^[a-z|A-Z]+$/;
var $userNameInput = $('#UserName');
var $emailInput = $('#Email');
var $pwdInput = $('#Password');
var $valInput = $('#ValidateCode');
var valueChecked = true;

function CheckUserInfo(type, value) {
    valueChecked = false;
    $.ajax({
        type: 'post',
        url: 'CheckUserInfo',
        data: { 'type': type, 'value': value },
        dataType: 'json',
        success: function (result) {
            $('.' + type + '_error').html(result.value);
            valueChecked = true;
        }
    });
}

function CheckFormData() {
    var email = $emailInput.val().trim();

    if (email == '' || !emailReg.test(email))
        $('.email_error').html('邮箱格式不正确');

    var userName = $userNameInput.val().trim();
    if (userName == '')
        $('.name_error').html('用户名不能为空');

    var pwd = $pwdInput.val();
    if (pwd == '' || pwd.length < 6 || pwd.length > 15)
        $('.pwd_error').html('密码长度必须是6~15位字符');
    else if(pwdReg.test(pwd))
        $('.pwd_error').html('密码必须同时包含数字、字母或字符');


    if ($valInput.val() == '')
        $('#valid_error').html('验证码不能为空');
    else
        $('#valid_error').html('');
}

$(document).ready(function () { //jquery ready 1

    $emailInput.blur(function () { //Email blur
        var email = $(this).val().trim();

        if (email == '')
            return;
        else if (!emailReg.test(email)) {
            $('.email_error').html('邮箱格式不正确');
            return;
        }

        CheckUserInfo('email', email);
    }); //Email blur

    $userNameInput.blur(function () { //UserName blur
        var userName = $(this).val().trim();

        if (userName == '')
            return;

        CheckUserInfo('name', userName);

    }); //UserName blur

    $pwdInput.blur(function () {
        var pwd = $(this).val();
        $('.pwd_error').html('');

        if (pwd == '')
            return;
        else if(pwd.length < 6 || pwd.length > 15)
            $('.pwd_error').html('密码长度必须是6~15位字符');
        else if (pwdReg.test(pwd))
            $('.pwd_error').html('密码必须同时包含字母、数字');

    });


    $valInput.blur(function () {
        var vald = $(this).val();

        if (vald != '')
            $('#valid_error').html('');

    });


    $('#reg_form').submit(function () {  //reg_form submit

        CheckFormData();

        if (!valueChecked || $('.email_error').html() != '' || $('.name_error').html() != ''
            || $('.pwd_error').html() != '' || $('#valid_error').html() != '') {
            return false;
        }

    }); //reg_form submit
});         //jquery ready 1