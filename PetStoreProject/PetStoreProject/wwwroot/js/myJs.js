﻿window.onload = function () {
    getCartBoxItems();
};
function addToCart(productOptionId, quantity) {
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
    $('#myModal').modal('hide');
}

function getCartBoxItems() {
    $.ajax({
        type: "POST",
        url: "/cart/GetCartBoxItems",
        success: function (response) {
            console.log(response.length)
            $('#list_item').empty();
            $('#total_item').html(response.length)
            let total_price = 0;
            if (response.length == 0) {
                $('#list_item').html('<p>Không có sản phẩm nào trong giỏ hàng</p>')
            }
            else {
                for (const element of response) {
                    let divSingleCart = $('<div>', {
                        class: "single-cart-box"
                    })

                    let divCartImg = $('<div>', {
                        class: "cart-img"
                    })

                    let divCartContent = $('<div>', {
                        class: "cart-content"
                    })

                    let img = $('<img>', {
                        src: element.imgUrl,
                        alt: "cart-image"
                    })

                    let h6 = $('<h6>', {
                        style: "margin: 0px"
                    })
                    let aTitle = $('<a>', {
                        href: 'http://localhost:5206/product/detail/' + element.productId
                    }).text(element.name)
                    let option = "";
                    if (!(element.size.name == null && element.attribute.name == null)) {
                        if (element.size.name != null && element.attribute.name != null) {
                            option += element.size.name + ', ' + element.attribute.name
                        }
                        else if (element.size.name == null) {
                            option += element.attribute.name
                        } else {
                            option += element.size.name
                        }
                    }
                    let spanOption = $('<div>', {
                        style: "font-size: 13px; margin-top: 3px; margin-bottom: 3px"
                    }).text(option)
                    let spanPrice = $('<span>').text(element.price.toLocaleString('en-US') + ' x ' + element.quantity)
                    h6.append(aTitle)
                    divCartImg.append(img)
                    divCartContent.append(h6)
                    divCartContent.append(spanOption)
                    divCartContent.append(spanPrice)

                    divSingleCart.append(divCartImg)
                    divSingleCart.append(divCartContent)
                    $('#list_item').append(divSingleCart)

                    total_price += parseFloat(element.price) * parseFloat(element.quantity)
                }
            }
            let x = total_price.toLocaleString('en-US') + " VND"
            $('#total_price').html(x);
            
        },
        error: function (xhr, status, error) {
            console.error('Error: ' + error);
        }
    });
}

function deleteCartItem(productOptionId) {
    $('#' + productOptionId).remove()
    let list = document.getElementsByClassName('item');
    if (list.length == 0) {
        let x = `
                <div class="container" style="width: 100%; align-items: center; padding-top: 20px">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="alert alert-warning" role="alert">
                                <h4 class="alert-heading">Giỏ hàng trống!</h4>
                                <p>Giỏ hàng của bạn đang trống, hãy chọn sản phẩm để mua sắm.</p>
                                <hr>
                                <p class="mb-0">
                                    Nhấn vào <a href="/home/index" style="text-decoration:underline">đây</a> để quay lại trang chủ.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
                `;
        $('#cart_content').html(x);
    }
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
    let total_amount = 0.0;
    let subTotalElements = document.getElementsByClassName('product-subtotal');
    for (let i = 1; i < subTotalElements.length; i++) {
        total_amount += parseFloat(subTotalElements[i].innerText.replace(/,/g, ''))
    }
    $('#amount').html(total_amount.toLocaleString('en-US'))
}

