namespace Client.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpClient("customer", c =>
            c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CustomerApiUrl"]!));

            builder.Services.AddHttpClient("lookup", c =>
            c.BaseAddress = new Uri(builder.Configuration["ApiSettings:LookupApiUrl"]!));


            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Customers}/{action=Index}/{id?}");

            app.Run();

        }
    }
}
