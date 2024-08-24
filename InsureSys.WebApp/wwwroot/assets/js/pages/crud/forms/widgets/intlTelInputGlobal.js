(function ($) {
    $.fn.intlTelInputGlobal = function () {
        $(this).intlTelInput({
            initialCountry: "us",  // assign country code i.e 'in','gb','us' for displaying fixed contry
            // if initialCountry set to auto then country will be based on geoIp
            geoIpLookup: function (success) {
                // Get your api-key at https://ipdata.co/
                fetch("https://api.ipdata.co/?api-key=test")
                    .then(function (response) {
                        if (!response.ok) return success("");
                        return response.json();
                    })
                    .then(function (ipdata) {
                        success(ipdata.country_code);
                    });
            },
            separateDialCode: true,
            utilsScript: "/assets/js/pages/crud/forms/widgets/utils.js", //"https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.3/js/utils.min.js",
        });
    };

    $.fn.getiso2 = function (dialCode, priority) {
        return $(this).intlTelInput("getiso2code", dialCode, priority);
    };

    $.fn.setCountry = function (iso2) {
        $(this).intlTelInput("setCountry", iso2);
    };

    $.fn.getSelectedCountry = function () {
        return $(this).intlTelInput("getSelectedCountryData");
    };

    $.fn.getNumber = function () {
        return $(this).intlTelInput("getNumber");
    };

    $.fn.isValidNumber = function () {
        return $(this).intlTelInput("isValidNumber");
    };
})(jQuery);