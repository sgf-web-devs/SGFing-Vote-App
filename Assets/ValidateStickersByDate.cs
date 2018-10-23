using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ValidateStickersByDate : MonoBehaviour
{
    [SerializeField]
    public Transform[] NotValidBefore;
    [SerializeField]
    public Transform[] NotValidAfter;

    //System.DateTime ValidationDate = new System.DateTime(2018, 11, 6);
    System.DateTime ValidationDate = new System.DateTime(2018, 10, 24); // Testing for weds morning
    List<string> _NVB = new List<string>();
    List<string> _NVA = new List<string>();

    void Awake()
    {
        // Polls open at 6, but we'll play it safe with 5AM.
        ValidationDate = ValidationDate.AddHours(5);
        ValidationDate = ValidationDate.AddMinutes(30);

        bool isAfter = (System.DateTime.Compare(System.DateTime.Now, ValidationDate) < 0);
        //bool isAfter = System.DateTime.Now >= ValidationDate;

        foreach (Transform t in NotValidBefore) { _NVB.Add(t.name); }
        foreach (Transform t in NotValidAfter) { _NVA.Add(t.name); }

        foreach (Transform t in this.GetComponentInChildren<Transform>())
        {
            if (_NVB.Contains(t.name))
                t.gameObject.SetActive(!isAfter);

            if (_NVA.Contains(t.name))
                t.gameObject.SetActive(isAfter);
        }
    }
}
