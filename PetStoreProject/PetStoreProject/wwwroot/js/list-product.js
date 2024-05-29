
var selectedBrands = [];
var selectedColors = [];
var selectedSizes = [];
var pageSize = 20;
var pageIndex = 1;
var selectedSort = "";
var url = window.location.pathname;
var rangeInput = document.querySelectorAll(".range-input input"),
    priceInput = document.querySelectorAll(".price-input input"),
    range = document.querySelector(".slider .progress");
var priceGap = 1000;

const priceMinInit = priceInput[0].value;
const priceMaxInit = priceInput[1].value;

rangeInput.forEach(input => {
    input.addEventListener("input", e => {
        let minVal = parseInt(rangeInput[0].value),
            maxVal = parseInt(rangeInput[1].value);

        if ((maxVal - minVal) < priceGap) {
            if (e.target.className === "range-min") {
                rangeInput[0].value = maxVal - priceGap
            } else {
                rangeInput[1].value = minVal + priceGap;
            }
        } else {
            priceInput[0].value = minVal;
            priceInput[1].value = maxVal;
            range.style.left = ((minVal / rangeInput[0].max) * 100) + "%";
            range.style.right = 100 - (maxVal / rangeInput[1].max) * 100 + "%";
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("filter_button").addEventListener('click', function () {
        selectedBrands = [];
        selectedColors = [];
        selectedSizes = [];

        //---brands
        var checkboxesBrands = document.querySelectorAll(".brand-checkbox");
        checkboxesBrands.forEach(function (checkbox) {
            if (checkbox.checked) {
                selectedBrands.push(checkbox.value);
            }
        });
        console.log("filter: " + selectedBrands);

        //----price
        console.log("min: " + priceInput[0].value);
        console.log("max: " + priceInput[1].value);
        console.log("pmin: " + priceMinInit);
        console.log("pmax: " + priceMaxInit);

        //---size
        var checkboxesSize = document.querySelectorAll(".size-checkboxes");
        checkboxesSize.forEach(function (checkbox) {
            if (checkbox.checked) {
                selectedSizes.push(checkbox.value);
            }
        });
        console.log("Size: " + selectedSizes);

        //---color
        var checkboxesColor = document.querySelectorAll(".color-checkboxes");
        checkboxesColor.forEach(function (checkbox) {
            if (checkbox.checked) {
                selectedColors.push(checkbox.value);
            }
        });
        console.log("Color: " + selectedColors);


        LoadData(url, pageSize, 1, selectedBrands, selectedSort, priceInput[0].value, priceInput[1].value, selectedColors, selectedSizes);
    });

    document.querySelector("#clear_button").addEventListener('click', function () {
        var checkboxes = document.querySelectorAll(".brand-checkbox");
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });
        var checkboxes = document.querySelectorAll(".size-checkboxes");
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });
        var checkboxes = document.querySelectorAll(".color-checkboxes");
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = false;
        });
        selectedBrands = [];
        // Reset price inputs
        priceInput[0].value = priceMinInit;
        priceInput[1].value = priceMaxInit;

        // Update range inputs
        rangeInput[0].value = priceMinInit;
        rangeInput[1].value = priceMaxInit;

        // Update slider progress
        range.style.left = "0%";
        range.style.right = "0%";

        console.log("Filters cleared");
        LoadData(url, pageSize, 1, [], "", priceMinInit, priceMaxInit, [], []);
    });

});

function SelectedSort() {
    selectedSort = document.getElementById("selected_sort").value;
    console.log("selected_sort: change" + selectedSort);
    LoadData(url, pageSize, pageIndex, selectedBrands, selectedSort, priceInput[0].value, priceInput[1].value, selectedColors, selectedSizes);
}

