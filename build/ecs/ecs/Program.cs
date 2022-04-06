using Amazon.XRay.Recorder.Handlers.AwsSdk;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//AWS:Register service for AWS Systems Manager Parameter store
builder.Configuration.AddSystemsManager(configureSource => {

    // Parameter Store prefix to pull configuration data from.
    //AWS: Loads all SSM parameters starting with /eventbroker at startup to .NET configuration
    configureSource.Path = "/ecs/app/";

    // Reload configuration data every 5 minutes.
    configureSource.ReloadAfter = TimeSpan.FromMinutes(5);
});

//AWS:Register service for AWS Xray
AWSSDKHandler.RegisterXRayForAllServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var appName = builder.Configuration.GetValue<string>("appname");

//AWS: Enable X ray tracing middleware
app.UseXRay(appName);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
