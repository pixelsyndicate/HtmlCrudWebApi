/// <reference path="typings/jquery/jquery.d.ts" />


// each of the names in this javascript object needs to be spelled exactly the same as the properties in the product class created in C# to be maped by the Web Api engine.
// this can be instatiated by JS by calling Product = new Object();
var Product = {
    ProductId: 0,
    ProductName: "",
    IntroductionDate: "",
    Url: ""
}

var productsCollection = [];


function saveToSession(value: any) {
    sessionKey = "products";
    sessionStorage.setItem(sessionKey, JSON.stringify(value));
    console.log("Saving to session storage [" + sessionKey + "] : " + JSON.stringify(value));
}



function getFromSession(key  = "products") {
    sessionKey = key;
    var fromSession = sessionStorage.getItem(sessionKey);
    var objectToReturn: any[] = JSON.parse(fromSession);
    var stringFromSession = JSON.stringify(fromSession);
    console.log("Getting from session storage [" + key + "] : " + stringFromSession);

    if (objectToReturn != null) {
        if (objectToReturn.length === 1 || objectToReturn.length == undefined)
            return objectToReturn[0];
        else
            return objectToReturn;
    }

    return objectToReturn;

}




// CLICK EVENT HANDLERS

function updateClick() {
    // build project object from inputs
    Product = <{ ProductId: number; ProductName: string; IntroductionDate: string; Url: string }>(new Object());
    Product.ProductId = $("#productid").val();
    Product.ProductName = $("#productname").val();
    Product.IntroductionDate = $("#introdate").val();
    Product.Url = $("#url").val();

    if ($("#updateButton").text().trim() == "Add") {
        productAdd(Product);
    } else {
        productUpdate(Product);
    }

    $("#addButton").removeClass("hidden");
}

function addClick() {
    $("#editPanel").removeClass("hidden");
    $("#addButton").addClass("hidden");
    $("#updateButton").text("Add");
    formClear();
}

function cancelClick() {
    formClear();
    $("#editPanel").addClass("hidden");
    $("#addButton").removeClass("hidden");
}

// CRUD OPERATIONS


function productAdd(product) {
    $.ajax({
        url: "/api/Product", // use ajax to call post method of webapi
        type: "POST", // set type to POST
        contentType: "application/json;charset=utf-8", // add contentType property to specify i'm passing JSON
        data: JSON.stringify(product), // add data property to hold the serized json
        success: function (product) {
            // add current product to local storage
            productsCollection.push(product);
            // saveToStorage("products", productsCollection);
            // try to save products to session
            saveToSession(productsCollection);
            // console.log(JSON.stringify(product));
            productAddSuccess(product);
        },
        error: function (request, message, error) {
            console.log(product);
            handleException(request, message, error);
        }
    });
}

function productUpdate(product) {
    $.ajax({
        url: "/api/Product/" + product.ProductId, // use ajax to call put method of webapi
        type: "PUT", // set type to PUT
        contentType: "application/json;charset=utf-8", // add contentType property to specify i'm passing JSON
        data: JSON.stringify(product), // add data property to hold the serized json
        success: function (product) {
            // update the object in the local collection
            $.each(productsCollection, function () {
                if (this.ProductId == product.ProductId) {
                    this.ProductName = product.ProductName;
                    this.Url = product.Url;
                    this.IntroductionDate = product.IntroductionDate;
                }
            });

            // save the collection again
            saveToStorage("products", productsCollection);

            // try to save products to session
            saveToSession(productsCollection);

            console.log("updated with : " + JSON.stringify(product));
            productUpdateSuccess(product);
        },
        error: function (request, message, error) {
            console.log(product);
            handleException(request, message, error);
        }
    });
}

function productDelete(ctl) {
    // get the product id from the data- attribute
    var id = $(ctl).data("id");

    // call webapi to delete the product
    $.ajax({
        url: "/api/Product/" + id,
        type: "DELETE",
        success: function (product) {
            var data = $.grep(productsCollection, function (e) {
                return e.ProductId !== id;
            });

            productsCollection = data;
            saveToStorage("products", productsCollection);

            // try to save products to session
            saveToSession(productsCollection);

            $(ctl).parents("tr").remove();
        },
        error: function (request, message, error) {
            console.log("Product Id: " + id);
            handleException(request, message, error);
        }
    });
}

function productGet(ctl) {
    // get the product id from the data- attribute
    var id = $(ctl).data("id");

    // store product id in the hidden field
    $("#productid").val(id);

    // only get from webapi if its not in the local
    // only call product list if the local cache is empty
    if (productsCollection.length == 0) {
        console.log("local array is empty");
    }

    var storageCollection = getFromSession("products");


    if (storageCollection == null) {
        console.log("localSession Is Empty");

        // call web api to get a single product
        $.ajax(
            {
                url: '/api/Product/' + id,
                type: 'GET',
                dataType: 'json',
                success: function (product) {
                    console.log(product);

                    productToFields(product);

                    $("#editPanel").removeClass("hidden");
                    $("#addButton").addClass("hidden");

                    // change the update button text
                    $("#updateButton").text("Update");
                },
                error: function (request, message, error) {
                    console.log("Product Id: " + id);
                    handleException(request, message, error);
                }
            });
    } else {
        console.log("localSession Has Data");

        if (storageCollection.length == undefined || storageCollection.length === 1) {
            productsCollection = [];
            productsCollection.push(storageCollection);
        } else {
            productsCollection = storageCollection;
        }


        // find it
        var foundProduct = $.grep(productsCollection, function (e) { return e.ProductId === id; });
        productToFields(foundProduct[0]);

        $("#editPanel").removeClass("hidden");
        $("#addButton").addClass("hidden");

        // change the update button text
        $("#updateButton").text("Update");

    }
}



