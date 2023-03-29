using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour {

    float introLength = 12f;
    float outroLength = 10f;

    private void Start() {
        Scene currentScene = SceneManager.GetActiveScene();

        if(currentScene.name == "Intro") {
            StartCoroutine(GoToGameScene());
        }
        if (currentScene.name == "Outro") {
            StartCoroutine(GoToTitle());
        }

    }

    public void LoadGame() {
        SceneManager.LoadScene("Game");
    }
    public void LoadIntro() {
        SceneManager.LoadScene("Intro");
    }

    IEnumerator GoToGameScene() {
        yield return new WaitForSeconds(introLength);
        SceneManager.LoadScene("Game");
    }

    IEnumerator GoToTitle() {
        yield return new WaitForSeconds(outroLength);
        SceneManager.LoadScene("Title");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Game") {
            SceneManager.LoadScene("Outro");
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Game") {
            SceneManager.LoadScene("Outro");
        }
    }


}
