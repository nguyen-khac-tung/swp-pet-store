var numberViewProduct = 2;

fetch('https://vapi.vnappmob.com/api/province/')
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        let provinces = data.results;
        provinces.map(value => document.getElementById('consigneeProvince').innerHTML += '<option value="' + value.province_id + '">' + value.province_name + '</option>');
    })
    .catch(error => {
        console.error('There was a problem with the fetch operation:', error);
    });

function getProvinces(event) {
    let province_id = event.target.value;
    fetchDistricts(province_id);
}

function fetchDistricts(province_id) {
    fetch('https://vapi.vnappmob.com/api/province/district/' + province_id)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            let districts = data.results;
            document.getElementById('consigneeDistrict').innerHTML = '<option value="0">- Quận/Huyện/Thị xã -</option>';
            document.getElementById('consigneeWard').innerHTML = '<option value="0">- Xã/Phường/Thị trấn -</option>';
            districts.map(value => document.getElementById('consigneeDistrict').innerHTML += '<option value="' + value.district_id + '">' + value.district_name + '</option>');
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function getDistricts(event) {
    let district_id = event.target.value;
    fetchWards(district_id);
}

function fetchWards(district_id) {
    fetch('https://vapi.vnappmob.com/api/province/ward/' + district_id)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            let districts = data.results;
            document.getElementById('consigneeWard').innerHTML = '<option value="0">- Xã/Phường/Thị trấn -</option>';
            districts.map(value => document.getElementById('consigneeWard').innerHTML += '<option value="' + value.ward_id + '">' + value.ward_name + '</option>');
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}



var OrderItems = [];
var checkoutViewModel = "";

function ItemOrders(OrderId, ProductOptionId, Quantity, Price, PromotionId) {
    this.OrderId = OrderId;
    this.ProductOptionId = ProductOptionId;
    this.Quantity = Quantity;
    this.Price = Price;
    this.PromotionId = PromotionId;
}

function CheckOutViewModel(CheckoutId, OrderItems, OrderName, OrderPhone, OrderEmail, ConsigneeName,
    ConsigneePhone, ConsigneeProvince, ConsigneeDistrict, ConsigneeWard, ConsigneeAddressDetail, PaymentMethod, TotalAmount, DiscountId) {
    this.CheckoutId = 0;
    this.OrderItems = OrderItems;
    this.OrderName = OrderName;
    this.OrderEmail = OrderEmail;
    this.OrderPhone = OrderPhone;
    this.ConsigneeName = ConsigneeName;
    this.ConsigneePhone = ConsigneePhone;
    this.ConsigneeProvince = ConsigneeProvince;
    this.ConsigneeDistrict = ConsigneeDistrict;
    this.ConsigneeWard = ConsigneeWard;
    this.ConsigneeAddressDetail = ConsigneeAddressDetail;
    this.PaymentMethod = PaymentMethod;
    this.TotalAmount = TotalAmount;
    this.DiscountId = DiscountId;
}

function AddOrderItems() {
    var index = $('#product_index').val();
    for (var i = 0; i < index; i++) {
        var orderId = 0;
        var productOptionId = $('#product_productOptionId_' + i).val();
        var productPrice = $('#product_price_' + i).val();
        var productQuantity = $('#product_quantity_' + i).val();
        var promotionId = $('#product_promotionId_' + i).val();
        var items = new ItemOrders(
            orderId,
            productOptionId,
            productQuantity,
            productPrice,
            promotionId
        );
        OrderItems.push(items);
    }
}

function ProcessPay() {
    var productsByNull = $('#products_null').val();
    if (productsByNull == "0") {
        alert("Danh sách sản phẩm mua hàng trống!");
        return;
    }
    var orderName = $('#orderName').val();
    var checkOrderName = checkName(orderName, "orderName");
    if (checkOrderName == 0) return;

    var orderPhone = $('#orderPhone').val();
    var checkOrderPhone = checkPhone(orderPhone, "orderPhone");
    if (checkOrderPhone == 0) return;

    var consigneeName = $('#consigneeName').val();
    var checkConsigneeName = checkName(consigneeName, "consigneeName");
    if (checkConsigneeName == 0) return;

    var consigneePhone = $('#consigneePhone').val();
    var checkConsigneePhone = checkPhone(consigneePhone, "consigneePhone");
    if (checkConsigneePhone == 0) return;


    var consigneeProvince = checkSelect("consigneeProvince", "Tỉnh/Thành phố");
    if (consigneeProvince == 0) return;
    var selectConsigneeProvince = $('#consigneeProvince option:selected').text();

    var consigneeDistrict = checkSelect("consigneeDistrict", "Quận/Huyện/Thị xã/");
    if (consigneeDistrict == 0) return;
    var selectConsigneeDistrict = $('#consigneeDistrict option:selected').text();

    var consigneeWard = checkSelect("consigneeWard", "Xã/Phường/Thị trấn");
    if (consigneeWard == 0) return;
    var selectConsigneeWard = $('#consigneeWard option:selected').text();

    var orderEmail = "";
    if (validateEmail($('#orderEmail').val())) {
        orderEmail = $('#orderEmail').val();
    }
    var consigneeAddress = $('#consigneeAddress').val();

    var moneyToCheckout = $('#order_value').val();

    var paymentMethod = $('#paymentMethod').val();
    

    console.log(orderName + ' ' + orderPhone + ' ' + consigneeName + ' ' + consigneePhone +
        ' ' + selectConsigneeProvince + ' ' + selectConsigneeDistrict + ' ' + selectConsigneeWard +
        ' ' + orderEmail + ' address ' + consigneeAddress + ' amount: ' + moneyToCheckout + ' payment: ' + paymentMethod);

    AddOrderItems();

    checkoutViewModel = new CheckOutViewModel(
        0,
        OrderItems,
        orderName,
        orderPhone, 
        orderEmail,
        consigneeName,
        consigneePhone,
        selectConsigneeProvince,
        selectConsigneeDistrict,
        selectConsigneeWard,
        consigneeAddress,
        paymentMethod,
        moneyToCheckout,
        discountId
    );

    processInfoCheckout(checkoutViewModel);
    //console.log($('#product_index').val());
    //console.log(OrderItems);
    //console.log(checkoutViewModel);
}

function processInfoCheckout(checkoutViewModel) {
    $.ajax({
        url: '/Checkout/ProcessCheckout',
        type: 'post',
        contentType: 'application/json',
        data: JSON.stringify(checkoutViewModel),
        success: function (response) {
            window.location.href = "/checkout/" + response.urlTransfer + '?orderId=' + response.orderId + '&amount=' + response.amount;
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function checkName(name, typeName) {
    if (name.trim() === "") {
        $('#span_' + typeName).text("Vui lòng nhập tên!");
        return 0;
    }
    resetSpan();
    return 1;
}

function checkPhone(phoneNumber, typePhone) {
    const regex = /^(0|\+84)(3|5|7|8|9)\d{8}$/;

    if (phoneNumber.trim() === "" || !regex.test(phoneNumber)) {
        $('#span_' + typePhone).text("Số điện thoại không hợp lệ!");
        return 0;
    }
    resetSpan();
    return 1;
}

function checkSelect(typeSelect, stringSelect) {
    var selectValue = $('#' + typeSelect).val();
    if (selectValue == "0") {
        $('#span_' + typeSelect).text("Vui lòng chọn " + stringSelect + "!");
        return 0;
    }
    resetSpan();
    return 1;
}

function resetSpan() {
    $('.span_danger').text('');
}

function validateEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

document.addEventListener('DOMContentLoaded', function () {
    var orderName = document.getElementById('orderName');
    var consigneeName = document.getElementById('consigneeName');
    var orderPhone = document.getElementById('orderPhone');
    var consigneePhone = document.getElementById('consigneePhone');

    orderName.addEventListener('input', function () {
        consigneeName.value = orderName.value;
    });

    orderPhone.addEventListener('input', function () {
        consigneePhone.value = orderPhone.value;
    });
});


let title = '';
let reduce = 0;
let discountId = 0;
function showDiscount(event) {
    event.preventDefault()
    $('#staticBackdrop').modal('show')
}

function chooseDiscount() {
    title = $(this).parent().find('input[name="title"]').val();
    discountId = $(this).val();
    $('#title').text("Giảm " + title);
    $('#title').parent().removeClass('hide')
    reduce = $(this).parent().find('input[name="reduce"]').val();
    console.log(title);
}

function useDiscount() {
    $('#staticBackdrop').modal('hide')
    let moneyToCheckout = $('#moneyToCheckout').val()
    console.log(moneyToCheckout, reduce)
    moneyToCheckout = parseFloat(moneyToCheckout) - reduce;
    $('#money').text(moneyToCheckout.toLocaleString('en', 'US') + " VND")
    $('#moneyToCheckout').val(moneyToCheckout)
    $('#save').text(title)
}