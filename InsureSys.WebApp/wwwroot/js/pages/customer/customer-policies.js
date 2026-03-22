/**
 * Customer Form - Multi-Policy Support
 * Handles: Motor, Health, Life, Personal Accident insurance
 */

(function ($) {
    'use strict';

    const PolicyManager = {
        // Counters for each policy type
        motorPolicyCount: 0,
        healthPolicyCount: 0,
        lifePolicyCount: 0,
        personalAccidentPolicyCount: 0,

        // Arrays tracking policy indices
        motorPolicies: [],
        healthPolicies: [],
        lifePolicies: [],
        personalAccidentPolicies: [],

        init: function () {
            this.bindEvents();
            this.updateAllPolicyCounts();

            // Load existing policies if in edit mode
            this.loadExistingPolicies();
        },

        bindEvents: function () {
            const self = this;

            // Policy card clicks - toggle sections
            $('.policy-card[data-policy-type]').on('click', function () {
                const policyType = $(this).data('policy-type');
                self.togglePolicySection(policyType);
            });

            // Add another policy buttons
            $(document).on('click', '.btn-add-another', function (e) {
                e.preventDefault();
                const policyType = $(this).data('policy-type');
                self.addPolicy(policyType);
            });

            // Remove policy buttons
            $(document).on('click', '.btn-remove', function (e) {
                e.preventDefault();
                const $instance = $(this).closest('.policy-instance');
                const policyType = $(this).data('policy-type');
                const index = $instance.data('policy-index');
                self.removePolicy(policyType, $instance, index);
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
            const self = this;

            // Load existing motor policies
            const existingMotor = $('.policy-instance[data-policy-type="motor"]');
            if (existingMotor.length > 0) {
                this.motorPolicyCount = existingMotor.length;
                existingMotor.each((index, element) => {
                    this.motorPolicies.push(index);                    
                });
                this.showPolicySection('motor');
            }

            // Load existing health policies
            const existingHealth = $('.policy-instance[data-policy-type="health"]');
            if (existingHealth.length > 0) {
                this.healthPolicyCount = existingHealth.length;
                existingHealth.each((index, element) => {
                    this.healthPolicies.push(index);
                });
                this.showPolicySection('health');
            }

            // Load existing life policies
            const existingLife = $('.policy-instance[data-policy-type="life"]');
            if (existingLife.length > 0) {
                this.lifePolicyCount = existingLife.length;
                existingLife.each((index, element) => {
                    this.lifePolicies.push(index);
                });
                this.showPolicySection('life');
            }

            // Load existing personal accident policies
            const existingPA = $('.policy-instance[data-policy-type="personalaccident"]');
            if (existingPA.length > 0) {
                this.personalAccidentPolicyCount = existingPA.length;
                existingPA.each((index, element) => {
                    this.personalAccidentPolicies.push(index);
                });
                this.showPolicySection('personalaccident');
            }

            this.updateAllPolicyCounts();
        },

        togglePolicySection: function (policyType) {
            const $card = $(`.policy-card[data-policy-type="${policyType}"]`);
            const $section = $(`.policy-section[data-policy-type="${policyType}"]`);

            if ($card.hasClass('active')) {
                const hasPolicies = this.getPolicyArray(policyType).length > 0;
                if (!hasPolicies) {
                    $card.removeClass('active');
                    $section.removeClass('active').slideUp();
                }
            } else {
                $card.addClass('active');
                $section.addClass('active').slideDown();

                if (this.getPolicyArray(policyType).length === 0) {
                    this.addPolicy(policyType);
                }
            }
        },

        showPolicySection: function (policyType) {
            $(`.policy-card[data-policy-type="${policyType}"]`).addClass('active');
            $(`.policy-section[data-policy-type="${policyType}"]`).addClass('active').show();
        },

        addPolicy: function (policyType) {
            const self = this;
            const index = this.getPolicyCount(policyType);
            const policyArray = this.getPolicyArray(policyType);
            const $container = $(`#${policyType}-policies-container`);

            const $loadingIndicator = $('<div class="policy-loading">Loading policy form...</div>');
            $container.append($loadingIndicator);

            // Get the AJAX URL based on policy type
            const ajaxUrl = this.getAjaxUrl(policyType);

            $.ajax({
                url: ajaxUrl,
                type: 'GET',
                data: {
                    index: index,
                    policyNumber: policyArray.length + 1
                },
                success: function (html) {
                    $loadingIndicator.remove();
                    $container.append(html);

                    policyArray.push(index);
                    self.incrementPolicyCount(policyType);
                    self.updatePolicyCount(policyType);
                    self.initializePolicyComponents(policyType, index);

                    const $newPolicy = $container.find(`.policy-instance[data-policy-index="${index}"]`);
                    $('html, body').animate({
                        scrollTop: $newPolicy.offset().top - 100
                    }, 500);
                },
                error: function (xhr, status, error) {
                    $loadingIndicator.remove();
                    console.error(`Error loading ${policyType} policy form:`, error);
                    alert(`Error loading ${policyType} policy form. Please try again.`);
                }
            });

            this.showPolicySection(policyType);
        },

        removePolicy: function (policyType, $instance, index) {
            const self = this;

            const hasData = this.policyHasData($instance);
            if (hasData) {
                if (!confirm('This policy has data. Are you sure you want to remove it?')) {
                    return;
                }
            }

            $instance.fadeOut(300, function () {
                $(this).remove();

                const policyArray = self.getPolicyArray(policyType);
                const idx = policyArray.indexOf(index);
                if (idx > -1) {
                    policyArray.splice(idx, 1);
                }

                self.updatePolicyCount(policyType);

                if (policyArray.length === 0) {
                    $(`.policy-card[data-policy-type="${policyType}"]`).removeClass('active');
                    $(`.policy-section[data-policy-type="${policyType}"]`).removeClass('active').slideUp();
                }

                self.renumberPolicies(policyType);
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

        renumberPolicies: function (policyType) {
            $(`.policy-instance[data-policy-type="${policyType}"]`).each(function (displayIndex) {
                const typeName = policyType.charAt(0).toUpperCase() + policyType.slice(1);
                $(this).find('.policy-instance-title').text(`${typeName} Policy #${displayIndex + 1}`);
            });
        },

        updatePolicyCount: function (policyType) {
            const count = this.getPolicyArray(policyType).length;
            const $badge = $(`.policy-card[data-policy-type="${policyType}"] .policy-badge`);

            $badge.text(count);

            if (count > 0) {
                $badge.removeClass('inactive');
            } else {
                $badge.addClass('inactive');
            }
        },

        updateAllPolicyCounts: function () {
            this.updatePolicyCount('motor');
            this.updatePolicyCount('health');
            this.updatePolicyCount('life');
            this.updatePolicyCount('personalaccident');
        },

        initializePolicyComponents: function (policyType, index) {
            const $policy = $(`#${policyType}-policies-container .policy-instance[data-policy-index="${index}"]`);

            console.log(`Initializing components for ${policyType} policy index:`, index);

            //// Initialize all components
            this.initializeBootstrapSelect($policy);
            this.initializeDateTimePicker($policy);
            //this.initializeCharacterCounter($policy);
            this.initializeDecimalInputs($policy);
            this.refreshValidation();

            console.log(`✅ All components initialized for ${policyType} policy`, index);
        },

        initializeBootstrapSelect: function ($container) {
            $container.find('.selectpicker').each(function () {
                const $select = $(this);
                if ($select.data('selectpicker')) {
                    $select.selectpicker('destroy');
                }
                $select.selectpicker({
                    liveSearch: true,
                    size: 7,
                    style: 'btn-light'
                });
            });
            console.log('  ✓ Bootstrap-select initialized');
        },

        initializeDateTimePicker: function ($container) {
            $container.find('.kt_datetimepicker_6').each(function () {
                const $datepicker = $(this);
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
            console.log('  ✓ Datetimepicker initialized');
        },

        initializeCharacterCounter: function ($container) {
            $container.find('input.maximum-length-setup, input[maxlength], textarea[maxlength]').each(function () {
                const $input = $(this);
                const maxLength = parseInt($input.attr('maxlength'));

                if (!maxLength || maxLength <= 0 || $input.data('char-counter-initialized')) {
                    return;
                }

                $input.data('char-counter-initialized', true);

                let $wrapper = $input.parent();
                if ($wrapper.css('position') === 'static') {
                    $wrapper.css('position', 'relative');
                }

                const counterId = 'char-counter-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
                const $counter = $('<span></span>', {
                    id: counterId,
                    class: 'char-counter',
                    css: {
                        'position': 'absolute',
                        'bottom': '8px',
                        'right': '12px',
                        'font-size': '11px',
                        'color': '#B5B5C3',
                        'font-weight': '500',
                        'pointer-events': 'none',
                        'z-index': '10',
                        'background': 'rgba(255,255,255,0.9)',
                        'padding': '2px 4px',
                        'border-radius': '3px'
                    }
                });

                const updateCounter = function () {
                    const currentLength = $input.val().length;
                    $counter.text(currentLength + '/' + maxLength);

                    if (currentLength >= maxLength) {
                        $counter.css('color', '#F64E60');
                    } else if (currentLength > maxLength * 0.9) {
                        $counter.css('color', '#FFA800');
                    } else if (currentLength > maxLength * 0.7) {
                        $counter.css('color', '#8950FC');
                    } else {
                        $counter.css('color', '#B5B5C3');
                    }
                };

                $wrapper.append($counter);
                $input.on('input keyup change paste', updateCounter);
                updateCounter();
            });
            console.log('  ✓ Character counters initialized');
        },

        initializeDecimalInputs: function ($container) {
            const DECIMAL_REGEX_PATTERN = '^\\d+(\\.\\d{1,2})?$'; // matches PolicyBasicDetailsViewModel regex
            const DECIMAL_MAX = 10000000.00;

            $container.find('input[data-decimal="true"], input.text-right[type="text"]').each(function () {
                const $input = $(this);

                if ($input.data('decimal-initialized')) {
                    return;
                }

                $input.data('decimal-initialized', true);

                // ----- Formatting behaviour -----
                $input.on('blur', function () {
                    const raw = ($(this).val() || '').toString().replace(/,/g, '');
                    const value = parseFloat(raw);
                    if (!isNaN(value) && value > 0) {
                        $(this).val(value.toFixed(2));
                    }
                });

                $input.on('keypress', function (e) {
                    const char = String.fromCharCode(e.which);
                    if (!/[\d.]/.test(char) && e.which !== 8) {
                        e.preventDefault();
                    }
                    if (char === '.' && $(this).val().indexOf('.') !== -1) {
                        e.preventDefault();
                    }
                });

                // ----- Client-side validation aligned with PolicyBasicDetailsViewModel -----
                const name = ($input.attr('name') || '').toString();
                if (!name) {
                    return;
                }

                // Common decimal validation (regex + max range), as per model attributes
                if (!$input.attr('data-val')) {
                    $input.attr('data-val', 'true');
                }
                if (!$input.attr('data-val-regex')) {
                    $input.attr('data-val-regex', 'Invalid value entered');
                }
                if (!$input.attr('data-val-regex-pattern')) {
                    $input.attr('data-val-regex-pattern', DECIMAL_REGEX_PATTERN);
                }

                // Apply range rules based on property, mirroring PolicyBasicDetailsViewModel
                if (name.endsWith('.GrosssPremium')
                    || name.endsWith('.NetPremium')
                    || name.endsWith('.ODPremium')
                    || name.endsWith('.NCB')
                ) {
                    // [Range(0.01, 10000000.00, ErrorMessage = "Premium must be greater than 0")]
                    if (!$input.attr('data-val-range')) {
                        $input.attr('data-val-range', 'Value must be greater than 0');
                    }
                    if (!$input.attr('data-val-range-min')) {
                        $input.attr('data-val-range-min', '0.01');
                    }
                    if (!$input.attr('data-val-range-max')) {
                        $input.attr('data-val-range-max', DECIMAL_MAX.toString());
                    }
                } 
            });
            console.log('  ✓ Decimal inputs initialized');
        },

        handleSaveButtonClick: function () {
            const hasPolicies = this.hasAnyPolicies();
            if (hasPolicies) {                
                this.enableAllPolicyValidation();
                console.log('Validating customer and all policies...');
            } else {                
                this.removeAllPolicyValidation();
                console.log('Validating customer details only...');
            }

            this.refreshValidation();
            $('#frmCustomerPolicies').submit();
        },

        validateBeforeSubmit: function (e) {
            
            const hasPolicies = this.hasAnyPolicies();
            if (!hasPolicies) {
                e.preventDefault();
                alert('Please add at least one policy.');
                return false;
            }
            const $form = $('#frmCustomerPolicies');
            if ($form.valid && !$form.valid()) {
                this.scrollToFirstError();
                return false;
            }

            return true;
        },

        scrollToFirstError: function () {
            const $firstError = $('.input-validation-error:first, .field-validation-error:first');
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
            console.log('Removing all policy validation...');

            // Remove validation for all policy types
            const policyTypes = ['MotorPolicies', 'HealthPolicies', 'LifePolicies', 'PersonalAccidentPolicies'];

            policyTypes.forEach(policyType => {
                $(`[name^="${policyType}"]`).each(function () {
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
            });
        },

        enableAllPolicyValidation: function () {
            console.log('Enabling all policy validation...');

            // Enable validation for all policy types
            this.enablePolicyTypeValidation('motor', 'MotorPolicies');
            this.enablePolicyTypeValidation('health', 'HealthPolicies');
            this.enablePolicyTypeValidation('life', 'LifePolicies');
            this.enablePolicyTypeValidation('personalaccident', 'PersonalAccidentPolicies');
        },

        enablePolicyTypeValidation: function (policyType, prefix) {
            $(`.policy-instance[data-policy-type="${policyType}"]`).each(function () {
                const $instance = $(this);
                const index = $instance.data('policy-index');
                const instancePrefix = `${prefix}[${index}]`;

                // Generic: any field whose label has the "required" marker gets a required rule,
                // so future required fields don't need JS changes.
                
                $instance.find('label.required').each(function () {
                    const $label = $(this);

                    // Our helpers render: <label ...></label><div ...><input/select/textarea ...></div>
                    const $fieldContainer = $label.nextAll('div').first();
                    if (!$fieldContainer.length) {
                        return;
                    }

                    const $field = $fieldContainer.find('input, select, textarea').first();
                    if (!$field.length) {
                        return;
                    }

                    const name = ($field.attr('name') || '').toString();
                    if (!name || name.indexOf(instancePrefix) !== 0) {
                        // Not part of this policy instance (or has no name) – skip
                        return;
                    }

                    if (!$field.attr('data-val')) {
                        $field.attr('data-val', 'true');
                    }

                    if (!$field.attr('data-val-required')) {
                        const rawLabel = $.trim($label.text().replace('*', ''));
                        const msg = rawLabel ? `${rawLabel} is required` : 'This field is required';
                        $field.attr('data-val-required', msg);
                    }
                });
                
            });
        },

        refreshValidation: function () {
            const $form = $('#frmCustomerPolicies');
            $form.removeData('validator');
            $form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse($form);
        },

        // Helper methods
        getPolicyCount: function (policyType) {
            switch (policyType) {
                case 'motor': return this.motorPolicyCount;
                case 'health': return this.healthPolicyCount;
                case 'life': return this.lifePolicyCount;
                case 'personalaccident': return this.personalAccidentPolicyCount;
                default: return 0;
            }
        },

        incrementPolicyCount: function (policyType) {
            switch (policyType) {
                case 'motor': this.motorPolicyCount++; break;
                case 'health': this.healthPolicyCount++; break;
                case 'life': this.lifePolicyCount++; break;
                case 'personalaccident': this.personalAccidentPolicyCount++; break;
            }
        },

        getPolicyArray: function (policyType) {
            switch (policyType) {
                case 'motor': return this.motorPolicies;
                case 'health': return this.healthPolicies;
                case 'life': return this.lifePolicies;
                case 'personalaccident': return this.personalAccidentPolicies;
                default: return [];
            }
        },

        getAjaxUrl: function (policyType) {
            switch (policyType) {
                case 'motor': return '/Customer/GetMotorPolicyPartial';
                case 'health': return '/Customer/GetHealthPolicyPartial';
                case 'life': return '/Customer/GetLifePolicyPartial';
                case 'personalaccident': return '/Customer/GetPersonalAccidentPolicyPartial';
                default: return '';
            }
        },

        hasAnyPolicies: function () {
            return this.motorPolicies.length > 0 ||
                this.healthPolicies.length > 0 ||
                this.lifePolicies.length > 0 ||
                this.personalAccidentPolicies.length > 0;
        }
    };

    // Initialize on document ready
    $(document).ready(function () {
        PolicyManager.init();

        // Show toastr messages
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