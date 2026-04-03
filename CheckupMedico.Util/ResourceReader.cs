namespace CheckupMedico.Transversal.Util
{
    using CheckupMedico.Domain.Enum;

    public static class ResourceReader
    {
        private const string ImagesFolder = "Images";

        private static string GetResourcePath(string folderName, string resourceName)
        {
            var assemblyName = typeof(ResourceReader).Assembly.GetName().Name;
            return $"{assemblyName}.{folderName}.{resourceName}";
        }

        private static Stream GetResourceStream(string resourcePath)
        {
            var assembly = typeof(ResourceReader).Assembly;

            var stream = assembly.GetManifestResourceStream(resourcePath);

            if (stream == null)
                throw new FileNotFoundException($"Embedded resource not found: {resourcePath}");

            return stream;
        }

        public static byte[] GetImageAsByteArray(ImageTypes imageType)
        {
            var resourcePath = GetResourcePath(ImagesFolder, imageType.GetDescription());

            using var imageStream = GetResourceStream(resourcePath);
            using var memoryStream = new MemoryStream();

            imageStream.CopyTo(memoryStream);

            var bytes = memoryStream.ToArray();

            if (bytes.Length == 0)
                throw new Exception($"Image resource is empty: {resourcePath}");

            return bytes;
        }
    }
}
