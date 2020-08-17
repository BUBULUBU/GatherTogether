using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public InventoryObject inventory;
    public InventoryObject equipment;
    public GameObject inventoryUI;

    public Attribute[] attributes;

    private bool inventoryEnabled;

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null) return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                //Debug.Log(string.Concat("Removed ", _slot.ItemObject, " on ", _slot.parent.inventory.type, ", Allowed Items: ", string.Join(", ", _slot.AllowedItems)));

                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                        {
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                        }
                    }
                }
                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null) return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                //Debug.Log(string.Concat("Placed ", _slot.ItemObject, " on ", _slot.parent.inventory.type, ", Allowed Items: ", string.Join(", ", _slot.AllowedItems)));

                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if(attributes[j].type == _slot.item.buffs[i].attribute)
                        {
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);
                        }
                    }
                }
                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    public void Start()
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryEnabled = !inventoryEnabled;
        }

        if (inventoryEnabled)
        {
            inventoryUI.SetActive(true);
        }
        else
        {
            inventoryUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            if(inventory.AddItem(new Item(item.item), 1))
            {
                Destroy(other.gameObject);
            }

            StartCoroutine(SaveInventory());
        }
    }

    public void AttributeModified(Attribute attribute)
    {
        //Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }

    public IEnumerator SaveInventory()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", photonView.Owner.NickName);
        form.AddField("inventory_json", inventory.SaveData());
        form.AddField("equipment_json", equipment.SaveData());
        UnityWebRequest www = UnityWebRequest.Post("http://85.214.107.230/gathertogether/inventory_save.php", form);

        yield return www.SendWebRequest();

        if (www.downloadHandler.text == "0")
        {
            Debug.LogFormat("{0} inventory has been saved.", photonView.Owner.NickName);
        }
        else
        {
            Debug.LogFormat("Saving inventory for {0} failed. Error #{1}", photonView.Owner.NickName, www.downloadHandler.text);
        }
    }

    public IEnumerator LoadInventory()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", photonView.Owner.NickName);
        UnityWebRequest www = UnityWebRequest.Post("http://85.214.107.230/gathertogether/inventory_load.php", form);

        yield return www.SendWebRequest();

        if (www.downloadHandler.text != "0")
        {
            JsonUtility.FromJsonOverwrite(www.downloadHandler.text, inventory);

            Debug.Log("Inventory loaded for " + photonView.Owner.NickName);
        }
        else
        {
            Debug.LogFormat("Loading inventory for {0} failed. Error #{1}", photonView.Owner.NickName, www.downloadHandler.text);
        }

        WWWForm form2 = new WWWForm();
        form2.AddField("username", photonView.Owner.NickName);
        UnityWebRequest www2 = UnityWebRequest.Post("http://85.214.107.230/gathertogether/equipment_load.php", form2);

        yield return www2.SendWebRequest();

        if (www2.downloadHandler.text != "0")
        {
            JsonUtility.FromJsonOverwrite(www2.downloadHandler.text, equipment);

            for (int i = 0; i < equipment.GetSlots.Length; i++)
            {
                equipment.GetSlots[i].OnBeforeUpdate += OnBeforeSlotUpdate;
                equipment.GetSlots[i].OnAfterUpdate += OnAfterSlotUpdate;
            }

            Debug.Log("Equipment loaded for " + photonView.Owner.NickName);
        }
        else
        {
            Debug.LogFormat("Loading equipment for {0} failed. Error #{1}", photonView.Owner.NickName, www2.downloadHandler.text);
        }
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public PlayerManager parent;
    public Attributes type;
    public ModifieableInt value;

    public void SetParent(PlayerManager _parent)
    {
        parent = _parent;
        value = new ModifieableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}