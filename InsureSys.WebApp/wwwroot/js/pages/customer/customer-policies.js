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

        isCardActive: function (slug) {
            var $card = $('.policy-card[data-policy-type="' + slug + '"]');
            return $card.length && String($card.data('is-active')) !== 'false';
        },

        getNextStandardIndex: function () {
            var max = -1;
            $('#frmCustomerPolicies input[name^="StandardPolicies["]').each(function () {
                var name = $(this).attr('name') || '';
                var m = name.match(/^StandardPolicies\[(\d+)\]/);
                if (m) {
                    var n = parseInt(m[1], 10);
                    if (!isNaN(n)) max = Math.max(max, n);
                }
            });
            return max + 1;
        },

        getPolicyCount: function (slug) {
            var cfg = this.getConfigForSlug(slug);
            if (cfg && cfg.isStandardBucket) {
                return this.getNextStandardIndex();
            }
            return (this.policyIndices[slug] || []).length;
        },

        incrementPolicyCount: function (slug) {
            var cfg = this.getConfigForSlug(slug);
            if (cfg && cfg.isStandardBucket) {
                return;
            }
        },

        bindEvents: function () {
            var self = this;

            $(document).on('click', '.policy-card[data-policy-type]', function () {
                var slug = $(this).data('policy-type');
                if (String($(this).data('is-active')) === 'false') {
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

            this.updateAllPolicyCounts();
        },

        togglePolicySection: function (slug) {
            if (!this.isCardActive(slug)) return;

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
            if (!this.isCardActive(slug)) return;

            var self = this;
            var cfg = this.getConfigForSlug(slug);
            if (!cfg) {
                console.warn('No grid config for slug', slug);
                return;
            }

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
                    alert(msg);
                }
            });

            this.showPolicySection(slug);
        },

        removePolicy: function (slug, $instance, index) {
            var self = this;

            var hasData = this.policyHasData($instance);
            if (hasData) {
                if (!confirm('This policy has data. Are you sure you want to remove it?')) {
                    return;
                }
            }

            $instance.fadeOut(300, function () {
                $(this).remove();

                var policyArray = self.policyIndices[slug] || [];
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

        policyHasData: function ($instance) {
            var hasData = false;
            $instance.find('input[type="text"], input[type="number"], input[type="date"], select').each(function () {
                var val = $(this).val();
                if (val && val !== '' && val !== 'Select') {
                    hasData = true;
                    return false;
                }
            });
            return hasData;
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

                var name = ($input.attr('name') || '').toString();
                if (!name) return;

                if (!$input.attr('data-val')) {
                    $input.attr('data-val', 'true');
                }
                if (!$input.attr('data-val-regex')) {
                    $input.attr('data-val-regex', 'Invalid value entered');
                }
                if (!$input.attr('data-val-regex-pattern')) {
                    $input.attr('data-val-regex-pattern', DECIMAL_REGEX_PATTERN);
                }

                if (name.endsWith('.GrosssPremium') || name.endsWith('.NetPremium') || name.endsWith('.ODPremium') || name.endsWith('.NCB')) {
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
            var hasPolicies = this.hasAnyPolicies();
            if (!hasPolicies) {
                e.preventDefault();
                alert('Please add at least one policy.');
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
