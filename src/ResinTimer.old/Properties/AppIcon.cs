using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ResinTimer.Properties
{
    public class AppIcon
    {
        public static AppIcon[] All { get; }

        static AppIcon()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var iconRegex = new Regex("ResinTimer\\.Assets\\.Icons\\.(?<name>.*)\\.ico");

            All = assembly
                .GetManifestResourceNames()
                .Select(x => iconRegex.Match(x))
                .Where(x => x.Success)
                .Select(x =>
                {
                    using var stream = assembly.GetManifestResourceStream(x.Groups[0].Value);
                    return new AppIcon(x.Groups["name"].Value, new Icon(stream!));
                })
                .OrderBy(x => x.Name)
                .ToArray();
        }

        public string Name { get; }

        public Icon Icon { get; }

        public AppIcon(string name, Icon icon)
        {
            this.Name = name;
            this.Icon = icon;
        }
    }
}
