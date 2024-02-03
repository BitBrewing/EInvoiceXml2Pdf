using System.Xml.Serialization;

namespace EInvoiceXml2Pdf.Models
{
    public class EInvoiceEInvoiceData
    {
        public EInvoiceEInvoiceDataSellerInformation SellerInformation { get; set; }

        public EInvoiceEInvoiceDataBuyerInformation BuyerInformation { get; set; }

        public EInvoiceEInvoiceDataBasicInformation BasicInformation { get; set; }

        [XmlElement]
        public EInvoiceEInvoiceDataIssuItemInformation[] IssuItemInformation { get; set; }

        public EInvoiceEInvoiceDataSpecificInformation SpecificInformation { get; set; }

        public EInvoiceEInvoiceDataAdditionalInformation AdditionalInformation { get; set; }
    }
}

