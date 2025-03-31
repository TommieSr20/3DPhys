using System;
using System.Collections.Specialized;
using System.Linq;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.RemoteConfig.Authoring.Editor.Assets;
using Unity.Services.RemoteConfig.Authoring.Editor.Shared.Assets;
using Unity.Services.RemoteConfig.Authoring.Editor.Shared.Infrastructure.Collections;
using Unity.Services.RemoteConfig.Editor.Authoring.Core.Model;

namespace Unity.Services.RemoteConfig.Authoring.Editor.Deployment
{
	class RemoteConfigDeploymentProvider : DeploymentProvider, IDisposable
	{
		public override string Service => "Remote Config";
		public override Command DeployCommand { get; }
		public override Command ValidateCommand { get; }

		ObservableAssets<RemoteConfigAsset> m_Assets;

		public RemoteConfigDeploymentProvider(Command<RemoteConfigFile> deployCommand, ValidateCommand validateCommand)
		{
			DeployCommand = deployCommand;
			ValidateCommand = validateCommand;
			m_Assets = new ObservableAssets<RemoteConfigAsset>();
			m_Assets.CollectionChanged += OnCollectionChanged;
			m_Assets.ForEach(asset => DeploymentItems.Add(asset.Model));
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			var oldItems = args.OldItems?.Cast<RemoteConfigAsset>() ?? Array.Empty<RemoteConfigAsset>();
			var newItems = args.NewItems?.Cast<RemoteConfigAsset>() ?? Array.Empty<RemoteConfigAsset>();
			
			oldItems.ForEach(asset => DeploymentItems.Remove(asset.Model));
			newItems.ForEach(asset => DeploymentItems.Add(asset.Model));
		}

		public void Dispose()
		{
			m_Assets.CollectionChanged -= OnCollectionChanged;
			m_Assets.Dispose();
		}
	}
}