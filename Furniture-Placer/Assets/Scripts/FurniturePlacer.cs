using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FurniturePlacer : MonoBehaviour
{
    public Transform placementIndicator;
    public GameObject selectionUI;

    private List<GameObject> furniture = new List<GameObject>();
    private GameObject curSelected;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        selectionUI.SetActive(false);
    }

    private void Update()
    {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
        {
            Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject != null && furniture.Contains(hit.collider.gameObject))
                {
                    if(curSelected != null && hit.collider.gameObject != curSelected)
                    {
                        Select(hit.collider.gameObject);
                    }else if(curSelected == null)
                    {
                        Select(hit.collider.gameObject);
                    }
                }else
                {
                    Deselect();
                }
            }
        }

        if (curSelected != null && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved)
        {
            MoveSelected();
        }
    }
    void MoveSelected()
    {
        Vector3 curPos = cam.ScreenToViewportPoint(Input.touches[0].position);
        Vector3 lastPos = cam.ScreenToViewportPoint(Input.touches[0].position - Input.touches[0].deltaPosition);

        Vector3 touchDir = curPos - lastPos;

        Vector3 camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        curSelected.transform.position += (camRight * touchDir.x + camForward * touchDir.y);
    }
    void Select (GameObject selected)
    {
        if(curSelected != null)
        {
            ToggleSelectionVisul(curSelected, false);
        }
        curSelected = selected;
        ToggleSelectionVisul(curSelected, true);
        selectionUI.SetActive(true);
    }
    void Deselect()
    {
        if (curSelected != null)
        {
            ToggleSelectionVisul(curSelected, false);
        }
        curSelected = null;
        selectionUI.SetActive(false);
    }
    void ToggleSelectionVisul(GameObject obj, bool toggle)
    {
        obj.transform.Find("Selected").gameObject.SetActive(toggle);
    }
    public void PlaceFurniture (GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, placementIndicator.position, Quaternion.identity);
        furniture.Add(obj);
        Select(obj);
    }

    public void ScaleSelected (float rate)
    {
        curSelected.transform.localScale += Vector3.one * rate;
    }

    public void RotateSelected (float rate)
    {
        curSelected.transform.eulerAngles += Vector3.up * rate;
    }

    public void SetColor(Image buttonImage)
    {
        MeshRenderer[] meshRenderers = curSelected.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mr in meshRenderers)
        {
            if (mr.gameObject.name == "Selected")
                continue;
            mr.material.color = buttonImage.color;
        }
    }

    public void DeleteSelected()
    {
        furniture.Remove(curSelected);
        Destroy(curSelected);
        Deselect();
    }
}
