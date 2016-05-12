using UnityEngine;
using System.Collections.Generic;

public class Inventory {

	Dictionary<string, int> inventoryMap;
	int maxItems;

	/// <summary>
	/// Initializes a new instance of the <see cref="Inventory"/> class.
	/// </summary>
	/// <param name="maxItems">Max items.</param>
	public Inventory (int maxItems = 20) {
		inventoryMap = new Dictionary<string, int>();
		this.maxItems = maxItems;
	}

	/// <summary>
	/// Adds the item.
	/// </summary>
	/// <returns><c>true</c>, if item was added, <c>false</c> otherwise.</returns>
	/// <param name="item">Item.</param>
	/// <param name="amount">Amount.</param>
	public bool AddItem (string item, int amount) {
		if (amount <= 0) return false;
		
		if (inventoryMap.ContainsKey (item) == true) {
			inventoryMap [item] += amount;
		}
		else {
			inventoryMap.Add(item, amount);
		}
		return true;
	}

	/// <summary>
	/// Removes the item.
	/// Does not remove the amount if more than exists.
	/// </summary>
	/// <returns><c>true</c>, if item was removed, <c>false</c> otherwise.</returns>
	/// <param name="item">Item.</param>
	/// <param name="amount">Amount.</param>
	public bool RemoveItem (string item, int amount) {
		if (inventoryMap.ContainsKey(item) == false || amount <= 0) return false;
		
		if (inventoryMap[item] < amount) return false;

		inventoryMap [item] -= amount;
		return true;
	}
}
