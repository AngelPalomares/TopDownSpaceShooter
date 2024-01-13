using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    public static UICanvas instance;
    public Text HighScore;
    public Text Health;

    public Image HealthSlider;

    public float High;

    private void Awake()
    {
        instance = this;
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
