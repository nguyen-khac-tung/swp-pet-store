window.onload = function () {
	getCartBoxItems();
};
function addToCart(productOptionId, quantity) {
	console.log(productOptionId);
	$.ajax({
		type: "POST",
		url: "/cart/AddToCart",
		data: { productOptionId: productOptionId, quantity: quantity },
		success: function (response) {
			if (response.message != 'success') {
				alert(response.message)
			}
			else {
				getCartBoxItems();
			}
		},
		error: function (xhr, status, error) {
			console.error('The request failed!', status, error);
		}
	});
}

function getCartBoxItems() {
	$.ajax({
		type: "POST",
		url: "/cart/GetCartBoxItems",
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

				var h6 = $('<h6>', {
					style: "margin: 0px"
				})
				var aTitle = $('<a>', {
					href: 'http://localhost:5206/product/detail/'+response[index].productId
				}).text(response[index].name)
				var option = "";
				if (!(response[index].size.name == null && response[index].attribute.name == null)) {
					if (response[index].size.name != null && response[index].attribute.name != null) {
						option += response[index].size.name + ', ' + response[index].attribute.name
					}
					else if (response[index].size.name == null) {
						option += response[index].attribute.name
					} else {
						option += response[index].size.name
					}
				}
				var spanOption = $('<div>', {
					style: "font-size: 13px; margin-top: 3px; margin-bottom: 3px"
				}).text(option)
				var spanPrice = $('<span>').text(response[index].price + ' x ' + response[index].quantity)
				h6.append(aTitle)
				divCartImg.append(img)
				divCartContent.append(h6)
				divCartContent.append(spanOption)
				divCartContent.append(spanPrice)

				divSingleCart.append(divCartImg)
				divSingleCart.append(divCartContent)
				$('#list_item').append(divSingleCart)

				total_price += parseFloat(response[index].price) * parseFloat(response[index].quantity)
			}
			var x = total_price.toString() + " VND"
			console.log(x)
			$('#total_price').html(x);
			console.log(response);
		},
		error: function (xhr, status, error) {
			console.error('Error: ' + error);
		}
	});
}

function deleteCartItem(productOptionId) {
	$('#' + productOptionId).remove()
	$.ajax({
		url: "http://localhost:5206/cart/delete",
		type: "DELETE",
		data: { productOptionId: productOptionId },
		success: function (response) {
			getCartBoxItems();
		}
	});
	amountCart();
}

function amountCart() {
	var total_amount = 0.0;
	var subTotalElements = document.getElementsByClassName('product-subtotal')
	for (var i = 1; i < subTotalElements.length; i++) {
		total_amount += parseFloat(subTotalElements[i].innerText)
	}
	$('#amount').html(total_amount)
}

function total_price() {
	var quantity = document.getElementById("quantity").value
	var amount = price * quantity
	document.getElementById("amount").innerText = amount
}

function quickEditCartItem(oldProductOptionId, productId) {
	$('#quick_add_to_cart').attr('data-old-product-option-id', oldProductOptionId)
	quickView(productId)
	$('#myModal').modal('show');
}
function editCartItem(oldProductOptionId, newProductOptionId, quantity) {
	console.log(newProductOptionId)
	$.ajax({
		url: "http://localhost:5206/cart/Edit",
		method: "PUT",
		data: { oldProductOptionId: oldProductOptionId, newProductOptionId: newProductOptionId, quantity: quantity },
		success: function (response) {
			if (response.message != null) {
				alert(response.message)
			}
			else {
				updateCartItem(oldProductOptionId, response)
				getCartBoxItems()
				$('#myModal').modal('hide');
				amountCart();
			}
		}
	});
}

function updateCartItem(oldId, cartItem) {
	var option = "";
	if (!(cartItem.attribute.name == null && cartItem.size.name == null)) {
		if (cartItem.attribute.Name != null && cartItem.size.Name != null) {
			option += "( " + cartItem.attribute.Name + ", " + cartItem.size.name + " )";
		}
		else if (cartItem.attribute.name != null) {
			option += "( " + cartItem.attribute.name + " )";
		}
		else {
			option += "( " + cartItem.size.name + " )";
		}
	}
	// Find the row by ProductOptionId
	var row = $('#' + oldId);
	console.log(cartItem)
	// Update the product name and link
	row.find('.product-name a').attr('href', 'http://localhost:5206/product/detail/' + cartItem.productId)
		.html(cartItem.name + '</br>' + option);

	// Update the product image
	row.find('.product-thumbnail img').attr('src', cartItem.imgUrl);

	// Update the product price
	row.find('.product-price .amount').text(cartItem.price);

	// Update the product quantity
	row.find('.product-quantity input').val(cartItem.quantity);

	// Update the product subtotal
	row.find('.product-subtotal').text(parseFloat(cartItem.price) * parseFloat(cartItem.quantity));

	// Update the edit button
	row.find('a[title="Chỉnh sửa"]').attr('onclick', `quickEditCartItem(${cartItem.productOptionId}, ${cartItem.productId})`);

	// Update the delete button
	row.find('a[title="Xóa sản phẩm"]').attr('onclick', `deleteCartItem(${cartItem.productOptionId})`);
	row.attr('id', cartItem.productOptionId)
}

