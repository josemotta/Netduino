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
    /// Modbus codec for commands: reading multiple discrete data
    /// </summary>
    public class ModbusCodecForceMultipleCoils
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

            ModbusTcpCodec.PushDiscretes(
                command,
                body);
        }


        public override void ClientDecode(
            ModbusCommand command,
            ByteArrayReader body)
        {
            ModbusTcpCodec.PopRequestHeader(
                command,
                body);
        }

        #endregion
    }
}
