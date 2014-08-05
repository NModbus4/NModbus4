using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NModbus4")]
[assembly: AssemblyProduct("NModbus4")]
[assembly: AssemblyCompany("Maxwe11")]
[assembly: AssemblyCopyright("Licensed under MIT License.")]
[assembly: AssemblyDescription("Differs from this one (https://code.google.com/p/nmodbus) in following: " +
                               "removed FtdAdapter.dll, log4net, Unme.Common.dll dependencies, assembly renamed to NModbus.dll, " +
                               "target framework changed to .NET 4")]

// required for VBA applications

[assembly: ComVisible(true)]
[assembly: CLSCompliant(false)]
[assembly: Guid("95B2AE1E-E0DC-4306-8431-D81ED10A2D5D")]
[assembly: AssemblyVersion("1.12.0.0")]
#if !WindowsCE

[assembly: AssemblyFileVersion("1.12.0.0")]
#endif

#if !SIGNED

[assembly: InternalsVisibleTo(@"Modbus.UnitTests")]
[assembly: InternalsVisibleTo(@"DynamicProxyGenAssembly2")]
#endif