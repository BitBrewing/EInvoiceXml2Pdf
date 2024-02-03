namespace EInvoiceXml2Pdf.Models
{
    public class EInvoiceHeader
    {
        public string EIid { get; set; }

        public string EInvoiceTag { get; set; }

        public string Version { get; set; }

        public EInvoiceHeaderInherentLabel InherentLabel { get; set; }

        public EInvoiceHeaderUndefinedLabel UndefinedLabel { get; set; }
    }
}
