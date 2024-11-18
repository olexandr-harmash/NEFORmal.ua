using Microsoft.Extensions.Configuration;
using NEFORmal.ua.Dating.ApplicationCore.Interfaces;
using NEFORmal.ua.Dating.ApplicationCore.Services;

namespace NEFORmal.ua.Dating.Presentation.Configuration;

public static class ApplicationConfiguration
{
    public static WebApplicationBuilder ConfigureApplication(this WebApplicationBuilder  builder)
    {
        builder.Services.Configure<FileServiceOptions>(builder.Configuration.GetSection(FileServiceOptions.OptionString));

        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();

        return builder;
    }
}