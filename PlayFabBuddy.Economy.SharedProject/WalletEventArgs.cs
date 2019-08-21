using System;

namespace PlayFabBuddy.Economy
{
	public class WalletEventArgs : EventArgs
	{
		public string Currency { get; set; }

		public int Total { get; set; }

		public WalletEventArgs(string currency, int amount)
		{
			Currency = currency;
			Total = amount;
		}
	}
}
