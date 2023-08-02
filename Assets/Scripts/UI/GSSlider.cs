using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Game speed Slider Script
public class GSSlider : MonoBehaviour
{
    private Slider _slider;

    public TextMeshProUGUI textMeshProUGUI;
    // Start is called before the first frame update
    void Start()
    {
        _slider = gameObject.GetComponent<Slider>();
        _slider.onValueChanged.AddListener(delegate { ValueChanged(); } );
        
    }

    public void ValueChanged()
    {
        textMeshProUGUI.text = ("Speed: " + _slider.value +" Current speed: " + Time.timeScale);
    }
}
