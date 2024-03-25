#if UNITY_ANDROID
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using ZagaCore;

namespace FunZaga.Plugins.Auth
{
    public partial class AuthService
    {
        public AuthService()
        {
            InitializeAndSetupEvents();
        }
        
        // Setup authentication event handlers if desired
        private async void InitializeAndSetupEvents()
        {
            /*PlayGamesClientConfiguration  config = new PlayGamesClientConfiguration.Builder()
                // Requests an ID token be generated.
                .RequestServerAuthCode(false)
                .RequestIdToken()
                .Build();
            
            PlayGamesPlatform.InitializeInstance(config);*/
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            
            
            
            await UnityServices.InitializeAsync();
            Debug.Log("----------------->UnityServices Initialized.");
            Debug.Log(UnityServices.State);

            AuthenticationService.Instance.SignedIn += SignInSuccess;
            AuthenticationService.Instance.SignInFailed += SignInFailed;
            AuthenticationService.Instance.SignedOut += SignedOut;
            
            Debug.Log("------------Zaga: Login|LastAuthType:" + lastAuthType);
            Debug.Log("------------Zaga: NOT Authenticated");
            
            if (platformUtils.Platform == PlatformTypes.Unknown)
            {
                // let user know they can't play game on this platform.
                return;
            }
            Debug.Log("------------Zaga: None - Platform: Google");
            AuthenticatedLogin();
        }
        
        private void SignInSuccess()
        {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

            Refs.Instance.Bind<AuthService>(this);
            
            //TODO: make this fetch from unity back end?
            var clientData = new ClientData
            {
                EquippedItems = { "BasicGreatAxe", "Skull_Buddy" },
                EquippedMeshes = {
                    0, 0,
                    1, 0,
                    3, 15,
                    4, 18,
                    5, 15,
                    6, 18,
                },
                biped = Biped.Female
            };

            Refs.Instance.Bind(clientData);
        }

        private void SignInFailed(RequestFailedException err)
        {
            Debug.LogError(err);
        }

        private void SignedOut()
        {
            Debug.Log("Player signed out.");
        }
        
        private async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in anonymously succeeded!");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException exception)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(exception);
            }
        }
        
        private void AuthenticatedLogin()
        {
            Debug.LogFormat("------------Zaga: AuthenticatedLogin(google)");
            //PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.NoPrompt, PlayGamesSignInResult);
            PlayGamesPlatform.Instance.ManuallyAuthenticate(PlayGamesSignInResult);
        }

        private bool manualAuth = true;
        private async void PlayGamesSignInResult(SignInStatus signInStatus)
        {
            Debug.Log("------------------------");
            Debug.Log(signInStatus);
            switch (signInStatus)
            {
                case SignInStatus.Success:
            
                        // Continue with Play Games Services
                        //  print(PlayGamesPlatform.Instance.GetUserId());
                        Debug.Log("------------------->RequestServerSideAccess");
                        PlayGamesPlatform.Instance.RequestServerSideAccess(
                            /* forceRefreshToken= */ true,
                            async code =>
                            {
                                Debug.Log("---------------------> got code:");
                                Debug.Log(code);
                                SignInWithGoogleAsync(code);
                            });
                        
                        break;
                /*case SignInStatus.UiSignInRequired:
                    PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, PlayGamesSignInResult);
                    break;*/
                case SignInStatus.Canceled:
                    Debug.Log("Goole Play Games Signin Cancelled");
                    if (manualAuth)
                    {
                        manualAuth = false;
                        PlayGamesPlatform.Instance.ManuallyAuthenticate(PlayGamesSignInResult);
                    }
                    
                    break;
                case SignInStatus.InternalError:
                    Debug.Log("Goole Play Games Signin IneternalError");
                    break;
            }
        }
        

        async Task SignInWithGoogleAsync(string idToken)
        {
            try
            {
                Debug.Log("------------------------>SignInWithGoogleAsync:"+idToken);
                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(idToken, new SignInOptions
                {
                    CreateAccount = true
                });
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.Log("-------->AuthenticationException");
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.Log("-------->RequestFailedException");
                Debug.LogException(ex);
            }
        }
    }
}

#endif