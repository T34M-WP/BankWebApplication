using BankWebApplication.Data;
using BankWebApplication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

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
        // Add DbContext
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddControllersWithViews();

        // อาจจะต้องเพิ่มการลงทะเบียน JwtService ที่นี่หากมีการใช้งาน JwtService
        services.AddSingleton<JwtService>(new JwtService("eyJhbGciOiJIUzI1NiJ9.eyJSb2xlIjoiQWRtaW4iLCJJc3N1ZXIiOiJJc3N1ZXIiLCJVc2VybmFtZSI6IkphdmFJblVzZSIsImV4cCI6MTcwODk1OTY0MywiaWF0IjoxNzA4OTU5NjQzfQ.VYtVCgLPUcgvNrVRSx64RA9cf_9hxheaN5KqanoITrw", "eyJhbGciOiJIUzI1NiJ9.eyJSb2xlIjoiQWRtaW4iLCJJc3N1ZXIiOiJJc3N1ZXIiLCJVc2VybmFtZSI6IkphdmFJblVzZSIsImV4cCI6MTcwODk1OTY0MywiaWF0IjoxNzA4OTU5NjQzfQ.L-64q7bY5XRjMs3Edi2NN6Y_n7ThaGlUL6e7wI_Qi-I", "eyJhbGciOiJIUzI1NiJ9.eyJSb2xlIjoiQWRtaW4iLCJJc3N1ZXIiOiJJc3N1ZXIiLCJVc2VybmFtZSI6IkphdmFJblVzZSIsImV4cCI6MTcwODk1OTY0MywiaWF0IjoxNzA4OTU5NjQzfQ.oA7O1DfV_hB4GlIcV3J6ziiunjiPlWwAnj16XQHsovU"));

        services.AddScoped<TransactionService>();
    }

    // สร้าง middleware เพื่อตรวจสอบ logintoken และตั้งค่า User.Identity.IsAuthenticated
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // ตรวจสอบว่ามี logintoken ในคำขอหรือไม่
            if (context.Request.Cookies.ContainsKey("logintoken"))
            {
                // หากมี logintoken ให้ตั้งค่า User.Identity.IsAuthenticated เป็น true
                context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "authenticated") }, "cookie"));
            }

            await _next(context);
        }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // สร้าง middleware เพื่อตรวจสอบ logintoken และตั้งค่า User.Identity.IsAuthenticated
        app.UseMiddleware<AuthenticationMiddleware>();

        app.UseAuthorization();


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");
        });
    }


}
