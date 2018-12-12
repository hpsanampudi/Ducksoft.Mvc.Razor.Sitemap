using System.Collections.Generic;
using System.Linq;
using Ducksoft.Mvc.Razor.Sitemap.Models;
using Ducksoft.Mvc.Razor.Sitemap.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ducksoft.Mvc.Razor.Sitemap.Pages
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.RazorPages.PageModel" />
    public class SitemapModel : PageModel
    {
        /// <summary>
        /// The site map nodes
        /// </summary>
        private static readonly IList<ISitemapPage> siteMapNodes;

        /// <summary>
        /// Gets the raw XML data.
        /// </summary>
        /// <value>
        /// The raw XML data.
        /// </value>
        [BindProperty(SupportsGet = true)]
        public string RawXmlData => SerializeToRawXml(HttpContext?.Request);

        /// <summary>
        /// Initializes the <see cref="SitemapModel"/> class.
        /// </summary>
        static SitemapModel()
        {
            siteMapNodes = SitemapBuilder.Instance.SitemapPages;
        }

        /// <summary>
        /// Called when [get].
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// Serializes to raw XML.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private string SerializeToRawXml(HttpRequest request)
        {
            var baseUrlPath = GetBaseUrl(request);
            return new urlset
            {
                url = siteMapNodes.Select(node =>
                {
                    var pageLinkUrl = GetPageLinkUrl(baseUrlPath, node);
                    if (string.IsNullOrWhiteSpace(pageLinkUrl))
                    {
                        return null;
                    }

                    return new tUrl
                    {
                        loc = pageLinkUrl,
                        lastmod = node.LastModified.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                        changefreq = tChangeFreq.always,
                        changefreqSpecified = true,
                        priority = 0.5M,
                        prioritySpecified = true
                    };
                })
                .Where(node => node != null)
                .ToArray()
            }
            .SerializeToXml();
        }

        /// <summary>
        /// Gets the page link URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="sitemapPage">The sitemap page.</param>
        /// <returns></returns>
        private string GetPageLinkUrl(HttpRequest request, ISitemapPage sitemapPage) =>
            GetPageLinkUrl(GetBaseUrl(HttpContext?.Request), sitemapPage);

        /// <summary>
        /// Gets the page link URL.
        /// </summary>
        /// <param name="baseUrlPath">The base URL path.</param>
        /// <param name="sitemapPage">The sitemap page.</param>
        /// <returns></returns>
        private string GetPageLinkUrl(string baseUrlPath, ISitemapPage sitemapPage)
        {
            var pageLinkUrl = string.Empty;
            var areaUrlPath = GetArea(sitemapPage?.AreaName?.Trim() ?? string.Empty);
            var actionUrlPath = GetPage(sitemapPage?.PageName?.Trim() ?? string.Empty);

            if (string.IsNullOrWhiteSpace(baseUrlPath) ||
                (string.IsNullOrWhiteSpace(areaUrlPath) && string.IsNullOrWhiteSpace(actionUrlPath)))
            {
                return pageLinkUrl;
            }

            pageLinkUrl = $"{baseUrlPath}{areaUrlPath}{actionUrlPath}";
            return pageLinkUrl;
        }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private string GetBaseUrl(HttpRequest request) => (request != null) ?
            $"{request.Scheme}://{request.Host.ToUriComponent()}" : string.Empty;

        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <param name="srcAreaName">Name of the source area.</param>
        /// <returns></returns>
        private string GetArea(string srcAreaName)
        {
            var areaName = srcAreaName?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(areaName))
            {
                areaName = $"/{areaName}";
            }

            return areaName;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="srcPageName">Name of the source page.</param>
        /// <returns></returns>
        private string GetPage(string srcPageName)
        {
            var pageName = srcPageName?.Trim() ?? string.Empty;
            if (pageName.TrimStart('/').IsEqualTo("Index"))
            {
                pageName = string.Empty;
            }

            return pageName;
        }
    }
}