/// <reference path="typings/jquery/jquery.d.ts" />
function PTCController($scope, $http) {
    var vm = $scope;
    var dataService = $http;
    vm.uiState = {};
    // for all known products
    vm.products = [];
    // for single selected product
    vm.product = {};
    // private vars
    var pageMode = {
        LIST: 'List',
        EDIT: 'Edit',
        ADD: 'Add',
        EXCEPTION: 'Exception',
        VALIDATION: 'Validation'
    };
    // expose the click evetn functions through the Angular $scope
    vm.addClick = addClick;
    vm.cancelClick = cancelClick;
    vm.editClick = editClick;
    vm.deleteClick = deleteClick;
    vm.saveClick = saveClick;
    init();
    function getAll() {
        vm.uiState.isLoading = true;
        dataService.get("/api/Product/").then(function (response) {
            vm.products = response.data;
        }, function (response) {
            handleException(response);
        }).finally(function () {
            vm.uiState.isLoading = false;
        });
    }
    function get(id) {
        vm.uiState.isLoading = true;
        dataService.get("/api/Product/" + id).then(function (response) {
            vm.product = response.data;
            // clean up the date so it matches the browser settings
            // ReSharper disable once TsNotResolved
            vm.product.IntroductionDate = new Date(vm.product.IntroductionDate).toLocaleDateString();
            setUIState(pageMode.EDIT);
        }, function (error) {
            handleException(error);
        }).finally(function () {
            vm.uiState.isLoading = false;
        });
    }
    function setUIState(state) {
        // store the state 
        vm.uiState.mode = state;
        // do some things based on that state
        switch (state) {
            case pageMode.LIST:
                vm.uiState.isDetailAreaVisible = false;
                vm.uiState.isListAreaVisible = true;
                vm.uiState.isSearchAreaVisible = true;
                vm.uiState.isMessageAreaVisible = false;
                break;
            case pageMode.ADD:
                vm.uiState.isDetailAreaVisible = true;
                vm.uiState.isListAreaVisible = false;
                vm.uiState.isSearchAreaVisible = false;
                vm.uiState.messages = [];
                break;
            case pageMode.EDIT:
                vm.uiState.isDetailAreaVisible = true;
                vm.uiState.isListAreaVisible = false;
                vm.uiState.isSearchAreaVisible = false;
                vm.uiState.messages = [];
                break;
            case pageMode.EXCEPTION:
                vm.uiState.isMessageAreaVisible = true;
                break;
            case pageMode.VALIDATION:
                vm.uiState.isMessageAreaVisible = true;
                break;
        }
    }
    // CLICK EVENT HANDLERS
    function addClick() {
        vm.product = initEntity();
        setUIState(pageMode.ADD);
    }
    // help our the add method get good starter data
    function initEntity() {
        return {
            ProductId: 0,
            ProductName: '',
            IntroductionDate: new Date().toLocaleDateString(),
            Url: 'http://www.pdsa.com',
            Price: 0.00
        };
    }
    function cancelClick() {
        setUIState(pageMode.LIST);
    }
    function editClick(id) {
        get(id);
    }
    function deleteClick(id) {
        deleteData(id);
    }
    function deleteData(id) {
        if (confirm("Delete this Product?")) {
            dataService.delete("/api/Product/" + id)
                .then(function (result) {
                // get index of this
                var index = vm.products.map(function (p) { return p.ProductId; }).indexOf(id);
                // remove from product array
                vm.products.splice(index, 1);
                setUIState(pageMode.LIST);
            }, function (error) { handleException(error); });
        }
    }
    function saveClick() {
        saveData();
    }
    function saveData() {
        // todo: save data here.
        // insert or update the data
        if (vm.uiState.mode === pageMode.ADD) {
            insertData();
        }
        else if (vm.uiState.mode === pageMode.EDIT) {
            updateData();
        }
        // after action, see if there was a problem. else set to LIST
        if (vm.uiState.mode === pageMode.EXCEPTION || vm.uiState.mode === pageMode.VALIDATION) {
            // check for validation message
            setUIState(vm.uiState.mode);
        }
        else {
            // when go back to page, by default display a list
            setUIState(pageMode.LIST);
        }
    }
    function insertData() {
        if (validate()) {
            dataService.post("/api/Product/", vm.product).then(function (result) {
                // update product object
                vm.product = result.data;
                // add product to array
                vm.products.push(vm.product);
                setUIState(pageMode.LIST);
            }, function (error) {
                handleException(error);
            });
        }
    }
    function updateData() {
        if (validate()) {
            dataService.put("/api/Product/" + vm.product.ProductId, vm.product)
                .then(function (result) {
                // update the product object in memory
                vm.product = result.data;
                // get the index of this particular object as it sits in the local array
                var prodId = vm.product.ProductId;
                var index = vm.products.map(function (p) { return p.ProductId; }).indexOf(prodId);
                // update product in the products array in memory
                vm.products[index] = vm.product;
                setUIState(pageMode.LIST);
            }, function (error) {
                handleException(error);
            });
        }
    }
    function validate() {
        var ret = true;
        // fix up the date (assume is actually a date) to correct date to UTC (due to JSON formatting)
        if (vm.product.IntroductionDate != null) {
            var cleanedDate = vm.product.IntroductionDate.replace(/\u200E/g, "");
            vm.product.IntroductionDate = new Date(cleanedDate).toISOString();
            if (vm.product.IntroductionDate == null) {
                vm.uiState.mode = pageMode.VALIDATION;
                ret = false;
            }
        }
        return ret;
        // simulate a data validation error
        //vm.uiState.mode = pageMode.VALIDATION;
        //vm.uiState.messages.push({
        //    property: 'ProductName',
        //    message: 'Product Name must be filled in.'
        //});
        //vm.uiState.messages.push({
        //    property: 'Url',
        //    message: 'Url must be filled in.'
        //});
        // return false;
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
    function init() {
        vm.uiState = initUIState(); // first full object injection.
        setUIState(pageMode.LIST);
        // get all the products
        getAll();
    }
    // need handler for exceptions 
    // if http status code doesn't lie betwwn 200 and 299, this will run.
    function handleException(error) {
        // get error information
        var msg = {
            property: error.status,
            message: error.statusText
        };
        // add any additional error text to the message
        if (error.data != null) {
            msg.message += ' - ' + error.data.ExceptionMessage;
        }
        // reset the array and add the new error messages
        vm.uiState.messages = [];
        vm.uiState.messages.push(msg);
        // set the UI state to Exception for Validation so we can affect the layout
        setUIState(pageMode.EXCEPTION);
    }
}
//# sourceMappingURL=app.js.map