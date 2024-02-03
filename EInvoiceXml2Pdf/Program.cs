/*************************************************************************
 * 全电发票（XML）
 * 参考资料：财政部会计司关于公布电子凭证会计数据标准（试行版）的通知
 * https://kjs.mof.gov.cn/zt/kuaijixinxihuajianshe/dzpzkjsjbzshsd/sjbz/202305/t20230517_3885004.htm
 *
 * iTextSharp.LGPLv2.Core 只能选用 <= 3.4.5
 * 高于这个版本的 PdfPTable 后面的元素会跟 PdfPTable 重叠
 ***********************************************************************/
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using EInvoiceXml2Pdf.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

var inputFilePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "assets", "demo.xml");

// 加载全电发票-XML
var eInvoice = LoadEInvoice(inputFilePath);

var outputDirectoryPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "output");
if (!Directory.Exists(outputDirectoryPath))
    Directory.CreateDirectory(outputDirectoryPath);

Console.WriteLine("正在生成 PDF...");

var document = new Document(new Rectangle(610, 394), 13, 13, 10, 0);

var outputFilePath = Path.Join(outputDirectoryPath, Path.GetFileNameWithoutExtension(inputFilePath) + ".pdf");
if (File.Exists(outputFilePath))
    File.Delete(outputFilePath);

// 字体资源
var fontDirectoryPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Fonts");
var fieldBaseFont = CreateFont(Path.Join(fontDirectoryPath, "simkai.ttf"));
var contentBaseFont = CreateFont(Path.Join(fontDirectoryPath, "simsun.ttf"));
var idNumBaseFont = CreateFont(Path.Join(fontDirectoryPath, "cour.ttf"));

var fieldColor = new BaseColor(128, 0, 0);
var contentColor = new BaseColor(0, 0, 0);
var idNumColor = new BaseColor(0, 0, 0);
var titleFont = new Font(fieldBaseFont, 20.5F, Font.NORMAL, fieldColor);
var fieldFont = new Font(fieldBaseFont, 9F, Font.NORMAL, fieldColor);
var contentFont = new Font(contentBaseFont, 9F, Font.NORMAL, contentColor);
var idNumFont = new Font(idNumBaseFont, 12F, Font.NORMAL, idNumColor);

using var outputStream = File.OpenWrite(outputFilePath);
var writer = PdfWriter.GetInstance(document, outputStream);

document.Open();

// 内容总宽度
var contentWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

document
#region 标题
    .Add2(
        new PdfPTable(new float[] { 210, contentWidth - 420 / 2, 210 })
            .Set(x => x.TotalWidth, contentWidth)
            .Set(x => x.LockedWidth, true)
            .Set(x => x.SpacingAfter, 25)
            .Set(x => x.SpacingBefore, 0)
            .AddCell2(
                new PdfPCell()
                    .Set(x => x.BorderWidth, 0)
            )
            .AddCell2(() =>
            {
                // 标题下面的两条线
                var cb = writer.DirectContent;
                cb.SetLineWidth(0.6f);
                cb.SetColorStroke(fieldColor);
                cb.MoveTo(190, document.Top - 42f);
                cb.LineTo(document.PageSize.Width - 200, document.Top - 42f);
                cb.Stroke();

                cb.MoveTo(190, document.Top - 45f);
                cb.LineTo(document.PageSize.Width - 200, document.Top - 45f);
                cb.Stroke();

                return new PdfPCell(new Paragraph($"{eInvoice.Header.InherentLabel.EInvoiceType.LabelName}（{eInvoice.Header.InherentLabel.GeneralOrSpecialVAT.LabelName}）", titleFont)
                    .Set(x => x.Alignment, Element.ALIGN_CENTER))
                    .SetBorderWidth(0)
                    .SetPadding(0, 10, 0, 0);
            })
            .AddCell2(
                new PdfPCell()
                    .AddElement2(
                        new Paragraph()
                            .Add2(new Chunk("发票号码：", fieldFont))
                            .Add2(new Chunk(eInvoice.TaxSupervisionInfo.InvoiceNumber, contentFont))
                    )
                    .AddElement2(
                        new Paragraph()
                        .Add2(new Chunk("开票日期：", fieldFont))
                        .Add2(new Chunk(eInvoice.TaxSupervisionInfo.IssueTime.ToString("yyyy年MM月dd日"), contentFont))
                    )
                    .SetPadding(0, 10, 0, 0)
                    .SetBorderWidth(0)
            )
    )
