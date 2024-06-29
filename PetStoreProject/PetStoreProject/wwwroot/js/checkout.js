function ItemsCheckoutViewModel(Name, Option, Price, Quantity, ImgUrl) {
    this.Name = Name;
    this.Option = Option;
    this.Price = Price;
    this.Quantity = Quantity;
    this.ImgUrl = ImgUrl;
}

function selectedProductCheckout() {
    var selectedProduct = [];
    var totalCheckboxId = $('#totalCheckboxId').val();
    for (var i = 0; i < totalCheckboxId; i++) {
        var checkbox = $('#checkbox_' + i);
        if (checkbox.prop('checked')) {
            var productName = $('#product_name_' + i).val();
            var imgUrl = $('#product_img_' + i).attr('src');
            var productOption = $('#product_option_' + i).val();
            var productPrice = $('#product_price_' + i).text().replace(/[^0-9.-]+/g, ""); // Remove any non-numeric characters
            var productQuantity = $('#product_quantity_' + i).val();
            var cart = new ItemsCheckoutViewModel(
                productName,
                productOption,
                parseFloat(productPrice), // Convert price to float
                parseInt(productQuantity), // Convert quantity to int
                imgUrl
            );
            selectedProduct.push(cart);
        }
    }
    if (selectedProduct.length == 0) {
        alert("Bạn cần phải chọn 1 sản phẩm trong giỏ hàng để thanh toán!");
    } else {
        $.ajax({
            url: '/Checkout/Form',
            type: 'POST', // Use POST method
            contentType: 'application/json', // Specify the content type
            data: JSON.stringify(selectedProduct), // Convert data to JSON string
            success: function (response) {
                //console.log(response);
                /*$('body').html(response);*/
                window.location.href = "/checkout/Form";
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    }
}
