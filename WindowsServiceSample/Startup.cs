using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WindowsServiceSample
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

            services.AddControllers();

            var logger = new LoggerConfiguration().WriteTo.File(@"C:\Logs\Pinglog.txt").CreateLogger();
            services.AddSingleton<Serilog.ILogger>(logger);

            services.AddSingleton<IPingSettings>(new PingSettings()
            {
                Timeout = TimeSpan.FromSeconds(5),
                Frequency = TimeSpan.FromSeconds(30),
                Target = "trs.nednet.co.za"
            });


            services.AddHostedService<PingerService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }

    }
}
