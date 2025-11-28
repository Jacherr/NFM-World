namespace NFMWorld.Util;

public readonly record struct Font(string FontName, int Flags, int Size)
{
    public const int PLAIN = 0;
    public const int BOLD = 1;
    public const int ITALIC = 2;
}