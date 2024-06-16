var url = window.location.pathname;
var queryString = window.location.search;
var sortName = "";
var selectStatus = "";
window.addEventListener('load', function () {
    load(1, 10, "", "", "");

});

function load(pageIndex, pageSize, searchName, sortName, selectStatus) {

    $.ajax({
        url: url,
        type: 'Post',
        data: {
            pageIndex: pageIndex, pageSize: pageSize,
            searchName: searchName, sortName: sortName, selectStatus: selectStatus
        },
        success: function (response) {
            var accounts = response.accounts;

            if (accounts.length > 0) {
                $("#list-users").empty();
                $('#no_data').empty();
                $('.table-title').css('display', '');
                $('#list-users').css('display', '');
                $('#table-content').addClass('wg-table');
                var html = "";
                if (response.userType == 2) {
                    for (var i = 0; i < accounts.length; i++) {
                        html += elementHtmlEmployee(accounts[i]);
                    }
                } else {
                    for (var i = 0; i < accounts.length; i++) {
                        html += elementHtmlCustomer(accounts[i]);
                    }
                }
                $("#list-users").html(html);
                paging(response.currentPage, response.numberPage, response.pageSize);
                console.log("data: ", response);
            } else {
                $('#pagination').empty();
                $('.table-title').css("display", "none");
                $('#list-users').css("display", "none");
                $('#table-content').removeClass('wg-table');
                $('#no_data').empty();
                $('#table-content').append("<div id ='no_data'></div>");
                $('#no_data').html('<div class="text-tiny" style="text-align: center; margin-top: 20px;"><h3>Không có dữ liệu!</h3></div>');
                console.error("Unexpected response structure:", response);
            }

        },
        error: function (xhr, status, error) {
            console.error("Error:", xhr.responseText);
        }
    });
}

function elementHtmlEmployee(account) {
    var html = '<li class="user-item gap20" >';
    html += '<div class="image" >';
    html += '<img src="/areas/images/logo_user/user-avatar.svg.png" />';
    html += '</div>';
    html += '<div class="flex items-center justify-between gap20 flex-grow" id="list-body">';
    html += '<div class="name" >';
    html += '<a href="#" class="body-title-2" >' + account.fullName + '</a>';
    html += '</div>';
    if (account.phone != null) {
        html += '<div class="body-text phone">' + account.phone + '</div>';
    } else {
        html += '<div class="body-text phone" style="font-weight:600;  font-style: italic;" > - </div>';
    }
    html += '<div class="body-text email" >' + account.email + '</div>';
    if (account.isDelete == 1) {
        html += '<div class="body-text status" ><span class="account-non-active">Không kích hoạt</span></div>';
    } else {
        html += '<div class="body-text status" ><span class="account-active">Kích hoạt</span></div>';
    }
    html += '<div class="list-icon-function" >';
    html += '<div class="item eye">';
    html += '<i class="icon-eye" onclick="quickViewAccount(\'' + encodeURIComponent(JSON.stringify(account)) + '\')"></i>';
    html += '</div>';
    html += '<div class="item trash">';
    html += `<i class="icon-trash-2" onclick="deleteAccount(${account.accountId}, '${account.fullName}', '${account.isDelete}')"></i>`;
    html += '</div>';
    html += '</div>';
    html += '</div>';
    html += '</li>';
    return html;
}

function nextPage(pageIndex, pageSize) {
    var searchName = document.getElementById("search-input").value;

    console.log('PageIndex: ' + pageIndex);
    console.log('PageSize: ' + pageSize);
    console.log('SearchName: ' + searchName);

    load(pageIndex, pageSize, searchName, sortName, selectStatus);
}

document.addEventListener('DOMContentLoaded', function () {
    var searchInput = document.querySelector('input[name="name"]');

    searchInput.addEventListener('keypress', function (event) {
        if (event.key === 'Enter') {
            event.preventDefault(); // Ngăn chặn hành động mặc định của form
            search();
        }
    });
});

function search() {
    var searchName = document.getElementById("search-input").value;

    console.log("Search query:", searchName);
    load(1, 10, searchName, sortName, selectStatus);
}

function quickViewAccount(accountJson) {
    var account = JSON.parse(decodeURIComponent(accountJson));

    console.log(account);
    $('#profilePopup').modal('show');
    $('#user-name').empty();
    $('#role').empty();
    $('#gender').val('');
    $('#dob').val('');
    $('#phone').val('');
    $('#address').val('');
    $('#email').val('');

    $('#user-name').append(account.fullName);

    if (account.role != null) {
        switch (account.role.roleId) {
            case 1:
                $('#role').append('<span>Quản trị viên</span>');
                break;
            case 2:
                $('#role').append('<span>Nhân viên</span>');
                break;
            case 3:
                $('#role').append('<span>Khách hàng</span>');
                break;
            default:
                $('#role').append('<span>-</span>');
                break;
        }
    } else {
        $('#role').append('<span>Chưa cập nhật</span>');
    }

    if (account.gender != null) {
        if (account.gender == 1) {
            $('#gender').val("Nam");
        } else {
            $('#gender').val("Nữ");
        }
    } else {
        $('#gender').val("-");
    }

    $('#dob').val(account.doB);
    if (account.phone != null) {
        $('#phone').val(account.phone);

    } else {
        $('#phone').val('-');
    }
    if (account.address != null && !(account.address.empty)) {
        $('#address').val(account.address);
    } else {
        $('#address').val('-');
    }
    $('#email').val(account.email);
}

function closeQuickViewAccount() {
    $('#alert-role').empty();
    $('#profilePopup').modal('hide');
}

