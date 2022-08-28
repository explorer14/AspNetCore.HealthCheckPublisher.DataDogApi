#addin nuget:?package=Cake.SemVer&version=4.0.0
#addin nuget:?package=semver&version=2.0.4
#addin nuget:?package=Cake.FileHelpers&version=3.3.0
#addin nuget:?package=Cake.Json&version=5.2.0

using System;
using System.Net.Http;
using System.IO;

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var doNotBuild = Argument("doNotBuild", false);
var artifactDirPath = "./artifacts/";
var packagePublishDirPath = "./publish/";
var semVer = CreateSemVer(1,0,0);
var solutionFilePath = "./DatadogApiClient.sln";

Setup(ctx=>
{
    var buildNumber = EnvironmentVariable("BUILD_BUILDNUMBER");

    if(!string.IsNullOrWhiteSpace(buildNumber))
    {
        Information($"The build number was {buildNumber}");
        semVer = buildNumber;
    }
    else
    {
        Information($"The build number was empty, using the default semantic version of {semVer.ToString()}");
    }

	SetUpNuget();
});

Teardown(ctx=>
{

});

Task("Restore")
    .Does(() => {		
		DotNetCoreRestore(solutionFilePath);	
});

Task("Build")
    .WithCriteria(doNotBuild == false)
	.IsDependentOn("Restore")
    .Does(()=>{        
		var config = new DotNetCoreBuildSettings
		{
			Configuration = configuration,
			NoRestore = true
		};
        DotNetCoreBuild(solutionFilePath, config);        
});

Task("Test")
	 .IsDependentOn("Build")
     .Does(() =>
 {
     var settings = new DotNetCoreTestSettings
     {
         Configuration = configuration,
		 NoBuild = true,
         ArgumentCustomization = args => 
            args.Append("--collect:\"XPlat Code Coverage\"")
     };

     var projectFiles = GetFiles("./tests/**/*.Tests.csproj");

     foreach(var file in projectFiles)
     {
         Information($"Running test suite on: {file.FullPath}");
         DotNetCoreTest(file.FullPath, settings);
     }     
 });

Task("Verify-PR")
    .IsDependentOn("Test")
    .Does(()=>
{
    Information("Finished verifying PR!");
});

Task("PushToNuget")
	.IsDependentOn("Pack")
	.Does(()=>
{
    var files = GetFiles("./artifacts/AspNetCore.HealthCheckPublisher.DataDogApi.*.nupkg");

    foreach(var file in files)
    {
        Information(file.FullPath);
        var settings = new DotNetCoreNuGetPushSettings
        {
            Source = "https://skynetcode.pkgs.visualstudio.com/_packaging/skynetpackagefeed/nuget/v3/index.json",
            ApiKey = "gibberish",
            SkipDuplicate = true
        };

        DotNetCoreNuGetPush(file.FullPath, settings);
    }
});

Task("Pack")
	.IsDependentOn("Test")    
	.Does(()=>{
		var settings = new DotNetCorePackSettings
		{
		    Configuration = configuration,
		    OutputDirectory = artifactDirPath,
			NoBuild = true,
			NoRestore = true
		};

		DotNetCorePack("./src/AspNetCore.HealthCheckPublisher.DataDogApi/AspNetCore.HealthCheckPublisher.DataDogApi.csproj", 
            settings);
});

void SetUpNuget()
{
	var feed = new
	{
		Name = "SkynetNuget",
	    Source = "https://skynetcode.pkgs.visualstudio.com/_packaging/skynetpackagefeed/nuget/v3/index.json"
	};

	if (!DotNetCoreNuGetHasSource(name:feed.Name))
	{
	    var nugetSourceSettings = new DotNetCoreNuGetSourceSettings
                             {
                                Source = feed.Source,
                                UserName = "skynetcode",
                                Password = EnvironmentVariable("SYSTEM_ACCESSTOKEN"),
				                StorePasswordInClearText = true
                             };		
        try
        {
            DotNetCoreNuGetAddSource(
                name:feed.Name,
                settings:nugetSourceSettings);
        }
        catch(Exception ex)
        {
            Warning(ex.Message);
        }
	}	
}

RunTarget(target);