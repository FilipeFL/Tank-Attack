using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RTSHealthUI : MonoBehaviour
{
    [SerializeField] private RTSHealth health;

    [SerializeField] private GameObject healthBarContainer;
    [SerializeField] private Image healthBarImage;

    private void Awake()
    {
        health.ClientOnHealthUpdate += HandleOnHealthUpdate;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdate -= HandleOnHealthUpdate;
    }

    private void HandleOnHealthUpdate(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float) currentHealth / maxHealth;
    }

    private void OnMouseEnter()
    {
        healthBarContainer.SetActive(true);
    }

    private void OnMouseExit()
    {
        healthBarContainer.SetActive(false);
    }

}
