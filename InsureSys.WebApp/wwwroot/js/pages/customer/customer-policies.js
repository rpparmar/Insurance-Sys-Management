/**
 * Customer policies — multi-type grid driven by master + policy-type-grid-config JSON.
 */
(function ($) {
    'use strict';

    function parseGridConfig() {
        var $el = $('#policy-type-grid-config');
        if (!$el.length) return [];
        try {
            return JSON.parse($el.text() || '[]');
        } catch (e) {
            console.warn('policy-type-grid-config parse failed', e);
            return [];
        }
    }

    const PolicyManager = {
        gridConfig: [],
        /** slug -> tracked indices (for badges / remove); standard bucket uses global StandardPolicies indices */
        policyIndices: {},

        init: function () {
            this.gridConfig = parseGridConfig();
            this.policyIndices = {};
            var self = this;
            this.gridConfig.forEach(function (c) {
                self.policyIndices[c.slug] = [];
            });

            this.bindEvents();
            this.updateAllPolicyCounts();
            this.loadExistingPolicies();
        },

        getConfigForSlug: function (slug) {
            return this.gridConfig.find(function (c) { return c.slug === slug; });
        },

        getPolicyPrefix: function (slug) {
            var cfg = this.getConfigForSlug(slug);
            return cfg && cfg.formCollectionPrefix ? cfg.formCollectionPrefix : '';
        },

        getPolicyArray: function (slug) {
            return this.policyIndices[slug] || [];
        },

        isCardActive: function (slug) {
            var $card = $('.policy-card[data-policy-type="' + slug + '"]');
            return $card.length && String($card.data('is-active')) !== 'false';
        },

        getNextStandardIndex: function () {
            return this.getNextIndexForPrefix('StandardPolicies');
        },

        getNextIndexForPrefix: function (prefix) {
            var max = -1;
            if (!prefix) {
                return 0;
            }
            var escaped = prefix.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
            var re = new RegExp('^' + escaped + '\\[(\\d+)\\]');
            $('#frmCustomerPolicies input[name^="' + prefix + '["]').each(function () {
                var name = $(this).attr('name') || '';
                var match = name.match(re);
                if (match) {
                    var n = parseInt(match[1], 10);
                    if (!isNaN(n)) {
                        max = Math.max(max, n);
                    }
                }
            });
            return max + 1;
        },

        getPolicyCount: function (slug) {
            var cfg = this.getConfigForSlug(slug);
            if (cfg && cfg.formCollectionPrefix) {
                return this.getNextIndexForPrefix(cfg.formCollectionPrefix);
            }
            return (this.policyIndices[slug] || []).length;
        },

        incrementPolicyCount: function (slug) {
            var cfg = this.getConfigForSlug(slug);
            if (cfg && cfg.isStandardBucket) {
                return;
            }
        },

        showNoInsuranceTypesAlert: function (title, message) {
            title = title || 'No Insurance Types Found';
            message = message || 'Please configure active insurance types in system settings before adding policies.';
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: title,
                    text: message,
                    icon: 'warning',
                    confirmButtonText: 'OK',
                    buttonsStyling: false,
                    customClass: {
                        confirmButton: 'btn btn-primary font-weight-bold'
                    }
                });
            } else if (typeof toastr !== 'undefined') {
                toastr.warning(message, title);
            } else {
                alert(title + '\n\n' + message);
            }
        },

        bindEvents: function () {
            var self = this;

            $(document).on('click', '.policy-card[data-policy-type]', function () {
                var slug = $(this).data('policy-type');
                if (String($(this).data('is-active')) === 'false') {
                    self.showNoInsuranceTypesAlert('Inactive Insurance Type', 'This insurance type is currently inactive in system settings.');
                    return;
                }
                self.togglePolicySection(slug);
            });

            $(document).on('click', '.btn-add-another', function (e) {
                e.preventDefault();
                var slug = $(this).data('policy-type');
                self.addPolicy(slug);
            });

            $(document).on('click', '.btn-remove', function (e) {
                e.preventDefault();
                var $instance = $(this).closest('.policy-instance');
                var slug = $(this).data('policy-type');
                var index = $instance.data('policy-index');
                self.removePolicy(slug, $instance, index);
            });

            $('#btnSavePolicies').on('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                self.handleSaveButtonClick();
            });
            $('#frmCustomerPolicies').on('submit', function (e) {
                return self.validateBeforeSubmit(e);
            });
        },

        loadExistingPolicies: function () {
            var self = this;
            $('.policy-instance').each(function () {
                var slug = $(this).data('policy-type');
                var index = $(this).data('policy-index');
                if (!slug || index === undefined) return;
                if (!self.policyIndices[slug]) self.policyIndices[slug] = [];
                self.policyIndices[slug].push(index);
            });

            this.gridConfig.forEach(function (c) {
                if ((self.policyIndices[c.slug] || []).length > 0) {
                    self.showPolicySection(c.slug);
                }
            });

            $('.policy-instance').each(function () {
                var slug = $(this).data('policy-type');
                var index = $(this).data('policy-index');
                if (slug === undefined || index === undefined) {
                    return;
                }
                self.initializePolicyComponents(slug, index);
            });

            this.updateAllPolicyCounts();
        },

        togglePolicySection: function (slug) {
            if (!this.gridConfig || this.gridConfig.length === 0) {
                this.showNoInsuranceTypesAlert('No Insurance Types Found', 'Please configure active insurance types in system settings before adding policies.');
                return;
            }

            var cfg = this.getConfigForSlug(slug);
            if (!cfg || !cfg.isActive) {
                this.showNoInsuranceTypesAlert('Inactive Insurance Type', 'This insurance type is currently inactive in system settings.');
                return;
            }

            var $card = $('.policy-card[data-policy-type="' + slug + '"]');
            var $section = $('.policy-section[data-policy-type="' + slug + '"]');

            if ($card.hasClass('active')) {
                var hasPolicies = (this.policyIndices[slug] || []).length > 0;
                if (!hasPolicies) {
                    $card.removeClass('active');
                    $section.removeClass('active').slideUp();
                }
            } else {
                $card.addClass('active');
                $section.addClass('active').slideDown();

                if ((this.policyIndices[slug] || []).length === 0) {
                    this.addPolicy(slug);
                }
            }
        },

        showPolicySection: function (slug) {
            $('.policy-card[data-policy-type="' + slug + '"]').addClass('active');
            $('.policy-section[data-policy-type="' + slug + '"]').addClass('active').show();
        },

        addPolicy: function (slug) {
            if (!this.gridConfig || this.gridConfig.length === 0) {
                this.showNoInsuranceTypesAlert('No Insurance Types Found', 'Please configure active insurance types in system settings before adding policies.');
                return;
            }

            if (!slug) {
                var activeConfig = this.gridConfig.find(function (c) { return c.isActive; });
                if (!activeConfig) {
                    this.showNoInsuranceTypesAlert('No Active Insurance Types', 'All configured insurance types are currently inactive in system settings.');
                    return;
                }
                slug = activeConfig.slug;
            }

            var cfg = this.getConfigForSlug(slug);
            if (!cfg) {
                console.warn('No grid config for slug', slug);
                this.showNoInsuranceTypesAlert('Unknown Insurance Type', 'The requested insurance type configuration could not be found.');
                return;
            }

            if (!cfg.isActive) {
                this.showNoInsuranceTypesAlert('Inactive Insurance Type', 'This insurance type is currently inactive and cannot be added.');
                return;
            }

            var self = this;
            var index = this.getPolicyCount(slug);
            var policyArray = this.policyIndices[slug] || [];
            var $container = $('#' + slug + '-policies-container');
            if (!$container.length) {
                console.warn('Missing container #' + slug + '-policies-container');
                return;
            }

            var $loadingIndicator = $('<div class="policy-loading">Loading policy form...</div>');
            $container.append($loadingIndicator);

            $.ajax({
                url: '/Customer/GetPolicyPartial',
                type: 'GET',
                data: {
                    insuranceTypeId: cfg.insuranceTypeId,
                    index: index,
                    policyNumber: policyArray.length + 1
                },
                success: function (html) {
                    $loadingIndicator.remove();
                    $container.append(html);

                    policyArray.push(index);
                    self.policyIndices[slug] = policyArray;
                    self.incrementPolicyCount(slug);
                    self.updatePolicyCount(slug);
                    self.initializePolicyComponents(slug, index);

                    var $newPolicy = $container.find('.policy-instance[data-policy-index="' + index + '"]');
                    if ($newPolicy.length) {
                        $('html, body').animate({
                            scrollTop: $newPolicy.offset().top - 100
                        }, 500);
                    }
                },
                error: function (xhr) {
                    $loadingIndicator.remove();
                    var msg = (xhr.responseText && xhr.responseText.length < 200) ? xhr.responseText : 'Could not load policy form.';
                    console.error('GetPolicyPartial failed', xhr.status, msg);
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            title: 'Error',
                            text: msg,
                            icon: 'error',
                            confirmButtonText: 'OK',
                            buttonsStyling: false,
                            customClass: {
                                confirmButton: 'btn btn-primary font-weight-bold'
                            }
                        });
                    } else {
                        alert(msg);
                    }
                }
            });

            this.showPolicySection(slug);
        },

        removePolicy: function (slug, $instance, index) {
            var self = this;

            var prefix = self.getPolicyPrefix(slug);
            if (!prefix) {
                if (typeof toastr !== 'undefined') {
                    toastr.error('Could not resolve policy type. Please refresh the page.');
                } else {
                    alert('Could not resolve policy type. Please refresh the page.');
                }
                return;
            }

            var exactFieldName = prefix + '[' + index + '].BasicDetails.PolicyId';
            var $policyIdInput = $instance.find('input[name="' + exactFieldName + '"]');
            var existingId = parseInt(($policyIdInput.val() || '0'), 10);
            var isExisting = !isNaN(existingId) && existingId > 0;            
            var confirmText = isExisting
                ? 'This will permanently delete this policy record.'
                : 'This policy has not been saved yet and will be discarded.';

            var runAfterConfirm = function () {
                if (isExisting) {
                    self.deleteExistingPolicy(existingId, slug, $instance, index);
                } else {
                    self.removePolicyFromDom(slug, $instance, index);
                }
            };

            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Remove Policy?',
                    text: confirmText,
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes, remove it',
                    cancelButtonText: 'Cancel',
                    buttonsStyling: false,
                    customClass: {
                        confirmButton: 'btn btn-danger font-weight-bold',
                        cancelButton: 'btn btn-secondary font-weight-bold mr-3'
                    },
                    reverseButtons: true
                }).then(function (result) {                    
                    if (result.value) {
                        runAfterConfirm();
                    }
                });
            } else if (window.confirm('Remove Policy?\n\n' + confirmText)) {
                runAfterConfirm();
            }
        },

        deleteExistingPolicy: function (policyId, slug, $instance, index) {            
            var self = this;
            var token = $('input[name="__RequestVerificationToken"]').val();
            var customerId = parseInt($('input[name="CustomerID"]').val() || '0', 10);

            $.ajax({
                url: '/Customer/DeletePolicy',
                type: 'POST',
                data: {
                    policyId: policyId,
                    customerId: customerId
                },
                headers: { 'RequestVerificationToken': token },
                success: function (response) {
                    if (response && response.success) {
                        self.removePolicyFromDom(slug, $instance, index);
                        if (typeof toastr !== 'undefined') {
                            toastr.success('Policy removed successfully.');
                        }
                    } else if (typeof toastr !== 'undefined') {
                        toastr.error((response && response.message) ? response.message : 'Failed to remove policy. Please try again.');
                    }
                },
                error: function () {
                    if (typeof toastr !== 'undefined') {
                        toastr.error('An error occurred while removing the policy. Please try again.');
                    }
                }
            });
        },

        removePolicyFromDom: function (slug, $instance, index) {
            var self = this;

            $instance.fadeOut(300, function () {
                $(this).remove();

                var policyArray = self.getPolicyArray(slug);                
                var idx = policyArray.indexOf(index);
                if (idx > -1) {
                    policyArray.splice(idx, 1);
                }
                self.policyIndices[slug] = policyArray;

                self.updatePolicyCount(slug);

                if (policyArray.length === 0) {
                    $('.policy-card[data-policy-type="' + slug + '"]').removeClass('active');
                    $('.policy-section[data-policy-type="' + slug + '"]').removeClass('active').slideUp();
                }

                self.renumberPolicies(slug);
            });
        },

        renumberPolicies: function (slug) {
            $('.policy-instance[data-policy-type="' + slug + '"]').each(function (displayIndex) {
                var title = $(this).find('.policy-instance-title').first();
                if (title.length) {
                    var raw = title.text().replace(/#\d+$/, '').trim();
                    title.text(raw + ' #' + (displayIndex + 1));
                }
            });
        },

        updatePolicyCount: function (slug) {
            var count = $('#' + slug + '-policies-container .policy-instance').length;
            var $badge = $('.policy-card[data-policy-type="' + slug + '"] .policy-badge');
            $badge.text(count);
            if (count > 0) {
                $badge.removeClass('inactive');
            } else {
                $badge.addClass('inactive');
            }
        },

        updateAllPolicyCounts: function () {
            var self = this;
            this.gridConfig.forEach(function (c) {
                self.updatePolicyCount(c.slug);
            });
        },

        initializePolicyComponents: function (slug, index) {
            var $policy = $('#' + slug + '-policies-container .policy-instance[data-policy-index="' + index + '"]');
            this.initializeBootstrapSelect($policy);
            this.initializeDateTimePicker($policy);
            this.initializeBootstrapMaxlength($policy);
            this.initializeDecimalInputs($policy);
            this.refreshValidation();
        },

        initializeBootstrapSelect: function ($container) {
            $container.find('.selectpicker').each(function () {
                var $select = $(this);
                if ($select.data('selectpicker')) {
                    $select.selectpicker('destroy');
                }
                $select.selectpicker({
                    liveSearch: true,
                    size: 7,
                    style: 'btn-light'
                });
            });
        },

        initializeDateTimePicker: function ($container) {
            $container.find('.kt_datetimepicker_6').each(function () {
                var $datepicker = $(this);
                if ($datepicker.data('datetimepicker')) {
                    $datepicker.datetimepicker('destroy');
                }
                $datepicker.datetimepicker({
                    todayHighlight: true,
                    autoclose: true,
                    pickerPosition: 'bottom-left',
                    todayBtn: true,
                    format: 'yyyy/mm/dd',
                    minView: 2
                });
            });
        },

        initializeBootstrapMaxlength: function ($container) {
            var maxlengthOptions = {
                warningClass: 'label label-warning label-rounded label-inline',
                limitReachedClass: 'label label-success label-rounded label-inline'
            };
            $container.find('.maximum-length-setup').maxlength(maxlengthOptions);
        },

        initializeDecimalInputs: function ($container) {
            var DECIMAL_REGEX_PATTERN = '^\\d+(\\.\\d{1,2})?$';
            var DECIMAL_MAX = 10000000.00;

            $container.find('input[data-decimal="true"], input.text-right[type="text"]').each(function () {
                var $input = $(this);
                if ($input.data('decimal-initialized')) {
                    return;
                }
                $input.data('decimal-initialized', true);

                // ----- Formatting behaviour -----
                $input.on('blur', function () {
                    var raw = ($(this).val() || '').toString().replace(/,/g, '');
                    var value = parseFloat(raw);
                    if (!isNaN(value) && value > 0) {
                        $(this).val(value.toFixed(2));
                    }
                });

                $input.on('keypress', function (e) {
                    var char = String.fromCharCode(e.which);
                    if (!/[\d.]/.test(char) && e.which !== 8) {
                        e.preventDefault();
                    }
                    if (char === '.' && $(this).val().indexOf('.') !== -1) {
                        e.preventDefault();
                    }
                });

                // ----- Client-side validation aligned with PolicyBasicDetailsViewModel -----
                var name = ($input.attr('name') || '').toString();
                if (!name) return;

                // Ensure data-val is active
                if (!$input.attr('data-val')) {
                    $input.attr('data-val', 'true');
                }

                // Remove the generic number validator — it fires before regex and shows
                // "must be a number" for invalid input, hiding the regex message.
                // The regex fully covers numeric format validation.
                $input.removeAttr('data-val-number');

                // Always override regex message (replaces server-generated default)
                $input.attr('data-val-regex', 'Invalid amount');
                $input.attr('data-val-regex-pattern', DECIMAL_REGEX_PATTERN);


                // Apply field-specific range rules
                if (name.endsWith('.GrosssPremium')) {
                    $input.attr('data-val-range', 'Gross Premium must be greater than 0');
                    $input.attr('data-val-range-min', '0.01');
                    $input.attr('data-val-range-max', DECIMAL_MAX.toString());
                } else if (
                    name.endsWith('.NetPremium') ||
                    name.endsWith('.ODPremium') ||
                    name.endsWith('.NCB')
                ) {
                    $input.attr('data-val-range', 'Value must be between 0.01 and 10,000,000');
                    $input.attr('data-val-range-min', '0.01');
                    $input.attr('data-val-range-max', DECIMAL_MAX.toString());
                }

            });
        },

        handleSaveButtonClick: function () {
            var hasPolicies = this.hasAnyPolicies();
            if (hasPolicies) {
                this.enableAllPolicyValidation();
            } else {
                this.removeAllPolicyValidation();
            }
            this.refreshValidation();
            $('#frmCustomerPolicies').submit();
        },

        validateBeforeSubmit: function (e) {
            if (!this.gridConfig || this.gridConfig.length === 0) {
                e.preventDefault();
                this.showNoInsuranceTypesAlert('Cannot Save Policies', 'No insurance types are configured in the system. Please configure active insurance types in system settings.');
                return false;
            }

            var hasPolicies = this.hasAnyPolicies();
            if (!hasPolicies) {
                e.preventDefault();
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        title: 'No Policies Added',
                        text: 'Please add at least one policy before saving.',
                        icon: 'warning',
                        confirmButtonText: 'OK',
                        buttonsStyling: false,
                        customClass: {
                            confirmButton: 'btn btn-primary font-weight-bold'
                        }
                    });
                } else if (typeof toastr !== 'undefined') {
                    toastr.warning('Please add at least one policy before saving.');
                } else {
                    alert('Please add at least one policy before saving.');
                }
                return false;
            }
            var $form = $('#frmCustomerPolicies');
            if ($form.valid && !$form.valid()) {
                this.scrollToFirstError();
                return false;
            }
            return true;
        },

        scrollToFirstError: function () {
            var $firstError = $('.input-validation-error:first, .field-validation-error:first');
            if ($firstError.length) {
                $('html, body').animate({
                    scrollTop: $firstError.offset().top - 100
                }, 500);
                if ($firstError.is('input, select, textarea')) {
                    $firstError.focus();
                }
            }
        },

        removeAllPolicyValidation: function () {
            var prefixes = [];
            this.gridConfig.forEach(function (c) {
                if (c.formCollectionPrefix && prefixes.indexOf(c.formCollectionPrefix) === -1) {
                    prefixes.push(c.formCollectionPrefix);
                }
            });
            prefixes.forEach(function (prefix) {
                $('[name^="' + prefix + '"]').each(function () {
                    $(this).removeAttr('data-val')
                        .removeAttr('data-val-required')
                        .removeAttr('data-val-number')
                        .removeAttr('data-val-regex')
                        .removeAttr('data-val-range')
                        .removeClass('input-validation-error');
                    var name = $(this).attr('name');
                    if (name) {
                        $('[data-valmsg-for="' + name + '"]').removeClass('field-validation-error')
                            .addClass('field-validation-valid')
                            .html('');
                    }
                });
            });
        },

        enableAllPolicyValidation: function () {
            var self = this;
            this.gridConfig.forEach(function (c) {
                self.enablePolicyTypeValidation(c.slug, c.formCollectionPrefix);
            });
        },

        enablePolicyTypeValidation: function (slug, prefix) {
            $('.policy-instance[data-policy-type="' + slug + '"]').each(function () {
                var $instance = $(this);
                var index = $instance.data('policy-index');
                var instancePrefix = prefix + '[' + index + ']';

                $instance.find('label.required').each(function () {
                    var $label = $(this);
                    var $fieldContainer = $label.nextAll('div').first();
                    if (!$fieldContainer.length) return;

                    var $field = $fieldContainer.find('input, select, textarea').first();
                    if (!$field.length) return;

                    var name = ($field.attr('name') || '').toString();
                    if (!name || name.indexOf(instancePrefix) !== 0) return;

                    if (!$field.attr('data-val')) {
                        $field.attr('data-val', 'true');
                    }
                    if (!$field.attr('data-val-required')) {
                        var rawLabel = $.trim($label.text().replace('*', ''));
                        var msg = rawLabel ? rawLabel + ' is required' : 'This field is required';
                        console.log(msg)
                        $field.attr('data-val-required', msg);
                    }
                });
            });
        },

        refreshValidation: function () {
            var $form = $('#frmCustomerPolicies');
            $form.removeData('validator');
            $form.removeData('unobtrusiveValidation');
            if ($.validator && $.validator.unobtrusive) {
                $.validator.unobtrusive.parse($form);
            }
        },

        hasAnyPolicies: function () {
            var self = this;
            return this.gridConfig.some(function (c) {
                return (self.policyIndices[c.slug] || []).length > 0;
            });
        }
    };

    $(document).ready(function () {
        PolicyManager.init();

        if (typeof toastr !== 'undefined') {
            toastr.options = {
                closeButton: true,
                progressBar: true,
                positionClass: 'toast-top-right',
                timeOut: 4000
            };
        }

        var $toastrMsg = $('.toastr-message');
        if ($toastrMsg.length && typeof toastr !== 'undefined') {
            var message = $toastrMsg.data('message');
            var type = $toastrMsg.data('type');
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
