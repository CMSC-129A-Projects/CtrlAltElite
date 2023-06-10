using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance { get; private set; }
    [SerializeField] private GameObject deathTransition;
    [SerializeField] private GameObject menuTransition;
    [SerializeField] private GameObject city1Transition;
    [SerializeField] private GameObject city2Transition;
    [SerializeField] private GameObject city3Transition;
    [SerializeField] private GameObject city4Transition;
    [SerializeField] private GameObject city5Transition;
    [SerializeField] private float loadSceneTime;
    [SerializeField] private float loadSceneTimeAfter;
    private Animator deathAnim, menuAnim, city1Anim, city2Anim, city3Anim, city4Anim, city5Anim;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one TransitionManager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        deathAnim = deathTransition.GetComponent<Animator>();
        menuAnim = menuTransition.GetComponent<Animator>();
        city1Anim = city1Transition.GetComponent<Animator>();
        city2Anim = city2Transition.GetComponent<Animator>();
        city3Anim = city3Transition.GetComponent<Animator>();
        city4Anim = city4Transition.GetComponent<Animator>();
        city5Anim = city5Transition.GetComponent<Animator>();
    }

    public void NextScene(int scene)
    {
        // 3 = city 1 
        // 4 = city 2
        // 5 = city 3
        // 6 = city 4
        // 7 = city 5
        if (scene == 3)
        {
            city1Transition.GetComponent<CityTransition>().ActivateTransition();
            city1Anim.SetTrigger("end");
        }
        else if (scene == 4)
        {
            city2Transition.GetComponent<CityTransition>().ActivateTransition();
            city2Anim.SetTrigger("end");
        }
        else if (scene == 5)
        {
            city3Transition.GetComponent<CityTransition>().ActivateTransition();
            city3Anim.SetTrigger("end");
        }
        else if (scene == 6)
        {
            city4Transition.GetComponent<CityTransition>().ActivateTransition();
            city4Anim.SetTrigger("end");
        }
        else if (scene == 7)
        {
            city5Transition.GetComponent<CityTransition>().ActivateTransition();
            city5Anim.SetTrigger("end");
        }

        // StartCoroutine(SwitchScene(scene));
        
    }

    private void ReverseTransition(int scene)
    {
        if (scene == 3)
        {
            city1Transition.GetComponent<CityTransition>().AllowMovePlayer();
            city1Anim.SetTrigger("start");
        }

        else if (scene == 4)
        {
            city2Transition.GetComponent<CityTransition>().AllowMovePlayer();
            city2Anim.SetTrigger("start");
        }
        else if (scene == 5)
        {
            city3Transition.GetComponent<CityTransition>().AllowMovePlayer();
            city3Anim.SetTrigger("start");
        }
        else if (scene == 6)
        {
            city4Transition.GetComponent<CityTransition>().AllowMovePlayer();
            city4Anim.SetTrigger("start");
        }
        else if (scene == 7)
        {
            city5Transition.GetComponent<CityTransition>().AllowMovePlayer();
            city5Anim.SetTrigger("start");
        }


    }
    public IEnumerator SwitchScene(int scene)
    {
        Debug.Log($"Switching to Scene {scene}");
        yield return new WaitForSeconds(loadSceneTime);
        SceneManager.LoadSceneAsync(scene);
        yield return new WaitForSeconds(loadSceneTimeAfter);
        ReverseTransition(scene);
    }

    public void PlayDeathTransition()
    {
        deathTransition.gameObject.SetActive(true);
        Debug.Log("PlayDeathTransition");
        deathTransition.GetComponent<DeathTransition>().ActivateDeathTransition();

        deathAnim.SetTrigger("Died");
    }

    public void PlayRespawnTransition()
    {
        Debug.Log("PlayRespawnTransition");
        deathAnim.SetTrigger("Respawn");
    }

}
