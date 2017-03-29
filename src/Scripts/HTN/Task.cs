//-----------------------------------------------------------------------------------------------
namespace HTN
{
   //-----------------------------------------------------------------------------------------------
   public abstract class Task
   {
      //-----------------------------------------------------------------------------------------------
      public enum eType : byte
      {
         INVALID_TASK,
         PRIMITIVE_TASK,
         COMPOUND_TASK,
         NUM_TASK_TYPES,
      }


      //-----------------------------------------------------------------------------------------------
      public enum eModifier : byte
      {
         BLOCKING_MODIFIER = (1 << 0),
         SYNC_MODIFIER = (1 << 1),
         RESERVABLE_MODIFIER = (1 << 2),
         NUM_TASK_MODIFIERS,
      }


      //-----------------------------------------------------------------------------------------------
      public string Name
      {
         get { return m_name; }
      }

      public eType Type
      {
         get { return m_type; }
      }

      public uint ModifierMask
      {
         get { return m_modifierMask; }
      }


      //-----------------------------------------------------------------------------------------------
      protected string m_name;
      protected eType m_type;
      protected uint m_modifierMask;


      //-----------------------------------------------------------------------------------------------
      public Task(string name, eType type, params eModifier[] modifiers)
      {
         m_name = name;
         m_type = type;

         foreach(byte mod in modifiers)
         {
            m_modifierMask |= mod;
         }
      }


      //-----------------------------------------------------------------------------------------------
      public Task(string name, eType type, uint modifierMask)
      {
         m_name = name;
         m_type = type;
         m_modifierMask = modifierMask;
      }


      //-----------------------------------------------------------------------------------------------
      public void AddModifier(eModifier mod)
      {
         m_modifierMask |= (byte)mod;
      }


      //-----------------------------------------------------------------------------------------------
      public void CombineModifierMasks(uint modifierMask)
      {
         m_modifierMask |= modifierMask;
      }


      //-----------------------------------------------------------------------------------------------
      public abstract Task Clone();
   }
}