using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Unity.RemoteConfig.Editor")]
namespace Unity.RemoteConfig.Editor.Core
{
    internal static class RemoteConfigEnvConf
    {
        internal const string pluginVersion = "4.1.1";

        //REST API Paths
        internal const string basePath = "https://remote-config-api.uca.cloud.unity3d.com/";
        internal const string queryParam = "?projectId={0}";
        internal const string environmentPath = basePath + "environments" + queryParam;
        internal const string getDefaultEnvironmentPath = basePath + "environments/default" + queryParam;
        internal const string getEnvironmentPath = basePath + "environments/{1}" + queryParam;
        internal const string getConfigPath = basePath + "environments/{1}/configs" + queryParam;
        internal const string postConfigPath = basePath + "configs" + queryParam;
        internal const string putConfigPath = basePath + "configs/{1}" + queryParam;
        internal const string getSchemasPath = basePath + "schemas?configId={1}" + "&projectId={0}&active=true";
        //Dashboard URLs
        internal const string dashboardBasePath = "https://dashboard.unity3d.com/";
        internal const string dashboardEnvironmentsPath = dashboardBasePath + "organizations/{2}/projects/{0}/remote-config/environments/";
        internal const string dashboardConfigsPath = dashboardBasePath + "organizations/{2}/projects/{0}/environments/{1}/remote-config/configs";
    }
}