function LoadData(url,pageSize, page, selectedBrands, selectedSort, priceInputMin, priceInputMax, selectedColors, selectedSizes) {
    $('#data-grid-view').empty();
    $('#list-view').empty();
    $.ajax({
        url: '/Product/Shop',
        type: 'post',
        data: {url: url,
            pageSize: pageSize, page: page, selectedBrands: selectedBrands, selectedSort: selectedSort,
            priceMin: priceInputMin, priceMax: priceInputMax, selectedColors: selectedColors, selectedSizes: selectedSizes
        },
        success: function (response) {
            if (response.data.length > 0) {
                var html1 = "";
                var htmlGridView = "";
                var html = "";
                var items = response.data;
                for (var index = 0; index < items.length; index++) {
                    const amount = items[index].productOption[0].price;
                    const formattedAmount = formatVND(amount);
                    htmlGridView += "<div id='data-grid-view' class='row border-hover-effect'>";
                    htmlGridView += "</div>";
                    html += "<!-- Single Product Start -->";
                    html += "<div class='col-xl-4 col-lg-6 col-md-6'>";
                    html += "<div class='single-template-product'>";
                    html += "<!-- Product Image Start -->";
                    html += "<div class='pro-img'>";

                    html += "<a href='/product/detail/" + items[index].productId + "'>";

                    if (items[index].productOption && items[index].productOption.length > 0) {
                        html += "<img style='max-height: 300px' class='primary-img' src='" + items[index].productOption[0].img_url + "' alt='single-product'>";
                    }

                    html += "</a>";
                    html += "<div class='item_quick_link'>";
                    html += "</div>";
                    html += "<div class='product-count-wrap'>";
                    html += "</div>";
                    html += "</div>";
                    html += "<!-- Product Image End -->";
                    html += "<!-- Product Content Start -->";
                    html += "<div class='product_content_wrap'>";
                    html += "<div class='product_content'>";
                    html += "<h4><a style='height: 50px;  href='/product/detail/" + items[index].productId + "'>" + items[index].name + "</a></h4>";
                    html += "<div class='grid_price'>";

                    if (items[index].productOption && items[index].productOption.length > 0) {
                        html += "<span class='regular-price'>" + formattedAmount + " VND</span>";
                    }

                    html += "</div>";
                    html += "</div>";
                    html += "<div class='item_add_cart'>";
                    //html += "<a class='grid_compare' href='compare.html' title='Compare'><i class='icofont-random'></i></a>";
                    html += "<a class='grid_cart' href = '/product/detail/" + items[index].productId + "' title = 'Add to Cart' > Thêm vào giỏ hàng </a>";
                    html += "<a class='grid_wishlist' href='wishlist.html' title='Wishlist'><i class='icofont-heart-alt'></i></a>";
                    html += "</div>";
                    html += "</div>";
                    html += "<!-- Product Content End -->";
                    html += "</div>";
                    html += "</div>";
                    html += "<!-- Single Product End -->";

                    html1 += "<div class='single-template-product'>";
                    html1 += "<!-- Product Image Start -->";
                    html1 += "<div class='pro-img'>";

                    html1 += "<a href='/product/detail/" + items[index].productId + "'>";
                    if (items[index].productOption && items[index].productOption.length > 0) {
                        html1 += "<img class='primary-img' src=" + items[index].productOption[0].img_url + "alt='single-product'>";
                    }
                    html1 += "</a>";
                    html1 += "<div class='item_quick_link'>";
                    html1 += "</div>";
                    html1 += "<div class='product - count - wrap'>";
                    html1 += "</div>";
                    html1 += "</div>";
                    html1 += "<!-- Product Image End -->";
                    html1 += "<!-- Product Content Start -->";
                    html1 += "<div class='product_content_wrap'>";
                    html1 += "<div class='product_content'>";
                    html1 += "<h4><a href='/product/detail/" + items[index].productId + "'>" + items[index].name + "</a></h4>";
                    html1 += "<div class='scrollable-description'>";
                    html1 += "<p class='list_des' " + items[index].description + "</p>";
                    html1 += "</div>";
                    html1 += "<div class='grid_price'>";
                    if (items[index].productOption && items[index].productOption.length > 0) {
                        html1 += "<span class='regular-price'>" + formattedAmount + " VND</span>";
                    }
                    html1 += "</div>";
                    html1 += "</div>";
                    html1 += "<div class='item_add_cart'>";
                    //html1 += "<a class='grid_compare' href='compare.html1' title='Compare'><i class='icofont-random'></i></a>";
                    html1 += "<a class='grid_cart' href='/product/detail/" + items[index].productId + "' title='Add to Cart'>Thêm vào giỏ hàng</a>";
                    html1 += "<a class='grid_wishlist' href='wishlist.html1' title='Wishlist'><i class='icofont-heart-alt'></i></a>";
                    html1 += "</div>";
                    html1 += "</div>";
                    html1 += "<!-- Product Content End -->";
                    html1 += "</div>";
                }
                console.log("pageSize:" + response.pageSize);
                $('#grid-view').html(htmlGridView);
                $('#data-grid-view').html(html);
                $('#list-view').html(html1);
                pageIndex = response.currentPage;
                Pagination(response.currentPage, response.numberPage, response.pageSize);
            } else {
                $('#grid-view').empty();
                var gridView = document.getElementById('grid-view');
                var htmlAnnounce = "<div id='no-products' style='text-align: center; margin-left: 20px;'><h4>Sản phẩm khách hàng lựa chọn hiện không có!</h4></div>";
                $('#grid-view').html(htmlAnnounce);
                $('#list-view').html(htmlAnnounce);
                $('#pagination').html("");
                console.error("Unexpected response structure:", response);
                
            }
        },
        error: function (xhr, status, error) {
            console.error("Error:", xhr.responseText);
        }

    });
    window.scrollTo(0, 0);
}

