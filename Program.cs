using TelemetryStreamParser;

Console.WriteLine(
    "======================================"
);

Console.WriteLine(
    "     UAV TELEMETRY STREAM PARSER"
);

Console.WriteLine(
    "======================================"
);

TelemetryParser parser = new TelemetryParser();

Console.WriteLine();
Console.WriteLine("PRUEBA 1: Paquete normal");
Console.WriteLine();

// Prueba 1: paquete normal.

byte[] stream1 =
{
    0xAA,
    0x55,

    0x03,

    0x04,

    0x10,
    0x20,
    0x30,
    0x40,

    0x12,
    0x13
};

ProcessStream(parser, stream1);

Console.WriteLine();
Console.WriteLine("PRUEBA 2: Basura antes del paquete");
Console.WriteLine();

// Prueba 2: datos basura antes del paquete.

byte[] stream2 =
{
    
    0x19,
    0xFF,
    0x81,
    0x42,

    
    0xAA,
    0x55,

    0x05,       

    0x03,       

    0x11,
    0x22,
    0x33,

    0xAB,
    0xCD
};

ProcessStream(parser, stream2);

Console.WriteLine();
Console.WriteLine("PRUEBA 3: Payload vacío");
Console.WriteLine();

// Prueba 3: payload vacío.

byte[] stream3 =
{
    0xAA,
    0x55,

    0x08,

    0x00,       // Payload Length = 0

    0x11,
    0x22        // CRC
};

ProcessStream(parser, stream3);

// Prueba 4: length inválido.

Console.WriteLine();
Console.WriteLine("PRUEBA 4: Length inválido");
Console.WriteLine();

byte[] stream4 =
{
    0xAA,
    0x55,

    0x09,

    0x65        
};

ProcessStream(parser, stream4);



// Prueba 5: fragmentación de paquete.


Console.WriteLine();
Console.WriteLine("PRUEBA 5: Packet Fragmentation");
Console.WriteLine();

byte[] fragment1 =
{
    0xAA,
    0x55,
    0x0A
};

byte[] fragment2 =
{
    0x04,
    0x50
};

byte[] fragment3 =
{
    0x60,
    0x70
};

byte[] fragment4 =
{
    0x80,
    0x12,
    0x34
};

Console.WriteLine("Fragmento 1:");
ProcessStream(parser, fragment1);

Console.WriteLine("Fragmento 2:");
ProcessStream(parser, fragment2);

Console.WriteLine("Fragmento 3:");
ProcessStream(parser, fragment3);

Console.WriteLine("Fragmento 4:");
ProcessStream(parser, fragment4);

// Mostrar estadísticas

parser.PrintStatistics();


static void ProcessStream(
    TelemetryParser parser,
    byte[] data
)
{
    foreach (byte b in data)
    {
        parser.ProcessByte(b);
    }
}