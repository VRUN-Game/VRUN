using VRSF.Utils;

namespace VRRun.Items
{
	public class UseItemButton : ButtonActionChoser 
	{
        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        // EMPTY
        #region PRIVATE_VARIABLES

        #endregion

    
        // EMPTY
        #region MONOBEHAVIOUR_METHODS
        #endregion

        
        #region PUBLIC_METHODS
        public void UseItem()
        {
            EventManager.ActivatePlayersInventoryItem();
        }
        #endregion


        // EMPTY
        #region PRIVATE_METHODS

        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}