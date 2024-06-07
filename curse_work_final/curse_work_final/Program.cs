using System.Threading.Tasks;
using curse_work_final;


public class Program
{
    public static async Task Main(string[] args)
    {
        SpaceWeatherBot spaceWeatherBot = new SpaceWeatherBot();
        await spaceWeatherBot.Start();
    }
}