using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Nunoguz_Boilerplate.Domain.Repositories;
using Nunoguz_Boilerplate.Domain.Services;
using Nunoguz_Boilerplate.Persistence.Contexts;
using Nunoguz_Boilerplate.Persistence.Repositories;
using Nunoguz_Boilerplate.Services;
using Nunoguz_Boilerplate.Shared;
using Nunoguz_Boilerplate.Shared.Generators;
using Nunoguz_Boilerplate.Shared.Middlewares;
using Nunoguz_Boilerplate.Shared.Utilities;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Text;
// Author 'Oguzhan Cakir'
namespace Nunoguz_Boilerplate
{
    public class Startup
    {
        private const string ApiVersion = "1.0";
        private readonly string CorsAll = "all";
        private readonly string CorsLimited = "limitedCors";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        //Reads from appsettings.json
        public IConfiguration Configuration { get; }

        readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region AppSettings
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            #endregion

            #region DbConnection
            // Should Be Your Own DB
            services.AddDbContext<DatabaseContext>
                (options => options.UseSqlServer(
                Configuration.GetConnectionString("DbConnection")));
            #endregion

            #region NewtonsoftJson
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            #endregion

            #region Injections
            // JWT Generators
            services.AddTransient<ITokenGenerator, TokenGenerator>();

            // Responses own model when unwanted request came and can't pass the validations
            services.AddScoped<ValidateModelAttribute>();

            // Generic Repository Implementation for EF - DB Access with ORM
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Repo and service injections of our models
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();
            #endregion

            #region Cors Config
            services.AddCors(options =>
            {
                options.AddPolicy(name: CorsLimited,
                              builder =>
                              {
                                  builder.WithOrigins(Configuration.GetSection("Application:AppDomain").Value)
                                  .AllowAnyHeader().AllowAnyMethod();
                              });

                options.AddPolicy(name: CorsAll,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                                      // .WithMethods("PUT", "DELETE", "GET");
                                      // To do it, Use [EnableCors("CorsPolicyName")] in Controller or endpoint
                                  });
            });
            #endregion

            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddDataProtection();

            #region Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        //ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        //ClockSkew = TimeSpan.FromMinutes(30)
                    };
                });
            #endregion

            // to use our custom apiResponse with validate models' errors (ModelState.IsValid)
            //services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = false; });

            #region Versioning
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
                o.ApiVersionReader = new HeaderApiVersionReader(); //"x-monosign-api-version"
            });

            services.AddVersionedApiExplorer(
                options =>
                {
                    // "'v'major[.minor][-status]" þeklinde
                    options.GroupNameFormat = "'v'VVV";
                    // Yalnýzca versioning url segmenti için gereklidir.
                    options.SubstituteApiVersionInUrl = true;
                });
            #endregion

            #region Swagger
            //services.AddTransient<IConfiguration<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Nunoguz Base Service",
                    Description = "",
                    Contact = new OpenApiContact
                    {
                        Name = "Oguzhan Cakir",
                        Email = "nunocakiroglu@gmail.com"
                    }

                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter Bearer [space] and then user token in the text input below.\r\n\r\n Bearer yazýp boþluktan sonra tokeni giriniz\r\n\r\nExample: Bearer 12345abcdef",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });

                //c.DescribeAllEnumsAsStrings();
                // to provide comment support on swagger 
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nunoguz_Boilerplate v1"));
            }

            app.UseMiddleware(typeof(ErrorHandlerMiddleware), ApiVersion);

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nunoguz_Boilerplate v1"));

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            //app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
            //    RequestPath = new PathString("/Resources")
            //});

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseCors(CorsAll);
            //app.UseCors(CorsLimited);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
