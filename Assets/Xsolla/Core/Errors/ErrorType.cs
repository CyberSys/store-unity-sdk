namespace Xsolla.Core
{
	public enum ErrorType
	{
		/// <summary>
		/// Description: Represents an undefined or uninitialized error state.
		/// Causes: The error type has not been set, is default-initialized, or was not mapped correctly from an unexpected API response.
		/// Recommended Actions: Inspect the raw error response payload, ensure SDK components are initialized, and check if the error is unhandled in mapping logic.
		/// </summary>
		Undefined,

		/// <summary>
		/// Description: Represents an error of unknown origin or type.
		/// Causes: A generic, unhandled exception occurred, or the backend returned an error code that has no specific mapping in the SDK.
		/// Recommended Actions: Check the error logs for additional context, report the issue to support if it persists, or handle it as a fallback generic error.
		/// </summary>
		UnknownError,

		/// <summary>
		/// Description: Indicates a failure during network communication.
		/// Causes: No internet connection, DNS resolution failure, network timeout, or connection dropped by the server.
		/// Recommended Actions: Verify the user's internet connection, implement a retry mechanism with exponential backoff, or check service status.
		/// </summary>
		NetworkError,

		/// <summary>
		/// Description: The user authentication token is invalid, expired, or malformed.
		/// Causes: Token lifetime expired, user logged out, or the token was incorrectly formatted or modified.
		/// Recommended Actions: Force user re-authentication to obtain a fresh token, or check that the token is passed correctly in request headers.
		/// </summary>
		InvalidToken,

		/// <summary>
		/// Description: The HTTP request is missing the required authorization header.
		/// Causes: The SDK attempted to call a secured API endpoint without attaching the authorization token.
		/// Recommended Actions: Ensure the user is logged in before calling this endpoint, or verify the request interceptor attaches the Authorization header.
		/// </summary>
		AuthorizationHeaderNotSent,

		/// <summary>
		/// Description: The requested HTTP method is not allowed by the endpoint.
		/// Causes: The SDK sent a POST request to a GET-only endpoint, or the backend API has been updated/restricted.
		/// Recommended Actions: Check the API documentation to verify the allowed HTTP methods, and inspect custom API calls or SDK version compatibility.
		/// </summary>
		MethodIsNotAllowed,

		/// <summary>
		/// Description: The requested operation or SDK feature is not supported on the current target platform.
		/// Causes: Platform-specific APIs (such as certain payment options, login methods, or browser components) are invoked on an unsupported build target (e.g. WebGL vs. Mobile).
		/// Recommended Actions: Check platform support flags before executing the method, or provide an alternative implementation for the current platform.
		/// </summary>
		NotSupportedOnCurrentPlatform,

		/// <summary>
		/// Description: The request payload contains invalid, incomplete, or malformed data.
		/// Causes: Required fields are missing, field validation failed on the client or server, or data formats (e.g., email structure) are incorrect.
		/// Recommended Actions: Validate client-side input parameters before sending the request, and inspect server-side validation error messages.
		/// </summary>
		InvalidData,

		/// <summary>
		/// Description: The requested product or item was not found in the catalog.
		/// Causes: Incorrect product SKU or ID, the product has been deleted or deactivated in the Publisher Account, or the store catalog is not published.
		/// Recommended Actions: Double-check the SKU/ID matches the catalog in the Xsolla Publisher Account, and ensure the catalog is published and active.
		/// </summary>
		ProductDoesNotExist,

		/// <summary>
		/// Description: An error occurred within the Pay Station payment service.
		/// Causes: Pay Station backend issues, invalid payment configuration, or failed transaction initialization.
		/// Recommended Actions: Check the Xsolla service status, verify payment settings (such as project ID and payment methods) in the Publisher Account.
		/// </summary>
		PayStationServiceException,

		/// <summary>
		/// Description: The specified user account could not be found.
		/// Causes: Incorrect username, email, or user ID provided during search, login, or inventory operations.
		/// Recommended Actions: Prompt the user to check their login credentials, or verify the user ID exists in the Xsolla Publisher Account database.
		/// </summary>
		UserNotFound,

		/// <summary>
		/// Description: The specified shopping cart does not exist or has expired.
		/// Causes: The cart ID is invalid, or the cart has been deleted/expired due to inactivity.
		/// Recommended Actions: Create a new cart or verify the active cart ID before performing cart operations.
		/// </summary>
		CartNotFound,

		/// <summary>
		/// Description: The specified order does not exist or cannot be found.
		/// Causes: The order ID is invalid, or the order has been archived or expired.
		/// Recommended Actions: Check the order ID, verify the order status via Xsolla Publisher Account, or wait for the order creation webhook to complete.
		/// </summary>
		OrderNotFound,

		/// <summary>
		/// Description: The coupon code entered by the user is invalid, expired, or already used.
		/// Causes: Typo in coupon code, coupon campaign expired, usage limit reached, or coupon doesn't apply to items in the cart.
		/// Recommended Actions: Prompt the user to verify the coupon code, check campaign dates, and ensure cart items meet coupon criteria.
		/// </summary>
		InvalidCoupon,

		/// <summary>
		/// Description: Password reset is not permitted or configured for the current project.
		/// Causes: Password reset functionality is disabled or misconfigured in the Xsolla Login project settings.
		/// Recommended Actions: Enable and configure password reset options in the Xsolla Publisher Account under Login settings.
		/// </summary>
		PasswordResetNotAllowedForProject,

		/// <summary>
		/// Description: User registration is not allowed for the project or is temporarily disabled.
		/// Causes: Registration settings are disabled in the Login project, or restrictions prevent new signups.
		/// Recommended Actions: Enable user registration in the Login section of the Xsolla Publisher Account, or check registration filters.
		/// </summary>
		RegistrationNotAllowedException,

		/// <summary>
		/// Description: Token verification failed on the server.
		/// Causes: Invalid signature, token tampering, or mismatched encryption/decryption keys between the server and identity provider.
		/// Recommended Actions: Check Login project settings, verify public keys or client secret configured in the Xsolla Publisher Account.
		/// </summary>
		TokenVerificationException,

		/// <summary>
		/// Description: The chosen username is already registered.
		/// Causes: Another user has already registered with the exact username.
		/// Recommended Actions: Prompt the user to choose a different, unique username, or suggest variations of their input.
		/// </summary>
		UsernameIsTaken,

		/// <summary>
		/// Description: The chosen email address is already registered.
		/// Causes: A user account already exists with the provided email address.
		/// Recommended Actions: Prompt the user to log in instead, reset their password, or use a different email address.
		/// </summary>
		EmailIsTaken,

		/// <summary>
		/// Description: The user account is registered but not yet activated or verified.
		/// Causes: The user has not clicked the activation link sent to their email.
		/// Recommended Actions: Instruct the user to check their email for the activation link, or provide a button to resend the activation email.
		/// </summary>
		UserIsNotActivated,

		/// <summary>
		/// Description: A CAPTCHA verification is required to complete the operation.
		/// Causes: Too many failed login/registration attempts, or security thresholds triggered on the backend.
		/// Recommended Actions: Implement and display a CAPTCHA widget in the user interface, and pass the CAPTCHA response token with the next request.
		/// </summary>
		CaptchaRequiredException,

		/// <summary>
		/// Description: The SDK project configuration settings are invalid or missing.
		/// Causes: Missing Project ID, Login ID, or Merchant ID in XsollaSettings, or mismatched credentials.
		/// Recommended Actions: Check and configure Xsolla project settings in the Unity Editor (Window > Xsolla > Edit Settings).
		/// </summary>
		InvalidProjectSettings,

		/// <summary>
		/// Description: The login credentials (username/email or password) are incorrect.
		/// Causes: User typed the wrong password, or the username/email does not match any registered account.
		/// Recommended Actions: Notify the user of invalid credentials, and prompt them to retry or recover their password.
		/// </summary>
		InvalidLoginOrPassword,

		/// <summary>
		/// Description: The authorization code is invalid or has expired.
		/// Causes: The OAuth2/OIDC authorization code has already been exchanged, was modified, or has timed out.
		/// Recommended Actions: Re-initiate the OAuth2 authorization flow to obtain a new authorization code.
		/// </summary>
		InvalidAuthorizationCode,

		/// <summary>
		/// Description: The maximum number of attempts to use or request authorization codes has been exceeded.
		/// Causes: Too many consecutive failed attempts to exchange authorization codes or request authentication within a short period.
		/// Recommended Actions: Rate-limit requests, inform the user to wait before attempting again, or implement a cooldown timer.
		/// </summary>
		ExceededAuthorizationCodeAttempts,

		/// <summary>
		/// Description: Multiple login URLs are configured or resolved, leading to ambiguity.
		/// Causes: Conflicting redirect or callback URL configurations in the Login project settings.
		/// Recommended Actions: Check and clean up the callback/redirect URLs in the Xsolla Publisher Account under Login settings.
		/// </summary>
		MultipleLoginUrlsException,

		/// <summary>
		/// Description: The submitted login or redirect URL was not found in the allowed configurations.
		/// Causes: The URL passed by the client is not registered in the whitelist of redirect URLs in the Publisher Account.
		/// Recommended Actions: Add the requested redirect/callback URL to the allowed list in the Login project settings on the Xsolla Publisher Account.
		/// </summary>
		SubmittedLoginUrlNotFoundException,

		/// <summary>
		/// Description: The social/friend operation cannot be performed due to the current relationship status.
		/// Causes: Attempting to accept a request when no pending request exists, or block a user who is already blocked.
		/// Recommended Actions: Refresh the user's friend list and state before performing friendship actions, and validate state transitions.
		/// </summary>
		IncorrectFriendState,

		/// <summary>
		/// Description: The operation timed out or reached its maximum allowed time.
		/// Causes: Network latency was too high, or a long-running process (like checking order status) did not complete in time.
		/// Recommended Actions: Retry the operation, increase timeout duration if appropriate, or check the status asynchronously later.
		/// </summary>
		TimeLimitReached
	}
}