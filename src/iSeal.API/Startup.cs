using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iSeal.API.Configuration;
using iSeal.Dal;
using iSeal.Dal.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace iSeal.API
{
    public class Startup
    {
        private readonly RootConfiguration _apiConfiguration;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;

            _apiConfiguration = new RootConfiguration();
            _configuration.GetSection("Tokens").Bind(_apiConfiguration);

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<iSealDbContext>(ob => ob
               .UseSqlServer(_configuration.GetConnectionString("iSealDatabase"))
            );

            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<iSealDbContext>()
                .AddDefaultTokenProviders();

            //Not sure if it chceks expiration date on token - need to check that
            services.AddAuthentication( opt => 
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(cfg =>
           {
               cfg.SaveToken = true;
               cfg.TokenValidationParameters = new TokenValidationParameters()
               {
                   ValidIssuer = _apiConfiguration.Issuer,
                   ValidAudience = _apiConfiguration.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiConfiguration.Key))
               };
           });

            services.AddSingleton(_apiConfiguration);

            services.AddScoped<IUserStore<User>, UserOnlyStore<User, iSealDbContext>>();
            services.AddTransient<iSealDbInitializer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env,
            iSealDbInitializer iSealDbInitializer)
        {
            app.UseDeveloperExceptionPage();
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();

            iSealDbInitializer.Seed().Wait();
        }
    }
}
