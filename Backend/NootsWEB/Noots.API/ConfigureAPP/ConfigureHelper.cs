﻿using System;
using System.Text;
using System.Threading.Tasks;
using Auth.Entities;
using Auth.Impl;
using Auth.Interfaces;
using Backgrounds;
using Billing;
using Common.Redis;
using Common.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Noots.Editor;
using Noots.Folders;
using Noots.History;
using Noots.Labels;
using Noots.Notes;
using Noots.Permissions;
using Noots.Personalization;
using Notifications;
using RelatedNotes;
using Search;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Sharing;
using Storage;
using Users;

namespace API.ConfigureAPP
{
    public static class ConfigureHelper
    {

        public static void Mediatr(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            // USER
            services.ApplyUsersDI();

            // Backgrounds
            services.ApplyBackgroundsDI();

            //Labels
            services.ApplyLabelsDI();

            //Notes
            services.ApplyNotesDI();


            // RELATED NOTES
            services.ApplyRelatedNotesDI();

            services.ApplyEditorModulesDI();

            //FOLDERS
            services.ApplyFoldersDI();

            //SHARE
            services.ApplySharingDI();


            // HISTORY
            services.ApplyHistorysDI();

            // SEARCH
            services.ApplySearchDI();

            //Files
            services.ApplyStorageDI();

            // Permissions
            services.ApplyPermissionsDI();

            // Personalizations
            services.ApplyPersonalizationDI();

            // Billing
            services.ApplyBillingDI();

            // Notifications 
            services.ApplyNotificationsModule();
        }

        public static void SetupLogger(this IServiceCollection services, IConfiguration configuration, string environment)
        {
            var elasticConnString = configuration["ElasticConfiguration:Uri"];
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink(elasticConnString, environment))
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(string elasticConnString, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(elasticConnString))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"NOOTS-API-{environment?.ToUpper().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM-dd}"
            };
        }


        public static void JWT(this IServiceCollection services, JwtTokenConfig config)
        {
            services.AddScoped<IJwtAuthManager, JwtAuthManager>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", x =>
            {
                x.RequireHttpsMetadata = config.Https;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.Secret)),
                    ValidAudience = config.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                x.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(HubSettings.endPoint))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context => {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void SetupSignalR(this IServiceCollection services, RedisConfig config)
        {
            var signalR = services.AddSignalR();

            if (config.Active)
            {
                signalR.AddStackExchangeRedis(config.Connection, options =>
                    {
                        options.Configuration.ChannelPrefix = "Noots-";
                    });
            }
        }
    }
}
