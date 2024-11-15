using WebApplication1edsf.Models;
using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Escript
{
	internal class Program
	{
        public static TemplateModel A = new TemplateModel();
        static void Main(string[] args)
		{
			foreach(string s in args)
			{
				Console.WriteLine(s);
			}
			string dir = ".\\firstCase\\";
			if (args.Length > 0)
			{
				dir = args[0];
			}
            string slidesname = "slides.txt";
            string scriptname = "script.txt";



            var builder = WebApplication.CreateBuilder(args);
			
            A.Start(dir+slidesname, dir+scriptname);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
				Path.Combine(builder.Environment.ContentRootPath, dir)),
                RequestPath = "/case"
            });

            app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();

		}
	}
}