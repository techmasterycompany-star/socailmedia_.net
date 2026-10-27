using Microsoft.Extensions.Options;
using Social_Media_Platform.Application.Options;

namespace Social_Media_Platform.Infrastructure.Options
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ApiSecret { get; set; } = null!;
        public string Folder {  get; set; } = null!;
    }
}
