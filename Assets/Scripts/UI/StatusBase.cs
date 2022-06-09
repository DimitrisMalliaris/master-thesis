using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBase : MonoBehaviour
{
    public string StatusTag;
    public Image current;
    [SerializeField] Slider pointOfNeed;
    [SerializeField] Slider pointOfSatisfaction;

    public float Current { set => current.fillAmount = value; }
    public float PointOfNeed { set => pointOfNeed.value = value; }
    public float PointOfSatisfaction { set => pointOfSatisfaction.value = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
