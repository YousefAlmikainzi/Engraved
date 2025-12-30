using UnityEngine;
using UnityEngine.UI;

public class ScoreCount : MonoBehaviour
{
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = "Score: " + ScoreManager.Instance.Score;
    }
}
