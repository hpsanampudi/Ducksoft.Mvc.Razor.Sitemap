# Ducksoft.Mvc.Razor.Sitemap
Dynamically creates sitemap.xml for ASP.Net Core 2.2 Razor pages.
- Nuget Package url: https://www.nuget.org/packages/Ducksoft.NetCore.Razor.Sitemap/

# Basic Configuration
### To add all pages to sitemap.xml
1. Add the above mentioned Nuget package to your ASP.Net Core Razor project.
2. In Starup.cs file, 
    - Under ConfigureServices method, configure SitemapRouteConvention and PageRoute.
        ```csharp
        public void ConfigureServices(IServiceCollection services) => 
            services.ConfigureMvcRazorPages(CompatibilityVersion.Version_2_2, "/Index", "Home");

        public static IServiceCollection ConfigureMvcRazorPages(this IServiceCollection services,
            CompatibilityVersion version, string startPageUrl = "", string startPageArea = "")
        {
            services.AddMvc()
                .SetCompatibilityVersion(version)
                .AddRazorPagesOptions(options =>
                {
                    var isSupportAreas = !string.IsNullOrWhiteSpace(startPageArea);
                    options.AllowAreas = isSupportAreas;
                    options.AllowMappingHeadRequestsToGetHandler = true;
                    if (isSupportAreas)
                    {
                        options.Conventions.AddAreaPageRoute(startPageArea, startPageUrl, string.Empty);
                    }
                    else if (!string.IsNullOrWhiteSpace(startPageUrl))
                    {
                        options.Conventions.AddPageRoute(startPageUrl, string.Empty);
                    }
                })
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.Add(new SitemapRouteConvention());
                })
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/Sitemap", "sitemap.xml");
                });

            return services;
        }
        ```

# Advance Configuration
### To add specific pages to sitemap.xml
1. Add `[Sitemap]` class attribute to razor page.
  ```csharp
[Sitemap]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
```
