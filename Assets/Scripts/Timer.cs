using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Image timeBar;
    [SerializeField]
    private
    float maxTime = 5.0f;
    float leftTime = 0;

    public void StartTimer(Vector3 vector3)
    {
        leftTime = maxTime;
        timeBar.transform.position = vector3;
    }

    private void Start()
    {
        timeBar = GetComponent<Image>();
        StopTimer();
    }

    public void StopTimer()
    {
        leftTime = 0;
        timeBar.fillAmount = 0;
    }

    void Update()
    {
        if(leftTime > 0)
        {
            leftTime -= Time.deltaTime;
            timeBar.fillAmount = leftTime / maxTime;
        }
    }
}
