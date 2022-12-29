using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            GameManager.Instance.OnButtonPressed("RETURN");
        if (Input.GetKeyDown(KeyCode.Tab))
            GameManager.Instance.OnButtonPressed("TAB");
        if (Input.GetKeyDown(KeyCode.J))
            GameManager.Instance.OnButtonPressed("J");
        if (Input.GetKeyDown(KeyCode.K))
            GameManager.Instance.OnButtonPressed("K");
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.OnButtonPressed("ESCAPE");
    }
}