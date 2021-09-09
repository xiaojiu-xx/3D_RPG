using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 挂载在Enemy上
public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;

    public bool isAlwaysVisiable;

    public float visibleTime;
    private float timeLeft;

    // 当前血量的Slider
    Image healthSlider;
    // 获得血量条Transform
    Transform UIbar; 
    Transform cam;

    CharacterState currentStates;

    private void Awake() 
    {
        // 获取Enemy身上的CharacterState
        currentStates = GetComponent<CharacterState>();
        currentStates.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if(canvas.renderMode == RenderMode.WorldSpace ){
                // 在此处场景中只有一个RenderMode.WorldSpace 的canvas 不会出问题
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                // 是否可见
                UIbar.gameObject.SetActive(isAlwaysVisiable);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if(currentHealth <= 0)
        {
            Destroy(UIbar.gameObject);
        }

        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime;

        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    // 上一帧渲染结束后执行 避免出现跟随UI抖动情况
    private void LateUpdate() 
    {
        if(UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;

            if(timeLeft <= 0 && !isAlwaysVisiable)
                UIbar.gameObject.SetActive(false);
            else
                timeLeft -= Time.deltaTime;
        }
    }
}
