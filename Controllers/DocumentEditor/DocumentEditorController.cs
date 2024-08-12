#region Copyright Syncfusion Inc. 2001-2024.
// Copyright Syncfusion Inc. 2001-2024. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Syncfusion.EJ2.DocumentEditor;
using Syncfusion.DocIORenderer;
using Microsoft.AspNetCore.Cors;
using Syncfusion.Pdf;
using WDocument = Syncfusion.DocIO.DLS.WordDocument;
using WFormatType = Syncfusion.DocIO.FormatType;

namespace SGCApp.Controllers.DocumentEditor
{
    [Route("api/[controller]")]
    public class DocumentEditorController : Controller
    {
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("Import")]
        public string Import(IFormCollection data)
        {

            Stream stream = new MemoryStream();
            string type = "docx";
            if (data.Files.Count == 0)
                return null;
            IFormFile file = data.Files[0];
            int index = file.FileName.LastIndexOf('.');
            type = index > -1 && index < file.FileName.Length - 1 ? file.FileName.Substring(index + 1) : "";
            file.CopyTo(stream);
            stream.Position = 0;

            WordDocument document = WordDocument.Load(stream, GetFormatType(type.ToLower()));
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            document.Dispose();
            return json;
        }


        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("SystemClipboard")]
        public string SystemClipboard([FromBody]CustomParameter param)
        {
            if (param.content != null && param.content != "")
            {
                WordDocument document = WordDocument.LoadString(param.content, GetFormatType(param.type.ToLower()));
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                document.Dispose();
                return json;
            }
            return "";
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("MailMerge")]
        public string MailMerge([FromBody]ExportData exportData)
        {
            Byte[] data = Convert.FromBase64String(exportData.documentData.Split(',')[1]);
            MemoryStream stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            try
            {
                Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(stream, Syncfusion.DocIO.FormatType.Docx);
                document.MailMerge.RemoveEmptyGroup = true;
                document.MailMerge.RemoveEmptyParagraphs = true;
                document.MailMerge.ClearFields = true;
                document.Save(stream, Syncfusion.DocIO.FormatType.Docx);
            }
            catch (Exception)
            { }
            string sfdtText = "";
            Syncfusion.EJ2.DocumentEditor.WordDocument worddocument = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(stream, Syncfusion.EJ2.DocumentEditor.FormatType.Docx);
            sfdtText = Newtonsoft.Json.JsonConvert.SerializeObject(worddocument);
            worddocument.Dispose();
            return sfdtText;
        }
       
       
        public class ExportData
        {
            public string fileName { get; set; }
            public string documentData { get; set; }
        }

        private string RetrieveFileType(string name)
        {
            int index = name.LastIndexOf('.');
            string format = index > -1 && index < name.Length - 1 ?
                name.Substring(index) : ".doc";
            return format;
        }

        internal static WFormatType GetWFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            switch (format.ToLower())
            {
                case ".dotx":
                    return WFormatType.Dotx;
                case ".docx":
                    return WFormatType.Docx;
                case ".docm":
                    return WFormatType.Docm;
                case ".dotm":
                    return WFormatType.Dotm;
                case ".dot":
                    return WFormatType.Dot;
                case ".doc":
                    return WFormatType.Doc;
                case ".rtf":
                    return WFormatType.Rtf;
                case ".html":
                    return WFormatType.Html;
                case ".txt":
                    return WFormatType.Txt;
                case ".xml":
                    return WFormatType.WordML;
                case ".odt":
                    return WFormatType.Odt;
                case ".md":
                    return WFormatType.Markdown;
                default:
                    throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            }
        }

        public class SaveParameter
        {
            public string Content { get; set; }
            public string FileName { get; set; }
            public string Format { get; set; }
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("Export")]
        public FileStreamResult Export([FromBody] SaveParameter data)
        {
            string fileName = data.FileName;
            string format = RetrieveFileType(string.IsNullOrEmpty(data.Format) ? fileName : data.Format);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "Document1.docx";
            }
            WDocument document;
            if (format.ToLower() == ".pdf")
            {
                Stream stream = WordDocument.Save(data.Content, FormatType.Docx);
                document = new Syncfusion.DocIO.DLS.WordDocument(stream, Syncfusion.DocIO.FormatType.Docx);
            }
            else
            {
                document = WordDocument.Save(data.Content);
            }
            return SaveDocument(document, format, fileName);
        }

        private FileStreamResult SaveDocument(WDocument document, string format, string fileName)
        {
            Stream stream = new MemoryStream();
            string contentType = "";
            if (format.ToLower() == ".pdf")
            {
                contentType = "application/pdf";
                DocIORenderer render = new DocIORenderer();
                PdfDocument pdfDocument = render.ConvertToPDF(document);
                stream = new MemoryStream();
                pdfDocument.Save(stream);
                pdfDocument.Close();
            }
            else
            {
                WFormatType type = GetWFormatType(format);
                switch (type)
                {
                    case WFormatType.Rtf:
                        contentType = "application/rtf";
                        break;
                    case WFormatType.WordML:
                        contentType = "application/xml";
                        break;
                    case WFormatType.Html:
                        contentType = "application/html";
                        break;
                    case WFormatType.Dotx:
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                        break;
                    case WFormatType.Docx:
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case WFormatType.Doc:
                        contentType = "application/msword";
                        break;
                    case WFormatType.Dot:
                        contentType = "application/msword";
                        break;
                    case WFormatType.Odt:
                        contentType = "application/vnd.oasis.opendocument.text";
                        break;
                    case WFormatType.Markdown:
                        contentType = "text/markdown";
                        break;
                }
                document.Save(stream, type);
            }
            document.Close();
            stream.Position = 0;
            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = fileName
            };
        }


        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("RestrictEditing")]
        public string[] RestrictEditing([FromBody]CustomRestrictParameter param)
        {
            if (param.passwordBase64 == "" && param.passwordBase64 == null)
                return null;
            return WordDocument.ComputeHash(param.passwordBase64, param.saltBase64, param.spinCount);
        }

        internal static FormatType GetFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 Document editor does not support this file format.");
            switch (format.ToLower())
            {
                case "dotx":
                case "docx":
                case "docm":
                case "dotm":
                    return FormatType.Docx;
                case "dot":
                case "doc":
                    return FormatType.Doc;
                case "rtf":
                case ".rtf":
                    return FormatType.Rtf;
                case "txt":
                    return FormatType.Txt;
                case "xml":
                    return FormatType.WordML;
                case "html":
                case ".html":
                    return FormatType.Html;
                default:
                    throw new NotSupportedException("EJ2 Document editor does not support this file format.");
            }
        }

        internal WDocument GetDocument(IFormCollection data)
        {
            Stream stream = new MemoryStream();
            if (data.Files.Count == 0)
                return null;
            IFormFile file = data.Files[0];

            file.CopyTo(stream);
            stream.Position = 0;

            WDocument document = new WDocument(stream, WFormatType.Docx);
            stream.Dispose();
            return document;
        }
    }

    public class CustomParameter
    {
        public string content { get; set; }
        public string type { get; set; }
    }

    public class CustomRestrictParameter
    {
        public string passwordBase64 { get; set; }
        public string saltBase64 { get; set; }
        public int spinCount { get; set; }
    }
}