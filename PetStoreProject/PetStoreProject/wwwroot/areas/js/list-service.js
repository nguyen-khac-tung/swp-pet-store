let pageIndex = 1;
let pageSize = 7;

let serviceFilter = {
    name: null,
    serviceType: null,
    status: null,
    sortServiceName: null,
    sortServiceId: null,
    sortPrice: null,
    sortUsedQuantity: null
}

function resetSortOption() {
    serviceFilter.sortServiceName = null;
    serviceFilter.sortServiceId = null;
    serviceFilter.sortPrice = null;
    serviceFilter.sortUsedQuantity = null;
}

function refreshServiceFilter() {
    serviceFilter.name = $("#service-name-search").val();
    serviceFilter.serviceType = $("#service-type-search").find(".choose").attr("data-service-type");
    serviceFilter.status = $("#service-status-search").find(".choose").attr("data-status");

    resetSortOption();
    serviceFilter.sortServiceId = 'Ascending';

    pageIndex = 1;
}

function searchServiceName(event) {
    event.preventDefault();
    refreshServiceFilter();
    fetchNewListService();
}

function searchServiceType(element) {
    $("#service-type-search").find("li a").removeClass("choose");
    $(element).addClass("choose");
    refreshServiceFilter();
    fetchNewListService();
}

function searchServiceStatus(element) {
    $("#service-status-search").find("li a").removeClass("choose");
    $(element).addClass("choose");
    refreshServiceFilter();
    fetchNewListService();
}

function selectServiceName() {
    let currentStateSort = serviceFilter.sortServiceName;
    resetSortOption();
    if (currentStateSort === null || currentStateSort === 'Descending') {
        serviceFilter.sortServiceName = 'Ascending';
    } else {
        serviceFilter.sortServiceName = 'Descending';
    }
    fetchNewListService();
}

function selectServiceId() {
    let currentStateSort = serviceFilter.sortServiceId;
    resetSortOption();
    if (currentStateSort === null || currentStateSort === 'Descending') {
        serviceFilter.sortServiceId = 'Ascending';
    } else {
        serviceFilter.sortServiceId = 'Descending';
    }
    fetchNewListService();
}

function selectPrice() {
    let currentStateSort = serviceFilter.sortPrice;
    resetSortOption();
    if (currentStateSort === null || currentStateSort === 'Descending') {
        serviceFilter.sortPrice = 'Ascending';
    } else {
        serviceFilter.sortPrice = 'Descending';
    }
    fetchNewListService();
}

function selectUsedQuantity() {
    let currentStateSort = serviceFilter.sortUsedQuantity;
    resetSortOption();
    if (currentStateSort === null || currentStateSort === 'Descending') {
        serviceFilter.sortUsedQuantity = 'Ascending';
    } else {
        serviceFilter.sortUsedQuantity = 'Descending';
    }
    fetchNewListService();
}

function selectPage(currentPage) {
    $("#paging-navigation").find("li").removeClass("active");
    $("#paging-navigation").find("#page-" + currentPage).addClass("active");

    pageIndex = currentPage;
    fetchNewListService();
}

