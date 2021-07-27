// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function fillPassword() {
    var pwd = generatePassword();
    $('#pwdInput').val(pwd);
}

function generatePassword() {
    var charset = "abcdefghijklmnopqrstuvwxyz0123456789";
    var retVal = "";
    var lenght = 6;
    for (var i = 0, n = charset.length; i < lenght; ++i) {
        retVal = retVal + charset.charAt(Math.floor(Math.random() * n));
    }
    return retVal;
}

function copyCreds() {
    var login = $('#UserNameInput').val();
    var pwd = $('#pwdInput').val();
    var creds = "Login: " + login + "\nPassword: " + pwd;

    var $input = $('#clipboadText');
    $input.val(creds).select();
    document.execCommand("copy");
    $input.val('');
}

function resetPassword() {
    var username = $('#UserNameInput').val();
    var pwd = $('#pwdInput').val();
    $.ajax({
        type: "GET",
        url: "/Account/ResetPassword/",
        data: {
            username: username,
            password: pwd
        },
        success: function (jsReturnArgs) {

            if (jsReturnArgs.Status === 400) {
                alert("Error.");
            } else {
                $('#infoSpan').text('Password changed');
            }
        },
        error: function (errorData) { alert(errorData); }
    });
}

(function () {
    if (typeof Object.defineProperty === 'function') {
        try { Object.defineProperty(Array.prototype, 'sortBy', { value: sb }); } catch (e) { }
    }
    if (!Array.prototype.sortBy) Array.prototype.sortBy = sb;

    function sb(f) {
        for (var i = this.length; i;) {
            var o = this[--i];
            this[i] = [].concat(f.call(o, o, i), o);
        }
        this.sort(function (a, b) {
            for (var i = 0, len = a.length; i < len; ++i) {
                if (a[i] != b[i]) return a[i] < b[i] ? -1 : 1;
            }
            return 0;
        });
        for (var i = this.length; i;) {
            this[--i] = this[i][this[i].length - 1];
        }
        return this;
    }
})();