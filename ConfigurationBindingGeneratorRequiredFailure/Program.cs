using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace ConfigurationBindingGeneratorRequiredFailure;

internal class Program
{
    static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Configuration
            .AddInMemoryCollection(new Dictionary<string, string?>()
            {
                //["config:ChildOptionsWithRequiredField:RequiredField"] = "value"
            });

        builder.Services
            .AddSingleton<IValidateOptions<OptionsWithChildWithRequiredField>, ValidateOptionsWithRequiredField>();

        builder.Services
            .Configure<OptionsWithChildWithRequiredField>(builder.Configuration.GetSection(OptionsWithChildWithRequiredField.ConfigurationSectionName));

        builder.Services.AddHostedService<HostedService>();

        var host = builder.Build();

        host.Run();
    }
}

internal class HostedService(IOptions<OptionsWithChildWithRequiredField> options, ILogger<HostedService> logger) : IHostedService
{
    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Required Field value: {RequiredField}", options.Value.ChildOptionsWithRequiredField.RequiredField);
        return Task.CompletedTask;
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public sealed record OptionsWithChildWithRequiredField
{
    public const string ConfigurationSectionName = "config";

    [Required]
    [ValidateObjectMembers]
    public required ChildOptionsWithRequiredField ChildOptionsWithRequiredField { get; set; }
}

public sealed record ChildOptionsWithRequiredField
{
    [Required]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. - Workaround for bug in Configuration Binding Generator.
    public /*required*/ string RequiredField { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

[OptionsValidator]
public partial class ValidateOptionsWithRequiredField : IValidateOptions<OptionsWithChildWithRequiredField>
{
}
