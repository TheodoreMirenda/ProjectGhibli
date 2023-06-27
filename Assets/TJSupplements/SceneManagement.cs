using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TJ.Supplements
{


public class SceneManagement : MonoBehaviour
{
    public Image img;
	public AnimationCurve curve;
    public string startGameSceneName;
    public string mainMenuSceneName;
    public GameObject settingsVolume;
    public GameObject settingsCanvas;

    public void OpenUI()
    {
        //Time.timeScale = 0;
        //settingsVolume.SetActive(true);
        //settingsCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void UnpauseGame()
    {
        //Time.timeScale = 1;
        //settingsVolume.SetActive(false);
        //settingsCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Awake()
    {
        if(img!=null)
        {
            img.enabled = true;
            StartCoroutine(FadeIn());
        }
    }
    public void StartGame()
    {
        Time.timeScale = 1;
        Debug.Log("starting game");
        StartCoroutine(FadeOut(startGameSceneName));
    }
    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        Debug.Log("returning to menu");
        StartCoroutine(FadeOut(mainMenuSceneName));
    }
    public void QuitGame()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public IEnumerator FadeOut(string scene)
	{
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime;
			float a = curve.Evaluate(t);
			img.color = new Color(0f, 0f, 0f, a);
			yield return 0;
		}
		SceneManager.LoadScene(scene);
	}
    IEnumerator FadeIn ()
	{
		float t = 1f;

		while (t > 0f)
		{
			t -= Time.deltaTime;
			float a = curve.Evaluate(t);
			img.color = new Color (0f, 0f, 0f, a);
			yield return 0;
		}
	}
}
}
