using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ducksoft.Mvc.Razor.Sitemap.Utilities;
using Microsoft.AspNetCore.Hosting;
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
        /// The environment
        /// </summary>
        private static IHostingEnvironment _environment = null;

        /// <summary>
        /// The host environment
        /// </summary>
        public IHostingEnvironment HostEnvironment = new Lazy<IHostingEnvironment>(
            () => _environment).Value;

        /// <summary>
        /// The content root path
        /// </summary>
        private readonly string contentRootPath;

        /// <summary>
        /// The source page routes list
        /// </summary>
        private List<ISitemapPage> pageRoutesList;

        /// <summary>
        /// The sitemap page attributes list
        /// </summary>
        private readonly IReadOnlyList<ISitemapPage> sitemapAttrList;


        /// <summary>
        /// Prevents a default instance of the <see cref="SitemapBuilder"/> class from being created.
        /// </summary>
        private SitemapBuilder()
        {
            contentRootPath = HostEnvironment?.ContentRootPath?.Trim() ?? string.Empty;
            pageRoutesList = new List<ISitemapPage>();
            sitemapAttrList = GetAllSitemapPageAttributes().AsReadOnly();
        }

        public static void Inject(IHostingEnvironment environment)
        {
            //Hp --> Logic: As we can't construct singleton object with paramaters, 
            //the below logic is work around implementation to acheive it.
            if (_environment != null)
            {
                return;
            }

            _environment = environment;
        }

        #region Interface: ISitemapBuilder implementation
        /// <summary>
        /// Gets the sitemap pages.
        /// </summary>
        /// <value>
        /// The sitemap pages.
        /// </value>
        public IList<ISitemapPage> SitemapPages
        {
            get
            {
                if (!sitemapAttrList?.Any() ?? true)
                {
                    return pageRoutesList;
                }

                var pageList = pageRoutesList
                    .Join(sitemapAttrList, L => L.RelativePath, R => R.RelativePath, (L, R) => L)
                    .ToList();

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
            var srcFilePath = Utility.GetCombinedPath(contentRootPath, srcRelativePath);
            var srcFileLastModified = Utility.GetLastModifiedDate(srcFilePath) ?? DateTime.Now;
            pageRoutesList.Add(new SitemapPage
            {
                AreaName = srcAreaName,
                PageName = srcPageName,
                RelativePath = srcRelativePath,
                FilePath = srcFilePath,
                LastModified = srcFileLastModified
            });
        }

        #endregion

        /// <summary>
        /// Gets all sitemap page attributes.
        /// </summary>
        /// <returns></returns>
        private List<ISitemapPage> GetAllSitemapPageAttributes()
        {
            return Assembly.GetEntryAssembly()
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
                    var relativePath = filePath.Replace(contentRootPath, string.Empty).ToUrlRelativePath();

                    return new SitemapPage
                    {
                        AreaName = areaName,
                        PageName = pageName,
                        RelativePath = relativePath,
                        FilePath = filePath,
                        LastModified = sitemapAttr.LastModified,
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
