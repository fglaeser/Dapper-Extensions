///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var sln = "DapperExtensions.sln";
var folder = ".\\DapperExtensions.Core";
var artifacts = ".\\artifacts";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(ctx =>
{
   // Executed BEFORE the first task.
    Information($"Running target [{target}] in configuration [{configuration}]");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
    .Does((context) =>
    {
        DotNetCoreRestore(sln);
    });

// Build using the build configuration specified as an argument.
 Task("Build").IsDependentOn("Restore")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings()
            {
                Configuration = configuration,
                ArgumentCustomization = args => args.Append("--no-restore"),
            };
        DotNetCoreBuild(sln, settings);

    });

Task("Pack").IsDependentOn("Build")
    .Does(()=> 
    {
        var settings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = artifacts,
            NoBuild = true,
            NoRestore = true
        };
        DotNetCorePack($"{folder}", settings);
    });

Task("Push").IsDependentOn("Pack")
    .Does(() => 
    {
     var settings = new DotNetCoreNuGetPushSettings
     {
         Source = "http://nuget.test.andreani.com.ar/nuget/andreani",
         ApiKey = "04071421"
     };
     DotNetCoreNuGetPush($".\\artifacts\\*.nupkg", settings);
    });
	
Task("default").IsDependentOn("Push");

RunTarget(target);