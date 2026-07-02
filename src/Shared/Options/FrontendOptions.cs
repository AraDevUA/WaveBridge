using System.ComponentModel.DataAnnotations;

namespace Shared.Options;

public class FrontendOptions
{
    public const string SectionName = "Frontend";

    [Required]
    [Url]
    public string BaseUri { get; set; } = default!;
}
