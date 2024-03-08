using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    /* object references */
    public Transform ProgressContainer;

    public SpriteRenderer ProgressFiller;

    /* private vars */
    private float _fillerFullLength;

    void Awake()
    {
        this._fillerFullLength = this.ProgressFiller.size.x;
    }

    private Vector2 _tempSize;
    public void SetProgress(float progress)
    {
        _tempSize.x = progress * this._fillerFullLength;
        _tempSize.y = this.ProgressFiller.size.y;
        this.ProgressFiller.size = _tempSize;

        if (progress >= 0.8f)
        {
            this.SetFillerColor(Color.green);
        }
        else if (progress >= 0.5f)
        {
            this.SetFillerColor(Color.yellow);
        }
        else
        {
            this.SetFillerColor(Color.red);
        }
    }

    public void SetFillerColor(Color color)
    {
        this.ProgressFiller.color = color;
    }
}
