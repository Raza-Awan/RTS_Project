using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacementMode
{
    Fixed,
    Valid,
    Invalid
}

public class BuildingManager : MonoBehaviour
{
    public GameObject arrowsUI;
    public GameObject buildProgressUI;
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    public MeshRenderer[] meshComponents;
    private Dictionary<MeshRenderer, List<Material>> initialMaterials;

    public bool hasValidPlacement;
    public bool isFixed;
    public bool isBuilding;

    public Vector3 lastValidPos;

    private int _nObstacles;

    private void Awake()
    {
        hasValidPlacement = true;
        isFixed = true;
        _nObstacles = 0;

        _InitializeMaterials();
    }
    private void OnEnable()
    {
        isBuilding = true;
        buildProgressUI.SetActive(true);
    }

    private void Update()
    {
        if (CameraManager.instance._selectedBaseItem != null)
        {
            if (CameraManager.instance._selectedBaseItem.gameObject == this.gameObject)
            {
                arrowsUI.SetActive(true);
            }
            else
            {
                arrowsUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (isFixed) return;

        // ignore ground objects
        //if (_IsGround(other.gameObject)) return;

        if (other.gameObject.CompareTag("Building"))
        {
            _nObstacles++;
            SetPlacementMode(PlacementMode.Invalid);
            hasValidPlacement = false;
        }

        //_nObstacles++;
        //SetPlacementMode(PlacementMode.Invalid);
    }

    private void OnTriggerExit(Collider other)
    {
        //if (isFixed) return;

        // ignore ground objects
        //if (_IsGround(other.gameObject)) return;

        if (other.gameObject.CompareTag("Building"))
        {
            _nObstacles--;
            if (_nObstacles == 0)
                SetPlacementMode(PlacementMode.Fixed);
                hasValidPlacement = true;   
        }

        //_nObstacles--;
        //if (_nObstacles == 0)
        //    SetPlacementMode(PlacementMode.Valid);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _InitializeMaterials();
    }
#endif

    public void SetPlacementMode(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            isFixed = true;
            hasValidPlacement = true;
        }
        else if (mode == PlacementMode.Valid)
        {
            hasValidPlacement = true;
        }
        else
        {
            hasValidPlacement = false;
        }
        SetMaterial(mode);
    }

    public void SetMaterial(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            foreach (MeshRenderer r in meshComponents)
                r.sharedMaterials = initialMaterials[r].ToArray();
        }
        else
        {
            Material matToApply = mode == PlacementMode.Valid
                ? validPlacementMaterial : invalidPlacementMaterial;

            Material[] m; int nMaterials;
            foreach (MeshRenderer r in meshComponents)
            {
                nMaterials = initialMaterials[r].Count;
                m = new Material[nMaterials];
                for (int i = 0; i < nMaterials; i++)
                    m[i] = matToApply;
                r.sharedMaterials = m;
            }
        }
    }

    private void _InitializeMaterials()
    {
        if (initialMaterials == null)
            initialMaterials = new Dictionary<MeshRenderer, List<Material>>();
        if (initialMaterials.Count > 0)
        {
            foreach (var l in initialMaterials) l.Value.Clear();
            initialMaterials.Clear();
        }

        foreach (MeshRenderer r in meshComponents)
        {
            initialMaterials[r] = new List<Material>(r.sharedMaterials);
        }
    }

    private bool _IsGround(GameObject o)
    {
        return ((1 << o.layer) & BuildingPlacer.instance.groundLayerMask.value) != 0;
    }

    public void ResetToValidPosition()
    {
        if (hasValidPlacement == false)
        {
            transform.position = lastValidPos;
            isFixed = true;
            hasValidPlacement = true;
            SetPlacementMode(PlacementMode.Fixed);
        }
        if (hasValidPlacement == true)
        {
            isFixed = true;
            SetPlacementMode(PlacementMode.Fixed);
        }
    }
}
