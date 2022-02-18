/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.SceneManagement;

public class AdMob : MonoBehaviour
{
    public bool enableAds = false;
    public bool enableTestAds = false;

    [Header("App ID")]
    public string appId;

    [Header("Banner ad")]
    public string bannerAdUnitId;
    public string testBannerAdUnitId;
    private BannerView bannerView;

    [Header("Interstitial ad replay")]
    public string interstitialAdUnitId;
    public string testInterstitialAdUnitId;
    [Range(0, 100)] public float showPropabilitieInterstitialAd = 50;
    private int maxLoadTries = 10;
    private InterstitialAd interstitial;

    [Header("Rewarded ad")]
    public string rewardedAdUnitId;
    public string testRewardedAdUnitId;
    private RewardedAd rewardedAd;
    private bool rewardEarned = false;
    private int maxLoadTries2 = 10;
    private bool rewardAdIsLoaded = false;

    private GameHandler ghScrp;

    private AfterAdType currentAfterAdType = AfterAdType.none;

    public enum AfterAdType
    {
       none, replay, toMainMenue
    }



    private void Start()
    {
        ghScrp = GameObject.Find("GameHandler").GetComponent<GameHandler>();

        if (Application.isEditor || !enableAds)
            return;

        MobileAds.Initialize(initStatus => { });

        string tmpAddId;

        if (Application.platform == RuntimePlatform.Android)
            tmpAddId = appId;
        else if (Application.isEditor)
            tmpAddId = "unused";
        else
            tmpAddId = "unexpected_platform";

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(tmpAddId);

        init();
    }

    public void init()
    {
        ghScrp = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        
        maxLoadTries = 10;
        maxLoadTries2 = 10;

        requestBanner();

        requestInterstitial();

        requestRewardedAd();
    }


    private void requestBanner()
    {
        if(bannerView != null)
            bannerView.Destroy();

        string tmpAdUnitId;

        if (Application.platform == RuntimePlatform.Android)
        {
            if (!enableTestAds)
            {
                if (bannerAdUnitId == null || bannerAdUnitId.Length == 0)
                    return;
                tmpAdUnitId = bannerAdUnitId;
            }
            else
            {
                if (testBannerAdUnitId == null || testBannerAdUnitId.Length == 0)
                    return;
                tmpAdUnitId = testBannerAdUnitId;
            }
        }
        else if (Application.isEditor)
        {
            tmpAdUnitId = "unused";
        }
        else
        {
            tmpAdUnitId = "unexpected_platform";
        }
                
        // Create a 320x50 banner at the bottom of the screen.
        this.bannerView = new BannerView(tmpAdUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    private void requestInterstitial()
    {
        if (maxLoadTries <= 0)
            return;

        if (interstitial != null)
            interstitial.Destroy();

        string tmpAdUnitId;

        if (Application.platform == RuntimePlatform.Android)
        {
            if (!enableTestAds)
            {
                if (interstitialAdUnitId == null || interstitialAdUnitId.Length == 0)
                    return;
                tmpAdUnitId = interstitialAdUnitId;
            }
            else
            {
                if (testInterstitialAdUnitId == null || testInterstitialAdUnitId.Length == 0)
                    return;
                tmpAdUnitId = testInterstitialAdUnitId;
            }
        }
        else if (Application.isEditor)
        {
            tmpAdUnitId = "unused";
        }
        else
        {
            tmpAdUnitId = "unexpected_platform";
        }

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(tmpAdUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);

        maxLoadTries--;
    }

    public void requestRewardedAd()
    {
        rewardAdIsLoaded = false;

        if (maxLoadTries2 <= 0)
            return;

        string tmpAdUnitId;

        if (Application.platform == RuntimePlatform.Android)
        {
            if (!enableTestAds)
            {
                if (rewardedAdUnitId == null || rewardedAdUnitId.Length == 0)
                    return;
                tmpAdUnitId = rewardedAdUnitId;
            }
            else
            {
                if (testRewardedAdUnitId == null || testRewardedAdUnitId.Length == 0)
                    return;
                tmpAdUnitId = testRewardedAdUnitId;
            }
        }
        else if (Application.isEditor)
        {
            tmpAdUnitId = "unused";
        }
        else
        {
            tmpAdUnitId = "unexpected_platform";
        }

        this.rewardedAd = new RewardedAd(tmpAdUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);

        maxLoadTries2--;
    }



    public void showHideBanner(bool show)
    {
        if (bannerView == null)
            return;

        if (show)
            bannerView.Show();
        else
            bannerView.Hide();
    }



    public void showInterstitialAd(AfterAdType type)
    {
        currentAfterAdType = type;

        if (interstitial != null && interstitial.IsLoaded() && UnityEngine.Random.value < showPropabilitieInterstitialAd / 100)
        {
            interstitial.Show();
        }
        else
        {
            afterInterstitialAd();
        }
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);

        requestInterstitial();
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");

        afterInterstitialAd();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    private void afterInterstitialAd()
    {
        requestInterstitial();

        switch (currentAfterAdType)
        {
            case AfterAdType.none:
                break;
            case AfterAdType.replay:
                ghScrp.startNewGame();
                break;
            case AfterAdType.toMainMenue:
                ghScrp.showHideMainMenue(true);
                break;
            default:
                break;
        }
    }



    public void watchDoublePremiumCoinAd()
    {
        if (rewardedAd != null && rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
        }
        else
        {
            afterReward();
        }
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
        rewardAdIsLoaded = true;
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
        rewardAdIsLoaded = false;
        requestRewardedAd();
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        rewardEarned = true;

        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        afterReward();
    }

    public bool getRewardAdIsLoaded()
    {
        return rewardAdIsLoaded;
    }

    private void afterReward()
    {
        bool tmpRewardEarned = rewardEarned;
        rewardEarned = false;
        requestRewardedAd();
        ghScrp.premiumCoinsScrp.afterAd(tmpRewardEarned);
    }
}
*/