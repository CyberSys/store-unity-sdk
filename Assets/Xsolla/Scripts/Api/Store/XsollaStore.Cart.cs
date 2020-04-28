﻿using System;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_CART_CREATE_NEW = "https://store.xsolla.com/api/v2/project/{0}/cart";
		private const string URL_CART_ITEM_UPDATE = "https://store.xsolla.com/api/v2/project/{0}/cart/{1}/item/{2}";
		private const string URL_CART_ITEM_REMOVE = "https://store.xsolla.com/api/v2/project/{0}/cart/{1}/item/{2}";
		private const string URL_CART_GET_ITEMS = "https://store.xsolla.com/api/v2/project/{0}/cart/{1}";
		private const string URL_CART_CLEAR = "https://store.xsolla.com/api/v2/project/{0}/cart/{1}/clear";

		/// <summary>
		/// Creates a new cart on the Xsolla side.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get current user's cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart/get-user-cart"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CreateNewCart(string projectId, [NotNull] Action<Cart> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_CREATE_NEW, projectId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.CreateCartErrors);
		}

		/// <summary>
		/// Updates an existing item or creates the one in the cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update cart line item by cart ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart/put-item-by-cart-id"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="itemSku">SKU of item for purchase.</param>
		/// <param name="quantity">Quantity of purchased item.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="CreateNewCart"/>
		public void UpdateItemInCart(string projectId, string cartId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_ITEM_UPDATE, projectId, cartId, itemSku)).Append(AdditionalUrlParams);

			Quantity jsonObject = new Quantity { quantity = quantity };

			WebRequestHelper.Instance.PutRequest<Quantity>(urlBuilder.ToString(), jsonObject, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.AddToCartCartErrors);
		}

		/// <summary>
		/// Deletes all cart items.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete all cart line items by cart ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart/cart-clear-by-id"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void ClearCart(string projectId, string cartId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_CLEAR, projectId, cartId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PutRequest<Quantity>(urlBuilder.ToString(), null, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.AddToCartCartErrors);
		}

		/// <summary>
		/// Returns a user’s cart by ID.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get cart by ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart/get-cart-by-id"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetCartItems(string projectId, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_GET_ITEMS, projectId, cartId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.GetCartItemsErrors);
		}

		/// <summary>
		/// Delete item from the cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete cart line item by cart ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart/delete-item-by-cart-id"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="itemSku">Item SKU to delete.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RemoveItemFromCart(string projectId, string cartId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_ITEM_REMOVE, projectId, cartId, itemSku)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.DeleteRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.DeleteFromCartErrors);
		}
	}
}