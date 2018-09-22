using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using iSeal.API.Configuration;
using iSeal.API.Handlers;
using iSeal.Dal;
using iSeal.Dal.Contexts;
using iSeal.Dal.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace iSeal.API
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly RootConfiguration _apiConfiguration;
        private readonly IConfiguration _configuration;        

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;

            _apiConfiguration = new RootConfiguration();
            _configuration.GetSection("Tokens").Bind(_apiConfiguration);

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            if (_hostingEnvironment.IsEnvironment("Testing"))
            {
                services.AddDbContext<iSealDbContext>(
                    ob => ob.UseInMemoryDatabase("iSealDatabase"));

                services.AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = "Test Scheme";
                    opt.DefaultChallengeScheme = "Test Scheme";
                }).AddTestAuth(o => { });
            }
            else
            {
                services.AddDbContext<iSealDbContext>(
                    ob => ob.UseSqlServer(
                        _configuration.GetConnectionString("iSealDatabase"))
                    .UseLazyLoadingProxies());

                //Not sure if it chceks expiration date on token - need to check that
                //IT DOES NOT - FIX THAT
                services.AddAuthentication(opt =>
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
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiConfiguration.Key)),
                            RequireExpirationTime = true,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                        };
                    });
            }            

            services.AddIdentityCore<User>(opt => opt.User.RequireUniqueEmail = true)
                .AddEntityFrameworkStores<iSealDbContext>()
                .AddDefaultTokenProviders();

            

            services.AddHttpContextAccessor();

            Mapper.Initialize(cfg =>
            cfg.AddProfile<Configuration.MapperConfiguration>());

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
