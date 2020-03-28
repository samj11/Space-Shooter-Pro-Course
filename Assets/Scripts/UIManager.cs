using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _UIScore;
    [SerializeField]
    private Text _gameOverTxt;
    [SerializeField]
    private Text _restartTxt;

    private Player _player;

    [SerializeField]
    private Image currentLive;
    [SerializeField]
    private Sprite[] _imgLives;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _UIScore.text = "Score: " + 0;
        _player = GameObject.Find("Player").GetComponent<Player>();
        _gameOverTxt.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.Log("Game Manager NULL");
        }
    }

    public void UpdateScore(int playerScore)
    {
        _UIScore.text = "Score: " + playerScore;
    }

    public void UpdateLives(int livesCount)
    {
        currentLive.sprite = _imgLives[livesCount];
        if(livesCount == 0)
        {
            GameOverSequence();
        }
    }

    public void GameOverSequence()
    {
        _gameOverTxt.gameObject.SetActive(true);
        _restartTxt.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
        _gameManager.GameOver();
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverTxt.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            _gameOverTxt.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }

    }

}
