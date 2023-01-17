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

public class GameBoutiqueManager : FSystem
{

    private GameData gameData;
    public GameObject shopPanel;
    public Transform skinsContent;
    public Text coinShow;

    private UnityAction localCallback;
	private Skins all_skins;
	public GameObject prefabSkin; // Prefab de l'affichage d'un skin

	[Serializable]
	public class Skin
	{
        public string path;
		public string logo;
        public int id;
		public string name;
        public int price;
	}

	[Serializable]
	public class Skins
	{
		public List<Skin> skins = new List<Skin>();
	}

    // Start is called before the first frame update
    // L'instance
	public static GameBoutiqueManager instance;

	public GameBoutiqueManager()
	{
		instance = this;
	}

	protected override void onStart()
    {
        GameObject go = GameObject.Find("GameData");
		if (go != null)
			gameData = go.GetComponent<GameData>();
    }

    
	protected override void onProcess(int familiesUpdateCount) {
		//Debug.Log("Test");
        int money = PlayerPrefs.GetInt("money", 0);
        coinShow.text = money.ToString() + " coins";
	}
    

    // used on TitleScreen scene
	public void loadPanelShop()
	{
        string skinsPath = "Assets/Skins/Skins.json";
		
        string data = File.ReadAllText(skinsPath);

        all_skins = JsonUtility.FromJson<Skins>(data);

        foreach (Skin skin in all_skins.skins){

            PlayerPrefs.SetString("SkinMaterial" + skin.id.ToString(), skin.path);
            PlayerPrefs.SetInt("SkinPrice" + skin.id.ToString(), skin.price);

            GameObject s = UnityEngine.Object.Instantiate(prefabSkin, skinsContent);
            s.name = skin.name;

            Image theImage = s.GetComponent<Image>();

            var current = Resources.Load(skin.logo, typeof(Sprite)) as Sprite;

            theImage.sprite = current;

            Button bt = s.GetComponent<Button>();
        
            bt.onClick.AddListener(() => { BuySkin(skin.id);});

            GameObjectManager.bind(s);
        }
    }

    public void BuySkin(int i){

        int money = PlayerPrefs.GetInt("money", 0);
        int price = PlayerPrefs.GetInt("SkinPrice" + i.ToString(), 0);
        int check_possessed = PlayerPrefs.GetInt("SkinBuyed" + i.ToString(), 0);

        if(check_possessed == 1){
            ChangeSkin(i);
            localCallback = null;
            GameObjectManager.addComponent<MessageForUser>(MainLoop.instance.gameObject, new { message = "Changement de skin effectué !", OkButton = "", CancelButton = "OK", call = localCallback });

        }else if(price == 0){
            ConfirmBuySkin(i, price, money);
        }else if(money >= price){
            localCallback = () => {ConfirmBuySkin(i, price, money);};
            GameObjectManager.addComponent<MessageForUser>(MainLoop.instance.gameObject, new { message = "Es-tu sur de bien vouloir acheter ce skin pour " + price.ToString() + " pièces ?", OkButton = "OUI", CancelButton = "NON", call = localCallback });
        }else{
            localCallback = null;
            GameObjectManager.addComponent<MessageForUser>(MainLoop.instance.gameObject, new { message = "Tu n'as pas assez d'argent pour acheter ce skin", OkButton = "", CancelButton = "OK", call = localCallback });
        }
    }

    public void ConfirmBuySkin(int i, int price, int money){
        
        PlayerPrefs.SetInt("money", money-price);
        PlayerPrefs.SetInt("SkinBuyed" + i.ToString(), 1);
        //PlayerPrefs.Save();
        ChangeSkin(i);

        localCallback = null;
        GameObjectManager.addComponent<MessageForUser>(MainLoop.instance.gameObject, new { message = "Félicitation, tu viens d'acheter un nouveau skin. Ce dernier vient d'être appliqué !", OkButton = "", CancelButton = "OK", call = localCallback });
    }

    public void ChangeSkin(int i){
        //Debug.Log("Click : " + i.ToString());
        PlayerPrefs.SetInt("currentSkinIndex", i);
        set_current_skin();

        GameObjectManager.addComponent<ActionPerformedForLRS>(MainLoop.instance.gameObject, new
		{
			verb = "interacted",
			objectType = "item",
			activityExtensions = new Dictionary<string, string>() {
				{"action", i.ToString()}
			}
		});

    }

    public void set_current_skin(){
        int current_skin = PlayerPrefs.GetInt("currentSkinIndex", 0);
        var skin = PlayerPrefs.GetString("SkinMaterial" + current_skin, "Skins/Materials/Robot_Color");
        foreach(GameObject robotKyle in GameObject.FindGameObjectsWithTag("Robot2"))
        {
            robotKyle.GetComponent<Renderer>().material = Resources.Load(skin, typeof(Material)) as Material;
        }
    }

}
