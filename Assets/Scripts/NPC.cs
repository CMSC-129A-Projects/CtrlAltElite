using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Image npcImage;
    public List<string> dialogue = new List<string>();
    private int index = 0;
    private int lastIndex = 0;

    public GameObject contButton;
    public float wordSpeed;
    public bool playerIsClose;
    public bool isTyping;
    public GameObject popUp;

    public TextMeshProUGUI interactText;
    private Coroutine typingCoroutine;

    private Animator animator;

    void Start()
    {
        dialogueText.text = "";
        animator = GetComponent<Animator>();
        //interactText.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            isTyping = true;
            if (!dialoguePanel.activeInHierarchy)
            {
                
                dialoguePanel.SetActive(true);
                typingCoroutine = StartCoroutine(Typing());
                // popUp.SetActive(false);
                //interactText.text = "";
            }
            else if (dialogueText.text == dialogue[index])
            {
                NextLine();
            }
            lastIndex = index;
        }

        if (Input.GetKeyDown(KeyCode.Q) && dialoguePanel.activeInHierarchy)
        {
            StopCoroutine(typingCoroutine);
            RemoveText();
        }

        if (dialogueText.text == dialogue[index])
        {
            contButton.SetActive(true);
        }
    }

    public void RemoveText()
    {
        dialogueText.text = "";
        index = lastIndex;
        dialoguePanel.SetActive(false);
        isTyping = false;
        //interactText.text = "Press E";
    }

    IEnumerator Typing()
    {
        dialogueText.text = "";
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        contButton.SetActive(false);

        if (index < dialogue.Count - 1)
        {
            index++;
            typingCoroutine = StartCoroutine(Typing());
        }
        else
        {
            RemoveText();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 playerPosition = collision.transform.position;
            animator.SetTrigger("facePlayer");

            // Compare the position with the NPC's position
            if (playerPosition.x < transform.position.x)
            {
                // Colliding from the left
                Debug.Log("Colliding from the left");
                transform.localScale = new Vector3(1, 1, 1);
                popUp.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (playerPosition.x > transform.position.x)
            {
                // Colliding from the right
                Debug.Log("Colliding from the right");
                transform.localScale = new Vector3(-1, 1, 1);
                popUp.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTyping)
        {
            popUp.SetActive(true);
            playerIsClose = true;

        }

        if (other.CompareTag("Player") && isTyping)
        {
            popUp.SetActive(false);
            playerIsClose = true;
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTyping = false;
            popUp.SetActive(false);
            playerIsClose = false;
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            RemoveText();
            animator.SetTrigger("idle");
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

}
