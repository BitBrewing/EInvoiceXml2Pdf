namespace EInvoiceXml2Pdf.Models
{
    public class EInvoiceHeaderInherentLabel
    {
        public EInvoiceHeaderInherentLabelType InIssuType { get; set; }

        public EInvoiceHeaderInherentLabelType EInvoiceType { get; set; }

        public EInvoiceHeaderInherentLabelType GeneralOrSpecialVAT { get; set; }

        public EInvoiceHeaderInherentLabelType TaxpayerType { get; set; }
    }
}

