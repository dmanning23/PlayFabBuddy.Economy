using System.Linq;

namespace PlayFabBuddy.Economy
{
	public class StoreItem
	{
		/// <summary>
		/// the backend id of this item
		/// </summary>
		public string ItemId { get; set; }

		/// <summary>
		/// How much virtual currency this item costs
		/// </summary>
		public int Cost { get; set; }

		/// <summary>
		/// What type of virtual currency is used to buy this item
		/// </summary>
		public string Currency { get; set; }

		protected StoreItem()
		{
		}

		public StoreItem(PlayFab.ClientModels.StoreItem inventoryItem)
		{
			ItemId = inventoryItem.ItemId;
			var cost = inventoryItem.VirtualCurrencyPrices.First();
			Cost = (int)cost.Value;
			Currency = cost.Key;
		}
	}
}
