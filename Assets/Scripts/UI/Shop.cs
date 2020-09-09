using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Shop : MonoBehaviour {

	public static Shop instance = null;

	[System.Serializable]
	public class ItemForPuchase
	{
		public int price = 0;
		public int count = 1;
		public bool isPurchased = false;
		public Text priceText = null;
		public Button button = null;
		public Transform purchasedOverlay = null;
	}

	public List<ItemForPuchase> gunWeapons = new List<ItemForPuchase>();
	public List<ItemForPuchase> meleeWeapons = new List<ItemForPuchase>();
	public ItemForPuchase healthPack = new ItemForPuchase();
	public ItemForPuchase flarePack = new ItemForPuchase();
	public ItemForPuchase ammoPack = new ItemForPuchase();
	public ItemForPuchase freedom = new ItemForPuchase();

	public Text playerMoneyText = null;
	public Text playerFlareText = null;
	public Text playerAmmoText = null;

    public AudioClip PurchaseClip = null;

	bool isShopOpen = false;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		foreach(ItemForPuchase item in gunWeapons)
		{
			RegisterItem(item);
		}
		foreach(ItemForPuchase item in meleeWeapons)
		{
			RegisterItem(item);
		}
		RegisterItem(healthPack);
		RegisterItem(flarePack);
		RegisterItem(ammoPack);
		RegisterItem(freedom);
		//gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		var playerStats = PlayerController.instance.playerStats;
		playerMoneyText.text = playerStats.money.ToString();
		playerFlareText.text = playerStats.flares.ToString();
		playerAmmoText.text = playerStats.ammo.ToString();

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			CloseShop();
		}
	}

	public void OpenShop()
	{
		if(!isShopOpen)
		{
			GetComponent<Animator>().CrossFade("shop_open", 0.2f);
			isShopOpen = true;
			Time.timeScale = 0;
		}
	}

	public void CloseShop()
	{
		if(isShopOpen)
		{
			GetComponent<Animator>().CrossFade("shop_close", 0.2f);
			isShopOpen = false;
			Time.timeScale = 1;
		}
	}

	private bool IsItemPurchasedInList(List<ItemForPuchase> _items, int _index)
	{
		if(_index < 0 || _index >= _items.Count)
		{
			return false;
		}
		else
		{
			ItemForPuchase item = _items[_index];
			return item.isPurchased;
		}
	}

	public bool IsGunPurchased(int _index)
	{
		return IsItemPurchasedInList(gunWeapons, _index);
	}

	public bool IsMeleePurchased(int _index)
	{
		return IsItemPurchasedInList(meleeWeapons, _index);
	}

	public void RegisterItem(ItemForPuchase _item)
	{
		if(_item.isPurchased)
		{
			OnItemPurchased(_item);
			return;
		}
		if(_item.button)
		{
			_item.button.onClick.AddListener(() => 
			{ 
				OnItemPurchased(_item);
			});
		}
		if(_item.purchasedOverlay)
		{
			_item.purchasedOverlay.gameObject.SetActive(false);
		}
		if(_item.priceText)
		{
			_item.priceText.text = _item.price.ToString();
		}
	}

	public bool OnItemPurchased(ItemForPuchase _item)
	{
		var playerStats = PlayerController.instance.playerStats;
		if(playerStats.money >= _item.price)
		{
			if(_item == healthPack)
			{
				//don't purchase if health is full
				int maxHealth = 10;
				if(PlayerController.instance.character.health == maxHealth) //FIXME: hardcoded max health
				{
					return false;
				}
				PlayerController.instance.character.health = Mathf.Min (PlayerController.instance.character.health + _item.count, maxHealth);
			}
			else if(_item == flarePack)
			{
				playerStats.flares += _item.count;
			}
			else if(_item == ammoPack)
			{
				playerStats.ammo += _item.count;
			}
			else if(gunWeapons.Contains(_item))
			{
				if(_item.button)
				{
					_item.button.interactable = false;
				}
			}
			else if(meleeWeapons.Contains(_item))
			{
				if(_item.button)
				{
					_item.button.interactable = false;
				}
				playerStats.currentMelee = meleeWeapons.IndexOf(_item);
				if(playerStats.currentMelee+1 < meleeWeapons.Count)
				{
					meleeWeapons[playerStats.currentMelee+1].button.interactable = true;
				}
			}
			else if(_item == freedom)
			{
				Application.LoadLevel("SplashScene");
			}

			playerStats.money -= _item.price;
			_item.isPurchased = true;

			if(_item.purchasedOverlay)
			{
				_item.purchasedOverlay.gameObject.SetActive(true);
			}

            FAFAudio.Instance.PlayOnce2D(PurchaseClip, Vector3.zero, 0.3f);

			return true;
		}

		return false;
	}
}
