using System;

/*
 * Copyright 2012 Mario Vernari (http://cetdevelop.codeplex.com/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Cet.Develop.Windows.IO.Protocols
{
    /// <summary>
    /// Modbus codec for commands: writing multiple register data
    /// </summary>
    public class ModbusCodecReadMultipleRegisters
        : ModbusCommandCodec
    {
        #region Client codec

        public override void ClientEncode(
            ModbusCommand command,
            ByteArrayWriter body)
        {
            ModbusTcpCodec.PushRequestHeader(
                command,
                body);
        }


        public override void ClientDecode(
            ModbusCommand command,
            ByteArrayReader body)
        {
            var count = body.ReadByte() / 2;
            command.Data = new ushort[count];
            for (int i = 0; i < count; i++)
                command.Data[i] = body.ReadUInt16BE();
        }

        #endregion
    }
}
