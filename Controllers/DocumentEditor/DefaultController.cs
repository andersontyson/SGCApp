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
using Microsoft.AspNetCore.Mvc;

namespace SGCApp.Controllers {
    public partial class DocumentEditorController : Controller
    {
        public ActionResult Default()
        {
            List < object > exportItems = new List<object>();
            
            exportItems.Add(new { text = "Word Document (*.docx)", id = "word" });
            exportItems.Add(new { text = "Word Document (*.pdf)", id = "pdf" });
            ViewBag.ExportItems = exportItems;
            return View();
        }        
    }
}
