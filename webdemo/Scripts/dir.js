/// <reference path="typings/jquery/jquery.d.ts" />
// add a custom url validation directive
// set model validator named microsoft
// add it after your 
//   angular.module()
//  .controller()
//  .directive('urlMicrosoft', urlMicrosoft)
// and add the directive name to the html field using it
// <input type='text' required url-microsft id='Url' name='Url'>
function urlMicrosoft() {
    return {
        require: 'ngModel',
        link: function (scope, element, attributes, ngModel) {
            ngModel.$validators.microsoft = function (value) {
                if (value) {
                    return value.indexOf("microsoft") == -1;
                }
            };
        }
    };
}
//# sourceMappingURL=dir.js.map