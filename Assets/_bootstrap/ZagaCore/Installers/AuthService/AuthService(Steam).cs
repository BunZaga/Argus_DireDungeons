using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using ZagaCore;

#if UNITY_STEAM
using System;
using System.Linq;
using Steamworks;
using FunZaga.Events.Steam;
using FunZaga.Plugins.Steam;
#endif

namespace FunZaga.Plugins.Auth
{
    public partial class AuthService
    {
#if UNITY_STEAM
        private void OnSteamLoginSuccess()
        {
            Debug.Log("------------steam now ready, logging in");
            eventService.UnSubscribe<SteamLoginSuccess>(OnSteamLoginSuccess);
            Login();
        }
#endif
#if UNITY_EDITOR && !(UNITY_ANDROID || UNITY_IOS)
        public AuthService()
        {
            InitializeAndSetupEvents();
        }

        // Setup authentication event handlers if desired
        private async void InitializeAndSetupEvents()
        {
            await UnityServices.InitializeAsync();
            Debug.Log(UnityServices.State);

            AuthenticationService.Instance.SignedIn += SignInSuccess;
            AuthenticationService.Instance.SignInFailed += SignInFailed;
            AuthenticationService.Instance.SignedOut += SignedOut;

            Login();
        }

        private void SignInSuccess()
        {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

            Refs.Instance.Bind(this);

            //TODO: make this fetch from unity back end?
            var clientData = new ClientData
            {
                EquippedItems = { /*"BasicGreatAxe"*/"BasicSword", "BasicShield", "Box_Buddy" },
                EquippedMeshes = {
                    0, 0,
                    1, 0,
                    3, 15,
                    4, 18,
                    5, 15,
                    6, 18,
                    //8, 17
                },
                biped = Biped.Male
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
#endif
#if UNITY_STEAM
        private void Login()
        {
            Debug.Log("------------Zaga: Login|LastAuthType:" + lastAuthType);

            if (!SteamManager.Initialized)
            {
                Debug.Log("-----------steam not ready, waiting");
                eventService.Subscribe<SteamLoginSuccess>(OnSteamLoginSuccess);
                return;
            }

            Debug.Log("------------Zaga: NOT Authenticated");

            // first time user logged in on this device

            if (platformUtils.Platform == PlatformTypes.Unknown)
            {
                // let user know they can't play game on this platform.
                return;
            }

            Debug.Log("------------Zaga: None - Platform: Steam");

            AuthenticatedLogin();
        }

        /// <summary>
        /// Authenticate Client
        /// </summary>
        /// <param name="createAccount"></param>
        private async Task AuthenticatedLogin()
        {
            Debug.LogFormat("------------Zaga: AuthenticatedLogin(steam)");

            byte[] ticketBlob = new byte[1024];
            uint ticketSize;

            var steamService = Refs.Instance.Get<SteamService>();
            steamService.HTicket = SteamUser.GetAuthSessionTicket(ticketBlob, ticketBlob.Length, out ticketSize);
            Debug.Log(ticketSize);

            Array.Resize(ref ticketBlob, (int) ticketSize);

            var hexTicket = string.Concat(ticketBlob.Select(b => b.ToString("x2")));

            Debug.Log(hexTicket);

            Debug.Log("-------=====>logging in with new session.");

            SignInWithSteamAsync(hexTicket);
        }

        async Task LinkWithSteamAsync(string ticket)
        {
            try
            {
                await AuthenticationService.Instance.LinkWithSteamAsync(ticket);
                Debug.Log("Link is successful.");
            }
            catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            {
                // Prompt the player with an error message.
                Debug.LogError("This user is already linked with another account. Log in instead.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Link failed.");
                Debug.LogException(ex);
            }
        }

        private async Task SignInWithSteamAsync(string ticket)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithSteamAsync(ticket);
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.Log("------------ AuthenticationException ------------");
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.Log("------------ RequestFailedException ------------");
                Debug.LogException(ex);
            }
        }

#elif UNITY_EDITOR && !(UNITY_ANDROID || UNITY_IOS)
        private void Login()
        {
              SignInAnonymouslyAsync();
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
#endif
#if ((UNITY_STEAM || UNITY_EDITOR) && !(UNITY_ANDROID || UNITY_IOS))
        /*private async Task SignInWithSessionTokenAsync()
        {
            try
            {
                await AuthenticationService.Instance.Sign();
                Debug.Log("Sign in with session token succeeded!");
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
        }*/
#endif
    }
}