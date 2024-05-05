using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    AbsolutePath PackagePublishPath => AbsolutePath.Create("publish");

    string ProjectName => "AspNetCore.HealthCheckPublisher.DataDogApi";
    AbsolutePath ProjectToPack => AbsolutePath.Create($"./src/{ProjectName}/{ProjectName}.csproj");

    Target Compile => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(b => 
                b.SetConfiguration(Configuration));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var projects = Solution.GetAllProjects("*.Tests");

            foreach (var testProject in projects)
            {
                Console.WriteLine($"Running {testProject.Name}");
                DotNetTasks.DotNetTest(t=>
                    t.SetProjectFile(testProject)
                        .SetProcessArgumentConfigurator(a=>
                            a.Add("--collect:\"XPlat Code Coverage\"")));    
            }
        });

    Target VerifyPR => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            Console.WriteLine("Finished verifying PR");
        });

    Target Pack => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(p =>
                p.SetConfiguration(Configuration)
                    .SetProject(ProjectToPack)
                    .SetOutputDirectory(PackagePublishPath));
        });

    Target PushToNuget => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {
            DotNetTasks.DotNetNuGetPush(n => n
                .SetApiKey(Environment.GetEnvironmentVariable("NUGET_API_KEY"))
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetSkipDuplicate(true)
                .SetTargetPath(PackagePublishPath / ProjectName + ".*.nupkg"));
        });
}
