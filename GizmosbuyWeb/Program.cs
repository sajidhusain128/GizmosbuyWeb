using System.Globalization;
using System.Reflection;
using System.Text;
using FastReport;
using FastReport.Export.PdfSimple;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.BAL.Repository;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.Web.Middlewares;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twilio;

namespace GizmosbuyWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load log4net config
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var logger = LogManager.GetLogger(typeof(Program));

            try
            {
                logger.Info("Application starting...");

                var builder = WebApplication.CreateBuilder(args);

                builder.Configuration.AddEnvironmentVariables();

                var cultureInfo = new CultureInfo("en-IN");
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                IWebConfiguration? webConfiguration = builder.Configuration.GetSection("AppSettings").Get<WebConfiguration>();
                IWhatsAppSettings? whatsAppSettings = builder.Configuration.GetSection("WhatsAppSettings").Get<WhatsAppSettings>();

                if (webConfiguration == null)
                {
                    throw new InvalidOperationException("WebConfiguration section is missing or invalid in the configuration.");
                }
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // webConfiguration.ConnectionStrings;

                builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));


                builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

                //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                // .AddEntityFrameworkStores<ApplicationDbContext>();

                builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
                builder.Services.Configure<WebConfiguration>(builder.Configuration.GetSection("AppSettings"));
                builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));
                builder.Services.AddSingleton<ICacheService, CacheService>();
                builder.Services.AddScoped<IAuthenticationBL, AuthenticationBL>();
                builder.Services.AddScoped<ICommonBL, CommonBL>();
                builder.Services.AddScoped<IPurchaseBL, PurchaseBL>();
                builder.Services.AddScoped<ISalesBL, SalesBL>();
                builder.Services.AddScoped<IInventoryBL, InventoryBL>();
                builder.Services.AddScoped<IStoreTransferBL, StoreTransferBL>();
                builder.Services.AddScoped<IMasterBL, MasterBL>();
                builder.Services.AddScoped<IWebConfiguration, WebConfiguration>();
                builder.Services.AddScoped<IWhatsAppSettings, WhatsAppSettings>();
                builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddMemoryCache();

                TwilioClient.Init(whatsAppSettings.AccountSid, whatsAppSettings.AuthToken);

                //builder.Services.AddBALService(webConfiguration ?? new WebConfiguration());

                // Add services to the container.
                builder.Services.AddControllersWithViews()
                                .AddRazorRuntimeCompilation();
                builder.Services.AddRazorPages();

                //SiteKeys.Configure(webConfiguration ?? new WebConfiguration());
                var key = Encoding.ASCII.GetBytes(webConfiguration.SecretKey);

                //builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(webConfiguration.SesssionTimeoutMinutes);
                    options.Cookie.HttpOnly = true; // Make the session cookie inaccessible to client-side scripts
                    options.Cookie.IsEssential = true; // Make the session cookie essential for the application
                });

                builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.Cookie.Name = "MyCookieAuth";
                        options.LoginPath = "/Auth/Login";
                        options.LogoutPath = "/Auth/Logout";
                        options.AccessDeniedPath = "/Auth/AccessDenied";
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(webConfiguration.SesssionTimeoutMinutes);
                        options.SlidingExpiration = true;
                    });

                builder.Services.AddAuthorization();

                //builder.Services.AddApiVersioning(options =>
                //{
                //    options.DefaultApiVersion = new ApiVersion(1, 0);
                //    options.AssumeDefaultVersionWhenUnspecified = true;
                //    options.ReportApiVersions = true;
                //});

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(Constant.MyPolicy,
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
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseSession();

                app.UseCors(Constant.MyPolicy);

                app.MapGet("/", async context =>
                {
                    var authPrinciple = context.Request.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme).Result.Principal;
                    var IsAuthenticated = authPrinciple == null ? false : authPrinciple.Identity.IsAuthenticated;

                    if (!IsAuthenticated)
                    {
                        context.Response.Redirect("/Auth/Login", permanent: true);
                    }
                    return;
                });

                app.Use(async (context, next) =>
                {
                    var userId = context.Session.GetString("UserId");

                    if (string.IsNullOrEmpty(userId) && !context.Request.Path.Value.Contains("/Auth/Login"))
                    {
                        await context.Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        context.Request.HttpContext.Session.Clear();
                        context.Response.Redirect("/Auth/Login", permanent: true);
                        return;
                    }
                    await next();
                });



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

                app.MapGet("/downloadreport", async context =>
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
            catch (Exception ex)
            {
                logger.Error("Unhandled exception", ex);
                throw;
            }
        }
    }
}
