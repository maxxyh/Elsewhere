using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot: BaseItemSlot
{
	public override bool CanAddStack(Item item, int amount = 1)
	{
		return base.CanAddStack(item, amount) && Amount + amount <= item.maxStack;
	}

	public override bool CanReceiveItem(Item item)
	{
		return true;
	}
}
