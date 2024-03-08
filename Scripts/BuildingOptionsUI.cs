using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingOptionsUI : MonoBehaviour
{
    public static BuildingOptionsUI instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject infoButton;
    private Animator infoAnimator;

    public GameObject upgradeButton;
    private Animator upgradeAnimator;

    public GameObject trainButton;
    private Animator trainAnimator;

    public GameObject removeButton;
    private Animator removeAnimator;

    private const string SHOW = "Show";
    private const string HIDE = "Hide";

    private void Start()
    {
        infoAnimator = infoButton.GetComponent<Animator>();
        upgradeAnimator = upgradeButton.GetComponent<Animator>();
        trainAnimator = trainButton.GetComponent<Animator>();
        removeAnimator = removeButton.GetComponent<Animator>();
    }

    /// <summary>
    /// Info UI
    /// </summary>
    public void ShowInfoUI()
    {
        infoAnimator.Play(SHOW);
    }
    public void HideInfoUI() 
    {
        infoAnimator.Play(HIDE);
    }
    public void OnClickInfoUI()
    {

    }

    /// <summary>
    /// Upgrade UI
    /// </summary>
    public void ShowUpgradeUI()
    {
        upgradeAnimator.Play(SHOW);
    }
    public void HideUpgradeUI()
    {
        upgradeAnimator.Play(HIDE);
    }
    public void OnClickUpgradeUI()
    {

    }

    /// <summary>
    /// Train UI
    /// </summary>
    public void ShowTrainUI()
    {
        trainAnimator.Play(SHOW);
    }
    public void HideTrainUI()
    {
        trainAnimator.Play(HIDE);
    }
    public void OnClickTrainUI()
    {

    }

    /// <summary>
    /// Remove UI
    /// </summary>
    public void ShowRemoveUI()
    {
        removeAnimator.Play(SHOW);
    }
    public void HideRemoveUI()
    {
        removeAnimator.Play(HIDE);
    }
    public void OnClickRemoveUI()
    {
        if (CameraManager.instance._selectedBaseItem.gameObject != null)
        {
            Destroy(CameraManager.instance._selectedBaseItem.gameObject);
            HideInfoUI();
            HideUpgradeUI();
            HideTrainUI();
            HideRemoveUI();
        }
    }
}
