NModbus4
=======

[![Join the chat at https://gitter.im/Maxwe11/NModbus4](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Maxwe11/NModbus4?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://ci.appveyor.com/api/projects/status/xh645h4skw32pu6j?svg=true)](https://ci.appveyor.com/project/Maxwe11/nmodbus4)

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

Change log
=======

Version **2.0** of **NModbus4** introduces some breaking changes.
## NModbus4 2.0.5516.31020 ##
In case of slave receives request with invalid function exception response would be returned to master instead of throwing ArgumentException (issue [#2](/../../issues/2)). 

Also introduced new exception type `InvalidModbusRequestException`. You can subscribe on slave's event `ModbusSlaveRequestReceived` and throw this exception. Thus you can filter incoming requests ([#6](/../../issues/6)).

Another breaking change related with ReadWriteMultiple function (0x17). Now it works in accordance with the Modbus spec -- performs write operation before read(fixes [#6](/../../issues/6)).

**API addition**

1. `IModbusMaster` and its implementations now have Async versions of its methods which return `Task`:

        Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints);
        Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value);
        Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value);
        Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data);
        Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data);
        Task<ushort[]> ReadWriteMultipleRegistersAsync(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData);

2. Opposite to event `ModbusSlaveRequestReceived` added event `WriteComplete` which fired by slave after write operation completed.

## NModbus4 2.0.5522.34406 ##
 - fixes issue [#7](/../../issues/7)
 - fixes issue [#8](/../../issues/8)

Documentation
=======
Documentation is available in chm format (NModbus.chm)

The MIT License (MIT)
=======
Copyright (c) 2006 Scott Alexander, 2014 Maxwe11

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
