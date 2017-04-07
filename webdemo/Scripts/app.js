/// <reference path="typings/jquery/jquery.d.ts" />
function PTCController($scope, $http) {
    // the $parameters are extensions we are injecting during the constuctor
    // and are used in various ways.
    // $scope is our viewmodel
    var vm = $scope;
    // $http is our webapi access
    var webApi = $http;
    // a type of Enum
    var pageMode = {
        LIST: 'List',
        EDIT: 'Edit',
        ADD: 'Add',
        EXCEPTION: 'Exception',
        VALIDATION: 'Validation'
    };
    // flesh out the viewmodel some more.
    vm.uiState = {};
    vm.product = {};
    vm.products = [];
    // our viewmodel contains an object called uiState for 
    // an array called products to hold for all known products
    // for single selected product
    /**
    ** CLICK EVENT HANDLERS
    ** expose the click event functions through the Angular $scope
    **/
    vm.addClick = function () { vm.product = initEntity(); };
    vm.cancelClick = function () { setUIState(pageMode.LIST); };
    vm.editClick = function (id) { get(id); };
    vm.deleteClick = function (id) { deleteData(id); };
    // add validation check on form before saveData()
    vm.saveClick = function () {
        if (vm.productForm.$valid) {
            vm.productForm.$setPristine(); // reset form internal fields to a valid state
            saveData();
        }
        else {
            vm.uiState.isMessageAreaVisible = true;
        }
    };
    init();
    /* set default UiState, get all products */
    function init() {
        vm.uiState = initUIState(); // first full object injection.
        setUIState(pageMode.LIST);
        // get all the products
        getAll();
    }
    // create a uistate object literal
    function initUIState() {
        return {
            mode: pageMode.LIST,
            isDetailAreaVisible: false,
            isListAreaVisible: false,
            isMessageAreaVisible: false,
            isSearchAreaVisible: false,
            messages: [] // empty collection
        };
    }
    function setUIState(state) {
        var quikState = vm.uiState;
        // store the state 
        quikState.mode = state;
        // do some things based on that state
        switch (state) {
            case pageMode.LIST:
                quikState.isDetailAreaVisible = false;
                quikState.isListAreaVisible = true;
                quikState.isSearchAreaVisible = true;
                quikState.isMessageAreaVisible = false;
                break;
            case pageMode.ADD:
                quikState.isDetailAreaVisible = true;
                quikState.isListAreaVisible = false;
                quikState.isSearchAreaVisible = false;
                quikState.isMessageAreaVisible = false;
                quikState.messages = [];
                break;
            case pageMode.EDIT:
                quikState.isDetailAreaVisible = true;
                quikState.isListAreaVisible = false;
                quikState.isSearchAreaVisible = false;
                quikState.messages = [];
                break;
            case pageMode.EXCEPTION:
                quikState.isMessageAreaVisible = true;
                break;
            case pageMode.VALIDATION:
                quikState.isMessageAreaVisible = true;
                break;
        }
    }
    /*
        DATASERVICE AREA
    */
    // this inserts or updated based on current $scope.uiState.Mode
    function saveData() {
        if (vm.uiState.mode === pageMode.ADD) {
            insertData();
        }
        else if (vm.uiState.mode === pageMode.EDIT) {
            updateData();
        }
    }
    function getAll() {
        vm.uiState.isLoading = true;
        // .then(validResponseCallback, errorResponseCallback) is async
        webApi.get("/api/Product/")
            .then(function (resolveHandler) {
            vm.products = resolveHandler.data;
        }, function (rejectHandler) { handleException(rejectHandler); })
            .finally(function () { vm.uiState.isLoading = false; });
    }
    function get(id) {
        vm.uiState.isLoading = true;
        webApi.get("/api/Product/" + id)
            .then(function (response) {
            vm.product = response.data;
            vm.product.IntroductionDate = new Date(vm.product.IntroductionDate).toLocaleDateString();
            setUIState(pageMode.EDIT);
        }, 
        // called asynchronously if an error occurs or server returns response with an error status.
        function (error) { handleException(error); })
            .finally(function () { vm.uiState.isLoading = false; });
    }
    function insertData() {
        if (isoDate()) {
            webApi.post("/api/Product/", vm.product)
                .then(function (result) {
                vm.product = result.data;
                vm.products.push(vm.product);
                setUIState(pageMode.LIST);
            }, function (error) {
                var cleanedDate = vm.product.IntroductionDate;
                vm.product.IntroductionDate = new Date(cleanedDate).toLocaleDateString();
                handleException(error);
            });
        }
    }
    function updateData() {
        if (isoDate()) {
            webApi.put("/api/Product/" + vm.product.ProductId, vm.product)
                .then(function (result) {
                // update the product object in memory
                vm.product = result.data;
                // get the index of this particular object as it sits in the local array
                var prodId = vm.product.ProductId;
                var index = vm.products.map(function (x) { return x.ProductId; }).indexOf(prodId);
                // update product in the products array in memory
                vm.products[index] = vm.product;
                setUIState(pageMode.LIST);
            }, function (error) {
                // if http status code doesn't lie betwwn 200 and 299, this will run.
                // undo the funky ISO date display
                var cleanedDate = vm.product.IntroductionDate;
                vm.product.IntroductionDate = new Date(cleanedDate).toLocaleDateString();
                handleException(error);
            });
        }
    }
    function deleteData(id) {
        var ps = vm.products;
        if (confirm("Delete this Product?")) {
            webApi.delete("/api/Product/" + id).then(function (result) {
                // remove from product array, -or- we would need to refresh from the DB.
                ps.splice(ps.map(function (p) { return p.ProductId; }).indexOf(id), 1);
                // show the list view
                setUIState(pageMode.LIST);
            }, function (error) { handleException(error); });
        }
    }
    // help our the add method get good starter data
    function initEntity() {
        setUIState(pageMode.ADD);
        return {
            ProductId: 0,
            ProductName: '',
            IntroductionDate: new Date().toLocaleDateString(),
            Url: 'http://www.microsoft.com',
            Price: 0.00,
            Summary: ''
        };
    }
    function isoDate() {
        var ret = true;
        // fix up the date (assume is actually a date) to correct date to UTC (due to JSON formatting)
        if (vm.product.IntroductionDate != null) {
            var cleanedDate = vm.product.IntroductionDate.replace(/\u200E/g, "");
            vm.product.IntroductionDate = new Date(cleanedDate).toISOString();
            // we may have just changed it, so look again.
            if (vm.product.IntroductionDate == null) {
                vm.uiState.mode = pageMode.VALIDATION;
                ret = false;
            }
        }
        return ret;
    }
    function addValidationMessage(prop, msg) { vm.uiState.messages.push({ property: prop, message: msg }); }
    function addValidationMessages(errors) {
        for (var key in errors) {
            if (errors.hasOwnProperty(key)) {
                for (var i = 0; i < errors[key].length; i++) {
                    addValidationMessage(key, errors[key][i]);
                }
            }
        }
    }
    // if http status code doesn't lie betwwn 200 and 299, this will run.
    function handleException(error) {
        // clear the message array
        vm.uiState.messages = [];
        // sort the errors by http results
        switch (error.status) {
            case 400:
                addValidationMessages(error.data.ModelState);
                break;
            case 404:
                setUIState(pageMode.EXCEPTION);
                addValidationMessage('product', "The product you were requesting could not be found");
                break;
            case 500:
                setUIState(pageMode.EXCEPTION);
                addValidationMessage('product', "Status: " + error.status + " - Error Message: " + error.statusText);
                break;
            default:
        }
        vm.uiState.isMessageAreaVisible = (vm.uiState.messages.length > 0);
    }
    // from code-mag may-apr 2016 - extract prorperties from errors, by parsing the JSON object returned from WebAPI
    function handleAdvancedException(request, message, error) {
        var msg = '';
        switch (request.status) {
            case 500:
                setUIState(pageMode.EXCEPTION);
                msg = request.responseJSON.ExceptionMessage;
                addValidationMessage('product', "Status: " + request.status + " - Error Message: " + request.statusText);
                break;
            default:
                msg = "Status: " + request.status;
                msg += "\n" + "Error Message: " + request.statusText;
                break;
        }
        alert(msg);
    }
    // this method can be used in case 400: to get individual errors from the response
    function getModelStateErrors(errorText) {
        var response = null;
        var errors = [];
        // convert the error text from ModelState
        // into a JSON object
        try {
            response = JSON.parse(errorText);
        }
        catch (e) {
        }
        if (response != null) {
            // extract keys from the ModelState portion
            for (var key in response.ModelState) {
                // create list of error messages to display
                errors.push(response.ModelState[key]);
            }
        }
        return errors;
    }
}
//# sourceMappingURL=app.js.map