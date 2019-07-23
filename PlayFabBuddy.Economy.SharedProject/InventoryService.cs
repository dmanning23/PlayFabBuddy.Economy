using PlayFab.ClientModels;
using PlayFabBuddyLib;
using PlayFabBuddyLib.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayFabBuddy.Economy.SharedProject
{
	public class InventoryService : IInventoryService
	{
		#region Properties

		public Dictionary<string, InventoryItem> Inventory { get; private set; }

		public Dictionary<string, int> Wallet { get; private set; }

		IPlayFabClient _playfab;
		IPlayFabAuthService _auth;

		#endregion //Properties

		#region Methods

		#endregion //Methods

		public InventoryService(IPlayFabClient playfab, IPlayFabAuthService auth)
		{
			Inventory = new Dictionary<string, InventoryItem>();
			Wallet = new Dictionary<string, int>();

			_playfab = playfab;
			_auth = auth;
		}

		public async Task<string> GetInventory()
		{
			var result = await _playfab.GetUserInventoryAsync(new GetUserInventoryRequest() {
			});

			if (null == result.Error)
			{
				//Update the inventory
				foreach (var inventoryItem in result.Result.Inventory)
				{
					UpdateCachedItem(inventoryItem);
				}

				//update the wallet
				foreach (var virtualCurrency in result.Result.VirtualCurrency)
				{
					Wallet[virtualCurrency.Key] = virtualCurrency.Value;
				}
			}

			return result.Error?.ErrorMessage ?? string.Empty;
		}

		public async Task<string> AddVirtualCurrency(string currencyCode, int amount)
		{
			var result = await _playfab.AddUserVirtualCurrencyAsync(new AddUserVirtualCurrencyRequest()
			{
				Amount = amount,
				VirtualCurrency = currencyCode,
			});

			if (null == result.Error)
			{
				//update the wallet
				Wallet[result.Result.VirtualCurrency] = result.Result.Balance;
			}

			return result.Error?.ErrorMessage ?? string.Empty;
		}

		public async Task<string> PurchaseItem(string itemId)
		{
			if (!Inventory.ContainsKey(itemId))
			{
				return $"Error: itemId {itemId} not found in the inventory";
			}
			var item = Inventory[itemId];

			var result = await _playfab.PurchaseItemAsync(new PurchaseItemRequest()
			{
				ItemId = item.Id,
				Price = item.Cost,
				VirtualCurrency = item.Currency
			});

			if (null == result.Error)
			{
				//Update the inventory
				foreach (var purchasedItem in result.Result.Items)
				{
					UpdateCachedItem(purchasedItem);
				}

				//update the wallet
				if (Wallet.ContainsKey(item.Currency))
				{
					Wallet[item.Currency] = Wallet[item.Currency] - item.Cost;
				}
			}

			return result.Error?.ErrorMessage ?? string.Empty;
		}

		public async Task<string> ConsumeItem(string itemId, int count)
		{
			if (!Inventory.ContainsKey(itemId))
			{
				return $"Error: itemId {itemId} not found in the inventory";
			}
			var item = Inventory[itemId];

			var result = await _playfab.ConsumeItemAsync(new ConsumeItemRequest()
			{
				ItemInstanceId = item.ItemInstanceId,
				ConsumeCount= count
			});

			if (null == result.Error)
			{
				//Update the inventory
				item.NumUses = result.Result.RemainingUses;
			}

			return result.Error?.ErrorMessage ?? string.Empty;
		}

		private void UpdateCachedItem(ItemInstance inventoryItem)
		{
			if (Inventory.TryGetValue(inventoryItem.ItemId, out InventoryItem cachedInventoryItem))
			{
				cachedInventoryItem.NumUses = inventoryItem.RemainingUses;
			}
			else
			{
				//Add the inventory item
				Inventory[inventoryItem.ItemId] = new InventoryItem
				{
					Id = inventoryItem.ItemId,
					DisplayName = inventoryItem.DisplayName,
					NumUses = inventoryItem.RemainingUses,
					Cost = (int)inventoryItem.UnitPrice,
					Currency = inventoryItem.UnitCurrency,
					ItemInstanceId = inventoryItem.ItemInstanceId,
				};
			}
		}
	}
}
