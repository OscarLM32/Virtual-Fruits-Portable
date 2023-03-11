using System;
using UnityEngine;
using UnityEngine.Advertisements;
public class AdsManager : MonoBehaviour, IUnityAdsListener
{
     private const string _androidSufix = "Android";
     private const string _iosSufix = "iOS";
     
#if UNITY_ANDROID
     private string _gameId = "5179624";
     private DevicePlatform _devicePlatform = DevicePlatform.Android;
#else
     private string _gameId = "5179625";
     private DevicePlatform _devicePlatform = DevicePlatform.IOS;
#endif

     private Action _onAdEnd;

     private void Start()
     {
          Advertisement.Initialize(_gameId);
          Advertisement.AddListener(this);
     }

     public void PlayAd(Action adEndAction)
     {
          var adId = "Interstitial_";
          adId +=  _devicePlatform == DevicePlatform.Android ? _androidSufix : _iosSufix;

          _onAdEnd += adEndAction;
          if (!Advertisement.IsReady(adId))
               return;
          
          Advertisement.Show(adId);
     }

     public void OnUnityAdsReady(string placementId)
     {
          
     }

     public void OnUnityAdsDidError(string message)
     {
          _onAdEnd?.Invoke();
     }

     public void OnUnityAdsDidStart(string placementId)
     {
          Time.timeScale = 0;
     }

     public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
     {
          if(showResult is ShowResult.Finished or ShowResult.Skipped)
               _onAdEnd?.Invoke();

          Time.timeScale = 1;
     }
     
     
     private enum DevicePlatform
     {
          Android,
          IOS
     }
}
