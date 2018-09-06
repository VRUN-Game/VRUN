using VRSF.Utils;

namespace VRRun.Items
{
    /// <summary>
    /// Klasse, die das Event zum aktivieren eines Items im Level ausführt.
    /// </summary>
	public class UseItemButton : ButtonActionChoser 
	{
        public void UseItem()
        {
            EventManager.ActivatePlayersInventoryItem();
        }
    }
}