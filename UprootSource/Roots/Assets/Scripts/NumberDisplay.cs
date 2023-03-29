using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberDisplay : MonoBehaviour {
    [SerializeField] public int display;
    int prevDisplay;
    public float spacing = 0.5f;

    [SerializeField] List<Sprite> numbers;

    List<GameObject> digits;
    [SerializeField] GameObject digitTemplate;

    private void Awake() {
        digits = new List<GameObject>();
        CalculateDigits();
        FormatDigits();
    }

    private void Update() {
        if(prevDisplay != display) {
            CalculateDigits();
            FormatDigits();
        }
        prevDisplay = display;
    }

    private void CalculateDigits() {
        foreach (var obj in digits) {
            Destroy(obj);
        }
        digits.Clear();
        int num = display;
        int n;
        if (num == 0 || num == 1) n = 1;
        else if (num == 10) n = 2;
        else if (num == 100) n = 3;
        else {
            n = (int) Mathf.Ceil(Mathf.Log10(num));
        }

        for (int i = 0; i < n; i++) {

            int digit =  (int)Mathf.Floor(num / Mathf.Pow(10, i) % 10);

            AddDigit(digit);
        }

    }
    private void AddDigit(int digit) {
        GameObject digitObj = Instantiate(digitTemplate, transform.position, Quaternion.identity, this.transform);
        digitObj.GetComponent<SpriteRenderer>().sprite = numbers[digit];
        digits.Add(digitObj);
    }

    private void FormatDigits() {
        int length = digits.Count;
        float offset = 0;
        if (length == 2) offset = spacing/2;

        foreach (var digitObj in digits) {
            digitObj.transform.position += new Vector3(offset, 0, 0);
            offset -= spacing;
        }
    }
}
