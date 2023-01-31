using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool jouerContreIA = false;

    [SerializeField]
    GameObject red, green;

    bool isPlayer, hasGameFinished;

    [SerializeField]
    Text TXT_Tour;

    const string RED_MESSAGE = "Au tour du Rouge";
    const string GREEN_MESSAGE = "Au tour du jaune";

    Color RED_COLOR = new Color(214, 33, 33, 255) / 255;
    Color GREEN_COLOR = new Color(238, 228, 41, 255) / 255;

    Plateau myBoard;

    private void Awake()
    {
        isPlayer = true;
        hasGameFinished = false;
        //TXT_Tour.text = RED_MESSAGE;
        //TXT_Tour.color = RED_COLOR;
        myBoard = new Plateau();
    }

    public void GameStart()
    {
        SceneManager.LoadScene("Menu");
    }

    public void JoueurVsJoueur()
    {
        jouerContreIA = false;
        SceneManager.LoadScene("Game");
    }

    public void JoueurVsIA()
    {
        jouerContreIA = true;
        SceneManager.LoadScene("Game");
    }

    public void Quitter()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
        Debug.Log("quitter");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //If GameFinsished then return
            if (hasGameFinished) return;

            //Raycast2D
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (!hit.collider) return;

            if (hit.collider.CompareTag("appui"))
            {
                //Check out of Bounds
                if (hit.collider.gameObject.GetComponent<Colonne>().targetlocation.y > 350f) return;

                //Spawn the GameObject
                Vector3 spawnPos = hit.collider.gameObject.GetComponent<Colonne>().spawnLocation;
                Vector3 targetPos = hit.collider.gameObject.GetComponent<Colonne>().targetlocation;
                GameObject circle = Instantiate(isPlayer ? red : green);
                circle.transform.position = spawnPos;
                circle.GetComponent<Mouvement>().targetPostion = targetPos;

                //Increase the targetLocationHeight
                hit.collider.gameObject.GetComponent<Colonne>().targetlocation = new Vector3(targetPos.x, targetPos.y + 54f, targetPos.z);

                //UpdateBoard
                myBoard.UpdateBoard(hit.collider.gameObject.GetComponent<Colonne>().col - 1, isPlayer);
                if (myBoard.Result(isPlayer))
                {
                    TXT_Tour.text = (isPlayer ? "Red" : "Green") + " Wins!";
                    hasGameFinished = true;
                    return;
                }

                //TurnMessage
                TXT_Tour.text = !isPlayer ? RED_MESSAGE : GREEN_MESSAGE;
                TXT_Tour.color = !isPlayer ? RED_COLOR : GREEN_COLOR;

                //Change PlayerTurn
                isPlayer = !isPlayer;
            }

        }
    }
}
