using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using UnityEngine;


//-----------------------------------------------------------------------------------------------
public class Instantiator<T_InstantiatedType> where T_InstantiatedType : class
{
    //-----------------------------------------------------------------------------------------------
    static private Dictionary<string, System.Type> s_typeRegistrations;
    
    
    //-----------------------------------------------------------------------------------------------
    static public T_InstantiatedType CreateByName(string name, params object[] constructorArgs)
    {
        if (s_typeRegistrations == null)
        {
            FetchAllDerivedClasses(typeof(T_InstantiatedType));
        }

        System.Type type;
        if (s_typeRegistrations.TryGetValue(name, out type))
        {
            try
            {
                return (T_InstantiatedType)Activator.CreateInstance(type, constructorArgs);
            }
            catch
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }


    //-----------------------------------------------------------------------------------------------
    // Assumes a constructor taking name arguments for all derived classes
    static public T_InstantiatedType[] CreateOneOfEachDerivedClass()
    {
        if (s_typeRegistrations == null)
        {
            FetchAllDerivedClasses(typeof(T_InstantiatedType));
        }

        T_InstantiatedType[] instatiatedRegistrations = new T_InstantiatedType[s_typeRegistrations.Count];
        
        int registrationIndex = 0;        
        foreach (var registration in s_typeRegistrations)
        {
            T_InstantiatedType instantiation = (T_InstantiatedType)Activator.CreateInstance(registration.Value);
            instatiatedRegistrations[registrationIndex] = instantiation;
            ++registrationIndex;
        }

        return instatiatedRegistrations;
    }


    //-----------------------------------------------------------------------------------------------
    static public int GetNumberOfDerivedClasses()
    {
        return s_typeRegistrations.Count;
    }
    
    
    //-----------------------------------------------------------------------------------------------
    static private void FetchAllDerivedClasses(System.Type baseType)
    {
        if (s_typeRegistrations == null)
        {
            s_typeRegistrations = new Dictionary<string, System.Type>();
        }
        else
        {
            s_typeRegistrations.Clear();
        }

        var types = Assembly.GetAssembly(baseType).GetTypes();
        var filter = types.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseType));

        foreach (var type in filter)
        {
            s_typeRegistrations.Add(type.Name, type);
        }
    }


    //-----------------------------------------------------------------------------------------------
    public static void ListAll()
    {
        if (s_typeRegistrations == null)
        {
            FetchAllDerivedClasses(typeof(T_InstantiatedType));
        }

        foreach (var registration in s_typeRegistrations)
        {
            Debug.Log("Registered [" + registration.Key + "]");
        }
    }
}
