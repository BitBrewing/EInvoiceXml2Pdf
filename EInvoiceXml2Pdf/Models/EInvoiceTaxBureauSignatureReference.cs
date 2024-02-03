using System.Xml.Serialization;

namespace EInvoiceXml2Pdf.Models
{
    public class EInvoiceTaxBureauSignatureReference
    {
        [XmlAttribute]
        public string URI { get; set; }
    }
}

