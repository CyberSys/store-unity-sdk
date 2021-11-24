using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_BUY_ITEM = BASE_STORE_API_URL + "/payment/item/{1}";
		private const string URL_BUY_ITEM_FOR_VC = BASE_STORE_API_URL + "/payment/item/{1}/virtual/{2}";
		private const string URL_BUY_CURRENT_CART = BASE_STORE_API_URL + "/payment/cart";
		private const string URL_BUY_SPECIFIC_CART = BASE_STORE_API_URL + "/payment/cart/{1}";

		private const string URL_ORDER_GET_STATUS = BASE_STORE_API_URL + "/order/{1}";
		private const string URL_PAYSTATION_UI = "https://secure.xsolla.com/paystation3/?access_token=";
		private const string URL_PAYSTATION_UI_IN_SANDBOX_MODE = "https://sandbox-secure.xsolla.com/paystation3/?access_token=";

		private const string URL_CREATE_PAYMENT_TOKEN = BASE_STORE_API_URL + "/payment";

		/// <summary>
		/// Returns headers list such as <c>AuthHeader</c> and <c>SteamPaymentHeader</c>.
		/// </summary>
		/// <param name="token">Auth token taken from Xsolla Login.</param>
		/// <returns></returns>
		private List<WebRequestHeader> GetPaymentHeaders(Token token)
		{
			var headers = new List<WebRequestHeader>
			{
				WebRequestHeader.AuthHeader(token)
			};

			if (XsollaSettings.UseSteamAuth && XsollaSettings.PaymentsFlow == PaymentsFlow.SteamGateway)
			{
				var steamUserId = token.GetSteamUserID();
				if (!string.IsNullOrEmpty(steamUserId))
				{
					headers.Add(WebRequestHeader.SteamPaymentHeader(steamUserId));
				}
			}

			return headers;
		}

		/// <summary>
		/// Creates an order with a specified item. The created order will get a 'new' order status.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create order with specified item</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/payment/create-order-with-item"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <seealso cref="OpenPurchaseUi"/>
		public void ItemPurchase(string projectId, string itemSku, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			var tempPurchaseParams = GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_ITEM, projectId, itemSku);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, GetPaymentHeaders(Token.Instance), onSuccess, onError, Error.BuyItemErrors);
		}

		/// <summary>
		/// Returns unique identifier of the created order and
		/// the Pay Station token for the purchase of the specified product by virtual currency.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create order with specified item purchased by virtual currency</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/payment/create-order-with-item-for-virtual-currency"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="priceSku">Virtual currency SKU.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <seealso cref="OpenPurchaseUi"/>
		public void ItemPurchaseForVirtualCurrency(
			string projectId,
			string itemSku,
			string priceSku,
			[CanBeNull] Action<PurchaseData> onSuccess,
			[CanBeNull] Action<Error> onError,
			PurchaseParams purchaseParams = null)
		{
			TempPurchaseParams tempPurchaseParams = new TempPurchaseParams
			{
				sandbox = XsollaSettings.IsSandbox,
			};

			var url = string.Format(URL_BUY_ITEM_FOR_VC, projectId, itemSku, priceSku);
			var platformParam = GetPlatformUrlParam();
			url = ConcatUrlAndParams(url, platformParam);

			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, GetPaymentHeaders(Token.Instance), onSuccess, onError, Error.BuyItemErrors);
		}

		/// <summary>
		/// Creates an order with all items from the cart. The created order will get a 'new' order status.
		/// </summary>
		/// <remarks> Swagger method name:<c>Creates an order with all items from the current cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/payment/create-order/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		public void CartPurchase(string projectId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			var tempPurchaseParams = GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_CURRENT_CART, projectId);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, GetPaymentHeaders(Token.Instance), onSuccess, onError, Error.BuyCartErrors);
		}

		/// <summary>
		/// Creates an order with all items from the particular cart. The created order will get a 'new' order status.
		/// </summary>
		/// <remarks> Swagger method name:<c>Creates an order with all items from the particular cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/payment/create-order-by-cart-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		public void CartPurchase(string projectId, string cartId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			var tempPurchaseParams = GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_SPECIFIC_CART, projectId, cartId);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, GetPaymentHeaders(Token.Instance), onSuccess, onError, Error.BuyCartErrors);
		}

		/// <summary>
		/// Opens Pay Station in the browser with a retrieved Pay Station token.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/doc/pay-station"/>
		/// <param name="purchaseData">Contains Pay Station token for the purchase.</param>
		/// <param name="forcePlatformBrowser">Flag indicating whether to force platform browser usage ignoring plug-in settings.</param>
		/// <param name="onRestrictedPaymentMethod">Restricted payment method was triggered in an in-app browser.</param>
		/// <seealso cref="BrowserHelper"/>
		public void OpenPurchaseUi(PurchaseData purchaseData, bool forcePlatformBrowser = false, Action<int> onRestrictedPaymentMethod = null)
		{
			string url = XsollaSettings.IsSandbox ? URL_PAYSTATION_UI_IN_SANDBOX_MODE : URL_PAYSTATION_UI;
			BrowserHelper.Instance.OpenPurchase(
				url,
				purchaseData.token,
				forcePlatformBrowser,
				onRestrictedPaymentMethod);
		}

		/// <summary>
		/// Returns status of the specified order.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get order</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/order/get-order"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="orderId">Unique order identifier.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="ItemPurchaseForVirtualCurrency"/>
		/// <seealso cref="CartPurchase"/>
		public void CheckOrderStatus(string projectId, int orderId, [NotNull] Action<OrderStatus> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_ORDER_GET_STATUS, projectId, orderId);
			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError, Error.OrderStatusErrors);
		}

		/// <summary>
		/// Creates a new payment token.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create payment token</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/commerce-api/cart-payment/payment/create-payment"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="amount">The total amount to be paid by the user.</param>
		/// <param name="currency">Default purchase currency. Three-letter code per ISO 4217.</param>
		/// <param name="description">Purchase description. Used to describe the purchase if there are no specific items.</param>
		/// <param name="locale">:Interface language. Two-letter lowercase language code.</param>
		/// <param name="externalID">Transaction's external ID.</param>
		/// <param name="paymentMethod">Payment method ID.</param>
		/// <param name="customParameters">Your custom parameters represented as a valid JSON set of key-value pairs.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CreatePaymentToken(
			string projectId,
			float amount,
			string currency,
			string description,
			string locale = null,
			string externalID = null,
			int? paymentMethod = null,
			object customParameters = null,
			Action<TokenEntity> onSuccess = null,
			[CanBeNull] Action<Error> onError = null)
		{
			var url = string.Format(URL_CREATE_PAYMENT_TOKEN, projectId);

			var checkout = new CreatePaymentTokenRequest.Purchase.Checkout(amount, currency);
			var purchaseDescription = new CreatePaymentTokenRequest.Purchase.Description(description);
			var purchase = new CreatePaymentTokenRequest.Purchase(checkout, purchaseDescription);
			var settings = GeneratePaymentTokenSettings(currency, locale, externalID, paymentMethod);
			var requestBody = new CreatePaymentTokenRequest(purchase, settings, customParameters);

			WebRequestHelper.Instance.PostRequest<TokenEntity, CreatePaymentTokenRequest>(SdkType.Store, url, requestBody, GetPaymentHeaders(Token.Instance), onSuccess, onError, Error.BuyItemErrors);
		}

		/// <summary>
		/// Creates a new payment token.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create payment token</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/commerce-api/cart-payment/payment/create-payment"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="amount">The total amount to be paid by the user.</param>
		/// <param name="currency">Default purchase currency. Three-letter code per ISO 4217.</param>
		/// <param name="items">Used to describe a purchase if it includes a list of specific items.</param>
		/// <param name="locale">:Interface language. Two-letter lowercase language code.</param>
		/// <param name="externalID">Transaction's external ID.</param>
		/// <param name="paymentMethod">Payment method ID.</param>
		/// <param name="customParameters">Your custom parameters represented as a valid JSON set of key-value pairs.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CreatePaymentToken(
			string projectId,
			float amount,
			string currency,
			PaymentTokenItem[] items,
			string locale = null,
			string externalID = null,
			int? paymentMethod = null,
			object customParameters = null,
			Action<TokenEntity> onSuccess = null,
			[CanBeNull] Action<Error> onError = null)
		{
			var url = string.Format(URL_CREATE_PAYMENT_TOKEN, projectId);

			var checkout = new CreatePaymentTokenRequest.Purchase.Checkout(amount, currency);

			var purchaseItems = new List<CreatePaymentTokenRequest.Purchase.Item>(items.Length);
			foreach (var item in items)
			{
				var price = new CreatePaymentTokenRequest.Purchase.Item.Price(item.amount, item.amountBeforeDiscount);
				var purchaseItem = new CreatePaymentTokenRequest.Purchase.Item(item.name, price, item.imageUrl, item.description, item.quantity, item.isBouns);
				purchaseItems.Add(purchaseItem);
			}

			var purchase = new CreatePaymentTokenRequest.Purchase(checkout, purchaseItems.ToArray());
			var settings = GeneratePaymentTokenSettings(currency, locale, externalID, paymentMethod);
			var requestBody = new CreatePaymentTokenRequest(purchase, settings, customParameters);

			WebRequestHelper.Instance.PostRequest<TokenEntity, CreatePaymentTokenRequest>(SdkType.Store, url, requestBody, GetPaymentHeaders(Token.Instance), onSuccess, onError, Error.BuyItemErrors);
		}

		private TempPurchaseParams GenerateTempPurchaseParams(PurchaseParams purchaseParams)
		{
			var settings = new TempPurchaseParams.Settings();

			settings.ui = PayStationUISettings.GenerateSettings();
			settings.redirect_policy = RedirectPolicySettings.GeneratePolicy();
			if (settings.redirect_policy != null)
			{
				settings.return_url = settings.redirect_policy.return_url;
			}

			//Fix 'The array value is found, but an object is required' in case of empty values.
			if (settings.ui == null && settings.redirect_policy == null && settings.return_url == null)
				settings = null;

			var tempPurchaseParams = default(TempPurchaseParams);

			if (purchaseParams != null)
			{
				tempPurchaseParams = new TempPurchaseParams()
				{
					sandbox = XsollaSettings.IsSandbox,
					settings = settings,
					custom_parameters = purchaseParams.custom_parameters,
					currency = purchaseParams.currency,
					locale = purchaseParams.locale,
					quantity = purchaseParams.quantity
				};
			}
			else
			{
				tempPurchaseParams = new TempPurchaseParams()
				{
					sandbox = XsollaSettings.IsSandbox,
					settings = settings
				};
			}

			return tempPurchaseParams;
		}

		private CreatePaymentTokenRequest.Settings GeneratePaymentTokenSettings(string currency, string locale, string externalID, int? paymentMethod)
		{
			var baseSettings = new TempPurchaseParams.Settings();
			baseSettings.ui = PayStationUISettings.GenerateSettings();
			baseSettings.redirect_policy = RedirectPolicySettings.GeneratePolicy();

			if (baseSettings.redirect_policy != null)
				baseSettings.return_url = baseSettings.redirect_policy.return_url;

			var settings = new CreatePaymentTokenRequest.Settings();
			settings.return_url = baseSettings.return_url;
			settings.ui = baseSettings.ui;
			settings.redirect_policy = baseSettings.redirect_policy;

			settings.currency = currency;
			settings.locale = locale;
			settings.sandbox = XsollaSettings.IsSandbox;
			settings.external_id = externalID;
			settings.payment_method = paymentMethod;

			return settings;
		}
	}
}