function sortByName() {
    var searchName = document.getElementById("search-input").value;

    switch (sortName) {
        case "":
            sortName = "abc";
            break;
        case "abc":
            sortName = "zxy";
            break;
        case "zxy":
            sortName = "";
            break;
        default:
            break;
    }
    load(1, 10, searchName, sortName, selectStatus);
}

function selectedStatus() {
    var searchName = document.getElementById("search-input").value;

    selectStatus = $("#selected_status").val();

    load(1, 10, searchName, sortName, selectStatus);
}

/* Add account */

function addAccount() {
    $('#addAccount').modal('show');

}

$(document).ready(function () {
    $('#addAccount').on('shown.bs.modal', function () {
        // Reset the form
        $('#registrationForm')[0].reset();
        // Clear any validation messages
        $('#registrationForm .text-danger').html('');
    });
});

$(function () {
    $('form').submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#addAccount').modal('hide');
                    $('#notification').modal('show');
                    $('#title-noti').html('Thành Công!');
                    $('#body-noti').html('Tài khoản nhân viên đã tạo thành công. Tiếp tục các hoạt động!');
                } else {
                    $('span.text-danger').html('');

                    if (result.errors) {
                        $.each(result.errors, function (key, errorMessages) {
                            var errorMessage = errorMessages.join('<br>');
                            $('span[data-valmsg-for="' + key + '"]').html(errorMessage);
                        });
                    } else {
                        var errorMessage = `<div class="alert alert-danger alert-dismissible fade show rounded-0">
                                                                <span>${result.error}</span>
                                                                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                                                            </div>`;
                        $('#error-mess').html(errorMessage);
                    }
                }
            }
        });
    });
});

function deleteAccount(accountId, accountName, isDelete) {
    if (isDelete == 'false') {
        $('#error-mess').html("");
        $('#deleteAccount').modal('show');
        $('#nameAccountDelete').html(accountName);
        $('#accountId').val(accountId);
        $('#passwordAdmin').val("");
    } else {
        $('#notification').modal('show');
        $('#title-noti').html('Thất bại!');
        $('#body-noti').html('Tài khoản nhân viên đang trong trạng thái không kích hoạt. Tiếp tục các hoạt động!');
    }

}

$(document).ready(function () {
    $('#confirmDelete').click(function () {
        var accountId = $('#accountId').val();
        console.log("accountiD: " + accountId);
        var passwordAdmin = $('#passwordAdmin').val().trim();

        if (passwordAdmin === "") {
            var html = `<div class="alert alert-danger alert-dismissible fade show rounded-0">
                                                                            <span>Vui lòng nhập mật khẩu</span>
                                                                            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                                                                        </div>`;
            $('#error-mess').html(html);

            return;
        } else {
            $.ajax({
                url: "DeleteAccount",
                type: "post",
                data: { accountId: accountId, passwordAdmin: passwordAdmin },
                success: function (response) {
                    if (response.success) {
                        $('#deleteAccount').modal('hide');
                        $('#notification').modal('show');
                        $('#title-noti').html('Thành Công!');
                        $('#body-noti').html('Tài khoản nhân viên đã tạo xóa thành công. Tiếp tục các hoạt động!');
                        load(1, 10, "", sortName, selectStatus);
                    } else {
                        if (response.isExistAccount == false) {
                            var html = `<div class="alert alert-danger alert-dismissible fade show rounded-0">
                                                                                <span>Account không tồn tại trong hệ thống!</span>
                                                                                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                                                                            </div>`;
                            $('#error-mess').html(html);
                        } else if (response.passwordAdmin == false) {
                            var html = `<div class="alert alert-danger alert-dismissible fade show rounded-0">
                                                                                <span>Mật khẩu sai! Vui lòng nhập lại.</span>
                                                                                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                                                                            </div>`;
                            $('#error-mess').html(html);
                        } else {
                            $('#deleteAccount').modal('hide');
                            $('#notification').modal('show');
                            $('#title-noti').html('Thất bại!');
                            $('#body-noti').html('Tài khoản nhân viên đang trong trạng thái không kích hoạt. Tiếp tục các hoạt động!');
                        }
                    }
                }
            });
        }
    })
});


function showPassword() {
    var pass = document.getElementById("passwordAdmin");
    var icon = document.getElementById("eye-icon");
    if (pass.type === "password") {
        pass.type = "text";
        icon.classList.replace("bi-eye", "bi-eye-slash");
    } else {
        pass.type = "password";
        icon.classList.replace("bi-eye-slash", "bi-eye");
    }
}

/* Customer */
function elementHtmlCustomer(account) {
    var html = `<li class="user-item gap14">
                    <div class="image">
                        <img src="/areas/images/logo_user/user-avatar.svg.png" alt="">
                    </div>
                    <div class="flex items-center justify-between gap20 flex-grow" id="list-body">
                        <div class="name">
                            <a href="CustomerDetail?userId=${account.userId}" class="body-title-2">${account.fullName}</a>
                        </div>
                        <div class="body-text phone">${account.phone != null ? account.phone : "-"}</div>
                        <div class="body-text email">${account.email}</div>
                        <div class="body-text status">
                            ${account.isDelete == 1 ?
            '<span class="account-non-active">Không kích hoạt</span>' :
            '<span class="account-active">Kích hoạt</span>'}
                        </div>
                        <div class="list-icon-function">
                            <div class="item eye">
                                <a href="CustomerDetail?userId=${account.userId}"><i class="icon-eye"></i></a>
                            </div>
                        </div>
                    </div>
                </li>`;
    return html;
}
