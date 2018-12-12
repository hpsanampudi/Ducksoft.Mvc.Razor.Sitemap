# Ducksoft.Mvc.Razor.Sitemap
Dynamically creates sitemap.xml for ASP.Net Core 2.2 Razor pages.
- Nuget Package url: https://www.nuget.org/packages/Ducksoft.Mvc.Razor.Sitemap/

# Basic Configuration
### To add all pages to sitemap.xml
1. Add the above mentioned Nuget package to your ASP.Net Core Razor project.
2. In Starup.cs file, 
    - Inject the SitemapBuilder object with hosting environment in constructor.
      <pre>
      public Startup(IConfiguration configuration, IHostingEnvironment environment)
      {
          //Inject the SitemapBuilder object with hosting environment
          <b>SitemapBuilder.Inject(environment);</b>
          Configuration = configuration;
      }      
      </pre>
    - Under ConfigureServices method, configure SitemapRouteConvention and PageRoute.
      <pre>
        public void ConfigureServices(IServiceCollection services) => 
            <b>services.ConfigureMvcRazorPages</b>(<b>CompatibilityVersion.Version_2_2</b>, "/Index", "Home");
      </pre>
      <pre>
        public static IServiceCollection <b>ConfigureMvcRazorPages</b>(this IServiceCollection services,
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
                  <b>.AddRazorPagesOptions(options =>
                  {
                      options.Conventions.Add(new SitemapRouteConvention());
                  })
                  .AddRazorPagesOptions(options =>
                  {
                      options.Conventions.AddPageRoute("/Sitemap", "sitemap.xml");
                  });</b>

              return services;
          }
        </pre>

# Advance Configuration
### To add specific pages to sitemap.xml
1. Add <b>[Sitemap]</b> class attribute to razor page.
  <pre>
    <b>[Sitemap]</b>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Called when [get].
        /// </summary>
        public void OnGet()
        {
        }
    }
  </pre>

