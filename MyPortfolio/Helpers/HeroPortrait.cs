namespace MyPortfolio.Helpers
{
    /// <summary>
    /// The hero portrait is a file on disk (wwwroot/Files/Images/Profile.*) instead of a
    /// table row, so swapping it from the admin area needs no migration.
    /// </summary>
    public static class HeroPortrait
    {
        public const string RelativeFolder = "Files/Images";
        public const string BaseName = "Profile";

        const long MaxBytes = 2_097_152;
        static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        public static string FolderPath(IWebHostEnvironment env) =>
            Path.Combine(env.WebRootPath, RelativeFolder);

        public static FileInfo? Current(IWebHostEnvironment env)
        {
            var folder = FolderPath(env);
            if (!Directory.Exists(folder)) return null;

            return new DirectoryInfo(folder)
                .EnumerateFiles(BaseName + ".*")
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .FirstOrDefault();
        }

        /// <summary>
        /// Static files are cached by name, so the write stamp rides along as a version
        /// query — otherwise a replaced portrait stays invisible until a hard refresh.
        /// </summary>
        public static string? PublicUrl(IWebHostEnvironment env)
        {
            var file = Current(env);
            if (file is null) return null;

            return $"/{RelativeFolder}/{Uri.EscapeDataString(file.Name)}?v={file.LastWriteTimeUtc.Ticks}";
        }

        public static string? Store(IWebHostEnvironment env, IFormFile file, out string? error)
        {
            error = null;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                error = "Use a .jpg, .jpeg, .png or .webp file.";
                return null;
            }

            if (file.Length == 0 || file.Length > MaxBytes)
            {
                error = "The image has to be smaller than 2 MB.";
                return null;
            }

            var folder = FolderPath(env);
            Directory.CreateDirectory(folder);

            // Write first, then retire the previous copy: a failed write leaves the live portrait intact.
            var staging = Path.Combine(folder, $"{BaseName}-{Guid.NewGuid()}{extension}");
            var target = Path.Combine(folder, BaseName + extension);

            using (var stream = new FileStream(staging, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            foreach (var stale in new DirectoryInfo(folder).EnumerateFiles(BaseName + ".*"))
            {
                stale.Delete();
            }

            File.Move(staging, target, true);
            return target;
        }
    }
}
