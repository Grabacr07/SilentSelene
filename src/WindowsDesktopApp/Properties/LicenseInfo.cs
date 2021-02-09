using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WindowsDesktopApp.Properties
{
    public class LicenseInfo
    {
        public static LicenseInfo[] All { get; }

        static LicenseInfo()
        {
            var regex = new Regex("WindowsDesktopApp\\.Assets\\.Licenses\\.(?<name>.*)\\.txt"); // ToDo: Licenses path
            var assembly = Assembly.GetExecutingAssembly();

            All = assembly
                .GetManifestResourceNames()
                .Select(x => regex.Match(x))
                .Where(x => x.Success)
                .Select(x =>
                {
                    using var stream = assembly.GetManifestResourceStream(x.Groups[0].Value);
                    using var reader = new StreamReader(stream!);
                    return new LicenseInfo(x.Groups["name"].Value, reader.ReadToEnd());
                })
                .OrderBy(x => x.ProductName)
                .ToArray();
        }

        public string ProductName { get; }

        public string LicenseBody { get; }

        private LicenseInfo(string productName, string licenseBody)
        {
            this.ProductName = productName;
            this.LicenseBody = licenseBody;
        }
    }
}
