using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NextScene : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        GameState.LastAction = "NextDay";
        SceneManager.LoadScene(sceneName);
    }
}
