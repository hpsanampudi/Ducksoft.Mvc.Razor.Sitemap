using System;

namespace Ducksoft.Mvc.Razor.Sitemap.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISitemapPage
    {
        /// <summary>
        /// Gets the name of the area.
        /// </summary>
        /// <value>
        /// The name of the area.
        /// </value>
        string AreaName { get; }

        /// <summary>
        /// Gets the name of the page.
        /// </summary>
        /// <value>
        /// The name of the page.
        /// </value>
        string PageName { get; }

        /// <summary>
        /// Gets the relative path.
        /// </summary>
        /// <value>
        /// The relative path.
        /// </value>
        string RelativePath { get; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        string FilePath { get; }

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
        DateTime? LastModified { get; }
    }
}
