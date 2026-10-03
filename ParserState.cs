namespace TelemetryStreamParser
{
    public enum ParserState
    {
        WaitSync1,
        WaitSync2,
        ReadFrameId,
        ReadLength,
        ReadPayload,
        ReadCrcHigh,
        ReadCrcLow
    }
}
