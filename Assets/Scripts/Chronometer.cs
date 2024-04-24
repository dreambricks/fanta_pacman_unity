using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chronometer : MonoBehaviour
{

    private float elapsedTime;
    public Text timeText;
    [SerializeField] private ScreenManager screenManager;

    private void Update()
    {
        ShowTime();
    }


    void ShowTime()
    {
        elapsedTime = screenManager.elapsedTime;
        int minutos = Mathf.FloorToInt(elapsedTime / 60);
        int segundos = Mathf.FloorToInt(elapsedTime % 60);


        timeText.text = minutos.ToString("00") + ":" + segundos.ToString("00");
    }
}