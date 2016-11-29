/// <reference path="typings/jquery/jquery.d.ts" />

function PTCController($scope, $http) {

    // the $parameters are extensions we are injecting during the constuctor
    // and are used in various ways.
    // $scope is our viewmodel
    var viewModel = $scope;
    // $http is our webapi access
    var webApi = $http;

    // a type of Enum
    const pageMode = {
        LIST: 'List',
        EDIT: 'Edit',
        ADD: 'Add',
        EXCEPTION: 'Exception',
        VALIDATION: 'Validation'
    };

    // flesh out the viewmodel some more.
    viewModel.uiState = {};
    viewModel.product = {};
    viewModel.products = [];

    // our viewmodel contains an object called uiState for 
    // an array called products to hold for all known products
    // for single selected product




    /**
    ** CLICK EVENT HANDLERS
    ** expose the click event functions through the Angular $scope
    **/

    viewModel.addClick = () => {
        viewModel.product = initEntity();
        setUIState(pageMode.ADD);
    };
    viewModel.cancelClick = () => {
        setUIState(pageMode.LIST);
    };
    viewModel.editClick = (id: number) => {
        get(id);
    };
    viewModel.deleteClick = (id: number) => {
        deleteData(id);
    };

    // add validation check on form before saveData()
    viewModel.saveClick = () => {
        if (!viewModel.productForm.$valid) {
            viewModel.uiState.isValidationAreaVisible = true;
        } else {
            viewModel.productForm.$setPristine();    // reset form internal fields to a valid state
            saveData();
        }
    };

    init();

    /* set default UiState, get all products */
    function init() {
        viewModel.uiState = initUIState(); // first full object injection.
        setUIState(pageMode.LIST);

        // get all the products
        getAll();
    }


    // create a uistate object literal
    function initUIState() {
        return {
            mode: pageMode.LIST,
            isValidationAreaVisible: false,
            isDetailAreaVisible: false,
            isListAreaVisible: false,
            isMessageAreaVisible: false,
            isSearchAreaVisible: false,
            messages: [] // empty collection
        };
    }

    function setUIState(state) {

        const quikState = viewModel.uiState;
        // store the state 
        quikState.mode = state;

        // do some things based on that state
        switch (state) {
            case pageMode.LIST:
                quikState.isDetailAreaVisible = false;
                quikState.isListAreaVisible = true;
                quikState.isSearchAreaVisible = true;
                quikState.isMessageAreaVisible = false;
                quikState.isValidationAreaVisible = false;
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
                quikState.isValidationAreaVisible = true;
                break;
        }

    }

    /*
        DATASERVICE AREA
    */

    // this inserts or updated based on current $scope.uiState.Mode
    function saveData() {

        switch (viewModel.uiState.mode) {

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

        viewModel.uiState.isLoading = true;
        // .then(validResponseCallback, errorResponseCallback) is async
        webApi.get("/api/Product/")
            .then(result => {
                viewModel.products = result.data;
            }, error => { handleException(error); })
            .finally(() => { viewModel.uiState.isLoading = false; });
    }

    function get(id: number) {
        viewModel.uiState.isLoading = true;

        webApi.get("/api/Product/" + id)
            .then(response => {
                viewModel.product = response.data;
                viewModel.product.IntroductionDate = new Date(viewModel.product.IntroductionDate).toLocaleDateString();
                setUIState(pageMode.EDIT);
            },
            // called asynchronously if an error occurs or server returns response with an error status.
            error => { handleException(error); })
            .finally(() => { viewModel.uiState.isLoading = false; });
    }

    function insertData() {
        if (validateDate()) {
            webApi.post("/api/Product/", viewModel.product)
                .then(
                function (result) {
                    viewModel.product = result.data;
                    viewModel.products.push(viewModel.product);
                    setUIState(pageMode.LIST);
                },
                error => { handleException(error); });
        }
    }

    function updateData() {
        var p = viewModel.product;
        var ps = viewModel.products;
        if (validateDate()) {
            webApi.put("/api/Product/" + p.ProductId, p)
                .then(
                result => {
                    // update the product object in memory
                    p = result.data;

                    // get the index of this particular object as it sits in the local array
                    var prodId = p.ProductId;
                    var index = ps.map(function (x) { return x.ProductId; }).indexOf(prodId);

                    // update product in the products array in memory
                    ps[index] = viewModel.product;
                    setUIState(pageMode.LIST);
                },// if http status code doesn't lie betwwn 200 and 299, this will run.
                error => {
                    // if http status code doesn't lie betwwn 200 and 299, this will run.
                    handleException(error);
                });
        }
    }

    function deleteData(id: number) {
        var ps = viewModel.products;
        if (confirm("Delete this Product?")) {
            webApi.delete("/api/Product/" + id).then(result => {
                // remove from product array, -or- we would need to refresh from the DB.
                ps.splice(ps.map(p => p.ProductId).indexOf(id), 1);
                // show the list view
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
            Price: 0.00,
            Summary: ''
        };
    }


    function validateDate() {
        var ret = true;
        var p = viewModel.product;
        // fix up the date (assume is actually a date) to correct date to UTC (due to JSON formatting)
        if (p.IntroductionDate != null) {
            var cleanedDate = p.IntroductionDate.replace(/\u200E/g, "");
            p.IntroductionDate = new Date(cleanedDate).toISOString();

            // we may have just changed it, so look again.
            if (p.IntroductionDate == null) {
                viewModel.uiState.mode = pageMode.VALIDATION;
                ret = false;
            }
        }

        return ret;
    }



    // if http status code doesn't lie betwwn 200 and 299, this will run.
    function handleException(error) {
        // get error information
        var msg = {
            property: error.status,
            message: error.statusText
        };

        // add any additional error text to the message
        if (error.data != null && error.data != undefined) {
            msg.message += ' - ' + error.data.ExceptionMessage;
        }

        // reset the array and add the new error messages
        viewModel.uiState.messages = [];
        viewModel.uiState.messages.push(msg);

        // set the UI state to Exception for Validation so we can affect the layout
        setUIState(pageMode.EXCEPTION);
    }

}