function Pagination(currentPage, numberPage, pageSize) {
    if (numberPage > 0) {
        var str = '<nav aria-label="Page navigation example"> <ul class="pagination">';
        if (currentPage > 1) {
            str += `<li class="page-item"><a class="page-link" onclick="NextPage(${currentPage - 1},${pageSize})" href="javascript:void(0);">Trang trước</a></li>`;
        }
        var startPage = Math.max(1, currentPage - 2);
        var endPage = Math.min(numberPage, currentPage + 2);
        if (startPage > 1) {
            str += ` <li class="page-item"><a class="page-link" onclick="NextPage(${1},${pageSize})" href="javascript:void(0);">1</a></li>`;
            if (startPage > 2) {
                str += ` <li class="page-item"><a class="page-link" href="javascript:void(0);">...</a></li>`;
            }
        }
        for (let i = startPage; i <= endPage; i++) {
            if (currentPage === i) {
                str += ` <li class="page-item active"><a class="page-link" href="javascript:void(0);">${i}</a></li>`;
            }
            else {
                str += ` <li class="page-item"><a class="page-link" onclick="NextPage(${i},${pageSize})" href="javascript:void(0);">${i}</a></li>`;
            }
        }
        if (numberPage >= endPage + 1) {
            if (numberPage >= endPage + 2) {
                str += ` <li class="page-item"><a class="page-link" href="javascript:void(0);">...</a></li>`;
            }
            str += ` <li class="page-item"><a class="page-link" onclick="NextPage(${numberPage},${pageSize})" href="javascript:void(0);">${numberPage}</a></li>`;
        }
        if (currentPage != numberPage)
            str += ` <li class="page-item"><a class="page-link" onclick="NextPage(${currentPage + 1},${pageSize})" href="javascript:void(0);">Trang sau</a></li>`;
        str += "</ul></nav>";
        $('#pagination').html(str);
    }
}


function NextPage(page, pageSize) {
    console.log("Nextpage:");
    console.log("PriceM: " + priceInput[0].value);
    console.log("PriceMax: " + priceInput[1].value);

    LoadData(url, pageSize, page, selectedBrands, selectedSort, priceInput[0].value, priceInput[1].value, selectedColors, selectedSizes);
}
function ChangePageSize() {
    console.log("changePageSize:");
    console.log("PriceM: " + priceInput[0].value);
    console.log("PriceMax: " + priceInput[1].value);
    pageSize = parseInt(document.getElementById("pageSizeSelect").value);
    console.log(pageSize);
    LoadData(url, pageSize, 1, selectedBrands, selectedSort, priceInput[0].value, priceInput[1].value, selectedColors, selectedSizes);
}

function formatVND(amount) {
    return amount.toLocaleString('en-US', '#.###');
}