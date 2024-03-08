using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTimeProgressUI : MonoBehaviour
{
    public SpriteRenderer ProgressFiller;
    public TextMesh TimerLabel;
    public TextMesh TimerLabelShadow;

    private BuildingManager _baseItem;

    private Vector2 _tempSize;

    public float _buildTime;
    private float _buildStartTime;
    private float _fillerFullLength;

    // Start is called before the first frame update
    void Start()
    {
        _baseItem = GetComponentInParent<BuildingManager>();
        Init();
        //_buildTime = _baseItem.buildTime;
    }

    public void Init()
    {
        _fillerFullLength = ProgressFiller.size.x;
        _buildStartTime = Time.time;
        _buildTime = BuildingSpawner.Instance.buildTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (_baseItem.isBuilding == true)
        {
            //UpdateProgress();
        }
        UpdateProgress();
    }

    private void UpdateProgress()
    {

        if (_buildTime <= 0)
        {
            OnFinishBuild();
            return;
        }

        float elapsedTime = Time.time - _buildStartTime;
        float progress = elapsedTime / _buildTime;

        _tempSize.x = progress * _fillerFullLength;
        _tempSize.y = ProgressFiller.size.y;
        ProgressFiller.size = _tempSize;

        int timeToFinish = (int)(_buildTime - elapsedTime);
        TimerLabel.text = timeToFinish.ToString();
        TimerLabelShadow.text = timeToFinish.ToString();

        if (progress >= 1)
        {
            OnFinishBuild();
        }
    }

    private void OnFinishBuild()
    {
        _baseItem.isBuilding = false;
        _baseItem.buildProgressUI.SetActive(false);
    }
}
