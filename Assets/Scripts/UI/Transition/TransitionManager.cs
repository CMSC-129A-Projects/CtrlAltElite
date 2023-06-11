using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance { get; private set; }
    [SerializeField] private GameObject deathTransition;
    private Animator deathAnim;

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
