namespace EInvoiceXml2Pdf.Models
{
    public class EInvoiceTaxBureauSignature
    {
        public EInvoiceTaxBureauSignatureReference Reference { get; set; }

        public string SignatureAlgorithm { get; set; }

        public string SignatureFormat { get; set; }

        public string SignatureTime { get; set; }

        public string SignatureValue { get; set; }

        public object KeyInfo { get; set; }
    }
}

