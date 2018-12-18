using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ducksoft.Mvc.Razor.Sitemap.Utilities;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ducksoft.Mvc.Razor.Sitemap.Models
{
    /// <summary>
    /// Singleton class, which is used to perform operations realted to sitemap xml file.
    /// </summary>
    /// <seealso cref="Ducksoft.Mvc.Razor.Sitemap.Models.ISitemapBuilder" />
    public sealed class SitemapBuilder : ISitemapBuilder
    {
        /// <summary>
        /// Initializes the instance of singleton object of this class.
        /// Note: Static members are 'eagerly initialized', that is, immediately when class is
        /// loaded for the first time.
        /// .NET guarantees thread safety through lazy initialization
        /// </summary>
        private static readonly Lazy<SitemapBuilder> instance =
            new Lazy<SitemapBuilder>(() => new SitemapBuilder());

        /// <summary>
        /// Gets the instance of the singleton object: SitemapUtility.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static SitemapBuilder Instance => instance.Value;

        /// <summary>
        /// The source page routes list
        /// </summary>
        private List<ISitemapPage> pageRoutesList;

        /// <summary>
        /// Gets the page routes list.
        /// </summary>
        /// <value>
        /// The page routes list.
        /// </value>
        public IReadOnlyList<ISitemapPage> PageRoutesList => pageRoutesList.AsReadOnly();

        /// <summary>
        /// The sitemap attribute list
        /// </summary>
        private List<ISitemapPage> sitemapAttrList;

        /// <summary>
        /// Gets the sitemap attribute list.
        /// </summary>
        /// <value>
        /// The sitemap attribute list.
        /// </value>
        public IReadOnlyList<ISitemapPage> SitemapAttrList => sitemapAttrList.AsReadOnly();

        /// <summary>
        /// Prevents a default instance of the <see cref="SitemapBuilder"/> class from being created.
        /// </summary>
        private SitemapBuilder()
        {
            pageRoutesList = new List<ISitemapPage>();
            sitemapAttrList = GetAllSitemapPageAttributes();
        }

        #region Interface: ISitemapBuilder implementation
        /// <summary>
        /// Gets the sitemap pages.
        /// </summary>
        /// <value>
        /// The sitemap pages.
        /// </value>
        public IReadOnlyList<ISitemapPage> SitemapPages
        {
            get
            {
                if (!sitemapAttrList?.Any() ?? true)
                {
                    return PageRoutesList;
                }

                var pageList = pageRoutesList
                    .Join(sitemapAttrList,
                        L => new { L.AreaName, L.PageName },
                        R => new { R.AreaName, R.PageName },
                        (L, R) => new SitemapPage
                        {
                            AreaName = L.AreaName,
                            PageName = L.PageName,
                            RelativePath = L.RelativePath,
                            FilePath = R.FilePath,
                            LastModified = R.LastModified
                        })
                        .ToList()
                        .AsReadOnly();

                return pageList;
            }
        }

        /// <summary>
        /// Adds the specified route model.
        /// </summary>
        /// <param name="routeModel">The route model.</param>
        void ISitemapBuilder.Add(PageRouteModel routeModel)
        {
            var srcAreaName = routeModel?.AreaName?.Trim() ?? string.Empty;
            var srcPageName = GetRouteValue(routeModel, "page");
            bool isExists(ISitemapPage page)
            {
                var areaName = page?.AreaName?.Trim() ?? string.Empty;
                var pageName = page?.PageName?.Trim() ?? string.Empty;
                return areaName.IsEqualTo(srcAreaName) && pageName.IsEqualTo(srcPageName);
            };

            if ((!routeModel?.RouteValues?.Any() ?? true) || pageRoutesList.Any(isExists))
            {
                return;
            }

            var srcRelativePath = routeModel?.RelativePath?.Trim() ?? string.Empty;
            pageRoutesList.Add(new SitemapPage
            {
                AreaName = srcAreaName,
                PageName = srcPageName,
                RelativePath = srcRelativePath,
                FilePath = string.Empty,
                LastModified = null
            });
        }

        #endregion

        /// <summary>
        /// Gets all sitemap page attributes.
        /// </summary>
        /// <returns></returns>
        private List<ISitemapPage> GetAllSitemapPageAttributes()
        {
            var assembly = Assembly.GetEntryAssembly();
            var lastModified = Utility.GetLastModifiedDate(assembly.Location);
            return assembly
                .GetTypes()
                .Where(T => T.IsSubclassOf(typeof(PageModel)))
                .Select(T =>
                {
                    var sitemapPage = default(ISitemapPage);
                    var sitemapAttr = T.GetCustomAttributes<SitemapAttribute>().SingleOrDefault();
                    if (sitemapAttr == null)
                    {
                        return sitemapPage;
                    }

                    var areaName = sitemapAttr.AreaName?.Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(areaName))
                    {
                        areaName = T.GetAreaName();
                    }

                    var pageName = sitemapAttr.PageName?.Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(pageName))
                    {
                        pageName = T.GetPageName();
                    }

                    var filePath = sitemapAttr.FilePath;
                    return new SitemapPage
                    {
                        AreaName = areaName,
                        PageName = pageName,
                        RelativePath = string.Empty,
                        FilePath = filePath,
                        LastModified = sitemapAttr.LastModified ?? lastModified,
                    };
                })
                .Where(S => S != null)
                .ToList();
        }

        /// <summary>
        /// Gets the route value.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private string GetRouteValue(PageRouteModel model, string key)
        {
            var value = string.Empty;
            if (model?.RouteValues?.ContainsKey(key) ?? false)
            {
                value = model.RouteValues[key]?.Trim() ?? string.Empty;
            }

            return value;
        }

        /// <summary>
        /// Determines whether the specified source is equal.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <c>true</c> if the specified source is equal; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEqual(PageRouteModel source, PageRouteModel target)
        {
            var srcAreaName = source?.AreaName?.Trim() ?? string.Empty;
            var srcPageName = GetRouteValue(source, "page");

            var trgtAreaName = target?.AreaName?.Trim() ?? string.Empty;
            var trgtPageName = GetRouteValue(target, "page");

            return srcAreaName.IsEqualTo(trgtAreaName) && srcPageName.IsEqualTo(trgtPageName);
        }
    }
}