function quickView(productId) {
	$('#quick_attribute').empty();
	$('#quick_size').empty();
	$.ajax({
		type: "POST",
		url: "/Product/quickPreview",
		data: { productId: productId },
		success: function (response) {
			$('#quick_name').html(response.name);
			$('#quick_price').html(response.productOption[0].price);
			$('#quick_image').empty();
			$('#quick_amount').html(response.productOption[0].price);
			$('#quick_add_to_cart').attr('data-product-option-id', response.productOption[0].id)

			var imgDiv = $('<div>', {
				class: 'tab-pane large-img-style active'
			});
			var imgElement = $('<img>', {
				src: response.productOption[0].img_url,
				alt: ''
			});
			imgDiv.append(imgElement);
			$('#quick_image').append(imgDiv);

			$('#quick_brand').html('Thương hiệu: ' + response.brand);
			var jsonStr = JSON.stringify(response.productOption);

			if (response.attributes[0].name != null) {
				var divContainer = $('<div>', {
					class: 'color clearfix mb-30'
				});
				var label = $('<label>').text('Loại :');
				var ul = $('<ul>', {
					class: 'color-list scroll'
				});
				var list = ""
				for (var index = 0; index < response.attributes.length; index++) {
					if (response.attributes[index].name != null) {
						list += "<li style = 'cursor:pointer' onclick='quick_attribute_selected(" + index + "," + response.attributes[index].attributeId + "," + jsonStr + ")'" + "data = '" + response.attributes[index].attributeId + "'>" + response.attributes[index].name + "</li>"
					}
					else {
						index -= 1
					}
				}
				ul.append(list)
				divContainer.append(label);
				divContainer.append(ul);
				$('#quick_attribute').append(divContainer);
			}

			if (response.sizes[0].name != null) {
				var sizeLabel = $('<label>').text('Kích thước :');
				var sizeDiv = $('<div>', {
					class: 'color-list size_list'
				});
				var sizeUl = $('<ul>', {
					class: 'color-list size_list scroll'
				});

				var list = ""
				for (var index = 0; index < response.sizes.length; index++) {
					if (response.sizes[index].name != null) {
						list += "<li style = 'cursor:pointer' onclick='quick_size_selected(" + index + "," + response.sizes[index].sizeId + "," + jsonStr + ")'" + "data = '" + response.sizes[index].sizeId + "'>" + response.sizes[index].name + "</li>"
					}
					else {
						index -= 1
					}

				}
				sizeUl.append(list)
				sizeDiv.append(sizeUl);
				$('#quick_size').append(sizeLabel);
				$('#quick_size').append(sizeDiv);
			}

			var list_quick_size = document.getElementById('quick_size').querySelectorAll('li');
			var list_quick_attribute = document.getElementById('quick_attribute').querySelectorAll('li');
			if (list_quick_size.length > 0) {
				list_quick_size[0].classList.add('select');
			}

			if (list_quick_attribute.length > 0) {
				list_quick_attribute[0].classList.add('select');
			}
			document.getElementById('quick_quantity').value = 1

			console.log(response);
		},
		error: function (xhr, status, error) {
			console.error('Error: ' + error);
		}
	});
}

function quick_size_selected(index, size_id, productOption) {
	console.log(productOption)
	var list_li = document.getElementById('quick_size').querySelectorAll('li');
	list_li.forEach(function (li) {
		li.classList.remove('select');
	});
	list_li[index].classList.add('select');

	var attribute_id
	try {
		attribute_id = document.getElementById('quick_attribute').querySelectorAll('li.select')[0].getAttribute('data')
	} catch (error) {
		attribute_id = 1
	}

	quick_updatePriceAndImage(size_id, attribute_id, productOption);
}

function quick_attribute_selected(index, attribute_id, productOption) {
	console.log(productOption)
	var list_li = document.getElementById('quick_attribute').querySelectorAll('li');
	list_li.forEach(function (li) {
		li.classList.remove('select');
	});
	list_li[index].classList.add('select');

	var size_id
	try {
		size_id = document.getElementById('quick_size').querySelectorAll('li.select')[0].getAttribute('data')
	} catch (error) {
		size_id = 1
	}

	quick_updatePriceAndImage(size_id, attribute_id, productOption);
}

function quick_updatePriceAndImage(size_id, attribute_id, productOptions_json) {
	console.log(size_id)
	console.log(attribute_id)
	var quick_price, quick_img_url;

	for (var id = 0; id < productOptions_json.length; id++) {
		if (productOptions_json[id]['attribute']['attributeId'] == attribute_id && productOptions_json[id]['size']['sizeId'] == size_id) {
			quick_price = productOptions_json[id].price;
			quick_img_url = productOptions_json[id].img_url;
			$('#quick_add_to_cart').attr('data-product-option-id', productOptions_json[id].id)
			break;
		}
	}
	console.log(quick_img_url)

	document.querySelector('#quick_image img').setAttribute('src', quick_img_url);
	document.getElementById('quick_price').innerText = quick_price;
	quick_total_price();
}

function quick_total_price() {
	var quick_quantity = parseFloat(document.getElementById('quick_quantity').value);
	var quick_price = parseFloat(document.getElementById('quick_price').innerText);
	console.log(quick_price)
	var quick_amount = quick_price * quick_quantity;
	document.getElementById("quick_amount").innerText = quick_amount;
}

