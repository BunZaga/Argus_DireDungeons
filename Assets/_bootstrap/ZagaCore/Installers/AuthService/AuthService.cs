using System.Threading.Tasks;
using UnityEngine;
using ZagaCore;

namespace FunZaga.Plugins.Auth
{
    public class AuthErrorCode
    {
        public const long NONE = 0;
        public const long ACCOUNT_TOKEN_INVALID = 401;
        public const long ACCOUNT_NOT_FOUND = 404;
    }

    public enum AuthTypes
    {
        None = -1,
        Platform = 0
    }
    
    public partial class AuthService
    {
        private const string AUTH_TYPE = "AUTH_x0000";
        private const string AUTH_TOKEN = "AUTH_x0001";
        
        private Refs refs =>_refs ?? (_refs = Refs.Instance);
        private Refs _refs;
        private EventService eventService =>_eventService ?? (_eventService = Refs.Instance.Get<EventService>());
        private EventService _eventService;
        private PlatformUtils platformUtils => _platformUtils ?? (_platformUtils = Refs.Instance.Get<PlatformUtils>());
        private PlatformUtils _platformUtils;
        
        
        private AuthClient _client;

        public AuthTypes LastAuthType => lastAuthType;
        
        private AuthTypes lastAuthType
        {
            get
            {
                return (AuthTypes)PlayerPrefs.GetInt(AUTH_TYPE, -1);
            }
            set
            {
                PlayerPrefs.SetInt(AUTH_TYPE, (int)value);
            }
        }

        public string AuthToken => authToken;
        
        private string authToken
        {
            get
            {
                return PlayerPrefs.GetString(AUTH_TOKEN);
            }
            set
            {
                PlayerPrefs.SetString(AUTH_TOKEN, value);
            }
        }
        
        private async Task OnAuthSuccess()
        {
            Debug.Log("Authentication successful!!");
        }
        
        private void ClearAuthInfo()
        {
        }
    }
}