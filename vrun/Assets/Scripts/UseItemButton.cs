using VRSF.Utils;

namespace VRRun.Items
{
	public class UseItemButton : ButtonActionChoser 
	{
        public void UseItem()
        {
            EventManager.ActivatePlayersInventoryItem();
        }
    }
}