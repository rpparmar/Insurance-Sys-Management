using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.Helper
{
    public static class HtmlHelpers
    {
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

        public static IHtmlContent SimpleFormCheckBox(this IHtmlHelper htmlHelper, string label, string forExpression, string id = "", bool useSwitch = true)
        {
            // Label column
            var labelColumn = new TagBuilder("div");
            labelColumn.AddCssClass("col-md-4 col-xs-12 text-md-right");

            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("col-form-label");
            labelTag.InnerHtml.Append(label);
            labelColumn.InnerHtml.AppendHtml(labelTag);

            // Input (checkbox)
            string inputId = string.IsNullOrEmpty(id) ? forExpression : id;

            var checkbox = htmlHelper.CheckBox(forExpression, new { id = inputId });

            // Switch style container
            IHtmlContent switchWrapper;
            if (useSwitch)
            {
                var switchSpan = new TagBuilder("span");
                switchSpan.AddCssClass("switch switch-icon");

                var labelInner = new TagBuilder("label");

                // Manually render input and span inside label
                labelInner.InnerHtml.AppendHtml(checkbox);
                labelInner.InnerHtml.AppendHtml("<span></span>");

                switchSpan.InnerHtml.AppendHtml(labelInner);
                switchWrapper = switchSpan;
            }
            else
            {
                // Fallback to default checkbox only
                switchWrapper = checkbox;
            }

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
        public static IHtmlContent AccordianBaseTextBox(this IHtmlHelper htmlHelper, string label, string forExpression, bool isRequired = false, string placeholder = "", int maxlength = 0,bool isRegExpression = false)
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
            var inputElement = htmlHelper.TextBox(forExpression, formattedValue , inputAttributes);

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
            bool isRequired = false,
            bool isRegExpression = false)
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

            if (isRequired || isRegExpression)
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

        private static string? GetFormattedDateTime(string forExpression,object? model)
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
