window.onload = function () {
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
                showNotification('Thêm sản phẩm thành công', 'green');
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
            $('#list_item').empty();
            $('#total_item').html(response.length)
            let total_price = 0;
            if (response.length == 0) {
                $('#list_item').html('<h5>Không có sản phẩm nào trong giỏ hàng</h5>')
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
                <div class="row justify-content-center">
                    <div class="col-md-6 col-sm-10 text-center">
                        <div class="text-center">
                        <img style="width: 70%" src="/img/notfound/cart-empty.jpg" alt="Giỏ hàng trống" />
                        </div>
                        <div class="buttons-cart d-inline-block ">
                            <a style="margin:0" href="/">Tiếp tục mua sắm</a>
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
    showNotification('Sản phẩm đã được xóa', 'red')
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
                showNotification('Sản phẩm đã được lưu', 'green');
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
    $('#quick_add_to_cart').removeClass('out-of-stock');
    $('#quick_add_to_cart').html('Thêm vào giỏ hàng');
    $('#quick_quantity').attr('readonly', false);
    $('#myModal').modal('show');
    $('#quick_attribute').empty();
    $('#quick_size').empty();
    $.ajax({
        type: "POST",
        url: "http://localhost:5206/Product/quickPreview",
        data: { productId: productId },
        success: function (response) {
            console.log(response.productOption)
            $('#quick_name').html(response.name);
            $('#quick_price').html(response.productOption[0].price.toLocaleString('en-US'));
            $('#quick_image').empty();
            $('#quick_amount').html(response.productOption[0].price.toLocaleString('en-US'));
            $('#quick_add_to_cart').attr('data-product-option-id', response.productOption[0].id)

            let imgDiv = $('<div>', {
                class: 'tab-pane large-img-style active'
            });
            let imgElement = $('<img>', {
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
                for (let index = 0; index < response.attributes.length;) {
                    if (response.attributes[index].name != null) {
                        list += "<li id='quick_attribute_" + response.attributes[index].attributeId + "' style = 'cursor:pointer' onclick='quick_attribute_selected(" + index + "," + response.attributes[index].attributeId + "," + jsonStr + ")'" + "data = '" + response.attributes[index].attributeId + "'>" + response.attributes[index].name + "</li>"
                        index += 1
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
                for (let index = 0; index < response.sizes.length;) {
                    if (response.sizes[index].name != null) {
                        list += "<li id='quick_size_" + response.sizes[index].sizeId + "' style = 'cursor:pointer' onclick='quick_size_selected(" + index + "," + response.sizes[index].sizeId + "," + jsonStr + ")'" + "data = '" + response.sizes[index].sizeId + "'>" + response.sizes[index].name + "</li>"
                        index += 1
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
            let attributeId, sizeId;
            for (const element of response.productOption) {
                if (!element.isSoldOut) { // status is boolean
                    attributeId = element.attribute.attributeId;
                    sizeId = element.size.sizeId;
                    isStill = true
                    break;
                }

            }
            // updatePriceAndImage(); // Uncomment this line if you have a function to update price and image
            if (isStill) {
                let list_size = document.getElementById('quick_size').querySelectorAll('li');
                let list_attribute = document.getElementById('quick_attribute').querySelectorAll('li');

                if (list_size.length > 0) {
                    let sizeElement = document.getElementById('quick_size_' + sizeId);
                    if (sizeElement) {
                        sizeElement.classList.add('select');
                    } else {
                        console.warn('Size element not found: size_' + sizeId);
                    }
                }

                if (list_attribute.length > 0) {
                    let attributeElement = document.getElementById('quick_attribute_' + attributeId);
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
                let divSoldOut = $('<div>', {
                    class: 'overlay'
                }).text('Hết hàng')
                imgDiv.append(divSoldOut);
                imgElement.addClass('out-of-stock');
                $('#quick_add_to_cart').addClass('out-of-stock');
                $('#quick_add_to_cart').html('Đã bán hết');
                quickCheckOutOfStock(sizeId, attributeId, response.productOption);
                $('#quick_quantity').attr('readonly', true);
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
            if (element.isSoldOut) {
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
            if (element.isSoldOut) {
                $('#quick_attribute_' + element.attribute.attributeId).addClass('out-of-stock')
            }
        }
    }
    else if (list_size.length > 0) {
        for (const element of productOptions) {
            if (element.isSoldOut) {
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
    let quick_quantity = parseFloat(document.getElementById('quick_quantity').value);
    let quick_price = parseFloat(document.getElementById('quick_price').innerText.replace(/,/g, ''));
    let quick_amount = quick_price * quick_quantity;
    document.getElementById("quick_amount").innerText = quick_amount.toLocaleString('en-US');
}

function validateQuantity(input) {
    let value = parseInt(input.value);

    if (isNaN(value) || value < 1 || value > 10) {
        input.value = 1; // Đặt giá trị về 1 khi nhập sai
    }
}

function showNotification(message, color) {
    let notification = document.getElementById('notification');
    notification.innerText = message;
    notification.style.backgroundColor = color;
    notification.style.display = 'block';
    setTimeout(function () {
        notification.style.display = 'none';
    }, 2000);
}

function generateProductList(products) {
    let productList = $('#product-list');
    productList.empty();
    products.forEach(function (product) {
        let productItem = `
                        <li class="product-item gap14">
                            <div class="image no-bg">
                                <img src="${product.imgUrl}" alt="">
                            </div>
                            <div class="flex items-center justify-between gap20 flex-grow">
                                <div class="name">
                                    <a href="product-list.html" class="body-title-2">${product.name}</a>
                                </div>
                                <div class="body-text">#${product.id}</div>
                                <div class="body-text">${product.price.toLocaleString()} VND</div>
                                <div class="body-text">${product.soldQuantity}</div>
                                <div>
                                    <div class="${product.isSoldOut ? 'block-not-available' : 'block-available'}">
                                        ${product.isSoldOut ? 'Hết hàng' : 'Còn hàng'}
                                    </div>
                                </div>
                                <div class="list-icon-function">
                                    <div class="item eye" data-bs-toggle="modal" data-bs-target="#myModal" title="Xem chi tiết" onclick="quickView(${product.id})">
                                        <i class="icon-eye"></i>
                                    </div>
                                    <div class="item edit">
                                        <i class="icon-edit-3"></i>
                                    </div>
                                    <div class="item trash">
                                        <i class="icon-trash-2"></i>
                                    </div>
                                </div>
                            </div>
                        </li>
                    `;
        productList.append(productItem);
    });
}

function fetchProducts(pageSize, pageNumber) {
    $.ajax({
        url: 'http://localhost:5206/admin/product/fetchproduct', // Replace with your API endpoint
        type: 'POST',
        data: { pageSize: pageSize, pageNumber: pageNumber },
        success: function (response) {
            generateProductList(response.products);
            generatePageSize(response.pageSize, response.pageNumber);
            generatePagination(response.totalPageNumber, response.pageNumber, response.pageSize)
            console.log(response)
        },
        error: function (xhr, status, error) {
            console.error('Error fetching products:', error);
        }
    });
}

function generatePagination(totalPageNumber, currentPage, pageSize) {
    let parentElement = $('#pageNumber');
    parentElement.empty();
    if (totalPageNumber > 1) {
        if (currentPage > 1) {
            let prevPage = currentPage - 1;
            let prevPageLink = document.createElement('a');
            prevPageLink.setAttribute('class', 'page-link');
            prevPageLink.setAttribute('onclick', 'ChoosePage(' + prevPage + ', ' + pageSize + ')');
            prevPageLink.innerText = 'Trang trước';
            let prevPageItem = document.createElement('li');
            prevPageItem.setAttribute('class', 'page-item');
            prevPageItem.appendChild(prevPageLink);
            parentElement.append(prevPageItem);
        }

        let startPage = Math.max(1, currentPage - 2);
        let endPage = Math.min(totalPageNumber, currentPage + 2);

        if (startPage > 1) {
            let firstPageLink = document.createElement('a');
            firstPageLink.setAttribute('class', 'page-link');
            firstPageLink.setAttribute('onclick', 'ChoosePage(1, ' + pageSize + ')');
            firstPageLink.innerText = '1';
            let firstPageItem = document.createElement('li');
            firstPageItem.setAttribute('class', 'page-item');
            firstPageItem.appendChild(firstPageLink);
            parentElement.append(firstPageItem);

            if (startPage > 2) {
                let ellipsisItem = document.createElement('li');
                ellipsisItem.setAttribute('class', 'page-item');
                ellipsisItem.innerHTML = '<a class="page-link">...</a>';
                parentElement.append(ellipsisItem);
            }
        }

        for (let i = startPage; i <= endPage; i++) {
            if (currentPage == i) {
                let activePageItem = document.createElement('li');
                activePageItem.setAttribute('class', 'page-item active');
                activePageItem.innerHTML = '<a class="page-link">' + i + '</a>';
                parentElement.append(activePageItem);
            } else {
                let pageLink = document.createElement('a');
                pageLink.setAttribute('class', 'page-link');
                pageLink.setAttribute('onclick', 'ChoosePage(' + i + ', ' + pageSize + ')');
                pageLink.innerText = i;
                let pageItem = document.createElement('li');
                pageItem.setAttribute('class', 'page-item');
                pageItem.appendChild(pageLink);
                parentElement.append(pageItem);
            }
        }

        if (totalPageNumber >= endPage + 1) {
            if (totalPageNumber >= endPage + 2) {
                let ellipsisItem = document.createElement('li');
                ellipsisItem.setAttribute('class', 'page-item');
                ellipsisItem.innerHTML = '<a class="page-link">...</a>';
                parentElement.append(ellipsisItem);
            }

            let lastPageLink = document.createElement('a');
            lastPageLink.setAttribute('class', 'page-link');
            lastPageLink.setAttribute('onclick', 'ChoosePage(' + totalPageNumber + ', ' + pageSize + ')');
            lastPageLink.innerText = totalPageNumber;
            let lastPageItem = document.createElement('li');
            lastPageItem.setAttribute('class', 'page-item');
            lastPageItem.appendChild(lastPageLink);
            parentElement.append(lastPageItem);
        }

        if (currentPage != totalPageNumber) {
            let nextPage = currentPage + 1;
            let nextPageLink = document.createElement('a');
            nextPageLink.setAttribute('class', 'page-link');
            nextPageLink.setAttribute('onclick', 'ChoosePage(' + nextPage + ', ' + pageSize + ')');
            nextPageLink.innerText = 'Trang sau';
            let nextPageItem = document.createElement('li');
            nextPageItem.setAttribute('class', 'page-item');
            nextPageItem.appendChild(nextPageLink);
            parentElement.append(nextPageItem);
        }
    }
}

function generatePageSize(pageSize, pageNumber) {
    let p = $('#pageSize')
    let pS = $('<select>', {
        id: 'size',
        onchange: 'ChoosePageSize(' + pageNumber + ')'
    })
    p.empty();
    let pageSizeList = [10, 20, 30];
    pageSizeList.forEach(function (size) {
        let option = `<option value="${size}" ${pageSize==size?'selected':''}>${size}</option>`;
        pS.append(option);
    });
    p.append(pS);
}
