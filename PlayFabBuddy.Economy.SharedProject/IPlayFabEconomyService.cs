using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayFabBuddy.Economy
{
	public interface IPlayFabEconomyService
	{
		/// <summary>
		/// A cache of the user's inventory.
		/// Key is item id
		/// </summary>
		Dictionary<string, InventoryItem> Inventory { get; }

		/// <summary>
		/// A cache of the user's virtual currency.
		/// Key is currency code, value is current amount
		/// </summary>
		Dictionary<string, int> Wallet { get; }

		/// <summary>
		/// Retrieve the user's complete inventory from the backend.
		/// All data retrieved will be cached in the Inventory dictionary.
		/// </summary>
		/// <returns>Contains the error message if an error occurred, otherwise will be empty.</returns>
		Task<string> GetInventory();

		/// <summary>
		/// Add virtual currency to the user's account.
		/// This feature is DISABLED by default on the playfab backend, because it is trivial for users to cheat using this method.
		/// You will have to manually turn this feature on, or else it won't work.
		/// Instead of using this method, PlayFab recommends building out the virtual currency economy using server-side CloudScript.
		/// </summary>
		/// <param name="currencyCode"></param>
		/// <param name="amount"></param>
		/// <returns>Contains the error message if an error occurred, otherwise will be empty.</returns>
		Task<string> AddVirtualCurrency(string currencyCode, int amount);

		/// <summary>
		/// Buy an item
		/// </summary>
		/// <param name="itemId"></param>
		/// <returns>Contains the error message if an error occurred, otherwise will be empty.</returns>
		Task<string> PurchaseItem(string itemId, int cost, string currency);

		/// <summary>
		/// Use an item
		/// </summary>
		/// <param name="itemId"></param>
		/// <param name="count"></param>
		/// <returns>Contains the error message if an error occurred, otherwise will be empty.</returns>
		Task<string> ConsumeItem(string itemId, int count);

		Task<List<StoreItem>> GetStore(string storeId);
	}
}
