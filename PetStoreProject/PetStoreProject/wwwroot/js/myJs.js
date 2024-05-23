function addToCart(productOptionId, quantity) {
	console.log(productOptionId);
	$.ajax({
		type: "POST",
		url: "/cart/AddItem",
		data: { productOptionId: productOptionId, quantity: quantity },
		success: function (response) {
			if (response.message != 'success') {
				alert(response.message)
			}
			else {
				getCartItems();
			}

		},
		error: function (xhr, status, error) {
			console.error('The request failed!', status, error);
		}
	});
}

function getCartItems() {
	/*
			<div class="single-cart-box">
                <div class="cart-img">
                    <a href="#">
                        <img src="~/img/products/p1.jpg" alt="cart-image">
                    </a>
                </div>
                <div class="cart-content">
                    <h6>
                        <a href="product-details.html">Product Title Here 1</a>
                    </h6>
                    <span>M, Red</span>
                    <span class="cart-price">35.00 x 1</span>
                </div>
            </div>
	*/
	$.ajax({
		type: "POST",
		url: "/cart/GetCartItems",
		success: function (response) {
			$('#list_item').empty();
			$('#total_item').html(response.length)
			var total_price = 0;
			for (var index = 0; index < response.length; index++) {
				var divSingleCart = $('<div>', {
					class: "single-cart-box"
				})

				var divCartImg = $('<div>', {
					class: "cart-img"
				})

				var divCartContent = $('<div>', {
					class: "cart-content"
				})

				var img = $('<img>', {
					src: response[index].imgUrl,
					alt: "cart-image"
				})

				var h6 = $('<h6>')
				var aTitle = $('<a>', {
					href: 'product/detail/'+response[index].productId
				}).text(response[index].name)

				var spanOption = $('<span>').text(response[index].size.name + ',' + response[index].attribute.name)
				var spanPrice = $('<span>').text(response[index].price + 'x' + response[index].quantity)
				h6.append(aTitle)
				divCartImg.append(img)
				divCartContent.append(h6)
				divCartContent.append(spanOption)
				divCartContent.append(spanPrice)

				divSingleCart.append(divCartImg)
				divSingleCart.append(divCartContent)
				$('#list_item').append(divSingleCart)

				total_price += parseFloat(response[index].price) * parseFloat(response[index].quantity)
				$('#total_price').html(total_price)
			}
			console.log(response);
		},
		error: function (xhr, status, error) {
			console.error('Error: ' + error);
		}
	});
}