// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Linq;
using System.Threading;
using Nuke.Common;
using Nuke.Common.Execution;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Logger;
using static Nuke.Common.Tools.DotNet.DotNetTasks;


[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    // ReSharper disable once NotAccessedField.Local
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });

    // ReSharper disable once InconsistentNaming
    public const string ALL_IN_ONE_SLN = "Xoom.Platform";
        public virtual Target CreateAllInOneSolution => _ => _
            .Executes(() =>
            {
                var allInOneSln = RootDirectory / (ALL_IN_ONE_SLN + ".sln");
                if (FileExists(allInOneSln))
                    DeleteFile(allInOneSln);
                DotNet($"new sln --name {ALL_IN_ONE_SLN}");
                var projects = GlobFiles(RootDirectory, $"**/*.csproj").NotEmpty().OrderBy(p => p);
                var count = 1;
                foreach (var proj in projects)
                {
                    Info($"Adding project #{count++} : {proj}");
                    DotNet($"sln {allInOneSln} add \"{proj}\"");
                    Thread.Sleep(200);
                }
            });



}
