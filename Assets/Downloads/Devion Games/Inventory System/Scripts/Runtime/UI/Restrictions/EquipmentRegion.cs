using System.Collections.Generic;
using System.Linq;

namespace DevionGames.InventorySystem.Restrictions
{
    public class EquipmentRegion : Restriction
    {
        [EquipmentPicker(true)]
        public DevionGames.InventorySystem.EquipmentRegion region;

        public override bool CanAddItem(Item item)
        {

            if (item == null || !(item is EquipmentItem equipmentItem)) { return false; }

            List<DevionGames.InventorySystem.EquipmentRegion> requiredRegions = new List<DevionGames.InventorySystem.EquipmentRegion>(equipmentItem.Region);

            EquipmentRegion[] restrictions = GetComponents<EquipmentRegion>();
            for (int i = requiredRegions.Count - 1; i >= 0; i--)
            {
                if (restrictions.Select(x => x.region.Name).Contains(requiredRegions[i].Name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}