/**
 * Customer Form - Customer Details Only
 * Handles two save options: Customer Only & Customer With Policies
 * Validates customer data regardless of option chosen
 */

(function ($) {
    'use strict';

    const CustomerFormManager = {
        init: function () {
            this.bindEvents();
            this.bindCountryStateCascade();
        },

        bindEvents: function () {
            const self = this;

            // Handle dropdown submit options
            $('.submit-option').on('click', function (e) {
                e.preventDefault();
                const submitType = $(this).data('submit-type');
                self.handleSubmit(submitType);
            });

            // Intercept form submission for validation
            $('#frmCustomer').on('submit', function (e) {
                return self.validateBeforeSubmit(e);
            });
        },

        bindCountryStateCascade: function () {
            const $country = $('#drp_CountryID');
            const $state = $('#drp_StateID');
            if (!$country.length || !$state.length) {
                return;
            }

            $country.on('change', function () {
                const countryId = $(this).val();
                CustomerFormManager.loadStatesByCountry(countryId, $state);
            });
        },

        loadStatesByCountry: function (countryId, $state) {
            if (!countryId) {
                $state.html('<option value="">Select</option>');
                if ($state.hasClass('selectpicker')) {
                    $state.selectpicker('refresh');
                }
                return;
            }

            $.ajax({
                url: '/Customer/GetStatesByCountry',
                type: 'GET',
                data: { countryId: countryId },
                success: function (response) {
                    let options = '<option value="">Select</option>';
                    if (response && response.length) {
                        response.forEach(function (item) {
                            options += '<option value="' + item.value + '">' + item.text + '</option>';
                        });
                    }
                    $state.html(options);
                    if ($state.hasClass('selectpicker')) {
                        $state.selectpicker('refresh');
                    }
                }
            });
        },

        handleSubmit: function (submitType) {
            console.log('Submit type selected:', submitType);

            // Set the hidden field value based on selection
            if (submitType === 'customeronly') {
                $('#hdnSubmitType').val('customeronly');
            } else if (submitType === 'customerwithpolicies') {
                $('#hdnSubmitType').val('customerwithpolicies');
            }

            // Submit the form
            $('#frmCustomer').submit();
        },

        validateBeforeSubmit: function (e) {
            const submitType = $('#hdnSubmitType').val();

            console.log('Form submitting with SubmitType:', submitType);

            // Check if form is valid (customer data validation)
            const $form = $('#frmCustomer');
            if ($form.valid && !$form.valid()) {
                // Form has validation errors
                this.scrollToFirstError();
                return false; // Prevent submission
            }

            // All good - allow submission
            return true;
        },

        scrollToFirstError: function () {
            const $firstError = $('.input-validation-error:first, .field-validation-error:first');
            if ($firstError.length) {
                $('html, body').animate({
                    scrollTop: $firstError.offset().top - 100
                }, 500);

                // Focus the field if it's an input
                if ($firstError.is('input, select, textarea')) {
                    $firstError.focus();
                }
            }
        }
    };

    // Initialize on document ready
    $(document).ready(function () {
        CustomerFormManager.init();

        // Show toastr messages if present
        const $toastrMsg = $('.toastr-message');
        if ($toastrMsg.length && typeof toastr !== 'undefined') {
            const message = $toastrMsg.data('message');
            const type = $toastrMsg.data('type');

            if (message) {
                if (type === 'success') {
                    toastr.success(message);
                } else {
                    toastr.error(message);
                }
            }
        }
    });

})(jQuery);