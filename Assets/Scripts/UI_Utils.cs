using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class UI_Utils
{
    public static GameObject NewListItem(GameObject _itemprefab, Transform _container)
    {
        GameObject newItem = GameObject.Instantiate(_itemprefab) as GameObject;

        newItem.transform.SetParent(_container);

        RectTransform _rt = newItem.GetComponent<RectTransform>();
        _rt.anchoredPosition3D = Vector3.zero;
        _rt.transform.localEulerAngles = Vector3.zero;
        _rt.localScale = Vector3.one;

        newItem.SetActive(true);
        return newItem;
    }

    public static bool IsPointerOverUIObject {
        get {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    } 
}