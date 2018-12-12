using System;
using System.Runtime.CompilerServices;
using Ducksoft.Mvc.Razor.Sitemap.Utilities;

namespace Ducksoft.Mvc.Razor.Sitemap.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <seealso cref="Ducksoft.Mvc.Razor.Sitemap.Models.ISitemapPage" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SitemapAttribute : Attribute, ISitemapPage
    {
        /// <summary>
        /// Gets the name of the area.
        /// </summary>
        /// <value>
        /// The name of the area.
        /// </value>
        public string AreaName { get; }

        /// <summary>
        /// Gets the name of the page.
        /// </summary>
        /// <value>
        /// The name of the page.
        /// </value>
        public string PageName { get; }

        /// <summary>
        /// Gets the relative file path.
        /// </summary>
        /// <value>
        /// The relative file path.
        /// </value>
        public string RelativePath { get; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; }

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapAttribute" /> class.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="areaName">Name of the area.</param>
        /// <param name="filePath">The file path.</param>
        public SitemapAttribute(string pageName = "", string areaName = "",
            [CallerFilePath]string filePath = "")
        {
            AreaName = areaName?.Trim() ?? string.Empty;
            PageName = pageName?.Trim() ?? string.Empty;
            RelativePath = string.Empty;

            //Hp --> Logic: Always take cshtml file path instead of actual csharp file passed.
            var cshtmlFilePath = string.Empty;
            var srcFilePath = filePath?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(srcFilePath))
            {
                var dirPath = Utility.GetDirectoryPath(srcFilePath);
                var fileNameWithoutExt = Utility.GetFileNameWithoutExtension(filePath);
                cshtmlFilePath = Utility.GetCombinedPath(dirPath, fileNameWithoutExt);
            }

            FilePath = cshtmlFilePath;
            LastModified = Utility.GetLastModifiedDate(cshtmlFilePath) ?? DateTime.Now;
        }
    }
}
