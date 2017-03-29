/*
   This file contains the abstract class for the influence game manager. The influence
   game manager is used for games to interact with the external influence library. It should
   be used to call functions in the influence system.
*/


//-----------------------------------------------------------------------------------------------
namespace Influence
{
   //-----------------------------------------------------------------------------------------------
   public interface IInfluenceGameManager
   {
      //-----------------------------------------------------------------------------------------------
      // Wrapper to initialize influence system. Good to use to populate the system with external data
      // Should be called once
      void InitializeInfluenceSystem();
      

      //-----------------------------------------------------------------------------------------------
      // These methods should register/unregister influence objects to/from the influence system
      void RegisterInfluenceObject(IInfluenceObject influenceObject);
      void UnregisterInfluenceObject(IInfluenceObject influenceObject);


      //-----------------------------------------------------------------------------------------------
      // Wrapper for updating influence system
      void UpdateInfluenceSystem();
   }
}
