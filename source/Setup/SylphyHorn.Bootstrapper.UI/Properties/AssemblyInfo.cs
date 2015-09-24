using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using SylphyHorn.Bootstrapper;
using WixToolset.Bootstrapper;

[assembly: AssemblyTitle("SylphyHorn.Bootstrapper.UI")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("grabacr.net")]
[assembly: AssemblyProduct("SylphyHorn")]
[assembly: AssemblyCopyright("Copyright © 2016 Manato KAMEYA")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
[assembly: BootstrapperApplication(typeof(Installer.SylphyHornBootstrapperApplication))]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: ComVisible(false)]
