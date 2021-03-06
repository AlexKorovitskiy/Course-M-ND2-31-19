﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Validation.Models;

namespace Validation
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(cfg =>
            {
                cfg.ConfigureClientsideValidation(x =>
                {
                    x.Add(typeof(CreditCardValidator), (context, rule, validator) => new CreditCardPropertyValidator(rule, validator));
                    x.Add(typeof(ExpirationYearValidator), (context, rule, validator) => new ExpirationYearProperyValidator(rule, validator));
                    x.Add(typeof(ExpirationMonthValidator), (context, rule, validator) => new ExpirationMonthPropertyValidator(rule, validator));
                    x.Add(typeof(SecurityCodeValidator), (context, rule, validator) => new SecurityCodePropertyValidator(rule, validator));
                    x.Add(typeof(AmountValidator), (context, rule, validator) => new AmountPropertyValidator(rule, validator));
                    x.Add(typeof(PostCodeValidator), (context, rule, validator) => new PostCodePropertyValidator(rule, validator));
                    });
                cfg.RegisterValidatorsFromAssemblyContaining<Startup>();
            });
            services.AddTransient<IValidator<Payment>, PaymentValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Pay}");
            });
        }
    }
}
