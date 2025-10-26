using UnityEngine;

public class StandSlot : MonoBehaviour
{
    public int slotId;
    public bool isOccupied = false;
    private GameObject currentItem;

    public bool CanPlaceItem(int itemId)
    {
        return !isOccupied && itemId == slotId;
    }

    public void PlaceItem(GameObject item)
    {
        if (item == null) return;

        item.transform.SetParent(null);
        //  item.transform.position = transform.position;
        item.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        isOccupied = true;
        currentItem = item;

        Debug.Log($"Item {item.name} doğru yere yerleştirildi! ✅");
    }
}
