﻿using System;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_CATALOG_GET_ITEMS = BASE_STORE_API_URL + "/items/virtual_items";
		private const string URL_CATALOG_GET_BUNDLE = BASE_STORE_API_URL + "/items/bundle/sku/{1}";
		private const string URL_CATALOG_GET_BUNDLES = BASE_STORE_API_URL + "/items/bundle";
		private const string URL_CATALOG_GET_ITEMS_IN_GROUP = BASE_STORE_API_URL + "/items/virtual_items/group/{1}";
		private const string URL_CATALOG_GET_GROUPS = BASE_STORE_API_URL + "/items/groups";

		/// <summary>
		/// Returns all items in catalog.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get virtual items list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-items"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetCatalog(string projectId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_ITEMS, projectId)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AnalyticHeaders, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns a specified bundle.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get specified bundle</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/bundles/catalog/get-bundle"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="sku"></param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetBundle(string projectId, string sku, [NotNull] Action<BundleItem> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_BUNDLE, projectId, sku)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError, Error.ItemsListErrors);
		}
		
		/// <summary>
		/// Returns all bundles in catalog.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get list of bundles</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/bundles/catalog/get-bundle-list"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetBundles(string projectId, [NotNull] Action<BundleItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_BUNDLES, projectId)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns items in a specific group.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get items list by specified group</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-items-group"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="groupExternalId">Group external ID.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetGroupItems(string projectId, string groupExternalId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_ITEMS_IN_GROUP, projectId, groupExternalId)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AnalyticHeaders, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns groups list.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get items groups list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/groups/get-item-groups"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		public void GetItemGroups(string projectId, [NotNull] Action<Groups> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_GROUPS, projectId)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AnalyticHeaders, onSuccess, onError);
		}
	}
}