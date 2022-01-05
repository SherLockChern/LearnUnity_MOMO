using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestLoopScrollView : MonoBehaviour {

    public LoopScrollView loopScrollView;
    public LoopScrollView loopScrollView2;
    public GameObject prefabGo;
    public GameObject prefabGo2;
    private void Start ()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            loopScrollView.Show(7, prefabGo, UpdateSV);
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            loopScrollView.ResetSize(5);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            loopScrollView.ResetSize(50);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            loopScrollView.UpdateList();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            loopScrollView2.Show(20, prefabGo2, UpdateSV);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            loopScrollView2.ResetSize(5);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            loopScrollView2.ResetSize(50);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            loopScrollView.UpdateList();
        }
    }

    private void UpdateSV(GameObject go, int index)
    {
        Text text = go.transform.Find("Text").GetComponent<Text>();
        text.text = index.ToString();
    }
}