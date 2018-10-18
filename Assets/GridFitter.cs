using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridFitter : MonoBehaviour {

    private float width;
    public float columns;

    // Use this for initialization
    void Start () {
        var spacingAmount = this.gameObject.GetComponent<GridLayoutGroup>().spacing.x;

        width = this.gameObject.GetComponent<RectTransform>().rect.width;

        var cellSize = (width / columns) - (spacingAmount);

        Vector2 newSize = new Vector2(cellSize, cellSize);
        this.gameObject.GetComponent<GridLayoutGroup>().cellSize = newSize;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
