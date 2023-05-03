using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelText : MonoBehaviour
{
    private TextMeshProUGUI _levelText;
    private MyPlayerPrefs.PlayerStats _instance;
    // Start is called before the first frame update
    void Start()
    {
        _levelText = gameObject.GetComponent<TextMeshProUGUI>();
        _instance = MyPlayerPrefs.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        _levelText.text = "Level: " + _instance.GetLevel();
    }
}
