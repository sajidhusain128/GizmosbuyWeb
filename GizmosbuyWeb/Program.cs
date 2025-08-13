using System.Reflection;
using System.Text;
using FastReport;
using FastReport.Export.PdfSimple;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.BAL.Repository;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.Web.Middlewares;
using log4net.Config;
using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GizmosbuyWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load log4net config
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            IWebConfiguration? webConfiguration = builder.Configuration.GetSection("AppSettings").Get<WebConfiguration>();

            if (webConfiguration == null)
            {
                throw new InvalidOperationException("WebConfiguration section is missing or invalid in the configuration.");
            }
            var connectionString = webConfiguration.ConnectionStrings;

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));


            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            // .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
            builder.Services.Configure<WebConfiguration>(builder.Configuration.GetSection("AppSettings"));
            builder.Services.AddScoped<IAuthenticationBL, AuthenticationBL>();
            builder.Services.AddScoped<ICommonBL, CommonBL>();
            builder.Services.AddScoped<IPurchaseBL, PurchaseBL>();
            builder.Services.AddScoped<ISalesBL, SalesBL>();
            builder.Services.AddScoped<IInventoryBL, InventoryBL>();
            builder.Services.AddScoped<IWebConfiguration, WebConfiguration>();
            builder.Services.AddHttpContextAccessor();

            //builder.Services.AddBALService(webConfiguration ?? new WebConfiguration());

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                            .AddRazorRuntimeCompilation();
            builder.Services.AddRazorPages();

            //SiteKeys.Configure(webConfiguration ?? new WebConfiguration());
            var key = Encoding.ASCII.GetBytes(webConfiguration.SecretKey);

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(webConfiguration.SesssionTimeoutMinutes);
            });

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = webConfiguration.Issuer,
                    ValidAudience = webConfiguration.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(webConfiguration.SesssionTimeoutMinutes);

                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.SlidingExpiration = true;
                options.Cookie.Expiration = TimeSpan.FromMinutes(webConfiguration.SesssionTimeoutMinutes);
            });

            builder.Services.AddAuthorization();

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy",
                    builder =>
                    {
                        builder.WithOrigins(webConfiguration.Issuer)
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                    });
            });
            builder.Services.AddMvc();
            builder.Services.AddFastReport();

            //builder.Services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

            //    // Add support for XML comments
            //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    options.IncludeXmlComments(xmlPath);
            //});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Shared/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCookiePolicy();
            app.UseSession();
            app.UseCors("MyPolicy");

            app.Use(async (context, next) =>
            {
                var JWToken = context.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(JWToken))
                {
                    context.Request.Headers.Append("Authorization", "Bearer " + JWToken);
                }
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();


            //bool enableSwagger = builder.Configuration.GetValue<bool>("AppSettings:EnableSwagger");

            ////eanble/disable swagger
            //if (enableSwagger)
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI(options =>
            //    {
            //        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
            //        options.RoutePrefix = "api/swagger"; // Set the Swagger UI at the root URL
            //    });
            //}

            app.MapGet("/report", async context =>
            {
                var report = new Report();
                report.Load("Reports/SalesReport.frx");
                report.Prepare();

                using var pdfStream = new MemoryStream();
                var pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, pdfStream);
                pdfStream.Position = 0;

                context.Response.ContentType = "application/pdf";
                context.Response.Headers.ContentDisposition = "attachment; filename=report.pdf";
                await context.Response.Body.WriteAsync(pdfStream.ToArray());
            });

            // Register your custom middleware early in the pipeline
            app.UseMiddleware<ExceptionLoggingMiddleware>();

            app.UseFastReport();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
