#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

#if UNITY_IOS
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
#endif


using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GooglePlayGameControl : MonoBehaviour
{
    private string Token;

#if UNITY_IOS
    private AppleAuthManager appleAuthManager;


#endif

    void Awake()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Activate();
#endif

#if UNITY_IOS
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }
#endif
    }

    public void LoginGooglePlayGames()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games successful.");

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Debug.Log("Authorization code: " + code);
                    Token = code;
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                });
            }
            else
            {
                Debug.LogError("Failed to retrieve Google play games authorization code");
                Debug.Log("Login Unsuccessful");
            }
        });
#endif

#if UNITY_IOS
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        this.appleAuthManager.LoginWithAppleId(loginArgs, credential =>
            {
                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    // Apple User ID
                    // You should save the user ID somewhere in the device
                    var userId = appleIdCredential.User;
                    //PlayerPrefs.SetString(AppleUserIdKey, userId);

                    // Email (Received ONLY in the first login)
                    var email = appleIdCredential.Email;

                    // Full name (Received ONLY in the first login)
                    var fullName = appleIdCredential.FullName;

                    // Identity token
                    Token = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);

                    // Authorization code
                    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);
                    Debug.Log($"Login success: {userId} | {email} | {fullName} | {Token}");
                    // And now you have all the information to create/login a user in your system
                }
            },
            error =>
            {
                // Something went wrong
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogError(error.GetAuthorizationErrorCode());
                Debug.LogError(error.LocalizedDescription);
                Debug.LogError(error.LocalizedFailureReason);
                Debug.LogError(error.LocalizedRecoveryOptions);
                Debug.LogError(error.LocalizedRecoverySuggestion);
            });

#endif
    }

    public void SocialLogin()
    {
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("Authentication successful");
                string userInfo = "Username: " + Social.localUser.userName +
                                  "\nUser ID: " + Social.localUser.id +
                                  "\nIsUnderage: " + Social.localUser.underage +
                                  "\nToken ID: " + Social.localUser.id;
                Debug.Log(userInfo);
            }
            else
                Debug.Log("Authentication failed");
        });

    }

    public void Login()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games successful.");

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Debug.Log("Authorization code: " + code);
                    Token = code;
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                });
            }
            else
            {
                Debug.LogError("Failed to retrieve Google play games authorization code");
                Debug.Log("Login Unsuccessful");
            }
        });
#endif
    }


    public void ShowToken()
    {
        Debug.Log("PlayGamesPlatform.Instance.localUser.id: " + Token);
    }
}
