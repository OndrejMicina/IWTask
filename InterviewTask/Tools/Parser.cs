using InterviewTask.Models;

namespace InterviewTask.Tools
{

    public class Parser
    {
        public Packet ParsePacket(string packetString)
        {
            string formatCode = packetString.Substring(0, 4);

            switch (formatCode)
            {
                case "0x0A":
                    return ParseFormat0x0A(packetString);
                case "0x0E":
                    return ParseFormat0x0E(packetString);
                case "0x0F":
                    return ParseFormat0x0F(packetString);
                default:
                    throw new Exception("Invalid packet format code.");
            }

        }

        private Format0x0A ParseFormat0x0A(string packetString)
        {
            Format0x0A packet = new Format0x0A();

            packet.Type = "0x0A";
            packet.Id = Convert.ToInt32(packetString.Substring(4, 2), 16);
            packet.State = DecodeEncodedState(packetString.Substring(6, 2));
            packet.IpAddr = DecodeIpAddress(packetString.Substring(8));

            return packet;
        }

        private Format0x0E ParseFormat0x0E(string packetString)
        {
            Format0x0E packet = new Format0x0E();

            packet.Type = "0x0E";
            packet.Battery = Convert.ToInt32(packetString.Substring(4, 2), 16);
            packet.Temperature = ConvertLSBHexToDecimal(packetString.Substring(6, 4));
            packet.Chars = DecodeFourAsciiCharacters(packetString.Substring(10));

            return packet;
        }

        private Format0x0F ParseFormat0x0F(string packetString)
        {
            Format0x0F packet = new Format0x0F();
            packet.Type = "0x0F";
            packet.Battery = (float)Convert.ToInt32(packetString.Substring(4, 4), 16) / 100;
            packet.Flags = Convert.ToInt32(packetString.Substring(8, 2), 16);
            packet.FlagNames = DecodeFlagNames(packet.Flags);
            packet.Char = packetString[16];
            packet.Axis1 = ConvertLSBHexToDecimal(packetString.Substring(10, 4));
            packet.Axis2 = Convert.ToInt32(packetString.Substring(14, 4), 16);

            ApplyAsciiCharModifications(packet);

            return packet;
        }
        private string DecodeEncodedState(string encodedStateHex)
        {
            int encodedState = Convert.ToInt32(encodedStateHex, 16);

            switch (encodedState)
            {
                case 0:
                    return "REQ";
                case 1:
                    return "RESP";
                case 2:
                    return "ACK";
                case 3:
                    return "END";
                default:
                    throw new Exception("Invalid encoded state.");
            }
        }

        private string DecodeIpAddress(string ipAddressHex)
        {
            List<string> bytes = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                bytes.Add(Convert.ToInt32(ipAddressHex.Substring(i * 2, 2), 16).ToString());
            }
            var res = string.Join(".", bytes);
            return res;
        }



        private string DecodeFourAsciiCharacters(string asciiHex)
        {
            char[] chars = new char[4];
            for (int i = 0; i < 8; i += 2)
            {

                chars[i / 2] = (char)Convert.ToInt32(asciiHex.Substring(i, 2), 16);
            }
            return new string(chars);
        }


        //Byte 3, encoded bits/flags (bits from left to right > -, Active, Error, Connected, Multi, Single, Acknowledged, Repeat)
        private string[] DecodeFlagNames(int flags)
        {
            string binaryString = Convert.ToString(flags, 2).PadLeft(8, '0');
            int[] binary = binaryString.Select(c => int.Parse(c.ToString())).ToArray();

            List<string> flagNames = new List<string>();

            if (binary[0] == 1) { flagNames.Add("-"); }
            if (binary[1] == 1) { flagNames.Add("Active"); }
            if (binary[2] == 1) { flagNames.Add("Error"); }
            if (binary[3] == 1) { flagNames.Add("Connected"); }
            if (binary[4] == 1) { flagNames.Add("Multi"); }
            if (binary[5] == 1) { flagNames.Add("Single"); }
            if (binary[6] == 1) { flagNames.Add("Acknowledged"); }
            if (binary[7] == 1) { flagNames.Add("Repeat"); }

            return flagNames.ToArray();
        }

        /*private bool GetBitValue(int value, int bitIndex)
        {
            return (value & (1 << bitIndex)) != 0;
        }*/



        // - if A, then two previous values must be multiplied by 10
        // - if B, then two previous values must be subtracted by 100
        // - if C, then two previous values must be added by 25

        private void ApplyAsciiCharModifications(Format0x0F packet)
        {
            switch (packet.Char)
            {
                case 'A':
                    packet.Axis1 *= 10;
                    packet.Axis2 *= 10;
                    break;
                case 'B':
                    packet.Axis1 -= 100;
                    packet.Axis2 -= 100;
                    break;
                case 'C':
                    packet.Axis1 += 25;
                    packet.Axis2 += 25;
                    break;
                default:
                    throw new Exception("Invalid ASCII character.");
            }
        }


        private static int ConvertLSBHexToDecimal(string input)
        {
            string swappedInput = input.Substring(2, 2) + input.Substring(0, 2);

            return Convert.ToInt32(swappedInput, 16);
        }
    }
}
