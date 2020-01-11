using PlayFab.ClientModels;
using PlayFabBuddyLib;
using PlayFabBuddyLib.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayFabBuddy.Economy
{
	public class PlayFabEconomyService : IPlayFabEconomyService
	{
		#region Properties

		public event EventHandler<WalletEventArgs> OnCurrencyChanged;

		public Dictionary<string, InventoryItem> Inventory { get; private set; }

		public Dictionary<string, int> Wallet { get; private set; }

		IPlayFabClient _playfab;
		IPlayFabAuthService _auth;

		object _lock = new object();

		#endregion //Properties

		#region Methods

		public PlayFabEconomyService(IPlayFabClient playfab, IPlayFabAuthService auth)
		{
			Inventory = new Dictionary<string, InventoryItem>();
			Wallet = new Dictionary<string, int>();

			_playfab = playfab;
			_auth = auth;

			_auth.OnLogout += Auth_OnLogout;
		}

		private void Auth_OnLogout(object sender, System.EventArgs e)
		{
			Inventory = new Dictionary<string, InventoryItem>();
			Wallet = new Dictionary<string, int>();
		}

		public async Task<string> GetInventory()
		{
			var result = await _playfab.GetUserInventoryAsync(new GetUserInventoryRequest());

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

				if (null != OnCurrencyChanged)
				{
					OnCurrencyChanged(this, new WalletEventArgs(currencyCode, result.Result.Balance));
				}
			}

			return result.Error?.ErrorMessage ?? string.Empty;
		}

		public async Task<string> PurchaseItem(string itemId, int cost, string currency)
		{
			var result = await _playfab.PurchaseItemAsync(new PurchaseItemRequest()
			{
				ItemId = itemId,
				Price = cost,
				VirtualCurrency = currency
			});

			if (null == result.Error)
			{
				//Update the inventory
				foreach (var purchasedItem in result.Result.Items)
				{
					UpdateCachedItem(purchasedItem);
				}

				//update the wallet
				if (Wallet.ContainsKey(currency))
				{
					Wallet[currency] = Wallet[currency] - cost;

					if (null != OnCurrencyChanged)
					{
						OnCurrencyChanged(this, new WalletEventArgs(currency, Wallet[currency]));
					}
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
				ConsumeCount = count
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
			lock (_lock)
			{
				if (Inventory.TryGetValue(inventoryItem.ItemId, out InventoryItem cachedInventoryItem))
				{
					cachedInventoryItem.NumUses = inventoryItem.RemainingUses;
				}
				else
				{
					//Add the inventory item
					Inventory[inventoryItem.ItemId] = new InventoryItem(inventoryItem);
				}
			}
		}

		public async Task<List<StoreItem>> GetStore(string storeId)
		{
			var result = await _playfab.GetStoreItemsAsync(new GetStoreItemsRequest()
			{
				StoreId = storeId,
			});

			var storeItems = new List<PlayFabBuddy.Economy.StoreItem>();

			if (null == result.Error)
			{
				foreach (var item in result.Result.Store)
				{
					storeItems.Add(new PlayFabBuddy.Economy.StoreItem(item));
				}
			}

			return storeItems;
		}

		public async Task<List<CatalogItem>> GetCatalog(string catalogId = "1.0")
		{
			var result = await _playfab.GetCatalogItemsAsync(new GetCatalogItemsRequest()
			{
				CatalogVersion = catalogId,
			});

			var items = new List<PlayFabBuddy.Economy.CatalogItem>();

			if (null == result.Error)
			{
				foreach (var item in result.Result.Catalog)
				{
					items.Add(new PlayFabBuddy.Economy.CatalogItem(item));
				}
			}

			return items;
		}

		#endregion //Methods
	}
}
