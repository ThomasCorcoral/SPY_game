using UnityEngine;
using FYFY;

public class BoutiqueManager_wrapper : BaseWrapper
{
	public UnityEngine.GameObject shopPanel;
	public UnityEngine.Transform skinsContent;
	public UnityEngine.GameObject prefabSkin;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "shopPanel", shopPanel);
		MainLoop.initAppropriateSystemField (system, "skinsContent", skinsContent);
		MainLoop.initAppropriateSystemField (system, "prefabSkin", prefabSkin);
	}

	public void loadPanelShop()
	{
		MainLoop.callAppropriateSystemMethod (system, "loadPanelShop", null);
	}

	public void ChangeSkin(System.Int32 i)
	{
		MainLoop.callAppropriateSystemMethod (system, "ChangeSkin", i);
	}

}
