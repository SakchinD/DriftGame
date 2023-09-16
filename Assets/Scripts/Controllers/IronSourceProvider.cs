using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Zenject;

namespace Monetization.Ads.Providers
{
	public interface IIronSourceProvider
	{
		UniTask<bool> ShowRewardedVideoAsync();
    }

    public class IronSourceProvider : IIronSourceProvider, IInitializable 
	{
        private static readonly string AppID = "1ba33f895";

		private static readonly float TimeOut = 10f;

		private bool isInit;

        public void Initialize()
        {
            InitAsync().Forget();
        }

		public async UniTask InitAsync()
		{
			Debug.Log("IRONSOURCE");
			IronSource.Agent.validateIntegration();

			IronSource.Agent.shouldTrackNetworkState(true);

			IronSource.Agent.init(AppID, IronSourceAdUnits.REWARDED_VIDEO);

			isInit = true;

			Debug.Log(isInit);

			var ctx = new CancellationTokenSource(TimeSpan.FromSeconds(TimeOut));
			await UniTask.WaitWhile(() => !isInit, cancellationToken: ctx.Token);
			LoadRewardedVideo();
		}

		public bool IsRewardedVideoReady()
		{
			if (!IsInit())
				return false;

			var isReady = IronSource.Agent.isRewardedVideoAvailable();
			
			return isReady;
		}

		public async UniTask<bool> ShowRewardedVideoAsync()
		{
			var isClosed = false;
			var success = false;
			IronSourceRewardedVideoEvents.onAdRewardedEvent += Success;
			IronSourceRewardedVideoEvents.onAdClosedEvent += VideoClosed;
			IronSource.Agent.showRewardedVideo();

			await UniTask.WaitUntil(() => isClosed);
			LoadRewardedVideo();
			return success;

			void Success(IronSourcePlacement placement, IronSourceAdInfo ironSourceAdInfo)
			{
				IronSourceRewardedVideoEvents.onAdRewardedEvent -= Success;
				
				success = true;
			}

			void VideoClosed(IronSourceAdInfo ironSourceAdInfo)
			{
				IronSourceRewardedVideoEvents.onAdClosedEvent -= VideoClosed;
				isClosed = true;
			}
		}

		public bool IsInterstitialReady()
		{
			return IsInit() && IronSource.Agent.isInterstitialReady();
		}

		public async UniTask<bool> ShowInterstitialAsync()
		{
			var isClosed = false;
			var success = false;
			IronSourceInterstitialEvents.onAdShowSucceededEvent += Success;
			IronSourceInterstitialEvents.onAdShowFailedEvent += Fail;
			IronSource.Agent.showInterstitial();

			await UniTask.WaitUntil(() => isClosed);

			return success;

			void Success(IronSourceAdInfo adInfo)
			{
				IronSourceInterstitialEvents.onAdShowSucceededEvent -= Success;
				IronSourceInterstitialEvents.onAdShowFailedEvent -= Fail;
				success = true;
				isClosed = true;
				LoadInterstitial();
			}

			void Fail(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
			{
				IronSourceInterstitialEvents.onAdShowSucceededEvent -= Success;
				IronSourceInterstitialEvents.onAdShowFailedEvent -= Fail;
				Debug.LogError(ironSourceError.ToString());
				isClosed = true;
			}
		}

		public bool IsInit()
		{
			if (IronSource.Agent != null)
				return true;

			return false;
		}

		public void LoadRewardedVideo()
		{
			IronSource.Agent.loadRewardedVideo();
		}

		public void LoadInterstitial()
		{
			IronSource.Agent.loadInterstitial();
		}

		private void InitializationComplete()
		{
			IronSourceEvents.onSdkInitializationCompletedEvent -= InitializationComplete;
			isInit = true;
		}
    }
}
