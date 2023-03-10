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
using DIG.GBLXAPI;

public class BoutiqueManager : FSystem
{

    private GameData gameData;
    public GameObject shopPanel;
    public Transform skinsContent;
    public Renderer robot;
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
        public string description;
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
        set_current_skin();
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
        if(PlayerPrefs.GetInt("boutiqueLoaded",0) == 0){
            foreach (Skin skin in all_skins.skins){
                PlayerPrefs.SetString("SkinMaterial" + skin.id.ToString(), skin.path);
                PlayerPrefs.SetInt("SkinPrice" + skin.id.ToString(), skin.price);

                GameObject s = UnityEngine.Object.Instantiate(prefabSkin, skinsContent);
                s.name = skin.name;

                GameObject b = s.transform.GetChild(0).gameObject;
                Image theImage = b.GetComponent<Image>();
                var current = Resources.Load(skin.logo, typeof(Sprite)) as Sprite;
                theImage.sprite = current;
                Button bt = b.GetComponent<Button>();
                bt.onClick.AddListener(() => { BuySkin(skin.id);});

                GameObject im = s.transform.GetChild(2).gameObject;
                Button bt2 = im.GetComponent<Button>();
                bt2.onClick.AddListener(() => { BuySkin(skin.id);});

                GameObject desc = s.transform.GetChild(1).gameObject;
                Text t = desc.GetComponent<Text>();
                t.text = skin.description;

                GameObject p = s.transform.GetChild(3).gameObject;
                Text pt = p.GetComponent<Text>();
                if(PlayerPrefs.GetInt("SkinBuyed" + skin.id.ToString(), 0) == 1){
                    pt.text = "Achete";
                }else if(skin.price == 0){
                    PlayerPrefs.SetInt("SkinBuyed" + skin.id.ToString(), 1);
                    pt.text = "Achete";
                }else{
                    pt.text = skin.price.ToString() + " coins";
                }

                
                Button bt3 = p.GetComponent<Button>();
                bt3.onClick.AddListener(() => { BuySkin(skin.id);});

                GameObjectManager.bind(s);
            }
            PlayerPrefs.SetInt("boutiqueLoaded",1);
        }else{
            for(int i = 0; i < skinsContent.transform.childCount; i++){
                GameObject current = skinsContent.transform.GetChild(i).gameObject;
                GameObject p = current.transform.GetChild(3).gameObject;
                Text pt = p.GetComponent<Text>();
                if(PlayerPrefs.GetInt("SkinBuyed" + all_skins.skins[i].id.ToString(), 0) == 1){
                    pt.text = "Achete";
                }else if(all_skins.skins[i].price == 0){
                    PlayerPrefs.SetInt("SkinBuyed" + all_skins.skins[i].id.ToString(), 1);
                    pt.text = "Achete";
                }
            }
        }
    }

    public void BuySkin(int i){

        int money = PlayerPrefs.GetInt("money", 0);
        int price = PlayerPrefs.GetInt("SkinPrice" + i.ToString(), 0);
        int check_possessed = PlayerPrefs.GetInt("SkinBuyed" + i.ToString(), 0);

        if(check_possessed == 1){
            ChangeSkin(i);
            localCallback = null;
            GameObjectManager.addComponent<MessageForUser>(MainLoop.instance.gameObject, new { message = "Changement de skin effectu?? !", OkButton = "", CancelButton = "OK", call = localCallback });

        }else if(price == 0){
            ConfirmBuySkin(i, price, money);
        }else if(money >= price){
            localCallback = () => {ConfirmBuySkin(i, price, money);};
            GameObjectManager.addComponent<MessageForUser>(MainLoop.instance.gameObject, new { message = "Es-tu sur de bien vouloir acheter ce skin pour " + price.ToString() + " pi??ces ?", OkButton = "OUI", CancelButton = "NON", call = localCallback });
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
        GameObjectManager.addComponent<MessageForUser>(MainLoop.instance.gameObject, new { message = "F??licitation, tu viens d'acheter un nouveau skin. Ce dernier vient d'??tre appliqu?? !", OkButton = "", CancelButton = "OK", call = localCallback });
    
        GameObject current = skinsContent.transform.GetChild(i).gameObject;
        GameObject p = current.transform.GetChild(3).gameObject;
        Text pt = p.GetComponent<Text>();
        pt.text = "Achete";
    
    }

    public void ChangeSkin(int i){
        //Debug.Log("Click : " + i.ToString());
        PlayerPrefs.SetInt("currentSkinIndex", i);
        //PlayerPrefs.Save();
        var skin = PlayerPrefs.GetString("SkinMaterial" + i, "Skins/Materials/Robot_Color");
        robot.material = Resources.Load(skin, typeof(Material)) as Material;

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
        robot.material = Resources.Load(skin, typeof(Material)) as Material;
    }

}
