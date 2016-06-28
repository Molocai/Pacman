using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class PacmanAI : MonoBehaviour
{
    // Vitesse de déplacement
    public float Speed;

    // Direction vers laquelle se dirige le personnage
    // (sert pour les animations entre autres)
    private Vector3 Direction;

    // Le tile sur lequel se tient le pacman
    private TileManager.Tile CurrentTile;

    // Variables utilitaires
    private List<TileManager.Tile> Tiles = new List<TileManager.Tile>();
    private TileManager Manager;
    private bool _deadPlaying = false;
    private GameManager GM;
    private ScoreManager SM;
    private GameGUINavigation GUINav;

    /// <summary>
    /// Appelé à la mort de pacman
    /// </summary>
    public void ResetVariables()
    {
        Direction = new Vector3(1, 0);
    }

    void Awake()
    {
        // Initialisation des variables
        Manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        SM = GameObject.Find("Game Manager").GetComponent<ScoreManager>();
        GUINav = GameObject.Find("UI Manager").GetComponent<GameGUINavigation>();
        
        Tiles = Manager.tiles;

    }

    void Start()
    {
    }

    #region Fonctions d'affichage
    void Animate()
    {
        GetComponent<Animator>().SetFloat("DirX", Direction.x);
        GetComponent<Animator>().SetFloat("DirY", Direction.y);
    }

    IEnumerator PlayDeadAnimation()
    {
        _deadPlaying = true;
        GetComponent<Animator>().SetBool("Die", true);
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().SetBool("Die", false);
        _deadPlaying = false;

        if (GameManager.lives <= 0)
        {
            Debug.Log("Treshold for High Score: " + SM.LowestHigh());
            if (GameManager.score >= SM.LowestHigh())
                GUINav.getScoresMenu();
            else
                GUINav.H_ShowGameOverScreen();
        }

        else
            GM.ResetScene();
    }
    #endregion

    void FixedUpdate()
    {
        // Suivant l'état du jeu on appel l'IA ou l'animation de mort
        switch (GameManager.gameState)
        {
            case GameManager.GameState.Game:
                // C'est ici qu'on peut faire un appel à l'IA
                // FSM.Run(AI_IdleState);
                Animate();
                break;

            case GameManager.GameState.Dead:
                if (!_deadPlaying)
                    StartCoroutine("PlayDeadAnimation");
                break;
        }

        // Mise à jours de la tile actuelle par rapport à la position du pacman
        Vector3 CurrentPos = new Vector3(transform.position.x + 0.499f, transform.position.y + 0.499f);
        CurrentTile = Tiles[Manager.Index((int)CurrentPos.x, (int)CurrentPos.y)];
    }

    /// <summary>
    /// Déplace pacman en direction d'une tile
    /// </summary>
    /// <param name="t">La tile cible</param>
    void MoveTowardsTile(TileManager.Tile t)
    {
        Vector2 p = Vector2.MoveTowards(transform.position, new Vector3(t.x, t.y), Speed);

        Direction = new Vector3(t.x, t.y) - transform.position;
        GetComponent<Rigidbody2D>().MovePosition(p);
    }
}
