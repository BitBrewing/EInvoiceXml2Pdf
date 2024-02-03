using System;
using System.Linq.Expressions;
using System.Reflection;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

public static class PdfPTableExtensions
{
    public static PdfPTable AddCell2<T>(this T element, PdfPCell child) where T : PdfPTable
    {
        element.AddCell(child);
        return element;
    }

    public static PdfPTable AddCell2<T>(this T element, Func<PdfPCell> child) where T : PdfPTable
    {
        element.AddCell(child());
        return element;
    }
}

public static class RectangleExtensions
{
    public static T SetBorderWidth<T>(this T element, float all) where T : Rectangle
    {
        element.BorderWidth = all;
        return element;
    }

    public static T SetBorderWidth<T>(this T element, float? left, float? top, float? right, float? bottom) where T : Rectangle
    {
        if (left != null) element.BorderWidthLeft = left.Value;
        if (top != null) element.BorderWidthTop = top.Value;
        if (right != null) element.BorderWidthRight = right.Value;
        if (bottom != null) element.BorderWidthBottom = bottom.Value;
        return element;
    }
}

public static class PdfPCellExtensions
{
    public static T AddElement2<T>(this T element, IElement child) where T : PdfPCell
    {
        element.AddElement(child);
        return element;
    }

    public static T SetPadding<T>(this T element, float all) where T : PdfPCell
    {
        element.Padding = all;
        return element;
    }

    public static T SetPadding<T>(this T element, float? left, float? top, float? right, float? bottom) where T : PdfPCell
    {
        if (left != null) element.PaddingLeft = left.Value;
        if (top != null) element.PaddingTop = top.Value;
        if (right != null) element.PaddingRight = right.Value;
        if (bottom != null) element.PaddingBottom = bottom.Value;
        return element;
    }
}

public static class ITextElementArrayExtensions
{
    public static T Add2<T>(this T element, IElement child) where T : ITextElementArray
    {
        element.Add(child);
        return element;
    }

    public static T Add2<T>(this T element, Func<IElement> child) where T : ITextElementArray
    {
        element.Add(child());
        return element;
    }
}

public static class IElementListenerExtensions
{
    public static T Add2<T>(this T element, IElement child) where T : IElementListener
    {
        element.Add(child);
        return element;
    }

    public static T Add2<T>(this T element, Func<IElement> child) where T : IElementListener
    {
        element.Add(child());
        return element;
    }
}

public static class IElementExtensions
{
    public static T Set<T, P>(this T element, Expression<Func<T, P>> expression, P value) where T : IElement
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            var property = memberExpression.Member as PropertyInfo;

            if (property != null)
            {
                property.SetValue(element, value, null);
            }
            else
            {
                throw new ArgumentException("Argument must be a property");
            }
        }
        else
        {
            throw new ArgumentException("Expression is not a member access");
        }

        return element;
    }
}

