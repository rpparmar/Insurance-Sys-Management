function bindDropdownByDependency({
    sourceSelector,           // ID of the triggering dropdown
    targetSelector,           // ID of the dependent dropdown 
    endpointUrl,              // URL to call 
    paramName,                // Parameter name to send in AJAX
    includeDefaultOption = true,
    defaultOptionText = "Select",
    useSelectPicker = true,
    showError = true
}) {    
        const selectedValue = $(sourceSelector).val();
        const $target = $(targetSelector);

        // Reset target dropdown
        $target.empty();

        if (includeDefaultOption) {
            $target.append($('<option>', {
                value: '',
                text: defaultOptionText
            }));
        }

        // Proceed only if source has value
        if (selectedValue) {
            $.ajax({
                url: endpointUrl,
                type: 'POST',
                data: { [paramName]: selectedValue },
                success: function (response) {
                    if (Array.isArray(response) && response.length > 0) {
                        $.each(response, function (i, item) {
                            $target.append($('<option>', {
                                value: item.value,
                                text: item.text,
                                selected: item.selected || false
                            }));
                        });
                    }

                    if (useSelectPicker && $target.hasClass('selectpicker')) {
                        $target.selectpicker('refresh');
                    }
                },
                error: function () {
                    if (showError && typeof toastr !== 'undefined') {
                        toastr.error('Error occurred while fetching data.');
                    }

                    if (useSelectPicker && $target.hasClass('selectpicker')) {
                        $target.selectpicker('refresh');
                    }
                }
            });
        } else {
            if (useSelectPicker && $target.hasClass('selectpicker')) {
                $target.selectpicker('refresh');
            }
        }    
}
