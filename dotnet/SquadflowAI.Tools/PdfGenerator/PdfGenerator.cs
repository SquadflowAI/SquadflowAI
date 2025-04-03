using DinkToPdf;
using DinkToPdf.Contracts;
using SquadflowAI.Contracts;
using SquadflowAI.Tools.Interfaces;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace SquadflowAI.Tools.PdfGenerator
{
    public class PdfGenerator : ITool
    {
        public string Key => "pdf-generator";

        public PdfGenerator() { }

        public async Task<ToolResponseDto> ExecuteAsync(ToolConfigDto config)
        {
            var html = config.Input;

            var converter = new SynchronizedConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4
                },
                Objects = {
                    new ObjectSettings
                    {
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            var path = Path.Combine(Directory.GetCurrentDirectory(), "DynamicPDF.pdf"); // Output file

            var docTest = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Out = path
                },
                Objects = {
                new ObjectSettings
                {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
            };

            byte[] pdfBytes = null;
            try
            {
                // Convert and get the PDF as a byte array
                pdfBytes = converter.Convert(doc);

                Console.WriteLine("PDF generated successfully!");

                converter.Convert(docTest);
                // Save PDF to database
                //await SavePdfToDatabase(pdfBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            var response = new ToolResponseDto()
            {
                ByteData = pdfBytes,
                DataType = Contracts.Enums.ToolDataTypeEnum.Byte
            };

            return response;
        }

        private Task SavePdfToDatabase(byte[] pdfBytes)
        {
            //string connectionString = "YourDatabaseConnectionStringHere";
            //string query = "INSERT INTO PdfFiles (PdfData, CreatedAt) VALUES (@PdfData, @CreatedAt)";

            //using (var connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    using (var command = new SqlCommand(query, connection))
            //    {
            //        command.Parameters.AddWithValue("@PdfData", pdfBytes);
            //        command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            //        command.ExecuteNonQuery();
            //    }
            //}

            //Console.WriteLine("PDF saved to database successfully.");

            return null;
        }
    }
}
