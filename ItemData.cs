using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public int index;
        public bool collected=false;
    }
}
