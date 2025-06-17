using Microsoft.AspNetCore.Http;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Services.NodesTypes.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using UglyToad.PdfPig;

namespace SquadflowAI.Services.NodesTypes
{
    public class PdfInputNode : INode
    {
        private readonly string _tessDataFolder;
        public PdfInputNode() { }

        public string Id { get; private set; }

        public void Initialize(string id, IDictionary<string, string> parameters)
        {
            Id = id;
        }

        public async Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow, IDictionary<string, byte[]> parametersByte)
        {
            byte[] pdfByte = parametersByte["pdf"];

            //IFormFile uploadedFile = null;

            // From Gmail
            //byte[] pdfFromEmail = null;

            string textFromPdf = ExtractTextFromPdf(pdfByte);

            // From File Upload (e.g., frontend sends IFormFile)
            //using var stream = uploadedFile.OpenReadStream();
            //using var ms = new MemoryStream();
            //stream.CopyTo(ms);

            //byte[] pdfFromUpload = ms.ToArray();

            //string text2 = ExtractTextFromPdf(pdfByte);


            return "";
        }

        

        public string ExtractTextFromPdf(byte[] pdfBytes)
        {
            string tempPdfPath = SaveTempFile(pdfBytes);

            try
            {
                // 1. Try PdfPig (text-based)
                string text = TryPdfPig(tempPdfPath);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine("✅ Extracted using PdfPig.");
                    return text;
                }

                // 2. Fallback: OCR (image-based scan)
                Console.WriteLine("⚠️ PdfPig failed. Using OCR fallback.");
                return RunOcrOnPdf(tempPdfPath);
            }
            finally
            {
                File.Delete(tempPdfPath);
            }
        }

        private string SaveTempFile(byte[] bytes)
        {
            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
            File.WriteAllBytes(path, bytes);
            return path;
        }

        private string TryPdfPig(string path)
        {
            try
            {
                var sb = new StringBuilder();
                using (var doc = PdfDocument.Open(path))
                {
                    foreach (var page in doc.GetPages())
                        sb.AppendLine(page.Text);
                }

                var output = sb.ToString().Trim();
                return string.IsNullOrWhiteSpace(output) ? null : output;
            }
            catch
            {
                return null;
            }
        }

        private string RunOcrOnPdf(string path)
        {
            var sb = new StringBuilder();

            using (var pdfDoc = PdfiumViewer.PdfDocument.Load(path))
            using (var engine = new TesseractEngine(_tessDataFolder, "eng", EngineMode.Default))
            {
                for (int i = 0; i < pdfDoc.PageCount; i++)
                {
                    using (var image = pdfDoc.Render(i, 300, 300, true))
                    using (var bitmap = new Bitmap(image)) // Convert to Bitmap
                    using (var pix = PixConverter.ToPix(bitmap))
                    using (var page = engine.Process(pix))
                    {
                        sb.AppendLine(page.GetText());
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }


   public static class PixConverter
   {
        public static Pix ToPix(Bitmap image)
        {
            using (var stream = new MemoryStream())
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                // Save Bitmap to stream as PNG (lossless)
                image.Save(stream, format);
                stream.Position = 0;

                // Tesseract expects image bytes in memory
                return Pix.LoadFromMemory(stream.ToArray());
            }
        }
   }

}
