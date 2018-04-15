using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {

    public static GameControl instance;         //A reference to our game control script so we can access it statically.
    public Text Puntuacion;                      //A reference to the UI text component that displays the player's score.
    public GameObject FinJuego;             //A reference to the object that displays the text which appears when the player dies.

    private int score = 0;                      //The player's score.
    public bool gameOver = true;               //Is the game over?
    public float scrollSpeed = -1.5f;

    public GameObject btn_start;
    public GameObject btn_escenario;
    public GameObject btn_puntuacion;
    public InputField input_name;
    static string playerName = "Anonymous";
    static bool ScoredToServer = false;

    public GameObject GridMarcador;
    public GameObject GridMarcadorModelo;

    public GameObject canvasPunt;

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
        if (gameOver && Input.GetMouseButtonDown(0) && ! menuActivo)
        {
            //...reload the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            MostrarMenu(false, false);

        }
        if (! gameOver && menuActivo)
        {
            MostrarMenu(false, false);
            input_name.text = playerName;


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

        if (!ScoredToServer)
        {
            if (score > 0)
            {
                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://flappy-game-3699b.firebaseio.com/");

                // Get the root reference location of the database.
                DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;


                User user = new User(playerName, -score);
                string json = JsonUtility.ToJson(user);

                reference.Child("puntuaciones").Push().SetRawJsonValueAsync(json);
            }
            MostrarMenu(true, false);

            ScoredToServer = true;
        }

    }

    public class User
    {
        public string username;
        public int puntuacion;

        public User()
        {
        }

        public User(string username, int puntuacion)
        {
            this.username = username;
            this.puntuacion = puntuacion;
        }
    }

    private bool menuActivo = true;
    private void MostrarMenu(bool orden, bool puntuaciones)
    {
        {
            btn_start.SetActive(orden);
            btn_escenario.SetActive(orden);
            btn_puntuacion.SetActive(orden);
            input_name.gameObject.SetActive(orden);
            if (!puntuaciones)
            {
                menuActivo = orden;
            }
        }
    }
    public void Btn_start() {
        {
            ScoredToServer = false;
            playerName = input_name.text;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            input_name.text = playerName;
            MostrarMenu(false, false);

        }
    }
    public void Btn_escena()
    {
        {
        }
    }
    public void Btn_puntuaciones()
    {
        {
            canvasPunt.SetActive(true);
            MostrarMenu(false, true);


            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://flappy-game-3699b.firebaseio.com/");

            // Get the root reference location of the database.
            //DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("puntuaciones");

            
            
            int children = GridMarcador.transform.childCount;
            for (int i = 0 + 1; i < children; ++i)
            {
                Destroy(transform.GetChild(i));
            }
            //FirebaseDatabase.DefaultInstance.GetReference("puntuacioes").OrderByChild("puntuacion");
            //FirebaseDatabase.DefaultInstance.GetReference("puntuacioes").OrderByChild("puntuacion");
            FirebaseDatabase.DefaultInstance
            .GetReference("puntuaciones").OrderByChild("puntuacion")
            .ValueChanged += (object sender2, ValueChangedEventArgs e2) => {
                if (e2.DatabaseError != null)
                {
                    Debug.LogError(e2.DatabaseError.Message);
                }

                Debug.Log(e2.Snapshot);
                Debug.Log(e2.Snapshot.ChildrenCount);
                if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
                {

                    foreach (var childSnapshot in e2.Snapshot.Children)
                    {
                        var name = childSnapshot.Child("username").Value.ToString();
                        var puntuacion = childSnapshot.Child("puntuacion").Value.ToString();
                        Debug.Log(name.ToString());
                        Debug.Log(puntuacion.ToString());
                        //text.text = childSnapshot.ToString();
                        
                        GameObject temp = Instantiate(GridMarcadorModelo);
                        Text fieldName = temp.transform.GetChild(0).GetComponent<Text>();
                        Text fieldPunt = temp.transform.GetChild(1).GetComponent<Text>();

                        fieldName.text = name.ToString();
                        fieldPunt.text = (int.Parse(puntuacion.ToString()) * -1).ToString(); 

                        temp.transform.SetParent(GridMarcador.transform);
                        temp.SetActive(true);

                        

                        Debug.Log(temp);
                        //GridMarcador.AddComponent(temp);

                    }

                }

            };

        }
    }
    public void Btn_cerrarPuntuaciones()
    {
        canvasPunt.SetActive(false);
        MostrarMenu(true, true);
    }

}
