using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Ducksoft.Mvc.Razor.Sitemap.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ApplicationModels.IPageRouteModelConvention" />
    public class SitemapRouteConvention : IPageRouteModelConvention
    {
        /// <summary>
        /// Called to apply the convention to the <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.PageRouteModel" />.
        /// </summary>
        /// <param name="model">The <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.PageRouteModel" />.</param>
        public void Apply(PageRouteModel model) => ((ISitemapBuilder)SitemapBuilder.Instance).Add(model);
    }
}
