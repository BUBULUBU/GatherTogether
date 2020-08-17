using Boo.Lang;
using Boo.Lang.Environments;
using System.Collections;
using UnityEngine;

public class HandleWeapons : MonoBehaviour
{
    public ItemDatabaseObject database;
    public InventoryObject equipment;

    public List<ItemObject> validWeapons = new List<ItemObject>();

    private Animator myAnimator;

    private GameObject currentWeapon;

    private void Awake()
    {
        myAnimator = transform.GetComponent<Animator>();
    }

    void Start()
    {
        for (int i = 0; i < database.ItemObjects.Length; i++)
        {
            if (database.ItemObjects[i].type == ItemType.Weapon)
            {
                validWeapons.Add(database.ItemObjects[i]);
            }
        }

        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnBeforeSlotUpdate;
            equipment.GetSlots[i].OnAfterUpdate += OnAfterSlotUpdate;
        } 

        StartCoroutine(LoadWeapons(1));
    }

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null) return;

        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Equipment:
                for (int i = 0; i < validWeapons.Count; i++)
                {
                    if (_slot.ItemObject.name == validWeapons[i].name)
                    {
                        currentWeapon = transform.Find("Weapons/" + validWeapons[i].name).gameObject;
                        currentWeapon.SetActive(false);
                        myAnimator.SetBool(validWeapons[i].name, false);
                    }
                }
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
            case InterfaceType.Equipment:
                for (int i = 0; i < validWeapons.Count; i++)
                {
                    if(_slot.ItemObject.name == validWeapons[i].name)
                    {
                        currentWeapon = transform.Find("Weapons/" + validWeapons[i].name).gameObject;
                        currentWeapon.SetActive(true);
                        myAnimator.SetBool(validWeapons[i].name, true);
                    }
                }
                break;
            default:
                break;
        }
    }

    private IEnumerator LoadWeapons(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Trying to load weapons on start..");

        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            Debug.Log(equipment.GetSlots[i].ItemObject.name);

            //if (equipment.GetSlots[i].ItemObject != null) {

            //    for (int j = 0; j < validWeapons.Count; j++)
            //    {
            //        Debug.Log(equipment.GetSlots[i].ItemObject.name);
            //        Debug.Log(validWeapons[j].name);

            //        if (equipment.GetSlots[i].ItemObject.name == validWeapons[j].name)
            //        {
            //            currentWeapon = transform.Find("Weapons/" + validWeapons[j].name).gameObject;
            //            currentWeapon.SetActive(true);
            //            myAnimator.SetBool(validWeapons[j].name, true);
            //        }
            //    }

            //}
        }
    }
}
