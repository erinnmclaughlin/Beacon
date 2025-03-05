using Beacon.WebHost;

WebApplication
    .CreateBuilder(args)
    .BuildBeaconApplication()
    .ConfigurePipeline()
    .Run();

public partial class Program;