function quickEditCartItem(oldProductOptionId, productId) {
    $('#quick_add_to_cart').attr('data-old-product-option-id', oldProductOptionId)
    quickView(productId)
    $('#myModal').modal('show');
}
function editCartItem(oldProductOptionId, newProductOptionId, quantity) {
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
    let option = "";
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
    let row = $('#' + oldId);
    // Update the product name and link
    row.find('.product-name a').attr('href', 'http://localhost:5206/product/detail/' + cartItem.productId)
        .html(cartItem.name + '</br>' + option);

    // Update the product image
    row.find('.product-thumbnail img').attr('src', cartItem.imgUrl);

    // Update the product price
    row.find('.product-price .amount').text(cartItem.price.toLocaleString('en-US'));

    // Update the product quantity
    row.find('.product-quantity input').val(cartItem.quantity);

    // Update the product subtotal
    let sub_total = parseFloat(cartItem.price) * parseFloat(cartItem.quantity)
    row.find('.product-subtotal').text(sub_total.toLocaleString('en-US'))

    // Update the edit button
    row.find('a[title="Chỉnh sửa"]').attr('onclick', `quickEditCartItem(${cartItem.productOptionId}, ${cartItem.productId})`);

    // Update the delete button
    row.find('a[title="Xóa sản phẩm"]').attr('onclick', `deleteCartItem(${cartItem.productOptionId})`);
    row.attr('id', cartItem.productOptionId)
}

