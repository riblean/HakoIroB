using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLoader : MonoBehaviour
{
    [SerializeField] string ModeName;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);

        // GetComponent<Text>().text = ScoreManager.Instance.LoadScore(ModeName).Score.ToString();
    }
}
