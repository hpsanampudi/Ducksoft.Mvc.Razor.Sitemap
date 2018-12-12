using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Ducksoft.Mvc.Razor.Sitemap.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISitemapBuilder
    {
        /// <summary>
        /// Gets the sitemap pages.
        /// </summary>
        /// <value>
        /// The sitemap pages.
        /// </value>
        IList<ISitemapPage> SitemapPages { get; }

        /// <summary>
        /// Adds the specified route model.
        /// </summary>
        /// <param name="routeModel">The route model.</param>
        void Add(PageRouteModel routeModel);
    }
}
