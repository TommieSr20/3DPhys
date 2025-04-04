using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Authoring.Editor.Core.Logging;
using Unity.Services.CloudCode.Authoring.Editor.Projects.Exceptions;
using Unity.Services.CloudCode.Authoring.Editor.Projects.Settings;
using Unity.Services.CloudCode.Authoring.Editor.Shared.Infrastructure.SystemEnvironment;

namespace Unity.Services.CloudCode.Authoring.Editor.Projects
{
    class NodePackageManager : INodePackageManager, INodeJsRunner
    {
        const string k_Install = "install";
        const string k_Ci = "ci";
        const string k_Test = "test";
        const string k_LoggerTag = nameof(NodePackageManager);
        readonly IEnumerable<string> k_Init = new[] { "init", "-y" };
        readonly IEnumerable<string> k_Run = new[] { "run", "--silent" };

        readonly IProcessRunner m_ProcessRunner;
        readonly ILogger m_Logger;
        readonly ICloudCodeProjectSettings m_NodeSettings;

        public string WorkingDirectory { get; set; } = Directory.GetCurrentDirectory();

        public NodePackageManager(IProcessRunner processRunner, ICloudCodeProjectSettings settings, ILogger logger)
        {
            m_ProcessRunner = processRunner;
            m_NodeSettings = settings;
            m_Logger = logger;
        }

        public Task Init(CancellationToken cancellationToken = default)
        {
            return NpmRun(k_Init, default, cancellationToken);
        }

        public Task Install(CancellationToken cancellationToken = default)
        {
            return NpmRun(new[] {k_Install}, default, cancellationToken);
        }

        public Task Ci(CancellationToken cancellationToken = default)
        {
            return NpmRun(new[] {k_Ci}, default, cancellationToken);
        }

        public Task Test(CancellationToken cancellationToken = default)
        {
            return NpmRun(new[] {k_Test}, default, cancellationToken);
        }

        public bool CanRunScript(string script)
        {
            var projectFilePath = GetProjectFilePath();

            if (!string.IsNullOrEmpty(projectFilePath))
            {
                return new NodeProject(projectFilePath).HasScript(script);
            }

            return false;
        }

        public Task<string> RunScript(
            string script,
            IEnumerable<string> arguments = default,
            string stdIn = default,
            CancellationToken cancellationToken = default)
        {
            var npmArguments = new List<string>(k_Run) { script, "--" };
            if (arguments != null)
            {
                npmArguments.AddRange(arguments);
            }
            return NpmRun(npmArguments, stdIn, cancellationToken);
        }

        public async Task<bool> IsNodeJsAvailable()
        {
            try
            {
                await NpmRun(new List<string> { "--version" }, null, CancellationToken.None);
                return true;
            }
            catch (NpmCommandFailedException)
            {
                return false;
            }
        }

        public async Task<string> ExecNodeJs(
            IEnumerable<string> arguments = default,
            string stdIn = default,
            CancellationToken cancellationToken = default)
        {
            var nodeJsPath = m_NodeSettings.NodeJsPath;
            var npmPath = m_NodeSettings.NpmPath;
            var joinedArgs = ProcessArguments.Join(arguments);
            m_Logger.LogVerbose($"[{k_LoggerTag}] Running \"{nodeJsPath}\" {joinedArgs}");

            var startInfo = new ProcessStartInfo(nodeJsPath, joinedArgs)
            {
                WorkingDirectory = WorkingDirectory,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                EnsurePathContainsNodeAndNpm();

                var output = await m_ProcessRunner.RunAsync(startInfo, stdIn, cancellationToken);
                if (output.ExitCode != 0)
                {
                    throw new NpmCommandFailedException(startInfo, output);
                }

                return output.StdOut;
            }
            catch (Win32Exception e)
            {
                m_Logger.LogVerbose($"[{k_LoggerTag}] Error {e}");
                throw new NpmNotFoundException(nodeJsPath, npmPath);
            }
            catch (Exception e)
            {
                m_Logger.LogVerbose($"[{k_LoggerTag}] Error {e}");
                Console.Write(e.Message);
                throw;
            }
        }

        internal Task<string> NpmRun(IEnumerable<string> arguments, string stdIn, CancellationToken cancellationToken)
        {
            var nodeArguments = new List<string> { m_NodeSettings.NpmPath };
            nodeArguments.AddRange(arguments);

            return ExecNodeJs(nodeArguments, stdIn, cancellationToken);
        }

        void EnsurePathContainsNodeAndNpm()
        {
            var nodeJsPath = m_NodeSettings.NodeJsPath;
            var npmPath = m_NodeSettings.NpmPath;
            if (!SystemEnvironmentPathUtils.DoesEnvironmentPathContain(npmPath))
                SystemEnvironmentPathUtils.AddProcessToPath(npmPath);
            if (!SystemEnvironmentPathUtils.DoesEnvironmentPathContain(nodeJsPath))
                SystemEnvironmentPathUtils.AddProcessToPath(nodeJsPath);
        }

        string GetProjectFilePath()
        {
            var parentDir = WorkingDirectory;

            while (parentDir != null)
            {
                var path = Path.Join(parentDir, NodeProject.ProjectFile);

                if (File.Exists(path))
                {
                    return path;
                }

                parentDir = Directory.GetParent(parentDir)?.FullName;
            }

            return null;
        }
    }
}
