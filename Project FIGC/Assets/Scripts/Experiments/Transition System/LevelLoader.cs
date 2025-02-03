using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public Animator transition; // Animator a utilizar

    public float transitionTime = 1f; //Variable del tiempo de la transicion

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetMouseButtonDown(0)) //Trigger
        {
            LoadNextLevel(); 
        }*/
    }
    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));  //Argumento que se le pasa a la funcion
    }

    IEnumerator LoadLevel(int levelIndex) 
    {
        //Play Animation
        transition.SetTrigger("Start");
        //Wait
        yield return new WaitForSeconds(transitionTime);
        //Load scene
        SceneManager.LoadScene(levelIndex);
    }
}
