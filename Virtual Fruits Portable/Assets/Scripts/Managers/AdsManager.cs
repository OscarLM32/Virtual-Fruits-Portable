using System;
using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// Handles the logic behind playing interstitial ads inside the application
/// </summary>
public class AdsManager : MonoBehaviour, IUnityAdsListener
{
     /// <summary>
     /// Specific suffix that every Android device Ad unit uses
     /// </summary>
     private const string _androidSuffix = "Android";
     
     /// <summary>
     /// Specific suffix that every iOS device Ad unit uses
     /// </summary>
     private const string _iosSuffix = "iOS";
     
     
#if UNITY_ANDROID
     private const string _gameId = "5179624";
     private const DevicePlatform _devicePlatform = DevicePlatform.Android;
#else
     private const string _gameId = "5179625";
     private const DevicePlatform _devicePlatform = DevicePlatform.IOS;
#endif

     private Action _onAdEnd;

     private void Start()
     {
          Advertisement.Initialize(_gameId);
          Advertisement.AddListener(this);
     }

     /// <summary>
     /// Plays an ad of the proper type depending on the device and executes an action after is it completed
     /// </summary>
     /// <param name="adEndAction">An action to be executed when the ad is finished</param>
     public void PlayAd(Action adEndAction)
     {
          var adId = "Interstitial_";
          adId +=  _devicePlatform == DevicePlatform.Android ? _androidSuffix : _iosSuffix;

          _onAdEnd += adEndAction;
          if (!Advertisement.IsReady(adId))
               return;
          
          Advertisement.Show(adId);
     }

     public void OnUnityAdsReady(string placementId)
     {
          
     }

     /// <summary>
     /// This logic is executed when the ad gets an error and it executes the logic to be expected when an ad is
     /// finished playing
     /// </summary>
     /// <param name="message">Error message</param>
     public void OnUnityAdsDidError(string message)
     {
          _onAdEnd?.Invoke();
          
          Time.timeScale = 1;
     }

     /// <summary>
     /// This logic is played when the ad starts playing an it sets the timescale to 0 so the game is "paused"
     /// while the ad is playing
     /// </summary>
     /// <param name="placementId">Optional placement</param>
     public void OnUnityAdsDidStart(string placementId)
     {
          Time.timeScale = 0;
     }

     /// <summary>
     /// When the ad is finished playing it executes the logic of the action set when the ad was played
     /// </summary>
     /// <param name="placementId">Placement identifier</param>
     /// <param name="showResult">Result of the ad</param>
     public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
     {
          //There is no real need to check whether it has finished or has been skipped. Because the result is the same
          //if(showResult is ShowResult.Finished or ShowResult.Skipped)
          _onAdEnd?.Invoke();

          //Sets the timescale 1 to "unpause" the game
          Time.timeScale = 1;
     }
     
     
     private enum DevicePlatform
     {
          Android,
          IOS
     }
}
