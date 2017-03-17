namespace FCSPlayout.Domain
{
    public interface IMediaFilePathResolver
    {
        MediaFileStorage CurrentStorage { get; set; }
        //string Resolve(string filename);
        string GetDirectory(MediaFileStorage fileStorage);
    }

    public static class MediaFilePathResolverExtensions
    {
        public static string Resolve(this IMediaFilePathResolver resolver, string filename, MediaFileStorage fileStorage)
        {
            return System.IO.Path.Combine(resolver.GetDirectory(fileStorage), filename);
        }

        public static string Resolve(this IMediaFilePathResolver resolver, string filename)
        {
            return System.IO.Path.Combine(resolver.GetDirectory(resolver.CurrentStorage), filename);
        }
    }
}
