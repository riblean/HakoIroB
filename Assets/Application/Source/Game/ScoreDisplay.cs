using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Text text;

    public int Value
    {
        set
        {
            text.text = value.ToString();
            gameObject.SetActive(true);
        }
    }
}
