/**
 * Customer Form - Dynamic Policy Management with Partial Views
 * Loads policy forms via AJAX to maintain consistency with HTML helpers
 */

(function ($) {
    'use strict';

    const PolicyManager = {
        motorPolicyCount: 0,
        motorPolicies: [],

        init: function () {
            this.bindEvents();
            this.updatePolicyCount();

            const existingPolicies = $('.policy-instance[data-policy-type="motor"]');
            if (existingPolicies.length > 0) {
                this.motorPolicyCount = existingPolicies.length;
                existingPolicies.each((index, element) => {
                    this.motorPolicies.push(index);
                });
                this.showMotorSection();
                this.updatePolicyCount();
            }
        },

        bindEvents: function () {
            const self = this;

            $('.policy-card[data-policy-type="motor"]').on('click', function () {
                self.toggleMotorSection();
            });

            $(document).on('click', '.btn-add-another[data-policy-type="motor"]', function (e) {
                e.preventDefault();
                self.addMotorPolicy();
            });

            $(document).on('click', '.btn-remove[data-policy-type="motor"]', function (e) {
                e.preventDefault();
                const $instance = $(this).closest('.policy-instance');
                const index = $instance.data('policy-index');
                self.removeMotorPolicy($instance, index);
            });

            // UPDATED: Handle dropdown submit options
            $('.submit-option').on('click', function (e) {
                e.preventDefault();
                const submitType = $(this).data('submit-type');
                self.handleSubmit(submitType);
            });

            // UPDATED: Handle main save button click
            $('button[name="SubmitType"][value="Customer"]').on('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                self.handleSaveButtonClick();
            });

            // UPDATED: Intercept form submission for final validation
            $('#frmCustomer').on('submit', function (e) {
                return self.validateBeforeSubmit(e);
            });
        },

        toggleMotorSection: function () {
            const $card = $('.policy-card[data-policy-type="motor"]');
            const $section = $('.policy-section[data-policy-type="motor"]');

            if ($card.hasClass('active')) {
                if (this.motorPolicies.length === 0) {
                    $card.removeClass('active');
                    $section.removeClass('active').slideUp();
                }
            } else {
                $card.addClass('active');
                $section.addClass('active').slideDown();

                if (this.motorPolicies.length === 0) {
                    this.addMotorPolicy();
                }
            }
        },

        showMotorSection: function () {
            $('.policy-card[data-policy-type="motor"]').addClass('active');
            $('.policy-section[data-policy-type="motor"]').addClass('active').show();
        },

        addMotorPolicy: function () {
            const self = this;
            const index = this.motorPolicyCount;
            const $container = $('#motor-policies-container');

            const $loadingIndicator = $('<div class="policy-loading">Loading policy form...</div>');
            $container.append($loadingIndicator);

            $.ajax({
                url: '/Customer/GetMotorPolicyPartial',
                type: 'GET',
                data: {
                    index: index,
                    policyNumber: this.motorPolicies.length + 1
                },
                success: function (html) {
                    $loadingIndicator.remove();
                    $container.append(html);

                    self.motorPolicies.push(index);
                    self.motorPolicyCount++;
                    self.updatePolicyCount();
                    self.initializePolicyComponents(index);

                    const $newPolicy = $container.find(`.policy-instance[data-policy-index="${index}"]`);
                    $('html, body').animate({
                        scrollTop: $newPolicy.offset().top - 100
                    }, 500);
                },
                error: function (xhr, status, error) {
                    $loadingIndicator.remove();
                    console.error('Error loading policy form:', error);
                    alert('Error loading policy form. Please try again.');
                }
            });

            this.showMotorSection();
        },

        removeMotorPolicy: function ($instance, index) {
            const self = this;

            const hasData = this.policyHasData($instance);
            if (hasData) {
                if (!confirm('This policy has data. Are you sure you want to remove it?')) {
                    return;
                }
            }

            $instance.fadeOut(300, function () {
                $(this).remove();

                const idx = self.motorPolicies.indexOf(index);
                if (idx > -1) {
                    self.motorPolicies.splice(idx, 1);
                }

                self.updatePolicyCount();

                if (self.motorPolicies.length === 0) {
                    $('.policy-card[data-policy-type="motor"]').removeClass('active');
                    $('.policy-section[data-policy-type="motor"]').removeClass('active').slideUp();
                }

                self.renumberPolicies();
            });
        },

        policyHasData: function ($instance) {
            let hasData = false;

            $instance.find('input[type="text"], input[type="number"], input[type="date"], select').each(function () {
                const val = $(this).val();
                if (val && val !== '' && val !== 'Select') {
                    hasData = true;
                    return false;
                }
            });

            return hasData;
        },

        renumberPolicies: function () {
            $('.policy-instance[data-policy-type="motor"]').each(function (displayIndex) {
                $(this).find('.policy-instance-title').text(`Motor Policy #${displayIndex + 1}`);
            });
        },

        updatePolicyCount: function () {
            const count = this.motorPolicies.length;
            const $badge = $('.policy-card[data-policy-type="motor"] .policy-badge');

            $badge.text(count);

            if (count > 0) {
                $badge.removeClass('inactive');
            } else {
                $badge.addClass('inactive');
            }
        },

        initializePolicyComponents: function (index) {
            $(`#motor-policies-container .policy-instance[data-policy-index="${index}"]`)
                .find('.selectpicker').selectpicker('refresh');

            $(`#motor-policies-container .policy-instance[data-policy-index="${index}"]`)
                .find('.kt_datetimepicker_6').datetimepicker({
                    todayHighlight: true,
                    autoclose: true,
                    pickerPosition: 'bottom-left',
                    todayBtn: true,
                    format: 'yyyy/mm/dd',
                    minView: 2
                });

            const $form = $('#frmCustomer');
            if ($form.data('validator')) {
                $form.removeData('validator');
                $form.removeData('unobtrusiveValidation');
                $.validator.unobtrusive.parse($form);
            }
        },

        // NEW: Handle main save button click - smart decision
        handleSaveButtonClick: function () {
            const hasPolicies = this.motorPolicies.length > 0;

            if (hasPolicies) {
                // Has policies - validate everything (Customer + Policies)
                $('#hdnSubmitType').val('Full');
                this.enableMotorPolicyValidation();

                // Show user feedback
                console.log('Validating customer and motor policies...');
            } else {
                // No policies - validate only customer
                $('#hdnSubmitType').val('Customer');
                this.removeMotorPolicyValidation();

                console.log('Validating customer details only...');
            }

            // Re-parse validation
            this.refreshValidation();

            // Submit the form
            $('#frmCustomer').submit();
        },

        // Handle dropdown submit options
        handleSubmit: function (submitType) {
            const hasPolicies = this.motorPolicies.length > 0;

            if (submitType === 'Customer') {
                // Customer only - remove policy validation
                $('#hdnSubmitType').val('Customer');
                this.removeMotorPolicyValidation();
            } else if (submitType === 'Full') {
                // Full save - validate everything
                if (!hasPolicies) {
                    // User selected Full but no policies exist
                    if (!confirm('No policies added yet. Do you want to save customer details only?')) {
                        return; // Don't submit
                    }
                    $('#hdnSubmitType').val('Customer');
                    this.removeMotorPolicyValidation();
                } else {
                    $('#hdnSubmitType').val('Full');
                    this.enableMotorPolicyValidation();
                }
            }

            this.refreshValidation();
            $('#frmCustomer').submit();
        },

        // NEW: Validate before final submission
        validateBeforeSubmit: function (e) {
            const submitType = $('#hdnSubmitType').val();
            const hasPolicies = this.motorPolicies.length > 0;

            console.log('Form submission - SubmitType:', submitType, 'Has Policies:', hasPolicies);

            // If submitting full but no policies, show warning
            if (submitType === 'Full' && !hasPolicies) {
                e.preventDefault();
                alert('Please add at least one policy or choose "Customer Details only" from the dropdown.');
                return false;
            }

            // Check if form is valid
            const $form = $('#frmCustomer');
            if ($form.valid && !$form.valid()) {
                // Form has validation errors
                this.scrollToFirstError();
                return false;
            }

            // All good - allow submission
            return true;
        },

        // NEW: Scroll to first validation error
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
        },

        removeMotorPolicyValidation: function () {
            console.log('Removing motor policy validation...');

            $('[name^="MotorPolicies"]').each(function () {
                $(this).removeAttr('data-val')
                    .removeAttr('data-val-required')
                    .removeAttr('data-val-number')
                    .removeAttr('data-val-regex')
                    .removeAttr('data-val-range')
                    .removeClass('input-validation-error');

                const name = $(this).attr('name');
                $(`[data-valmsg-for="${name}"]`).removeClass('field-validation-error')
                    .addClass('field-validation-valid')
                    .html('');
            });
        },

        enableMotorPolicyValidation: function () {
            console.log('Enabling motor policy validation...');

            // Validation attributes are already in HTML from partial view
            // Just ensure they're present
            $('.policy-instance[data-policy-type="motor"]').each(function () {
                const index = $(this).data('policy-index');

                // Re-add validation to required fields if removed
                const requiredFields = [
                    { name: `Policies[${index}].PolicyNumber`, msg: 'Enter policy number' },
                    { name: `Policies[${index}].Company`, msg: 'Select insurer' },
                    { name: `Policies[${index}].PolicyStartDate`, msg: 'Select policy start date' },
                    { name: `Policies[${index}].PolicyDueDate`, msg: 'Select policy due date' },
                    { name: `Policies[${index}].GrosssPremium`, msg: 'Enter gross premium' }
                ];

                requiredFields.forEach(field => {
                    const $field = $(`[name="${field.name}"]`);
                    if ($field.length && !$field.attr('data-val')) {
                        $field.attr('data-val', 'true')
                            .attr('data-val-required', field.msg);
                    }
                });
            });
        },

        refreshValidation: function () {
            const $form = $('#frmCustomer');
            $form.removeData('validator');
            $form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse($form);
        }
    };

    // Initialize on document ready
    $(document).ready(function () {
        PolicyManager.init();

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