#endregion
#region 购买方与销售方信息
    .Add2(
        new PdfPTable(new float[] { 34, contentWidth - 68 / 2, 34, contentWidth - 68 / 2 })
            .Set(x => x.TotalWidth, contentWidth)
            .Set(x => x.LockedWidth, true)
            // 第一排
            .AddCell2(
                new PdfPCell(new Phrase("购买方信息", fieldFont))
                    .Set(x => x.BorderColor, fieldColor)
                    .Set(x => x.Rowspan, 2)
                    .SetPadding(3, 6, 3, 6)
            )
            .AddCell2(
                new PdfPCell(
                    new Phrase()
                        .Add2(
                            new Chunk("名称：", fieldFont)
                        )
                        .Add2(
                            new Chunk(eInvoice.EInvoiceData.BuyerInformation.BuyerName, contentFont)
                        )
                )
                .Set(x => x.BorderColor, fieldColor)
                .SetPadding(3, 10, 3, 10)
                .SetBorderWidth(0, null, null, 0)
            )
            .AddCell2(
                new PdfPCell(new Phrase("销售方信息", fieldFont))
                    .Set(x => x.BorderColor, fieldColor)
                    .Set(x => x.Rowspan, 2)
                    .SetPadding(3, 6, 3, 6)
            )
            .AddCell2(
                new PdfPCell(
                    new Phrase()
                        .Add2(
                            new Chunk("名称：", fieldFont)
                        )
                        .Add2(
                            new Chunk(eInvoice.EInvoiceData.SellerInformation.SellerName, contentFont)
                        )
                )
                .Set(x => x.BorderColor, fieldColor)
                .SetPadding(3, 10, 3, 0)
                .SetBorderWidth(0, null, null, 0)
            )
            // 第二排
            .AddCell2(
                new PdfPCell(
                    new Phrase()
                        .Add2(
                            new Chunk("统一社会信用代码/纳税人识别号：", fieldFont)
                        )
                        .Add2(
                            new Chunk(eInvoice.EInvoiceData.BuyerInformation.BuyerIdNum, idNumFont)
                                .SetHorizontalScaling(0.9F)
                        )
                )
                .Set(x => x.BorderColor, fieldColor)
                .SetBorderWidth(0, 0, null, null)
                .SetPadding(3, 5, 3, 0)
            )
            .AddCell2(
                new PdfPCell(
                    new Phrase()
                        .Add2(
                            new Chunk("统一社会信用代码/纳税人识别号：", fieldFont)
                        )
                        .Add2(
                            new Chunk(eInvoice.EInvoiceData.SellerInformation.SellerIdNum, idNumFont)
                                .SetHorizontalScaling(0.9F)
                        )
                )
                .Set(x => x.BorderColor, fieldColor)
                .SetPadding(3, 5, 3, 0)
                .SetBorderWidth(0, 0, null, null)
            )
    )
