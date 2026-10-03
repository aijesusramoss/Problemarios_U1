using System;
using System.Collections.Generic;

namespace TelemetryStreamParser
{
    public class TelemetryParser
    {
        // Bytes especiales definidos por el protocolo.
        private const byte SyncByte1 = 0xAA;
        private const byte SyncByte2 = 0x55;
        private const int MaxPayloadLength = 64;

        private ParserState state = ParserState.WaitSync1;

        // Datos temporales del paquete actual.
        private byte frameId;
        private byte payloadLength;
        private readonly List<byte> payload = new List<byte>();
        private byte crcHigh;
        private byte crcLow;

        private int validStructureCount;
        private int invalidLengthCount;

        // La persona encargada del CRC puede suscribirse a este evento.
        public event Action<TelemetryFrame>? FrameReceived;

        public void ProcessByte(byte incomingByte)
        {
            Console.WriteLine(
                $"Byte recibido: 0x{incomingByte:X2} | Estado: {state}"
            );

            switch (state)
            {
                case ParserState.WaitSync1:
                    ProcessWaitSync1(incomingByte);
                    break;
                case ParserState.WaitSync2:
                    ProcessWaitSync2(incomingByte);
                    break;
                case ParserState.ReadFrameId:
                    ProcessFrameId(incomingByte);
                    break;
                case ParserState.ReadLength:
                    ProcessLength(incomingByte);
                    break;
                case ParserState.ReadPayload:
                    ProcessPayload(incomingByte);
                    break;
                case ParserState.ReadCrcHigh:
                    ProcessCrcHigh(incomingByte);
                    break;
                case ParserState.ReadCrcLow:
                    ProcessCrcLow(incomingByte);
                    break;
                default:
                    Reset();
                    break;
            }
        }

        private void ProcessWaitSync1(byte incomingByte)
        {
            if (incomingByte == SyncByte1)
            {
                Console.WriteLine("Sync Byte 1 encontrado.");
                state = ParserState.WaitSync2;
            }
        }

        private void ProcessWaitSync2(byte incomingByte)
        {
            if (incomingByte == SyncByte2)
            {
                Console.WriteLine("Sync Byte 2 encontrado.");
                Console.WriteLine("Inicio de paquete detectado.");
                state = ParserState.ReadFrameId;
            }
            else if (incomingByte == SyncByte1)
            {
                // En AA AA 55, el segundo AA se toma como un nuevo inicio.
                Console.WriteLine(
                    "Se recibió otro 0xAA. Se mantiene esperando 0x55."
                );
            }
            else
            {
                Console.WriteLine(
                    "Sync inválido. Regresando a WaitSync1."
                );
                state = ParserState.WaitSync1;
            }
        }

        private void ProcessFrameId(byte incomingByte)
        {
            frameId = incomingByte;
            Console.WriteLine($"Frame ID recibido: 0x{frameId:X2}");
            state = ParserState.ReadLength;
        }

        private void ProcessLength(byte incomingByte)
        {
            payloadLength = incomingByte;
            Console.WriteLine($"Payload Length recibido: {payloadLength}");

            if (payloadLength > MaxPayloadLength)
            {
                Console.WriteLine("ERROR: Payload Length inválido.");
                invalidLengthCount++;
                Reset();
                return;
            }

            payload.Clear();

            if (payloadLength == 0)
            {
                Console.WriteLine(
                    "Payload vacío. Pasando directamente al CRC."
                );
                state = ParserState.ReadCrcHigh;
            }
            else
            {
                state = ParserState.ReadPayload;
            }
        }

        private void ProcessPayload(byte incomingByte)
        {
            payload.Add(incomingByte);
            Console.WriteLine(
                $"Payload byte {payload.Count}/{payloadLength}: " +
                $"0x{incomingByte:X2}"
            );

            if (payload.Count == payloadLength)
            {
                Console.WriteLine("Payload completo recibido.");
                state = ParserState.ReadCrcHigh;
            }
        }

        private void ProcessCrcHigh(byte incomingByte)
        {
            crcHigh = incomingByte;
            Console.WriteLine($"CRC High recibido: 0x{crcHigh:X2}");
            state = ParserState.ReadCrcLow;
        }

        private void ProcessCrcLow(byte incomingByte)
        {
            crcLow = incomingByte;
            Console.WriteLine($"CRC Low recibido: 0x{crcLow:X2}");
            CompleteFrame();
        }

        private void CompleteFrame()
        {
            TelemetryFrame frame = new TelemetryFrame
            {
                FrameId = frameId,
                PayloadLength = payloadLength,
                Payload = new List<byte>(payload),
                CrcHigh = crcHigh,
                CrcLow = crcLow
            };

            validStructureCount++;

            Console.WriteLine();
            Console.WriteLine("Estructura completa del paquete recibida.");
            frame.Print();

            // Aquí se puede conectar la validación CRC-16 de la persona 2.
            FrameReceived?.Invoke(frame);

            Reset();
        }

        private void Reset()
        {
            state = ParserState.WaitSync1;
            frameId = 0;
            payloadLength = 0;
            payload.Clear();
            crcHigh = 0;
            crcLow = 0;

            Console.WriteLine("Parser reiniciado.");
            Console.WriteLine();
        }

        public void PrintStatistics()
        {
            Console.WriteLine();
            Console.WriteLine("========== ESTADÍSTICAS ==========");
            Console.WriteLine(
                $"Paquetes con estructura completa: {validStructureCount}"
            );
            Console.WriteLine(
                $"Longitudes inválidas: {invalidLengthCount}"
            );
            Console.WriteLine("==================================");
        }
    }
}