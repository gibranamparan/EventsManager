using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jerry.GeneralTools
{
    public class VMConfirmModalAttributes
    {
        public string controller, action, javascriptFunction, primaryMessage, modalTitle, modalID, partialViewURL;
        public dynamic routeVals, confirmButtonHtmlAttributes;
        public object modelForPartial;
        public ViewDataDictionary datosDeVista;
        public Style modalStyle;
        public Size size;
        public CallType callType = CallType.NONE;

        public class Style
        {
            private const string SUCCESS = "success";
            private const string INFO = "info";
            private const string WARNING = "warning";
            private const string DANGER = "danger";
            private const string DEFAULT = "default";

            public enum StyleTypes
            { SUCCESS, INFO, WARNING, DANGER }

            public StyleTypes modalType { get; set; }

            public Style(StyleTypes type)
            {
                this.modalType = type;
            }

            public override string ToString()
            {
                string res = DEFAULT;
                if (this.modalType == StyleTypes.DANGER)
                    res = DANGER;
                if (this.modalType == StyleTypes.SUCCESS)
                    res = SUCCESS;
                if (this.modalType == StyleTypes.INFO)
                    res = INFO;
                if (this.modalType == StyleTypes.WARNING)
                    res = WARNING;
                return res;
            }
        }
        public class Size
        {
            private const string LARGE = "modal-lg";
            private const string SMALL = "modal-sm";

            public enum ModalSize
            { LARGE, SMALL }

            public ModalSize size { get; set; }

            public Size() { }
            public Size(ModalSize size)
            {
                this.size = size;
            }

            public override string ToString()
            {
                string res = "";
                if (this.size == ModalSize.LARGE)
                    res = LARGE;
                if (this.size == ModalSize.SMALL)
                    res = SMALL;
                return res;
            }
        }
        public enum CallType
        {
            NONE, JAVASCRIPT, POSTBACK, PARTIAL_VIEW
        }
    }
}