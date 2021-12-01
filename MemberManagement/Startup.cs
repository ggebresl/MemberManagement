using MemberManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;


namespace MemberManagement
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
             
        }

        public IConfiguration Configuration { get; }
      

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddMemoryCache();

            services.AddSession();

            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();
                options.Filters.Add(new AuthorizeFilter(policy));  //Global Authorization filter unless the contorll is declared wity allow anonymous 
            }).AddXmlSerializerFormatters();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });


            var connStr = Configuration.GetConnectionString("TigrayComAppDB");

            services.AddDbContextPool<AppDbContext>(Options =>
            Options.UseSqlServer(connStr));


            //step 2. Add ASP.NET Core Identity Services
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 2;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

            //register the SignInManager to associate with it in the dependency injection setup
            services.AddScoped<SignInManager<User>, SignInManager<User>>();

            //for unit testin only
            // services.AddScoped<IUserloginRepository, MockUserloginRepostiory>();

            services.AddRouting(optins =>
            {
                optins.LowercaseUrls = true;
                optins.AppendTrailingSlash = true;


            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                          ILogger<Startup> logger)
        {
            
            if (env.IsDevelopment())
            {
              //  logger.LogInformation("Middle Ware Incoming request");
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
           
           else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))
         
            {
              //  app.UseExceptionHandler("/Home/Error");
                
            }

          
     

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            //Step 3. Add Authentication middleware

            app.UseAuthentication();
            app.UseAuthorization();
            AppDbContext.CreateAdminUser(app.ApplicationServices).Wait();
            /*
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });*/

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=account}/{action=login}/{id?}");
            });

        }
    }

}

