using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain
{
    public static class Constants
    {
        public static class SuccessMessages
        {
            public const string MsgUpdateSuccess = "Record updated successfully";
            public const string MsgInsertSuccess = "Record saved successfully";
        }
        public static class ErrorMessages
        {
            public const string MsgUpdateFailure = "Record not updated,something went wrong";
            public const string MsgInsertFailure = "Record not saved,something went wrong";
            public const string MsgSpecializedTemplateInUse = "The {0} form is already assigned to '{1}'. Deactivate that type or choose a different form template.";
        }
        public static class AlertMessages
        {
            public const string MsgNoRecords = "No such record exists";
            public const string MsgActivateRecords = "Are you sure want to active this reord?";
            public const string MsgDeActivateRecords = "Are you sure want to inactive this record?";
        }
        public static class Common
        {
            public const int DefaultPageSize = 10;
        }
        public static class ScriptMessages
        {
            public const string MsgStatusChangeSuccess = "Status changed successfully";
            public const string MsgDeleteSuccess = "Deleted Successfully!";
            public const string MsgActivateRecords = "Are you sure want to active this reord?";
            public const string MsgDeActivateRecords = "Are you sure want to inactive this record?";
            public const string MsgDeleteConfirmation = "Are you sure want to delete this {0} permanently?";
            public const string MsgDeleteConscent = "You will not be able to revert this!";
            public const string CancelBtnTitle = "Cancel";
            public const string CancelBtnInnerText = "Yes, delete it!";
        }
    }

}
