using System;
using System.Collections.Generic;

namespace TelemetryStreamParser
{
    public class TelemetryFrame
    {
        public byte FrameId { get; set; }

        public byte PayloadLength { get; set; }

        public List<byte> Payload { get; set; } = new List<byte>();

        public byte CrcHigh { get; set; }

        public byte CrcLow { get; set; }

        // El CRC llega en formato Big Endian: primero el byte alto y luego el bajo.
        public ushort ReceivedCrc
        {
            get
            {
                return (ushort)((CrcHigh << 8) | CrcLow);
            }
        }

        public void Print()
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("       PAQUETE RECIBIDO");
            Console.WriteLine("=================================");
            Console.WriteLine($"Frame ID: 0x{FrameId:X2}");
            Console.WriteLine($"Payload Length: {PayloadLength}");

            if (Payload.Count > 0)
            {
                Console.WriteLine(
                    $"Payload: {BitConverter.ToString(Payload.ToArray())}"
                );
            }
            else
            {
                Console.WriteLine("Payload: vacío");
            }

            Console.WriteLine($"CRC High: 0x{CrcHigh:X2}");
            Console.WriteLine($"CRC Low:  0x{CrcLow:X2}");
            Console.WriteLine($"CRC recibido: 0x{ReceivedCrc:X4}");
            Console.WriteLine("=================================");
            Console.WriteLine();
        }
    }
}