function productList() {

    // only call product list if the local cache is empty
    if (productsCollection.length == 0) {
        console.log("local array is empty");
    }

    var storageCollection = getFromSession("products");


    if (storageCollection == null || storageCollection.length < 1) {
        console.log("localSession Is Empty");

        // call web api to get list of products
        $.ajax(
            {
                url: '/api/Product/',
                type: 'GET',
                dataType: 'json',
                success: function (products) {
                    productsCollection = products;
                    saveToSession(productsCollection);
                    productListSuccess(products);
                },
                error: function (request, message, error) {
                    handleException(request, message, error);
                }
            });

    } else {
        console.log("localSession Has Data");
        if (storageCollection.length == undefined || storageCollection.length === 1) {
            productsCollection = [];
            productsCollection.push(storageCollection);
        } else {
            productsCollection = storageCollection;
        }

        productListSuccess(productsCollection);
    }


}

function productListSuccess(products) {
    $.each(products, function (index, product) {
        // add a row to the product table
        productAddRow(product);
    });
    // throw new Error("Not implemented");
};

function productAddRow(product) {
    // check if <tbody> tag exists, add one if not
    if ($("#productTable tbody").length == 0) {
        $("#productTable").append("<tbody></tbody>");
    }

    // append row to the <table>
    $("#productTable tbody").append(productBuildTableRow(product));
}

function productBuildTableRow(product) {
    var ret = "<tr>" + "<td>" +
        "<button type='button'  " +
        " onclick='productGet(this)' " +
        " class='btn btn-default' " +
        " data-id='" + product.ProductId + "'>" +
        "<span class='glyphicon glyphicon-edit'></span>" +
        "</button>" + "</td>" +

        "<td>" + product.ProductName + "</td>" +
        "<td>" + product.IntroductionDate + "</td>" +
        "<td>" + product.Url + "</td>" +

        "<td>" +
        "<button type='button'  " +
        " onclick='productDelete(this)' " +
        " class='btn btn-default' " +
        " data-id='" + product.ProductId + "'>" +
        "<span class='glyphicon glyphicon-remove'></span>" +
        "</button>" +
        "</td>" +
        "</tr>";

    return ret;
}

function productToFields(product) {
    $("#productname").val(product.ProductName);
    $("#introdate").val(product.IntroductionDate);
    $("#url").val(product.Url);
}

function productAddSuccess(product) {
    productAddRow(product);
    $("#editPanel").addClass("hidden");
    formClear();
}

function productUpdateSuccess(product) {

    productUpdateInTable(product);
}

function productUpdateInTable(product) {
    // find product in the <table>: look for the associated button and note the first row that is a parent for it.
    var row = $("#productTable button[data-id='" + product.ProductId + "']").parents("tr")[0];

    // add changed product to the table
    $(row).after(productBuildTableRow(product));

    // remove originaly
    $(row).remove();

    formClear();

    // change the update button text
    $("#updateButton").text("Add");

    $("#editPanel").addClass("hidden");
}

function formClear() {
    $("#productname").val("");
    $("#introdate").val("");
    $("#url").val("");
}

function handleException(request, message, error) {
    var msg = "";
    msg += "Code: " + request.status + "\n"; // 404
    msg += "Text: " + request.statusText + "\n"; // Not Found
    if (request.responseJSON != null) {
        msg += "Message: " + request.responseJSON.Message + "\n";
    }
    console.log(msg);
    alert(msg);
}

function saveToStorage(key, value) {
    if (typeof (Storage) !== "undefined") {
        localStorage.setItem(key, JSON.stringify(value));
        console.log("Saving to local storage [" + key + "] : " + JSON.stringify(value));
    } else {
        console.log("local HTML5 storage is not available.");
    }
}

function getFromStorage(key) {
    if (typeof (Storage) !== "undefined") {
        var toReturn = localStorage.getItem(key);
        console.log("Retrieving from local storage [" + key + "] : " + JSON.stringify(toReturn));
        return JSON.parse(toReturn);
    } else {
        console.log("local HTML5 storage is not available.");

        return productsCollection;
    }
}

// -----------
// -- moved session methods to typescript: SessionLogic.ts
// -----------
//function saveToSession(key, value) {

//    sessionStorage.setItem(key, JSON.stringify(value));
//    console.log("Saving to session storage [" + key + "] : " + JSON.stringify(value));
//}

//function saveToSession(value) {
//    var key = "products";
//    sessionStorage.setItem(key, JSON.stringify(value));
//    console.log("Saving to session storage [" + key + "] : " + JSON.stringify(value));
//}

//function getFromSession(key) {
//    var fromSession = sessionStorage.getItem(key);
//    console.log("Getting from session storage [" + key + "] : " + JSON.stringify(fromSession));
//    return JSON.parse(fromSession);
//}

//function getFromSession() {
//    var key = "products";
//    var fromSession = sessionStorage.getItem(key);
//    console.log("Getting from session storage [" + key + "] : " + JSON.stringify(fromSession));
//    return JSON.parse(fromSession);
//}

// call the product list function after the webpage loads
$(document).ready(function () {
    productList();
    $("#editPanel").addClass("hidden");
});