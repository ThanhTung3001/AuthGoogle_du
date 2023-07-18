using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;

namespace auth_social.Controllers;

[Route("/api/auth/")]
public class AuthController : ControllerBase
{
    private const string GoogleClientId = "596808682079-his2vhopiu2ob31m1o4k75tfpsrnltra.apps.googleusercontent.com";
    private const string GoogleClientSecret = "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";
    private const string GoogleRedirectUri = "http://localhost:5244/api/auth/google-callback";

    private readonly GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(
        new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = GoogleClientId,
                ClientSecret = GoogleClientSecret
            },
            Scopes = new[] { Oauth2Service.Scope.UserinfoEmail, Oauth2Service.Scope.UserinfoProfile },
            DataStore = new FileDataStore("Drive.Api.Auth.Store")
        });

    public ActionResult Login()
    {
        var authUrl = flow.CreateAuthorizationCodeRequest(GoogleRedirectUri).Build();
        return Redirect(authUrl.ToString());
    }
    
    [HttpGet]
    [Route(("google-callback"))]
    public async Task<ActionResult> GoogleCallback(string code)
    {

        
        var token = await flow.ExchangeCodeForTokenAsync(
            userId: "tunglvt",
            code: code,
            redirectUri: GoogleRedirectUri,
            taskCancellationToken: CancellationToken.None);

        var oauthService = new Oauth2Service(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = new UserCredential(flow, User.Identity.Name, token),
                ApplicationName = "Du App"
            });

        var userInfo = await oauthService.Userinfo.Get().ExecuteAsync();
        var userEmail = userInfo.Email;

        // You can use the userEmail to create or authenticate a user in your system

        return RedirectToAction("Index", "Home");
    }
}