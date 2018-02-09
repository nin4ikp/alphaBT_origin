using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneOnScript : MonoBehaviour {

    public Image black;
    public Animator anim;
    public GameObject audio;
    public AudioSource audioSource;

    public void LoadByIndex (int sceneIndex)
    {
        audioSource = audio.GetComponent<AudioSource>();
        StartCoroutine(FadeImage(sceneIndex));
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator FadeImage(int sceneIndex)
    {
        audioSource.Play();
        anim.SetBool("fade", true);
        //float fadeTime = GameObject.Find("Start_Button").GetComponent<Fading>().BeginFade(1);
        yield return new WaitUntil(() => black.color.a == 1);
        anim.SetBool("fade", false);
        //SceneManager.LoadScene(sceneIndex);
    }
}
