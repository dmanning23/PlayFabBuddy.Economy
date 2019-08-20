using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayFabBuddy.Economy
{
	public class CatalogItem : StoreItem
	{
		public string ItemClass { get; set; }

		public string DisplayName { get; set; }

		public CatalogItem(PlayFab.ClientModels.CatalogItem inventoryItem)
		{
			ItemClass = inventoryItem.ItemClass;
			DisplayName = inventoryItem.DisplayName;

			ItemId = inventoryItem.ItemId;
			var cost = inventoryItem.VirtualCurrencyPrices.First();
			Cost = (int)cost.Value;
			Currency = cost.Key;
		}
	}
}
