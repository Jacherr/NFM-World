using NFMWorld.Util;

namespace NFMWorld.Mad;

public class GameSparker
{
    private static ContO[] cars;

    private static readonly string[] CarRads = { "2000tornados" };

    private static long accumulator = 0;
    private static long lastFrameTime = 0;
    private static int physics_dt = 47;

    public static void Load()
    {
        new Medium();

        Medium.D();
        
        cars = new ContO[10];

        FileUtil.LoadFiles("../data/cars", CarRads, (ais, id) =>
        {
            cars[id] = new ContO(ais);
            if (!cars[id].Shadow)
            {
                throw new Exception("car does not have a shadow");
            }
        });
    }

    public static void GameTick()
    {
        if(lastFrameTime == 0) 
            lastFrameTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        accumulator += (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - lastFrameTime;


        while(accumulator >= physics_dt)
        {
            accumulator -= physics_dt;
            Medium.Around(cars[0], true);
        }

        Medium.D();
        cars[0].D();

        lastFrameTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
}