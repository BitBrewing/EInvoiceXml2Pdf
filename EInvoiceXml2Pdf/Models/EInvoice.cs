using System.Xml.Serialization;

namespace EInvoiceXml2Pdf.Models
{
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class EInvoice
    {
        public EInvoiceHeader Header { get; set; }

        public EInvoiceEInvoiceData EInvoiceData { get; set; }

        public EInvoiceSellerAuthentication SellerAuthentication { get; set; }

        public EInvoiceTaxSupervisionInfo TaxSupervisionInfo { get; set; }

        public EInvoiceTaxBureauSignature TaxBureauSignature { get; set; }
    }
}

