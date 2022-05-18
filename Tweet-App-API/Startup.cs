using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tweet_App_API.CustomAuthorization;
using Tweet_App_API.DataBaseLayer;
using Tweet_App_API.Model;
using Tweet_App_API.Services;
using Tweet_App_API.TokenHandler;

namespace Tweet_App_API
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
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false

                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("whocanedit",
                    policy => policy.Requirements.Add(new ManageUserResourceEdit()));
            });

            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tweet_App_API", Version = "v1" });
            });

            services.Configure<TweetAppDBSettings>(
            Configuration.GetSection(nameof(TweetAppDBSettings)));

            services.AddSingleton<ITweetAppDBSettings>(sp =>
                sp.GetRequiredService<IOptions<TweetAppDBSettings>>().Value);


            services.AddSingleton<IDBClient, DBClient>();
            services.AddSingleton<IUserServices, UserServices>();
            services.AddSingleton<ITweetService, TweetService>();
            services.AddSingleton<IAuthorizationHandler, CanOnlyEditAndDeleteItsResource>();
            services.AddSingleton<IGuidService, GuidService>();
            services.AddSingleton<IJwtAuthenticationManager, JwtAuthenticationManager>();
            services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();

            services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(Configuration["TweetAppDBSettings:ConnectionString"]));

            services.AddControllers();
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env , ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tweet_App_API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

          

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
