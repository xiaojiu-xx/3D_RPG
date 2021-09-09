using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instacne;

    public static T Instance
    {
        get{ return instacne; }
    }

    protected virtual void Awake() 
    {
        if(instacne != null)
        {   
            Destroy(gameObject);
        }
        else
        {
            instacne = (T)this;
        }
    }

    public static bool IsInitialized
    {
        get{ return instacne != null;}
    }

    protected virtual void OnDestroy() 
    {
        if(instacne == this)
        {
            instacne = null;
        }
    }
}
