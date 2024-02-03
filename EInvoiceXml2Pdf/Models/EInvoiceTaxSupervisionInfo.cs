using System.Xml.Serialization;

namespace EInvoiceXml2Pdf.Models
{
    public  class EInvoiceTaxSupervisionInfo
    {
        public string InvoiceNumber { get; set; }

        [XmlElement(DataType = "date")]
        public DateTime IssueTime { get; set; }

        public string TaxBureauCode { get; set; }

        public string TaxBureauName { get; set; }
    }
}

