using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BethanysPieShop.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BethanysPieShop
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();


            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPieRepository, PieRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            
            
            /* ******  NOTE: Here we are NOT doing the traditional Interface and Implementation when registering 
                         Here we are telling exactly how the ShoppingCart needs to be instantiated ... note it needs 
                         a parameter for instantiation. This parameter is the ServiceProvider, which is what sp is here
                         ShoppingCart.GetCart() is a static method that creates a new shopping cart. The shopping cart's constructor
                         is private. It needs a ServiceProvider so that it can have access to the other services registered in the DI
                         
                         Those classes that need to use the Shopping cart can just DI it by doing      ...ShoppingCart sc...
                         there is NO NEED for an interface.
            *********** /
            services.AddScoped<ShoppingCart>(sp => ShoppingCart.GetCart(sp));


            services.AddHttpContextAccessor();
            services.AddSession();                      /* *** Support for Session cookie *** */

            services.AddControllersWithViews();         //services.AddMvc(); would also work still. it is from  core 2.1
            services.AddRazorPages();                   /* *** RAZOR PAGES *** */

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();                           /* *** Support for Session cookie *** */

            app.UseRouting();
            app.UseAuthentication();                    /* *** Authentication middleware *** */
            app.UseAuthorization();                     /* *** Authorization middleware  *** */

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();              /* *** RAZOR PAGES can co-exist with MVC framework *** */
            });
        }
    }
}
