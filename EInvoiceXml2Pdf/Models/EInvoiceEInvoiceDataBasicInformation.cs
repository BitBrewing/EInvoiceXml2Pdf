using System.Xml.Serialization;

namespace EInvoiceXml2Pdf.Models
{
    public class EInvoiceEInvoiceDataBasicInformation
    {
        public string TotalAmWithoutTax { get; set; }

        public string TotalTaxAm { get; set; }

        [XmlElement("TotalTax-includedAmount")]
        public string TotalTaxincludedAmount { get; set; }

        [XmlElement("TotalTax-includedAmountInChinese")]
        public string TotalTaxincludedAmountInChinese { get; set; }

        public string Drawer { get; set; }

        public string RequestTime { get; set; }
    }
}

