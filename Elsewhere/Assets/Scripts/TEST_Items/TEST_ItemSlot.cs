using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TEST_ItemSlot: TEST_BaseItemSlot
{
	public override bool CanAddStack(TEST_Item item, int amount = 1)
	{
		return base.CanAddStack(item, amount) && Amount + amount <= item.maxStack;
	}

	public override bool CanReceiveItem(TEST_Item item)
	{
		return true;
	}
}
