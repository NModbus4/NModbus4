NModbus4
=======

|           |Build Status|Code Coverage|
|-----------|:----------:|:-----------:|
|**Mono**|[![Build Status](https://travis-ci.org/NModbus4/NModbus4.svg?branch=portable-3.0)](https://travis-ci.org/NModbus4/NModbus4)||
|**MS .NET**|[![Build status](https://ci.appveyor.com/api/projects/status/9irkluk69cr0f5ed?svg=true)](https://ci.appveyor.com/project/Maxwe11/nmodbus4-ss8e4)|[![Coverage Status](https://coveralls.io/repos/NModbus4/NModbus4/badge.svg?branch=portable-3.0&service=github)](https://coveralls.io/github/NModbus4/NModbus4?branch=portable-3.0)|

[![Join the chat at https://gitter.im/NModbus4/NModbus4](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/NModbus4/NModbus4?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

NModbus is a C# implementation of the Modbus protocol.
Provides connectivity to Modbus slave compatible devices and applications.
Supports serial ASCII, serial RTU, TCP, and UDP protocols.
NModbus4 it's a fork of NModbus(https://code.google.com/p/nmodbus).
NModbus4 differs from original NModbus in following:

1. removed USB support(FtdAdapter.dll)
2. removed log4net dependency
3. removed Unme.Common.dll dependency
4. assembly renamed to NModbus4.dll
5. target framework changed to .NET 4

Install
=======

To install NModbus4, run the following command in the Package Manager Console

    PM> Install-Package NModbus4

Documentation
=======
Documentation is available in chm format (NModbus.chm)

License
=======
NModbus4 is licensed under the [MIT license](LICENSE.txt).
