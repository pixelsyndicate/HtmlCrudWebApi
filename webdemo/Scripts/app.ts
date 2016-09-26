/// <reference path="typings/jquery/jquery.d.ts" />

function PTCController($scope, $http) {

    var vm = $scope;
    var dataService = $http;

    vm.uiState = {};
    vm.products = [];

    // private vars
    const pageMode = {
        LIST: 'List',
        EDIT: 'Edit',
        ADD: 'Add',
        EXCEPTION: 'Exception',
        VALIDATION: 'Validation'
    };

    function getAll() {

        vm.uiState.isLoading = true;

        dataService.get("/api/Product").then(

            function (response) { // this callback will be called asynchronously when the response is available
                vm.products = response.data;
            },
            function (response) { // called asynchronously if an error occurs or server returns response with an error status.
                handleException(response);
            }).finally(
            function () {
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
        setUIState(pageMode.ADD);
    }

    function cancelClick() {
        setUIState(pageMode.LIST);
    }

    function editClick() {
        // todo: get data here
        setUIState(pageMode.EDIT);
    }
    function deleteClick(id) {

        // todo: delete data here
        vm.messages = [];
        vm.uiState.messages.push({
            property: 'ProductName',
            message: "Unable to delete product " + id + ". Not Yet Implimented"
        });
        vm.uiState.mode = pageMode.VALIDATION;
        saveData();
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

        if (vm.uiState.mode === pageMode.EXCEPTION || vm.uiState.mode === pageMode.VALIDATION) {
            // check for validation message
            setUIState(vm.uiState.mode);
        } else {
            // when go back to page, by default display a list
            setUIState(pageMode.LIST);
        }


    }



    function insertData() {
        if (validate()) {

        }
    }

    function updateData() {
        if (validate()) {

        }
    }

    function validate() {
        // simulate a data validation error
        vm.uiState.mode = pageMode.VALIDATION;

        vm.uiState.messages.push({
            property: 'ProductName',
            message: 'Product Name must be filled in.'
        });

        vm.uiState.messages.push({
            property: 'Url',
            message: 'Url must be filled in.'
        });

        return false;
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


    // expose the click evetn functions through the Angular $scope
    vm.addClick = addClick;
    vm.cancelClick = cancelClick;
    vm.editClick = editClick;
    vm.deleteClick = deleteClick;
    vm.saveClick = saveClick;
    init();


}