function quickView(productId) {
    $('#myModal').modal('show');
    $('#quick_attribute').empty();
    $('#quick_size').empty();
    $.ajax({
        type: "POST",
        url: "/Product/quickPreview",
        data: { productId: productId },
        success: function (response) {
            $('#quick_name').html(response.name);
            $('#quick_price').html(response.productOption[0].price.toLocaleString('en-US'));
            $('#quick_image').empty();
            $('#quick_amount').html(response.productOption[0].price.toLocaleString('en-US'));
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
            let jsonStr = JSON.stringify(response.productOption);

            if (response.attributes[0].name != null) {
                let divContainer = $('<div>', {
                    class: 'color clearfix mb-30'
                });
                let label = $('<label>').text('Loại :');
                let ul = $('<ul>', {
                    class: 'color-list scroll'
                });
                let list = ""
                for (let index = 0; index < response.attributes.length; index++) {
                    if (response.attributes[index].name != null) {
                        list += "<li id='quick_attribute_" + response.attributes[index].attributeId + "' style = 'cursor:pointer' onclick='quick_attribute_selected(" + index + "," + response.attributes[index].attributeId + "," + jsonStr + ")'" + "data = '" + response.attributes[index].attributeId + "'>" + response.attributes[index].name + "</li>"
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
                let sizeLabel = $('<label>').text('Kích thước :');
                let sizeDiv = $('<div>', {
                    class: 'color-list size_list'
                });
                let sizeUl = $('<ul>', {
                    class: 'color-list size_list scroll'
                });

                let list = ""
                for (let index = 0; index < response.sizes.length; index++) {
                    if (response.sizes[index].name != null) {
                        list += "<li id='quick_size_" + response.sizes[index].sizeId + "' style = 'cursor:pointer' onclick='quick_size_selected(" + index + "," + response.sizes[index].sizeId + "," + jsonStr + ")'" + "data = '" + response.sizes[index].sizeId + "'>" + response.sizes[index].name + "</li>"
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
            let isStill = false;
            let attributeId, sizeId, price, img_url;
            for (const element of response.productOption) {
                if (element.status) { // status is boolean
                    attributeId = element.attribute.attributeId;
                    sizeId = element.size.sizeId;
                    price = element.price;
                    img_url = element.img_url;
                    isStill = true
                    break;
                }

            }
            // updatePriceAndImage(); // Uncomment this line if you have a function to update price and image
            if (isStill) {
                console.log('Con hang')
                let list_size = document.getElementById('quick_size').querySelectorAll('li');
                let list_attribute = document.getElementById('quick_attribute').querySelectorAll('li');

                if (list_size.length > 0) {
                    var sizeElement = document.getElementById('quick_size_' + sizeId);
                    if (sizeElement) {
                        sizeElement.classList.add('select');
                    } else {
                        console.warn('Size element not found: size_' + sizeId);
                    }
                }

                if (list_attribute.length > 0) {
                    var attributeElement = document.getElementById('quick_attribute_' + attributeId);
                    if (attributeElement) {
                        attributeElement.classList.add('select');
                    } else {
                        console.warn('Attribute element not found: attribute_' + attributeId);
                    }
                }
                if (list_attribute.length > 0 || list_size.length > 0) {
                    quickUpdatePriceAndImage(sizeId, attributeId, response.productOption)
                }
            }
            else {
                $('#quick_add_to_cart').addClass('out-of-stock');
                quickCheckOutOfStock(sizeId, attributeId, response.productOption);
            }
            document.getElementById('quick_quantity').value = 1

        },
        error: function (xhr, status, error) {
            console.error('Error: ' + error);
        }
    });
}

function quickCheckOutOfStock(sizeId, attributeId, productOptions) {
    let list_size = document.getElementById('quick_size').querySelectorAll('li');
    let list_attribute = document.getElementById('quick_attribute').querySelectorAll('li');
    if (list_size.length > 0 && list_attribute.length > 0) {
        for (const element of productOptions) {
            if (!element.status) {
                console.log(element.status)
                if (element.attribute.attributeId == attributeId) {
                    $('#quick_size_' + element.size.sizeId).addClass('out-of-stock')
                }
                else if (element.size.sizeId == sizeId) {
                    $('#quick_attribute_' + element.attribute.attributeId).addClass('out-of-stock')
                }
            }
        }
    }
    else if (list_attribute.length > 0) {
        for (const element of productOptions) {
            if (!element.status) {
                $('quick_attribute_' + element.attribute.attributeId).addClass('out-of-stock')
            }
        }
    }
    else if (list_size.length > 0) {
        for (const element of productOptions) {
            if (!element.status) {
                $('#quick_size_' + element.size.sizeId).addClass('out-of-stock')
            }
        }
    }
}

function quick_size_selected(index, size_id, productOption) {
    let list_size = document.getElementById('quick_size').querySelectorAll('li');
    let list_attr = document.getElementById('quick_attribute').querySelectorAll('li');
    list_size.forEach(function (li) {
        li.classList.remove('select');
        li.classList.remove('out-of-stock')
    });
    list_attr.forEach(function (li) {
        li.classList.remove('out-of-stock')
    });
    list_size[index].classList.add('select');

    let attribute_id
    try {
        attribute_id = document.getElementById('quick_attribute').querySelectorAll('li.select')[0].getAttribute('data')
    } catch (error) {
        attribute_id = 1
    }

    quickUpdatePriceAndImage(size_id, attribute_id, productOption);
}

function quick_attribute_selected(index, attribute_id, productOption) {
    let list_attr = document.getElementById('quick_attribute').querySelectorAll('li');
    let list_size = document.getElementById('quick_size').querySelectorAll('li');
    list_attr.forEach(function (li) {
        li.classList.remove('select');
        li.classList.remove('out-of-stock')
    });
    list_size.forEach(function (li) {
        li.classList.remove('out-of-stock')
    });
    list_attr[index].classList.add('select');

    let size_id
    try {
        size_id = document.getElementById('quick_size').querySelectorAll('li.select')[0].getAttribute('data')
    } catch (error) {
        size_id = 1
    }

    quickUpdatePriceAndImage(size_id, attribute_id, productOption);
}

function quickUpdatePriceAndImage(size_id, attribute_id, productOptions_json) {
    let quick_price, quick_img_url;

    for (const element of productOptions_json) {
        if (element['attribute']['attributeId'] == attribute_id && element['size']['sizeId'] == size_id) {
            quick_price = element.price;
            quick_img_url = element.img_url;
            $('#quick_add_to_cart').attr('data-product-option-id', element.id)
            break;
        }
    }
    document.querySelector('#quick_image img').setAttribute('src', quick_img_url);
    document.getElementById('quick_price').innerText = quick_price.toLocaleString('en-US');
    quick_total_price();
    quickCheckOutOfStock(size_id, attribute_id, productOptions_json);
}

function quick_total_price() {
    var quick_quantity = parseFloat(document.getElementById('quick_quantity').value);
    var quick_price = parseFloat(document.getElementById('quick_price').innerText.replace(/,/g, ''));
    var quick_amount = quick_price * quick_quantity;
    document.getElementById("quick_amount").innerText = quick_amount.toLocaleString('en-US');
}

