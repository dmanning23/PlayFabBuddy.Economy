
namespace PlayFabBuddy.Economy
{
	public class InventoryItem
	{
		/// <summary>
		/// the backend id of this item
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// The display name of this item
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// If a consumable item, the number of uses left.
		/// Null for non-consumables
		/// </summary>
		public int? NumUses { get; set; }

		/// <summary>
		/// How much virtual currency this item costs
		/// </summary>
		public int Cost { get; set; }

		/// <summary>
		/// What type of virtual currency is used to buy this item
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// This is used by the backend.
		/// </summary>
		public string ItemInstanceId { get; set; }
	}
}
