using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NModbus4")]
[assembly: AssemblyProduct("NModbus4")]
[assembly: AssemblyCompany("Maxwe11")]
[assembly: AssemblyCopyright("Licensed under MIT License.")]
[assembly: AssemblyDescription("NModbus is a C# implementation of the Modbus protocol. " +
           "Provides connectivity to Modbus slave compatible devices and applications. " +
           "Supports serial ASCII, serial RTU, TCP, and UDP protocols. "+
           "NModbus4 it's a fork of NModbus(https://code.google.com/p/nmodbus)")]

[assembly: CLSCompliant(false)]
[assembly: Guid("95B2AE1E-E0DC-4306-8431-D81ED10A2D5D")]
[assembly: AssemblyVersion("2.1.0.0")]
[assembly: AssemblyInformationalVersion("2.1.0")]

#if !SIGNED
[assembly: InternalsVisibleTo(@"Modbus.UnitTests")]
[assembly: InternalsVisibleTo(@"DynamicProxyGenAssembly2")]
#endif