using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour, IDragHandler
{
    RectTransform m_transform = null;

    void Start()
    {
        m_transform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

	void Update ()
    {
		
	}
}
