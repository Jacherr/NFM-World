namespace NFMWorld.DriverInterface;

public interface IFontMetrics
{
    public int StringWidth(string astring);
    public int Height(string astring);
}