#endregion
#region 明细
    .Add2(() =>
    {
        var table = new PdfPTable(new float[] { 170, 114, 62, 120, 120, 120, 106, 126 })
            .Set(x => x.TotalWidth, contentWidth)
            .Set(x => x.LockedWidth, true);

        // 表头
        table
            .AddCell2(
                new PdfPCell(new Phrase("项目名称", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                     .SetBorderWidth(null, 0, 0, null)
                     .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase("规格型号", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                     .SetBorderWidth(0, 0, 0, null)
                     .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase("单 位", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                     .SetBorderWidth(0, 0, 0, null)
                     .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase("数 量", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                     .SetBorderWidth(0, 0, 0, null)
                     .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase("单 价", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_RIGHT)
                     .SetBorderWidth(0, 0, 0, null)
                     .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase("金 额", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_RIGHT)
                     .SetBorderWidth(0, 0, 0, null)
                     .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase("税率/征收率", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.BorderWidthTop, 0)
                     .Set(x => x.BorderWidthRight, 0)
                     .Set(x => x.BorderWidthLeft, 0)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                     .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase("税 额", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.BorderWidthTop, 0)
                     .Set(x => x.BorderWidthLeft, 0)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_RIGHT)
                     .SetPadding(5, 4, 5, 6)
            );

        // 内容
        foreach (var item in eInvoice.EInvoiceData.IssuItemInformation)
        {
            table
                //项目名称
                .AddCell2(
                    new PdfPCell(new Phrase(item.ItemName, contentFont))
                        .Set(x => x.BorderColor, fieldColor)
                        .Set(x => x.VerticalAlignment, Element.ALIGN_TOP)
                        .SetBorderWidth(null, 0, 0, 0)
                        .SetPadding(0)
                )
                //规格型号
                .AddCell2(
                    new PdfPCell(new Phrase(item.SpecMod, contentFont))
                        .Set(x => x.BorderColor, fieldColor)
                        .Set(x => x.VerticalAlignment, Element.ALIGN_TOP)
                        .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                        .SetBorderWidth(0)
                        .SetPadding(0)
                )
                //单位
                .AddCell2(
                    new PdfPCell(new Phrase(item.MeaUnits, contentFont))
                        .Set(x => x.BorderColor, fieldColor)
                        .Set(x => x.VerticalAlignment, Element.ALIGN_TOP)
                        .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                        .SetBorderWidth(0)
                        .SetPadding(0)
                )
                //数量
                .AddCell2(
                    new PdfPCell(new Phrase(item.Quantity, contentFont))
                        .Set(x => x.BorderColor, fieldColor)
                        .Set(x => x.VerticalAlignment, Element.ALIGN_TOP)
                        .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                        .SetBorderWidth(0)
                        .SetPadding(0)
                )
                //单价
                .AddCell2(
                    new PdfPCell(new Phrase(item.UnPrice, contentFont))
                        .Set(x => x.BorderColor, fieldColor)
                        .Set(x => x.VerticalAlignment, Element.ALIGN_TOP)
                        .Set(x => x.HorizontalAlignment, Element.ALIGN_RIGHT)
                        .SetBorderWidth(0)
                        .SetPadding(0)
                )
                //金额
                .AddCell2(
                    new PdfPCell(new Phrase(item.Amount, contentFont))
                        .Set(x => x.BorderColor, fieldColor)
                        .Set(x => x.VerticalAlignment, Element.ALIGN_TOP)
                        .Set(x => x.HorizontalAlignment, Element.ALIGN_RIGHT)
                        .SetBorderWidth(0)
                        .SetPadding(0)
                )
                //税率/征收率
                .AddCell2(
                    new PdfPCell(new Phrase(FormatTaxRate(item.TaxRate), contentFont))
                        .Set(x => x.BorderColor, fieldColor)
                        .Set(x => x.VerticalAlignment, Element.ALIGN_TOP)
                        .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                        .SetBorderWidth(0)
                        .SetPadding(0)
                )
                //税额
                .AddCell2(
                    new PdfPCell(new Phrase(item.ComTaxAm, contentFont))
                        .Set(x => x.BorderColor, fieldColor)
                        .Set(x => x.VerticalAlignment, Element.ALIGN_TOP)
                        .Set(x => x.HorizontalAlignment, Element.ALIGN_RIGHT)
                        .SetBorderWidth(0, 0, null, 0)
                        .SetPadding(0)
                )
                ;
        }

        // 占位
        table.AddCell2(
            new PdfPCell()
                .Set(x => x.BorderColor, fieldColor)
                .Set(x => x.BorderWidthTop, 0)
                .Set(x => x.BorderWidthBottom, 0)
                .Set(x => x.Colspan, 8)
                .Set(x => x.FixedHeight, 110 - table.TotalHeight)
                .SetPadding(0)
        );

        return table;
    })
#endregion
#region 合计
    .Add2(
        new PdfPTable(new float[] { 247, 460, 240 })
            .Set(x => x.TotalWidth, contentWidth)
            .Set(x => x.LockedWidth, true)
            .AddCell2(
                new PdfPCell(new Phrase("合\u3000\u3000\u3000\u3000计", fieldFont))
                    .Set(x => x.BorderColor, fieldColor)
                    .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                    .SetBorderWidth(null, 0, 0, null)
                    .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase("¥" + eInvoice.EInvoiceData.BasicInformation.TotalAmWithoutTax, contentFont))
                    .Set(x => x.BorderColor, fieldColor)
                    .Set(x => x.HorizontalAlignment, Element.ALIGN_RIGHT)
                    .SetBorderWidth(0, 0, 0, null)
                    .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase("¥" + eInvoice.EInvoiceData.BasicInformation.TotalTaxAm, contentFont))
                    .Set(x => x.BorderColor, fieldColor)
                    .Set(x => x.HorizontalAlignment, Element.ALIGN_RIGHT)
                    .SetBorderWidth(0, 0, null, null)
                    .SetPadding(5, 4, 5, 6)
            )
    )
#endregion
#region 价税合计
    .Add2(
        new PdfPTable(new float[] { 247, 414, 287 })
            .Set(x => x.TotalWidth, contentWidth)
            .Set(x => x.LockedWidth, true)
            .AddCell2(
                new PdfPCell(new Phrase("价税合计（大写）", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.BorderWidthTop, 0)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                     .Set(x => x.VerticalAlignment, Element.ALIGN_MIDDLE)
                     .SetPadding(5, 4, 5, 6)
            )
            .AddCell2(
                new PdfPCell(
                     new Phrase()
                        .Add2(() =>
                        {
                            var totalImage = LoadTotalImage();
                            var image = Image.GetInstance(totalImage);
                            image.ScaleAbsolute(14, 14);

                            return new Chunk(image, 0, 0);
                        })
                        .Add2(
                            new Chunk(eInvoice.EInvoiceData.BasicInformation.TotalTaxincludedAmountInChinese, contentFont)
                                .SetTextRise(4f)
                         )
                    )
                    .Set(x => x.BorderColor, fieldColor)
                    .SetBorderWidth(0, 0, 0, null)
                    .Set(x => x.VerticalAlignment, Element.ALIGN_MIDDLE)
                    .SetPadding(5)
            )
            .AddCell2(
                new PdfPCell(
                    new Phrase()
                        .Add2(
                            new Chunk("（小写）", fieldFont)
                                .SetTextRise(1.2f)
                        )
                        .Add2(
                            new Chunk("¥" + eInvoice.EInvoiceData.BasicInformation.TotalTaxincludedAmount, contentFont)
                                .SetTextRise(1.2f)
                        )
                    )
                    .Set(x => x.BorderColor, fieldColor)
                    .Set(x => x.BorderWidthTop, 0)
                    .Set(x => x.BorderWidthLeft, 0)
                    .Set(x => x.VerticalAlignment, Element.ALIGN_MIDDLE)
                    .SetPadding(5, 4, 5, 6)
            )
    )
#endregion
#region 备注
    .Add2(
        new PdfPTable(new float[] { 18, contentWidth - 18 })
            .Set(x => x.TotalWidth, contentWidth)
            .Set(x => x.LockedWidth, true)
            .AddCell2(
                new PdfPCell(new Phrase("备\r\n\r\n注", fieldFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.BorderWidthTop, 0)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_CENTER)
                     .Set(x => x.VerticalAlignment, Element.ALIGN_MIDDLE)
                     .Set(x => x.FixedHeight, 56)
                     .SetPadding(3, 6, 3, 6)
            )
            .AddCell2(
                new PdfPCell(new Phrase(eInvoice.EInvoiceData.AdditionalInformation.Remark, contentFont))
                     .Set(x => x.BorderColor, fieldColor)
                     .Set(x => x.BorderWidthTop, 0)
                     .Set(x => x.BorderWidthLeft, 0)
                     .Set(x => x.HorizontalAlignment, Element.ALIGN_LEFT)
                     .Set(x => x.VerticalAlignment, Element.ALIGN_TOP)
                     .Set(x => x.FixedHeight, 56)
                     .SetPadding(0)
            )
    )
#endregion
#region 开票人
    .Add2(
        new Paragraph()
            .Set(x => x.SpacingBefore, 12)
            .Set(x => x.FirstLineIndent, 45)
            .Add2(
                new Chunk("开票人：", fieldFont)
            )
            .Add2(
                new Chunk(eInvoice.EInvoiceData.BasicInformation.Drawer, contentFont)
            )
    )
#endregion
    ;

document.Close();
writer.Close();

Console.WriteLine($"正在打开 PDF: {outputFilePath}...");
Process.Start(new ProcessStartInfo
{
    FileName = outputFilePath,
    UseShellExecute = true,
});

static byte[] LoadTotalImage()
{
    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EInvoiceXml2Pdf.Resources.Images.Total.gif");
    using var mem = new MemoryStream();
    stream.CopyTo(mem);

    return mem.ToArray();
}

static BaseFont CreateFont(string fontFilePath)
{
    return BaseFont.CreateFont(fontFilePath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
}

static EInvoice LoadEInvoice(string xmlFilePath)
{
    var xmlSerializer = new XmlSerializer(typeof(EInvoice));

    using var xmlFileStream = File.OpenRead(xmlFilePath);
    return (EInvoice)xmlSerializer.Deserialize(xmlFileStream);
}

// 格式化税率
static string FormatTaxRate(string taxRate)
{
    /*
     * 注意税率可能为 0.06，也可能为***
     * 结果需要去掉后面的0，因此不能直接使用ToString("P2")
     */
    if (decimal.TryParse(taxRate, out var result))
    {
        var taxRateStr = (result * 100).ToString();
        if (taxRateStr.IndexOf('.') != -1)
        {
            taxRateStr = taxRateStr.TrimEnd('0').TrimEnd('.');
        }
        return taxRateStr + "%";
    }
    return taxRate;
}