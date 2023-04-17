using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public partial class XsollaAuth : MonoSingleton<XsollaAuth>
	{
		private const string URL_USER_CONSOLE_AUTH = "https://livedemo.xsolla.com/sdk/sdk-shadow-account/auth";

		#region Comment
		/// <summary>
		/// This method is used for authenticating users in Xsolla Login,
		/// who play on the consoles and other platforms
		/// where Xsolla Login isn't used. You must implement it
		/// on your server side.
		/// Integration flow on the server side:
		/// <list type="number">
		///		<item>
		///			<term>Generate server JWT</term>
		///			<description>
		///				<list type="bullet">
		///					<item>
		///						<term>Connect OAuth 2.0 server client.</term>
		///						<description>Follow the [instructions](https://developers.xsolla.com/doc/login/security/connecting-oauth2/#login_features_connecting_oauth2_connecting_client) to connect the client and cope copy the <b>Client ID</b> and <b>Secret key</b>.
		///						</description>
		///					</item>
		///					<item>
		///						<term>Implement method: </term>
		///						<description>
		///							<see href="https://developers.xsolla.com/login-api/oauth-20/generate-user-jwt"/>
		///							with `application/x-www-form-urlencoded` payload parameters:
		///							<list type="bullet">
		///								<item>
		///									<description>client_id=YOUR_CLIENT_ID</description>
		///								</item>
		///								<item>
		///									<description>client_secret=YOUR_CLIENT_SECRET</description>
		///								</item>
		///								<item>
		///									<description>grant_type=client_credentials</description>
		///								</item>
		///							</list>
		///						</description>
		///					</item>
		///				</list>
		///			</description>
		///		</item>
		///		<item>
		///			<term>Implement auth method </term>
		///			<description>
		///				<see href="https://developers.xsolla.com/login-api/jwt/auth-by-custom-id"/>
		///				with:
		///				<list type="bullet">
		///					<item>
		///						<term>Query parameters </term>
		///						<description><c>?publisher_project_id=XsollaSettings.StoreProjectId</c></description>
		///					</item>
		///					<item>
		///						<term>Headers </term>
		///						<description>
		///						`Content-Type: application/json` and `X-SERVER-AUTHORIZATION: YourGeneratedJwt`
		///						</description>
		///					</item>
		///					<item>
		///						<term>[More information about authentication via custom ID](https://developers.xsolla.com/sdk/unity/authentication/auth-via-custom-id/).</term>
		///					</item>
		///				</list>
		///
		///			</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <param name="userId">Social platform (XBox, PS4, etc) user unique identifier.</param>
		/// <param name="platform">Platform name (XBox, PS4, etc).</param>
		/// <param name="successCase">Called after successful user authentication. Authentication data including the JWT will be received.</param>
		/// <param name="failedCase">Called after the request resulted with an error.</param>
		#endregion
		public void SignInConsoleAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase)
		{
			var with_logout = XsollaSettings.InvalidateExistingSessions ? "1" : "0";
			var url = $"{URL_USER_CONSOLE_AUTH}?user_id={userId}&platform={platform}&with_logout={with_logout}";
			WebRequestHelper.Instance.GetRequest(SdkType.Login, url, (TokenEntity result) => { successCase?.Invoke(result.token); }, failedCase);
		}
	}
}
