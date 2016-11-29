/// <reference path="typings/jquery/jquery.d.ts" />

function PTCController($scope, $http) {

    // the $parameters are extensions we are injecting during the constuctor
    // and are used in various ways.
    // $scope is our viewmodel
    var vm = $scope;
    // $http is our webapi access
    var dataService = $http;

    // our viewmodel contains an object called uiState
    vm.uiState = {};

    // an array called products to hold for all known products
    vm.products = [];

    // for single selected product
    vm.product = {};

    // private vars
    const pageMode = {
        LIST: 'List',
        EDIT: 'Edit',
        ADD: 'Add',
        EXCEPTION: 'Exception',
        VALIDATION: 'Validation'
    };

    /* 
    CLICK EVENT HANDLERS
    */
    // expose the click event functions through the Angular $scope

    vm.addClick = () => { vm.product = initEntity(); setUIState(pageMode.ADD); };
    vm.cancelClick = () => { setUIState(pageMode.LIST); };
    vm.editClick = (id: number) => { get(id); };
    vm.deleteClick = (id: number) => { deleteData(id); };

    // add validation check on form before saveData()
    vm.saveClick = () => {
        if (!vm.productForm.$valid) {
            vm.uiState.isMessageAreaVisible = true;
        } else {
            vm.productForm.$setPristine();    // reset form internal fields to a valid state
            saveData();
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

    /*
        DATASERVICE AREA
    */

    // this inserts or updated based on current $scope.uiState.Mode
    function saveData() {

        switch (vm.uiState.mode) {

            case pageMode.ADD:
                insertData();
                break;

            case pageMode.EDIT:
                updateData();
                break;

        }

        //if (vm.uiState.mode === pageMode.ADD) {
        //    insertData();
        //} else if (vm.uiState.mode === pageMode.EDIT) {
        //    updateData();
        //}
    }

    function getAll() {

        vm.uiState.isLoading = true;
        // .then(validResponseCallback, errorResponseCallback) is async
        dataService.get("/api/Product/")
            .then(result => {
                vm.products = result.data;
            }, error => { handleException(error); })
            .finally(() => { vm.uiState.isLoading = false; });
    }

    function get(id: number) {
        vm.uiState.isLoading = true;

        dataService.get("/api/Product/" + id)
            .then(response => {
                vm.product = response.data;
                vm.product.IntroductionDate = new Date(vm.product.IntroductionDate).toLocaleDateString();
                setUIState(pageMode.EDIT);
            },
            // called asynchronously if an error occurs or server returns response with an error status.
            error => { handleException(error); })
            .finally(() => { vm.uiState.isLoading = false; });
    }

    function insertData() {
        if (validateDate()) {
            dataService.post("/api/Product/", vm.product)
                .then(
                function (result) {
                    vm.product = result.data;
                    vm.products.push(vm.product);
                    setUIState(pageMode.LIST);
                },
                error => { handleException(error); });
        }
    }

    function updateData() {

        if (validateDate()) {
            dataService.put("/api/Product/" + vm.product.ProductId, vm.product)
                .then(
                result => {
                    // update the product object in memory
                    vm.product = result.data;

                    // get the index of this particular object as it sits in the local array
                    var prodId = vm.product.ProductId;
                    var index = vm.products.map(function (p) { return p.ProductId; }).indexOf(prodId);

                    // update product in the products array in memory
                    vm.products[index] = vm.product;
                    setUIState(pageMode.LIST);
                },// if http status code doesn't lie betwwn 200 and 299, this will run.
                error => {
                    // if http status code doesn't lie betwwn 200 and 299, this will run.
                    handleException(error);
                });
        }
    }

    function deleteData(id: number) {
        if (confirm("Delete this Product?")) {
            dataService.delete("/api/Product/" + id)
                .then(function (result) {

                    // get index of this
                    var index = vm.products.map(function (p) {
                        return p.ProductId;
                    })
                        .indexOf(id);
                    // remove from product array
                    vm.products.splice(index, 1);

                    setUIState(pageMode.LIST);
                },
                error => { handleException(error); });
        }
    }


    // help our the add method get good starter data
    function initEntity() {
        return {
            ProductId: 0,
            ProductName: '',
            IntroductionDate: new Date().toLocaleDateString(),
            Url: 'http://www.microsoft.com',
            Price: 0.00
        };
    }


    function validateDate() {
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

