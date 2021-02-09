using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: ComVisible(false)]
[assembly: Guid("00000000-0000-0000-0000-000000000000")] // ToDo: assembly: Guid
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

namespace ResinTimer.Properties
{
    internal static class AssemblyInfo
    {
        static AssemblyInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();

            Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()!.Title;
            Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()!.Description;
            Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()!.Company;
            Product = assembly.GetCustomAttribute<AssemblyProductAttribute>()!.Product;
            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()!.Copyright;
            Version = assembly.GetName().Version ?? new Version(0, 0, 0);
            VersionString = $"{Version.ToString(3)}{(Version.Revision <= 0 ? "" : $" rev.{Version.Revision}")}";
            Guid = Guid.Parse(assembly.GetCustomAttribute<GuidAttribute>()!.Value);
        }

        public static string Title { get; }

        public static string Description { get; }

        public static string Company { get; }

        public static string Product { get; }

        public static string Copyright { get; }

        public static Version Version { get; }

        public static string VersionString { get; }

        public static Guid Guid { get; }
    }
}
