// ============================================================
//  TrackNGo Mati — PDF/Report Export Service
//  Uses DinkToPdf for PDF generation; CSV is built-in
// ============================================================
using System;
using System.Collections.Generic;
using System.Text;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace TrackNGoMati.Services
{
    public class ExportService
    {
        private readonly IConverter _pdfConverter;

        public ExportService(IConverter pdfConverter)
        {
            _pdfConverter = pdfConverter;
        }

        /// <summary>
        /// Generates a CSV byte array from a list of rows.
        /// </summary>
        public byte[] GenerateCsv(IEnumerable<string[]> rows, string[] headers)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", Array.ConvertAll(headers, h => $"\"{h}\"")));
            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",", Array.ConvertAll(row, cell => $"\"{(cell ?? "").Replace("\"", "\"\"")}\"")));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        /// <summary>
        /// Generates a PDF from an HTML string using DinkToPdf.
        /// Returns null if the converter is unavailable (wkhtmltopdf not installed).
        /// </summary>
        public byte[]? GeneratePdf(string title, string htmlBody)
        {
            try
            {
                var doc = new HtmlToPdfDocument
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ColorMode   = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize   = PaperKind.A4,
                        Margins     = new MarginSettings { Top = 15, Bottom = 15, Left = 15, Right = 15 }
                    },
                    Objects =
                    {
                        new ObjectSettings
                        {
                            HtmlContent = WrapHtml(title, htmlBody),
                            WebSettings = { DefaultEncoding = "utf-8" }
                        }
                    }
                };
                return _pdfConverter.Convert(doc);
            }
            catch
            {
                // wkhtmltopdf native binary not found — return null
                return null;
            }
        }

        private static string WrapHtml(string title, string body) => $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <style>
    body {{ font-family: Arial, sans-serif; font-size: 11px; color: #0F172A; }}
    h1   {{ font-size: 18px; color: #1A56DB; border-bottom: 2px solid #1A56DB; padding-bottom: 8px; }}
    table {{ width: 100%; border-collapse: collapse; margin-top: 16px; }}
    th    {{ background: #1A56DB; color: #fff; padding: 8px; text-align: left; font-size: 10px; }}
    td    {{ border: 1px solid #E2E8F0; padding: 7px; }}
    tr:nth-child(even) {{ background: #EBF2FF; }}
    .footer {{ margin-top: 24px; font-size: 9px; color: #64748B; text-align: center; }}
  </style>
</head>
<body>
  <h1>City Government of Mati — TrackNGo Mati</h1>
  <p style=""color:#64748B;margin-bottom:16px;"">Report: <strong>{title}</strong> &nbsp;|&nbsp; Generated: {DateTime.Now:yyyy-MM-dd HH:mm}</p>
  {body}
  <div class=""footer"">TrackNGo Mati v1.0 &middot; EODB-Compliant &middot; RA 11032 Compliant &middot; Confidential</div>
</body>
</html>";
    }
}
