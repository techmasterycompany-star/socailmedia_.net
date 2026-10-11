using DotNetEnv;

namespace Social_Media_Platform.API;

internal static class DotEnvLoader
{
    public static void Load()
    {
        foreach (var startPath in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            for (var directory = new DirectoryInfo(startPath);
                 directory is not null;
                 directory = directory.Parent)
            {
                var envFile = Path.Combine(directory.FullName, ".env");
                if (File.Exists(envFile))
                {
                    Env.Load(envFile);
                    return;
                }
            }
        }
    }
}
