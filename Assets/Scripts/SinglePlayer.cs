using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer : MonoBehaviour
{
    public static SinglePlayer instance;

    public bool Singleplayer = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(instance);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
