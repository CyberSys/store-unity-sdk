using System.Collections.Generic;

namespace Xsolla.Core
{
	/// <summary>
	/// Purchase parameters.
	/// </summary>
	public class PurchaseParams
	{
		/// <summary>
		/// Default purchase currency.
		/// </summary>
		public string currency;

		/// <summary>
		/// Interface language.
		/// </summary>
		public string locale;

		/// <summary>
		/// Quantity of purchased items.
		/// </summary>
		public int? quantity;

		/// <summary>
		/// Transaction external id.
		/// </summary>
		public string external_id;

		/// <summary>
		/// Project specific parameters.
		/// </summary>
		public Dictionary<string, object> custom_parameters;

		public ShippingData shipping_data;
		public Dictionary<string, object> shipping_method;

		/// <summary>
		/// Payment method ID.
		/// </summary>
		public int? payment_method;

		/// <summary>
		/// Whether to show a button to close Pay Station.
		/// </summary>
		public bool? close_button;

		/// <summary>
		/// The icon of the Close button in the payment UI. Can be `arrow` or `cross`.
		/// </summary>
		public string close_button_icon;

		/// <summary>
		/// Whether the Google Pay button is placed on top of the Payment UI.
		/// </summary>
		public bool? google_pay_quick_payment_button;
	}
}