function fetchNewListService() {
    console.log('----------------------------------------')
    console.log('name: ', serviceFilter.name);
    console.log('serviceType: ', serviceFilter.serviceType);
    console.log('status: ', serviceFilter.status);
    console.log('sortServiceId:', serviceFilter.sortServiceId);
    console.log('sortServiceName:', serviceFilter.sortServiceName);
    console.log('sortPrice:', serviceFilter.sortPrice);
    console.log('sortUsedQuantity:', serviceFilter.sortUsedQuantity);
    console.log('pageIndex:', pageIndex);
    console.log('url:', url);
    $.ajax({
        type: 'POST',
        url: url,
        data: {
            serviceFilterVM: serviceFilter,
            pageIndex: pageIndex,
            pageSize: pageSize
        },

        success: function (response) {
            if (response.listService != null && response.listService.length > 0) {
                let content = '';
                response.listService.forEach(function (service) {
                    content += `<li id="" class="countries-item">
                                <div class="body-text">
                                    <img src="${service.imageUrl}" alt="Image" />
                                    <div>
                                        ${service.name}
                                    </div>
                                </div>
                                <div class="body-text">#${service.serviceId}</div>
                                <div class="body-text">${service.price.toLocaleString('en-US')} VND</div>
                                <div class="body-text">${service.usedQuantity}</div>
                                <div class="body-text">Dịch Vụ ${service.type}</div>
                                <div>`;
                    if (service.isDelete) {
                        content += '<div class="block-not-available" > Ngừng kinh doanh</div>';
                    }
                    else {
                        content += '<div class="block-available">Còn kinh doanh</div>';
                    }

                    content += `</div >
                                <div>
                                    <div class="list-icon-function">
                                        <a class="item eye " data-serviceid="${service.serviceId}"><i class="icon-eye"></i></a>
                                        <a class="item edit " data-serviceid="${service.serviceId}"><i class="icon-edit-3"></i></a>
                                        <a class="item trash " data-serviceid="${service.serviceId}"><i class="icon-trash-2"></i></a>
                                    </div>
                                </div>
                            </li> `;
                });

                $("#table-services").empty();
                $("#table-services").html(content);
                generatePagingNavigation(pageIndex, response.numberOfPage);
            } else {
                $('#table-services').empty();
                let content = `<div class="empty-order-history">
                                    <i class="fas fa-box-open"></i>
                                    <h6>Dịch vụ thú cưng trống.</h6>
                                </div>`;
                $('#table-services').html(content);
                $("#paging-navigation").empty();
            }
        },

        error(xhr, status, error) {
            console.error("Error:", error);
        }
    });
}

function generatePagingNavigation(currentPage, numberPage) {
    if (numberPage > 0) {
        let content = '';
        if (currentPage > 1) {
            content += `<li id="page-${currentPage - 1}" class="page-item" > <a class="page-link" onclick="selectPage(${currentPage - 1})" href="javascript:void(0);">Trang trước</a></li> `;
        } else {
            content += `<li class="page-item"> <a class="page-link" href="javascript:void(0);" style="pointer-events:none; cursor:default; color:#b7b7b7;">Trang trước</a></li> `;
        }
        var startPage = Math.max(1, currentPage - 2);
        var endPage = Math.min(numberPage, currentPage + 2);
        if (startPage > 1) {
            content += ` <li id="page-1" class="page-item" > <a class="page-link" onclick="selectPage(1)" href="javascript:void(0);">1</a></li> `;
            if (startPage > 2) {
                content += ` <li class="page-item" > <a class="page-link" href="javascript:void(0);">...</a></li> `;
            }
        }
        for (let i = startPage; i <= endPage; i++) {
            if (currentPage === i) {
                content += ` <li id="page-${i}" class="page-item active" > <a class="page-link" href="javascript:void(0);">${i}</a></li> `;
            }
            else {
                content += ` <li id="page-${i}" class="page-item" > <a class="page-link" onclick="selectPage(${i})" href="javascript:void(0);">${i}</a></li> `;
            }
        }
        if (numberPage >= endPage + 1) {
            if (numberPage >= endPage + 2) {
                content += ` <li class="page-item" > <a class="page-link" href="javascript:void(0);">...</a></li> `;
            }
            content += ` <li id="page-${numberPage}" class="page-item" > <a class="page-link" onclick="selectPage(${numberPage})" href="javascript:void(0);">${numberPage}</a></li> `;
        }
        if (currentPage != numberPage) {
            content += ` <li id="page-${currentPage + 1}" class="page-item" > <a class="page-link" onclick="selectPage(${currentPage + 1})" href="javascript:void(0);">Trang sau</a></li> `;
        } else {
            content += `<li class="page-item" > <a class="page-link" href="javascript:void(0);" style="pointer-events:none; cursor:default; color:#b7b7b7;">Trang sau</a></li> `;
        }
        $('#paging-navigation').html(content);
    } else {
        $('#paging-navigation').empty();
    }
}
