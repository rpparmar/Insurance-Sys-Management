using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Insurancesys.web.Helper
{
    public static class HtmlHelpers
    {
        #region Simple form inputs
        public static IHtmlContent SimpleFormTextBox(this IHtmlHelper htmlHelper, string label, string forExpression, bool isRequired = false, string placeholder = "", int maxlength = 0, bool isRegExpression = false)
        {
            // Create label element
            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");
            if (isRequired)
                labelContent.AddCssClass("required");

            // Create input element
            var htmlAttribute = maxlength > 0
                ? (object)new { @class = "form-control maximum-length-setup", maxlength, placeholder }
                : (object)new { @class = "form-control maximum-length-setup", placeholder };
            var inputContent = htmlHelper.TextBox(forExpression, null, htmlAttribute);


            // Create input column container
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-8 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(inputContent);
            if (isRequired || isRegExpression)
            {
                // Create validation message
                var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validationMessage);
            }
            // Create label column container
            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-4 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            // Create form-group row
            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public static IHtmlContent SimpleFormDropdown(this IHtmlHelper htmlHelper, string label, string forExpression, IEnumerable<SelectListItem> selectList, bool isRequired = false, string placeholder = "Select", bool isRegExpression = false)
        {
            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");
            if (isRequired)
                labelContent.AddCssClass("required");

            var dropdownAttributes = new Dictionary<string, object>
            {
                { "class", "form-control selectpicker" },
                { "data-size", "7" },
                { "data-live-search", "true" },
                { "id", $"drp{forExpression}" }
            };
            var optionLabel = string.IsNullOrEmpty(placeholder) ? "Select" : placeholder;
            var dropdownContent = htmlHelper.DropDownList(forExpression, selectList, optionLabel, dropdownAttributes);

            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-8 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(dropdownContent);
            if (isRequired || isRegExpression)
            {
                var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validationMessage);
            }

            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-4 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public static IHtmlContent SimpleFormCheckBox(this IHtmlHelper htmlHelper, string label, string forExpression, string id = "", bool useSwitch = true)
        {
            // Label column
            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-4 col-xs-12 text-md-right");

            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-form-label");
            labelTag.InnerHtml.Append(label);
            labelColumn.InnerHtml.AppendHtml(labelTag);

            var switchWrapper = BuildCheckBoxSwitchContent(htmlHelper, forExpression, id, useSwitch);

            // Input column
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-8 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(switchWrapper);

            // Form group row
            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public static IHtmlContent SimpleFormMuliSelectDropdown(this IHtmlHelper htmlHelper,
            string label,
            string selectId,
            IEnumerable<SelectListItem> selectList,
            string onChangeFunction = "",
            string? cssClass = "form-control selectpicker")
        {
            // Create the <label> tag
            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-form-label");
            labelTag.InnerHtml.Append(label);

            // Label column div
            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-4 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelTag);

            // Create the <select> tag
            var selectTag = new TagBuilder("select");
            selectTag.Attributes.Add("id", selectId);
            selectTag.Attributes.Add("multiple", "multiple");
            selectTag.Attributes.Add("tabindex", "null");
            if (!string.IsNullOrWhiteSpace(onChangeFunction))
            {
                selectTag.Attributes.Add("onchange", onChangeFunction);
            }
            selectTag.AddCssClass(cssClass ?? "form-control selectpicker");

            // Append <option> tags from SelectListItem
            foreach (var item in selectList)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = item.Value;
                if (item.Selected)
                    option.Attributes["selected"] = "selected";
                option.InnerHtml.Append(item.Text);
                selectTag.InnerHtml.AppendHtml(option);
            }
            // Input column div
            var selectColumn = new TagBuilder("div");
            selectColumn.AddCssClass("col-md-8 col-xs-12");
            selectColumn.InnerHtml.AppendHtml(selectTag);

            // Row wrapper
            var formGroup = new TagBuilder("div");
            formGroup.AddCssClass("form-group row");
            formGroup.InnerHtml.AppendHtml(labelColumn);
            formGroup.InnerHtml.AppendHtml(selectColumn);

            return formGroup;
        }
        #endregion

        #region Accordian based inputs
        public static IHtmlContent AccordianBaseTextBox(this IHtmlHelper htmlHelper, string label, string forExpression, bool isRequired = false, string placeholder = "", int maxlength = 0, bool isRegExpression = false)
        {
            // Create label element
            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");
            if (isRequired)
                labelContent.AddCssClass("required");

            // Create input element
            var htmlAttribute = maxlength > 0
                ? (object)new { @class = "form-control maximum-length-setup", maxlength, placeholder }
                : (object)new { @class = "form-control maximum-length-setup", placeholder };
            var inputContent = htmlHelper.TextBox(forExpression, null, htmlAttribute);


            // Create input column container
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-4 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(inputContent);
            if (isRequired || isRegExpression)
            {
                // Create validation message
                var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validationMessage);
            }
            // Create label column container
            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-5 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            // Create form-group row
            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public sealed class AccordianTextBoxWithTooltipOptions
        {
            public bool IsRequired { get; set; }
            public string Placeholder { get; set; } = "";
            public int MaxLength { get; set; }
            public bool IsRegExpression { get; set; }
            public bool IsDisabled { get; set; }
            public bool IsReadOnly { get; set; }

            public bool ShowEditButton { get; set; }
            public string EditButtonId { get; set; } = "";
            public string EditButtonTitle { get; set; } = "Edit";
            public string EditButtonCssClass { get; set; } = "btn btn-sm btn-clean btn-icon ml-2";
            public string EditButtonIconCssClass { get; set; } = "la la-edit text-primary";
        }

        public static IHtmlContent AccordianBaseTextBoxWithTooltip(this IHtmlHelper htmlHelper, string label, string forExpression, string tooltip, AccordianTextBoxWithTooltipOptions? options = null)
        {
            options ??= new AccordianTextBoxWithTooltipOptions();

            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");
            if (options.IsRequired)
                labelContent.AddCssClass("required");

            var htmlAttributes = new Dictionary<string, object>
            {
                { "class", "form-control maximum-length-setup" },
                { "placeholder", options.Placeholder }
            };
            if (options.MaxLength > 0)
                htmlAttributes["maxlength"] = options.MaxLength;
            if (options.IsDisabled)
                htmlAttributes["disabled"] = "disabled";
            if (options.IsReadOnly)
                htmlAttributes["readonly"] = "readonly";

            var inputContent = htmlHelper.TextBox(forExpression, null, htmlAttributes);

            var iconSpan = new TagBuilder("span");
            iconSpan.AddCssClass("ml-2");
            iconSpan.Attributes["data-toggle"] = "tooltip";
            iconSpan.Attributes["data-placement"] = "right";
            iconSpan.Attributes["tabindex"] = "0";
            iconSpan.Attributes["title"] = tooltip;
            iconSpan.InnerHtml.AppendHtml("<i class=\"fas fa-info-circle text-muted\" aria-hidden=\"true\"></i>");

            var inputWrap = new TagBuilder("div");
            inputWrap.AddCssClass("d-flex align-items-center flex-nowrap");
            inputWrap.InnerHtml.AppendHtml(inputContent);
            inputWrap.InnerHtml.AppendHtml(iconSpan);

            if (options.ShowEditButton)
            {
                var editButton = new TagBuilder("button");
                editButton.Attributes["type"] = "button";
                if (!string.IsNullOrWhiteSpace(options.EditButtonId))
                    editButton.Attributes["id"] = options.EditButtonId;
                editButton.AddCssClass(options.EditButtonCssClass);
                editButton.Attributes["title"] = options.EditButtonTitle;
                editButton.InnerHtml.AppendHtml($"<i class=\"{options.EditButtonIconCssClass}\"></i>");
                inputWrap.InnerHtml.AppendHtml(editButton);
            }

            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-4 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(inputWrap);
            var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger w-100" });
            inputColumn.InnerHtml.AppendHtml(validationMessage);

            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-5 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public sealed class AccordianPasswordOptions
        {
            public bool IsRequired { get; set; }
            public string Placeholder { get; set; } = "";
            public string AutoComplete { get; set; } = "new-password";
        }

        public static IHtmlContent AccordianBasePasswordBox(this IHtmlHelper htmlHelper, string label, string forExpression, AccordianPasswordOptions? options = null)
        {
            options ??= new AccordianPasswordOptions();

            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");
            if (options.IsRequired)
                labelContent.AddCssClass("required");

            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-5 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            var htmlAttributes = new Dictionary<string, object>
            {
                { "class", "form-control" },
                { "placeholder", options.Placeholder },
                { "autocomplete", options.AutoComplete }
            };

            var inputContent = htmlHelper.Password(forExpression, null, htmlAttributes);

            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-4 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(inputContent);
            inputColumn.InnerHtml.AppendHtml(htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" }));

            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public static IHtmlContent AccordianBasePhoneNumberWithCode(this IHtmlHelper htmlHelper, string label, string forExpression, bool isRequired = false, string placeholder = "Enter 10-digit phone number", string countryCode = "+91", bool isDisabled = false)
        {
            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");
            if (isRequired)
                labelContent.AddCssClass("required");

            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-5 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            var inputGroup = new TagBuilder("div");
            inputGroup.AddCssClass("input-group");

            var prependDiv = new TagBuilder("div");
            prependDiv.AddCssClass("input-group-prepend");
            prependDiv.InnerHtml.AppendHtml($"<span class='input-group-text'>{countryCode}</span>");
            inputGroup.InnerHtml.AppendHtml(prependDiv);

            var htmlAttributes = new Dictionary<string, object>
            {
                { "class", "form-control" },
                { "placeholder", placeholder },
                { "maxlength", 10 },
                { "pattern", "[6-9][0-9]{9}" },
                { "type", "tel" },
                { "inputmode", "numeric" }
            };
            if (isDisabled)
                htmlAttributes["disabled"] = "disabled";
            if (isRequired)
            {
                htmlAttributes["data-val"] = "true";
                htmlAttributes["data-val-required"] = $"{label} is required";
                htmlAttributes["data-val-regex"] = "Please enter a valid 10-digit phone number";
                htmlAttributes["data-val-regex-pattern"] = "^[6-9][0-9]{9}$";
            }

            var inputTag = htmlHelper.TextBox(forExpression, null, htmlAttributes);
            inputGroup.InnerHtml.AppendHtml(inputTag);

            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-4 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(inputGroup);

            if (isRequired)
            {
                var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validationMessage);
            }

            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public static IHtmlContent AccordianBaseFormCheckBox(this IHtmlHelper htmlHelper, string label, string forExpression, string id = "", bool useSwitch = true)
        {
            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");

            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-5 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-4 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(BuildCheckBoxSwitchContent(htmlHelper, forExpression, id, useSwitch));

            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public static IHtmlContent AccordianBaseDropdown(this IHtmlHelper htmlHelper, string label, string forExpression, IEnumerable<SelectListItem> selectList, bool isRequired = false, string placeholder = "", bool isRegExpression = false)
        {
            // Create label element
            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");
            if (isRequired)
                labelContent.AddCssClass("required");

            // Create dropdown with extra attributes
            var dropdownAttributes = new Dictionary<string, object>
            {
                { "class", "form-control selectpicker" },
                { "data-size", "7" },
                { "data-live-search", "true" },
                { "id", $"drp{forExpression}" }
            };
            var dropdownContent = htmlHelper.DropDownList(forExpression, selectList, "Select", dropdownAttributes);

            // Create input column container
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-4 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(dropdownContent);

            if (isRequired || isRegExpression)
            {
                var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validationMessage);
            }
            // Create label column container
            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-5 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            // Create form-group row
            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public static IHtmlContent AccordianBaseDateTimePicker(this IHtmlHelper htmlHelper,
            string label,
            string forExpression,
            string placeholder = "Select date & time",
            string pickerId = "kt_datetimepicker_3",
            bool isRequired = false)
        {
            // Label setup
            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");
            if (isRequired)
                labelContent.AddCssClass("required");

            // Label column
            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-5 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            // Input element
            var inputAttributes = new Dictionary<string, object>
            {
                { "class", "form-control" },
                { "placeholder", placeholder },
                { "id", pickerId },
                { "autocomplete", "off" }
            };

            // Get the model object and its type            
            string? formattedValue = GetFormattedDateTime(forExpression, htmlHelper.ViewData.Model);
            var inputElement = htmlHelper.TextBox(forExpression, formattedValue, inputAttributes);

            // Calendar icon
            var calendarIcon = new TagBuilder("i");
            calendarIcon.AddCssClass("la la-calendar glyphicon-th");

            var inputGroupText = new TagBuilder("span");
            inputGroupText.AddCssClass("input-group-text");
            inputGroupText.InnerHtml.AppendHtml(calendarIcon);

            var inputGroupAppend = new TagBuilder("div");
            inputGroupAppend.AddCssClass("input-group-append");
            inputGroupAppend.InnerHtml.AppendHtml(inputGroupText);

            // Input group
            var inputGroup = new TagBuilder("div");
            inputGroup.AddCssClass("input-group date");
            inputGroup.InnerHtml.AppendHtml(inputElement);
            inputGroup.InnerHtml.AppendHtml(inputGroupAppend);

            // Input column
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-4 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(inputGroup);

            // Form group row
            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }

        public static IHtmlContent AccordianBaseTextArea(this IHtmlHelper htmlHelper,
            string label,
            string forExpression,
            int rows = 2,
            int maxLength = 255,
            string placeholder = "Enter text",
            bool isRequired = false)
        {
            // Label
            var labelContent = new TagBuilder("label");
            labelContent.InnerHtml.Append(label);
            labelContent.AddCssClass("col-form-label");
            if (isRequired)
                labelContent.AddCssClass("required");

            // Label Column
            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-5 col-xs-12 text-md-right");
            labelColumn.InnerHtml.AppendHtml(labelContent);

            // TextArea Attributes
            var textareaAttributes = new Dictionary<string, object>
            {
                { "class", "form-control maximum-length-setup" },
                { "rows", rows },
                { "maxlength", maxLength },
                { "placeholder", placeholder }
            };

            var textarea = htmlHelper.TextArea(forExpression, textareaAttributes);

            // Input Column
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-md-4 col-xs-12");
            inputColumn.InnerHtml.AppendHtml(textarea);

            if (isRequired)
            {
                var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validationMessage);
            }

            // Form Group Row
            var formGroupRow = new TagBuilder("div");
            formGroupRow.AddCssClass("form-group row");
            formGroupRow.InnerHtml.AppendHtml(labelColumn);
            formGroupRow.InnerHtml.AppendHtml(inputColumn);

            return formGroupRow;
        }
        #endregion

        #region 2 Columns Horizontal Form inputs
        public static IHtmlContent TwoColsHorizontalFormTextBox(
            this IHtmlHelper htmlHelper,
            string label,
            string forExpression,
            bool isRequired = false,
            string placeholder = "",
            int maxlength = 0,
            bool isRegExpression = false
        )
        {
            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-lg-2 col-form-label text-right");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);

            // ---- INPUT ATTRIBUTES ----

            // Create input element
            var htmlAttributes = maxlength > 0
                ? (object)new { @class = "form-control maximum-length-setup", maxlength, placeholder }
                : (object)new { @class = "form-control", placeholder };

            var inputTag = htmlHelper.TextBox(forExpression, null, htmlAttributes);

            // ---- COLUMN WITH INPUT ----
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-lg-3");

            inputColumn.InnerHtml.AppendHtml(inputTag);

            // ---- Validation Message ----
            if (isRequired || isRegExpression)
            {
                // Create validation message
                var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validationMessage);
            }

            // ---- FINAL OUTPUT (Label + Column) ----
            var container = new HtmlContentBuilder();
            container.AppendHtml(labelTag);
            container.AppendHtml(inputColumn);

            return container;
        }
        public static IHtmlContent TwoColsHorizontalPhoneNumberWithCode(
    this IHtmlHelper htmlHelper,
    string label,
    string forExpression,
    bool isRequired = false,
    string placeholder = "Enter 10-digit phone number"
)
        {

            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-lg-2 col-form-label text-right");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);

            // ---- INPUT GROUP ----
            var inputGroup = new TagBuilder("div");
            inputGroup.AddCssClass("input-group");

            // ---- COUNTRY CODE PREPEND ----
            var prependDiv = new TagBuilder("div");
            prependDiv.AddCssClass("input-group-prepend");
            prependDiv.InnerHtml.AppendHtml("<span class='input-group-text'>+91</span>");
            inputGroup.InnerHtml.AppendHtml(prependDiv);

            // ---- INPUT ATTRIBUTES ----
            var htmlAttributes = new Dictionary<string, object>
            {
                { "class", "form-control" },
                { "placeholder", placeholder },
                { "maxlength", 10 },
                { "pattern", "[6-9][0-9]{9}" },
                { "type", "tel" },
                { "inputmode", "numeric" }
            };

            if (isRequired)
            {
                htmlAttributes.Add("data-val", "true");
                htmlAttributes.Add("data-val-required", $"{label} is required");
                htmlAttributes.Add("data-val-regex", "Please enter a valid 10-digit phone number");
                htmlAttributes.Add("data-val-regex-pattern", "^[6-9][0-9]{9}$");
            }

            // ---- INPUT TEXTBOX ----
            var inputTag = htmlHelper.TextBox(forExpression, null, htmlAttributes);
            inputGroup.InnerHtml.AppendHtml(inputTag);

            // ---- COLUMN WITH INPUT GROUP ----
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-lg-3");
            inputColumn.InnerHtml.AppendHtml(inputGroup);

            // ---- VALIDATION MESSAGE ----
            if (isRequired)
            {
                var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validationMessage);
            }

            // ---- FINAL OUTPUT (Label + Column) ----
            var container = new HtmlContentBuilder();
            container.AppendHtml(labelTag);
            container.AppendHtml(inputColumn);

            return container;
        }
        public static IHtmlContent TwoColsHorizontalDatePicker(
            this IHtmlHelper htmlHelper,
             string label,
             string forExpression,
             string placeholder = "Select Date",
             bool isRequired = false
        )
        {
            var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(forExpression);
            var fullId = TagBuilder.CreateSanitizedId(fullName, htmlHelper.IdAttributeDotReplacement);

            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-lg-2 col-form-label text-right");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);

            // ---- INPUT ----
            var input = new TagBuilder("input");
            input.TagRenderMode = TagRenderMode.SelfClosing;
            input.Attributes.Add("type", "text");
            input.Attributes.Add("id", fullId);
            input.Attributes.Add("name", fullName);
            input.Attributes.Add("placeholder", placeholder);
            input.Attributes.Add("data-date-format", "yyyy-mm-dd");
            input.AddCssClass("form-control kt_datetimepicker_6");

            // ---- VALUE (for edit / prefill) ----
            string? valueToRender = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out var entry) && entry != null)
            {
                valueToRender = entry.AttemptedValue;
            }
            else
            {
                var currentValue = htmlHelper.ViewData.Eval(forExpression);
                if (currentValue is DateTime dt)
                {
                    valueToRender = dt.ToString("yyyy/MM/dd");
                }
                else if (currentValue != null)
                {
                    valueToRender = currentValue.ToString();
                }
            }
            if (!string.IsNullOrWhiteSpace(valueToRender))
            {
                input.Attributes["value"] = valueToRender;
            }

            // ---- INPUT GROUP ----
            var appendIcon = new TagBuilder("div");
            appendIcon.AddCssClass("input-group-append");
            appendIcon.InnerHtml.AppendHtml(
                "<span class='input-group-text'><i class='la la-calendar glyphicon-th'></i></span>"
            );

            var inputGroup = new TagBuilder("div");
            inputGroup.AddCssClass("input-group date");
            inputGroup.InnerHtml.AppendHtml(input);
            inputGroup.InnerHtml.AppendHtml(appendIcon);

            // ---- COLUMN (col-lg-3) ----
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-lg-3");
            inputColumn.InnerHtml.AppendHtml(inputGroup);

            // ---- VALIDATION MESSAGE ----
            if (isRequired)
            {
                var validation = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validation);
            }

            // ---- RETURN (label + column) ----
            var wrapper = new HtmlContentBuilder();
            wrapper.AppendHtml(labelTag);
            wrapper.AppendHtml(inputColumn);

            return wrapper;
        }

        public static IHtmlContent TwoColsHorizontalDropdown(
            this IHtmlHelper htmlHelper,
            string label,
            string forExpression,
            IEnumerable<SelectListItem> selectList,
            bool isRequired = false,
            string placeholder = "Select",
            bool isRegExpression = false
        )
        {
            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-lg-2 col-form-label text-right");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);

            // ---- DROPDOWN ATTRIBUTES ----
            var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(forExpression);
            var fullId = TagBuilder.CreateSanitizedId(fullName, htmlHelper.IdAttributeDotReplacement);
            var dropdownAttributes = new Dictionary<string, object>
            {
                { "class", "form-control selectpicker" },
                { "data-size", "7" },
                { "data-live-search", "true" },
                { "id", $"drp_{fullId}" }
            };

            // ---- DROPDOWN ----
            var dropdown = htmlHelper.DropDownList(
                forExpression,
                selectList,
                placeholder,
                dropdownAttributes
            );

            // ---- INPUT COLUMN (col-lg-3) ----
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-lg-3");
            inputColumn.InnerHtml.AppendHtml(dropdown);

            // ---- VALIDATION MESSAGE ----
            if (isRequired || isRegExpression)
            {
                var validation = htmlHelper.ValidationMessage(
                    forExpression,
                    null,
                    new { @class = "text-danger" }
                );
                inputColumn.InnerHtml.AppendHtml(validation);
            }

            // ---- RETURN (label + dropdown) ----
            var wrapper = new HtmlContentBuilder();
            wrapper.AppendHtml(labelTag);
            wrapper.AppendHtml(inputColumn);

            return wrapper;
        }

        public static IHtmlContent TwoColsHorizontalTextArea(this IHtmlHelper htmlHelper,
            string label,
            string forExpression,
            int rows = 2,
            int maxLength = 255,
            string placeholder = "Enter text",
            bool isRequired = false)

        {
            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-lg-2 col-form-label text-right");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);

            // Create input element

            // TextArea Attributes
            var textareaAttributes = new Dictionary<string, object>
            {
                { "class", "form-control maximum-length-setup" },
                { "rows", rows },
                { "maxlength", maxLength },
                { "placeholder", placeholder }
            };

            var textarea = htmlHelper.TextArea(forExpression, textareaAttributes);

            // ---- COLUMN WITH INPUT ----
            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-lg-3");

            inputColumn.InnerHtml.AppendHtml(textarea);

            // ---- Validation Message ----
            if (isRequired)
            {
                // Create validation message
                var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                inputColumn.InnerHtml.AppendHtml(validationMessage);
            }

            // ---- FINAL OUTPUT (Label + Column) ----
            var container = new HtmlContentBuilder();
            container.AppendHtml(labelTag);
            container.AppendHtml(inputColumn);

            return container;
        }

        public static IHtmlContent TwoColsHorizontalFormDecimal(
            this IHtmlHelper htmlHelper,
            string label,
            string forExpression,
            bool isRequired = false,
            string placeholder = "0.00",
            string value = null)
        {
            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-lg-2 col-form-label text-right");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);

            // Only styling attributes
            var htmlAttributes = new Dictionary<string, object>
            {
                { "class", "form-control text-right" },
                { "placeholder", placeholder },
                { "type", "text" },
                { "data-decimal", "true" }  // For decimal formatting only
            };

            if (!string.IsNullOrEmpty(value))
                htmlAttributes.Add("value", value);

            var inputTag = htmlHelper.TextBox(forExpression, null, htmlAttributes);

            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-lg-3");
            inputColumn.InnerHtml.AppendHtml(inputTag);

            var validationMessage = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
            inputColumn.InnerHtml.AppendHtml(validationMessage);

            var container = new HtmlContentBuilder();
            container.AppendHtml(labelTag);
            container.AppendHtml(inputColumn);

            return container;
        }

        public static IHtmlContent TwoColsHorizontalFormCheckBox(this IHtmlHelper htmlHelper, string label, string forExpression, string id = "", bool useSwitch = true)
        {
            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-lg-2 col-form-label text-right");
            labelTag.InnerHtml.Append(label);

            var inputColumn = new TagBuilder("div");
            inputColumn.AddCssClass("col-lg-3");
            inputColumn.InnerHtml.AppendHtml(BuildCheckBoxSwitchContent(htmlHelper, forExpression, id, useSwitch));

            var container = new HtmlContentBuilder();
            container.AppendHtml(labelTag);
            container.AppendHtml(inputColumn);

            return container;
        }
        #endregion

        #region 3 Columns Vertical Form inputs
        public static IHtmlContent ThreeColsHorizontalFormCheckBox(this IHtmlHelper htmlHelper, string label, string forExpression, string id = "", bool useSwitch = true)
        {
            var col = new TagBuilder("div");
            col.AddCssClass("col-lg-4");

            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-form-label");
            labelTag.InnerHtml.Append(label);
            col.InnerHtml.AppendHtml(labelTag);

            col.InnerHtml.AppendHtml(BuildCheckBoxSwitchContent(htmlHelper, forExpression, id, useSwitch));

            return col;
        }

        public static IHtmlContent ThreeColsVerticalFormTextBox(
            this IHtmlHelper htmlHelper,
            string label,
            string forExpression,
            bool isRequired = false,
            string placeholder = "",
            int maxlength = 0,
            bool isRegExpression = false
        )
        {
            // ---- COLUMN WRAPPER (col-lg-4) ----
            var col = new TagBuilder("div");
            col.AddCssClass("col-lg-4");

            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);
            col.InnerHtml.AppendHtml(labelTag);

            // ---- INPUT ATTRIBUTES ----            
            var htmlAttributes = maxlength > 0
                ? (object)new { @class = "form-control maximum-length-setup", maxlength, placeholder }
                : (object)new { @class = "form-control maximum-length-setup", placeholder };

            // ---- INPUT TEXTBOX ----
            var input = htmlHelper.TextBox(forExpression, null, htmlAttributes);
            col.InnerHtml.AppendHtml(input);

            // ---- VALIDATION ----
            if (isRequired || isRegExpression)
            {
                var validation = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                col.InnerHtml.AppendHtml(validation);
            }

            // Return only the column; the caller wraps row if needed
            return col;
        }

        public static IHtmlContent ThreeColsVerticalDatePicker(
    this IHtmlHelper htmlHelper,
    string label,
    string forExpression,
    string placeholder = "Select Date",
    bool isRequired = false
)
        {
            // ---- OUTER COLUMN (col-lg-4) ----
            var col = new TagBuilder("div");
            col.AddCssClass("col-lg-4");

            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);
            col.InnerHtml.AppendHtml(labelTag);

            // ---- INPUT GROUP ----
            var inputGroup = new TagBuilder("div");
            inputGroup.AddCssClass("input-group date");

            // ---- INPUT ----
            var htmlAttributes = new Dictionary<string, object>
            {
                { "class", "form-control kt_datetimepicker_6" },
                { "placeholder", placeholder },
                { "autocomplete", "off" }
            };

            var input = htmlHelper.TextBox(forExpression, null, htmlAttributes);
            inputGroup.InnerHtml.AppendHtml(input);

            // ---- INPUT APPEND ICON ----
            var appendIcon = new TagBuilder("div");
            appendIcon.AddCssClass("input-group-append");
            appendIcon.InnerHtml.AppendHtml(
                "<span class='input-group-text'><i class='la la-calendar glyphicon-th'></i></span>"
            );
            inputGroup.InnerHtml.AppendHtml(appendIcon);

            // Add input group to column
            col.InnerHtml.AppendHtml(inputGroup);

            // ---- VALIDATION ----
            if (isRequired)
            {
                var validation = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                col.InnerHtml.AppendHtml(validation);
            }

            return col;
        }
        public static IHtmlContent ThreeColsVerticalDropdown(
    this IHtmlHelper htmlHelper,
    string label,
    string forExpression,
    IEnumerable<SelectListItem> selectList,
    bool isRequired = false,
    bool isRegExpression = false
)
        {
            // ---- COLUMN WRAPPER (col-lg-4) ----
            var col = new TagBuilder("div");
            col.AddCssClass("col-lg-4");

            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);
            col.InnerHtml.AppendHtml(labelTag);

            // ---- DROPDOWN ----
            var dropdown = htmlHelper.DropDownList(
                forExpression,
                selectList,
                new { @class = "form-control" }
            );
            col.InnerHtml.AppendHtml(dropdown);

            // ---- VALIDATION ----
            if (isRequired || isRegExpression)
            {
                var validation = htmlHelper.ValidationMessage(
                    forExpression,
                    null,
                    new { @class = "text-danger" }
                );
                col.InnerHtml.AppendHtml(validation);
            }

            // Return only the column
            return col;
        }

        public static IHtmlContent ThreeColsVerticalFormFileUpload(
    this IHtmlHelper htmlHelper,
    string label,
    string forExpression,
    bool isRequired = false,
    string acceptedFileTypes = "",
    string chooserText = "Choose file",
    bool allowMultiple = false
)
        {
            // ---- COLUMN WRAPPER (col-lg-4) ----
            var col = new TagBuilder("div");
            col.AddCssClass("col-lg-4");

            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);
            col.InnerHtml.AppendHtml(labelTag);

            // ---- CUSTOM FILE DIV ----
            var customFileDiv = new TagBuilder("div");
            customFileDiv.AddCssClass("custom-file");

            // ---- FILE INPUT ATTRIBUTES ----
            var htmlAttributes = new Dictionary<string, object>
    {
        { "class", "custom-file-input" },
        { "id", forExpression },
                { "type","file"}
    };

            if (!string.IsNullOrEmpty(acceptedFileTypes))
                htmlAttributes.Add("accept", acceptedFileTypes);
            if (allowMultiple)
                htmlAttributes.Add("multiple", "multiple");
            // ---- FILE INPUT ----
            var fileInput = htmlHelper.TextBox(
                forExpression,
                null,
                htmlAttributes
            // "file" 
            );

            customFileDiv.InnerHtml.AppendHtml(fileInput);

            // ---- CUSTOM FILE LABEL ----
            var fileLabel = new TagBuilder("label");
            fileLabel.AddCssClass("custom-file-label");
            fileLabel.Attributes.Add("for", forExpression);
            fileLabel.InnerHtml.Append(chooserText);
            customFileDiv.InnerHtml.AppendHtml(fileLabel);

            // Append custom-file div to column
            col.InnerHtml.AppendHtml(customFileDiv);

            // ---- VALIDATION ----
            if (isRequired)
            {
                var validation = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                col.InnerHtml.AppendHtml(validation);
            }

            return col;
        }

        public static IHtmlContent ThreeColsVerticalPhoneNumberWithCode(
    this IHtmlHelper htmlHelper,
    string label,
    string forExpression,
    bool isRequired = false,
    string placeholder = "Enter 10-digit phone number"
)
        {
            // ---- COLUMN WRAPPER (col-lg-4) ----
            var col = new TagBuilder("div");
            col.AddCssClass("col-lg-4");

            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);
            col.InnerHtml.AppendHtml(labelTag);

            // ---- INPUT GROUP ----
            var inputGroup = new TagBuilder("div");
            inputGroup.AddCssClass("input-group");

            // ---- COUNTRY CODE PREPEND ----
            var prependDiv = new TagBuilder("div");
            prependDiv.AddCssClass("input-group-prepend");
            prependDiv.InnerHtml.AppendHtml("<span class='input-group-text'>+91</span>");
            inputGroup.InnerHtml.AppendHtml(prependDiv);

            // ---- INPUT ATTRIBUTES ----
            var htmlAttributes = new Dictionary<string, object>
    {
        { "class", "form-control" },
        { "placeholder", placeholder },
        { "maxlength", 10 },
        { "pattern", "[6-9][0-9]{9}" },
        { "type", "tel" },
        { "inputmode", "numeric" }
    };

            if (isRequired)
            {
                htmlAttributes.Add("data-val", "true");
                htmlAttributes.Add("data-val-required", $"{label} is required");
                htmlAttributes.Add("data-val-regex", "Please enter a valid 10-digit phone number");
                htmlAttributes.Add("data-val-regex-pattern", "^[6-9][0-9]{9}$");
            }

            // ---- INPUT TEXTBOX ----
            var input = htmlHelper.TextBox(forExpression, null, htmlAttributes);
            inputGroup.InnerHtml.AppendHtml(input);

            col.InnerHtml.AppendHtml(inputGroup);

            // ---- VALIDATION ----
            if (isRequired)
            {
                var validation = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
                col.InnerHtml.AppendHtml(validation);
            }

            return col;
        }

        public static IHtmlContent ThreeColsVerticalFormDecimal(
    this IHtmlHelper htmlHelper,
    string label,
    string forExpression,
    bool isRequired = false,
    string placeholder = "0.00"
)
        {
            // ---- COLUMN WRAPPER (col-lg-4) ----
            var col = new TagBuilder("div");
            col.AddCssClass("col-lg-4");

            // ---- LABEL ----
            var labelTag = new TagBuilder("label");
            if (isRequired)
                labelTag.AddCssClass("required");
            labelTag.InnerHtml.Append(label);
            col.InnerHtml.AppendHtml(labelTag);

            // ---- INPUT GROUP ----
            var inputGroup = new TagBuilder("div");
            inputGroup.AddCssClass("input-group");

            // ---- ₹ PREPEND ----
            var prependDiv = new TagBuilder("div");
            prependDiv.AddCssClass("input-group-prepend");
            prependDiv.InnerHtml.AppendHtml("<span class='input-group-text'>₹</span>");
            inputGroup.InnerHtml.AppendHtml(prependDiv);

            // ---- INPUT ATTRIBUTES ----
            var htmlAttributes = new Dictionary<string, object>
    {
        { "class", "form-control text-right" },
        { "placeholder", placeholder },
        { "type", "text" },
        { "data-decimal", "true" },
        // ALWAYS add validation for numeric format only
        { "data-val", "true" },        
        // Add range validation for max 10 lakh
        { "data-val-range", $"{label} must not exceed 10,00,000.00" },
        { "data-val-range-min", "0" },
        { "data-val-range-max", "1000000.00" }
    };

            // Add required validation only if isRequired is true
            if (isRequired)
            {
                htmlAttributes.Add("data-val-required", $"{label} is required");
            }

            // ---- INPUT TEXTBOX ----
            var input = htmlHelper.TextBox(forExpression, null, htmlAttributes);
            inputGroup.InnerHtml.AppendHtml(input);

            col.InnerHtml.AppendHtml(inputGroup);

            // ---- VALIDATION (Always show validation message) ----
            var validation = htmlHelper.ValidationMessage(forExpression, null, new { @class = "text-danger" });
            col.InnerHtml.AppendHtml(validation);

            return col;
        }
        #endregion
        private static IHtmlContent BuildCheckBoxSwitchContent(IHtmlHelper htmlHelper, string forExpression, string id, bool useSwitch)
        {
            string inputId = string.IsNullOrEmpty(id) ? forExpression : id;
            var checkbox = htmlHelper.CheckBox(forExpression, new { id = inputId });

            if (!useSwitch)
                return checkbox;

            var switchSpan = new TagBuilder("span");
            switchSpan.AddCssClass("switch switch-icon");

            var labelInner = new TagBuilder("label");
            labelInner.InnerHtml.AppendHtml(checkbox);
            labelInner.InnerHtml.AppendHtml("<span></span>");

            switchSpan.InnerHtml.AppendHtml(labelInner);
            return switchSpan;
        }

        private static string? GetFormattedDateTime(string forExpression, object? model)
        {
            if (model != null && !string.IsNullOrEmpty(forExpression))
            {
                var property = model.GetType().GetProperty(forExpression);
                if (property != null)
                {
                    var value = property.GetValue(model);
                    if (value is DateTime dtValue)
                    {
                        return dtValue.ToString("yyyy/MM/dd HH:mm"); // match your picker format
                    }
                    else if (value is DateTime?)
                    {
                        var nullableDt = (DateTime?)value;
                        return nullableDt?.ToString("yyyy/MM/dd HH:mm");
                    }
                }
            }
            return null;
        }
    }
}
