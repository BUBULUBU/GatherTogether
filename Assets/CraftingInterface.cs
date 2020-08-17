using UnityEngine;
using UnityEngine.UI;

public class CraftingInterface : MonoBehaviour
{
    public ItemDatabaseObject database;
    public GameObject craftingPrefab;

    private Transform descriptionText;
    private Transform contentList;

    void Start()
    {
        contentList = transform.Find("GridScrollView/Viewport/Content");
        descriptionText = transform.parent.Find("txtDescriptionOfItem");

        AddGridItems();
    }

    private void AddGridItems()
    {
        GameObject tempItem;

        for (int i = 0; i < database.ItemObjects.Length; i++)
        {
            ItemObject item = database.ItemObjects[i];

            if(item.craftable)
            {
                tempItem = Instantiate(craftingPrefab, contentList.transform);
                tempItem.GetComponentInChildren<Text>().text = item.data.Name;
                tempItem.transform.GetChild(0).GetComponent<Image>().sprite = item.uiDisplay;
                tempItem.GetComponent<Button>().onClick.AddListener(() => UpdateDescription(item.description));
            }
        }
    }

    public void UpdateDescription(string description)
    {
        descriptionText.GetComponent<Text>().text = description;
    }
}