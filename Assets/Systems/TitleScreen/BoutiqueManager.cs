using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System;
using System.Data;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine.Networking;

public class BoutiqueManager : FSystem
{

    private GameData gameData;
    public GameObject shopPanel;
    public Transform skinsContent;

	private Skins all_skins;
	public GameObject prefabSkin; // Prefab de l'affichage d'un skin

	[Serializable]
	public class Skin
	{
        public string path;
		public string logo;
        public int id;
		public string name;
	}

	[Serializable]
	public class Skins
	{
		public List<Skin> skins = new List<Skin>();
	}

    // Start is called before the first frame update
    // L'instance
	public static BoutiqueManager instance;

	public BoutiqueManager()
	{
		instance = this;
	}

	protected override void onStart()
    {
        GameObject go = GameObject.Find("GameData");
		if (go != null)
			gameData = go.GetComponent<GameData>();
    }

    // used on TitleScreen scene
	public void loadPanelShop()
	{
        string skinsPath = "Assets/Skins/Skins.json";
		
        string data = File.ReadAllText(skinsPath);

        all_skins = JsonUtility.FromJson<Skins>(data);

        foreach (Skin skin in all_skins.skins){
            GameObject s = UnityEngine.Object.Instantiate(prefabSkin, skinsContent);
            s.name = skin.name;

            Image theImage = s.GetComponent<Image>();

            var current = Resources.Load(skin.logo, typeof(Sprite)) as Sprite;

            theImage.sprite = current;

            Button bt = s.GetComponent<Button>();
        
            bt.onClick.AddListener(() => { ChangeSkin(skin.id);});

            GameObjectManager.bind(s);
        }
    }

    public void ChangeSkin(int i){
        Debug.Log("Click : " + i.ToString());
        PlayerPrefs.SetInt("currentSkinIndex", i);
    }

}
