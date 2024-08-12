#region Copyright Syncfusion Inc. 2001-2024.
// Copyright Syncfusion Inc. 2001-2024. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Generic;
namespace CoreDemos
{
    public class ThemeList
    {
        public string Id { get; set; }
        public string Theme { get; set; }
        public int Index { get; set; }
        bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        public List<ThemeList> ThemeLists()
        {
            List<ThemeList> theme = new List<ThemeList>();
            theme.Add(new ThemeList { Id = "material3", Theme = "Material3", Index = 0 });
            //theme.Add(new ThemeList { Id = "material3-dark", Theme = "Material3 Dark", Index = 1});
            theme.Add(new ThemeList { Id = "fluent", Theme = "Fluent", Index = 1 });
            theme.Add(new ThemeList { Id = "fluent2", Theme = "Fluent2", Index = 2 });
            //theme.Add(new ThemeList { Id = "fluent-dark", Theme = "Fluent Dark", Index = 3});
            theme.Add(new ThemeList { Id = "bootstrap5", Theme = "Bootstrap v5", Index = 3 });
            // theme.Add(new ThemeList { Id = "bootstrap5-dark", Theme = "Bootstrap v5 Dark", Index = 4 });
            theme.Add(new ThemeList { Id = "tailwind", Theme = "Tailwind CSS", Index = 4 });
            //theme.Add(new ThemeList { Id = "tailwind-dark", Theme = "Tailwind CSS Dark", Index = 6});
            // theme.Add(new ThemeList { Id = "material", Theme = "Material", Index = 7 });
            // theme.Add(new ThemeList { Id = "bootstrap4", Theme = "Bootstrap v4", Index = 8 });
            //if (isDevelopment)
            //{
            //    theme.Add(new ThemeList { Id = "bootstrap", Theme = "Bootstrap", Index = 9 });
            //    theme.Add(new ThemeList { Id = "bootstrap-dark", Theme = "Bootstrap Dark", Index = 10 });
            //}
            theme.Add(new ThemeList { Id = "highcontrast", Theme = "High Contrast", Index = 5 });
            //theme.Add(new ThemeList { Id = "fluent2-highcontrast", Theme = "Fluent2 High Contrast", Index = 6 });
            return theme;
        }
    }
}
