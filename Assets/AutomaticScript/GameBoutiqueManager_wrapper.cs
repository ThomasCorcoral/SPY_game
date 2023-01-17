using UnityEngine;
using FYFY;

public class GameBoutiqueManager_wrapper : BaseWrapper
{
	public UnityEngine.GameObject shopPanel;
	public UnityEngine.Transform skinsContent;
	public UnityEngine.UI.Text coinShow;
	public UnityEngine.GameObject prefabSkin;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "shopPanel", shopPanel);
		MainLoop.initAppropriateSystemField (system, "skinsContent", skinsContent);
		MainLoop.initAppropriateSystemField (system, "coinShow", coinShow);
		MainLoop.initAppropriateSystemField (system, "prefabSkin", prefabSkin);
	}

	public void loadPanelShop()
	{
		MainLoop.callAppropriateSystemMethod (system, "loadPanelShop", null);
	}

	public void BuySkin(System.Int32 i)
	{
		MainLoop.callAppropriateSystemMethod (system, "BuySkin", i);
	}

	public void ChangeSkin(System.Int32 i)
	{
		MainLoop.callAppropriateSystemMethod (system, "ChangeSkin", i);
	}

	public void set_current_skin()
	{
		MainLoop.callAppropriateSystemMethod (system, "set_current_skin", null);
	}

}
