using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                ["config:Something"] = "value"
            });

        builder.Services
            .AddSingleton<IValidateOptions<OptionsWithChildWithRequiredField>, ValidateOptionsWithRequiredField>();

        builder.Services
            .Configure<OptionsWithChildWithRequiredField>(builder.Configuration.GetRequiredSection(OptionsWithChildWithRequiredField.ConfigurationSectionName));

        var host = builder.Build();

        host.Run();
    }
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
    public /*required*/ string RequiredField { get; set; }
}

[OptionsValidator]
public partial class ValidateOptionsWithRequiredField : IValidateOptions<OptionsWithChildWithRequiredField>
{
}
