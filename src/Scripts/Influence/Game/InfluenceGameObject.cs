//-----------------------------------------------------------------------------------------------
using UnityEngine;
using Influence;
using System.Collections.Generic;


//-----------------------------------------------------------------------------------------------
public class InfluenceGameObject : MonoBehaviour, IInfluenceObject
{
   //-----------------------------------------------------------------------------------------------
   public Dictionary<string, uint> InfluenceIDToTemplateSizeDictionary
   {
      get { return m_influenceIDToTemplateSizeDictionary; }
   }

   public string ObjectTag
   {
      get { return m_influenceObjectTag; }
   }

   public InfluenceObjectWorldPoint WorldPosition
   {
      get
      {
         Vector2 pos = transform.position;
         //Sprite sprite = GetComponent<SpriteRenderer>().sprite;
         //Vector2 pivot = sprite.pivot / sprite.pixelsPerUnit;
         //pos += pivot;
         return new InfluenceObjectWorldPoint(pos.x, pos.y);
      }
   }

   public float Rotation
   {
      get { return transform.rotation.eulerAngles.z; }
   }

   public string ThreatTag
   {
      get { return m_threatTag; }
   }


   //-----------------------------------------------------------------------------------------------
   [SerializeField]
   private string m_influenceObjectTag = "";

   [SerializeField]
   private string m_threatTag = "";

   // For Unity, since it can't serialize dictionaries
   [SerializeField]
   private List<string> m_influenceIDs = new List<string>();
   [SerializeField]
   public List<uint> m_templateSizes = new List<uint>();

   private Dictionary<string, uint> m_influenceIDToTemplateSizeDictionary = new Dictionary<string, uint>();

   private InfluenceGameManager m_influenceManager;

   
   //-----------------------------------------------------------------------------------------------
   public void Awake()
   {
      BuildTemplateSizeDictionary();
   }


   //-----------------------------------------------------------------------------------------------
   private void BuildTemplateSizeDictionary()
   {
      m_influenceIDToTemplateSizeDictionary.Clear();

      if (m_influenceIDs.Count != m_templateSizes.Count)
      {
         throw new System.Exception("Game object does not have equal number of influence ids and template sizes");
      }

      for (int templateIndex = 0; templateIndex < m_templateSizes.Count; ++templateIndex)
      {
         m_influenceIDToTemplateSizeDictionary.Add(m_influenceIDs[templateIndex].ToLower(), m_templateSizes[templateIndex]);
      }
   }

      
   //-----------------------------------------------------------------------------------------------
   public void OnEnable()
   {
      GetInfluenceGameManagerReferenceIfNeeded();

      if (m_influenceManager != null)
      {
         m_influenceManager.RegisterInfluenceObject(this);
      }
   }


   //-----------------------------------------------------------------------------------------------
   public void OnDisable()
   {
      if (m_influenceManager != null)
      {
         m_influenceManager.UnregisterInfluenceObject(this);
      }
   }


   //-----------------------------------------------------------------------------------------------
   private void GetInfluenceGameManagerReferenceIfNeeded()
   {
      if (m_influenceManager == null)
      {
         GameObject influenceGameManagerObj = GameObject.FindWithTag("InfluenceManager");
         if (influenceGameManagerObj != null)
         {
            m_influenceManager = influenceGameManagerObj.GetComponent<InfluenceGameManager>();
         }
         else
         {
            m_influenceManager = null;
         }
      }
   }


   //-----------------------------------------------------------------------------------------------
   public uint GetTemplateSizeForInfluenceType(string influenceID)
   {
      uint templateSize = 0U;
      m_influenceIDToTemplateSizeDictionary.TryGetValue(influenceID.ToLower(), out templateSize);

      return templateSize;
   }
}
