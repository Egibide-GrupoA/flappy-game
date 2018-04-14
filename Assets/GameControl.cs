using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class GameControl : MonoBehaviour {

    public static GameControl instance;         //A reference to our game control script so we can access it statically.
    public Text Puntuacion;                      //A reference to the UI text component that displays the player's score.
    public GameObject FinJuego;             //A reference to the object that displays the text which appears when the player dies.

    private int score = 0;                      //The player's score.
    public bool gameOver = false;               //Is the game over?
    public float scrollSpeed = -1.5f;


    void Awake()
    {
        //If we don't currently have a game control...
        if (instance == null)
            //...set this one to be it...
            instance = this;
        //...otherwise...
        else if (instance != this)
            //...destroy this one because it is a duplicate.
            Destroy(gameObject);
    }

    void Update()
    {
        //If the game is over and the player has pressed some input...
        if (gameOver && Input.GetMouseButtonDown(0))
        {
            //...reload the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void BirdScored()
    {
        //The bird can't score if the game is over.
        if (gameOver)
            return;
        //If the game is not over, increase the score...
        score++;
        //...and adjust the score text.
        Puntuacion.text = "Score: " + score.ToString();
    }

    public void BirdDied()
    {
        //Activate the game over text.
        FinJuego.SetActive(true);
        //Set the game to be over.
        gameOver = true;

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://flappy-game-3699b.firebaseio.com/");

        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
        
        reference.Child("aaa").Child("test").SetValueAsync(score);
    }
}
