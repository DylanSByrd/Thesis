using UnityEngine;
using System.Collections.Generic;
using System;

public class TestComponent : MonoBehaviour
{
    void Start()
    {

        Instantiator<Behavior>.ListAll();

        Behavior[] behaviors = Instantiator<Behavior>.CreateOneOfEachDerivedClass();
        foreach (Behavior behavior in behaviors)
        {
            behavior.Print();
        }

    }
}

public abstract class Behavior
{
    public abstract void Print();
}

public class SomeBehavior : Behavior
{
    public override void Print()
    {
        Debug.Log("Hi: SomeBehavior");
    }
}

public class SomeOtherBehavior : Behavior
{

    private int v;
    public SomeOtherBehavior()
    {
        v = 0;
    }

    public SomeOtherBehavior(int _v)
    {
        v = _v;
    }

    public override void Print()
    {
        Debug.Log("Hi: SomeOtherBehavior(" + v + ")");
    }
}

