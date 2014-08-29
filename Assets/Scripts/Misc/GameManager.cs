using UnityEngine;
using System.Collections;

public delegate void LevelWillBeLoaded();
public delegate void LevelWasLoaded();

public class GameManager : MonoBehaviour {
	
	#region Statics
	
	private static GameManager instance;
	
	public static GameManager Instance
    {
        get {
            if (instance == null) {
				GameObject go = GameObject.FindGameObjectWithTag("GameManager");
				
				if (go == null) {
					go = new GameObject("GameManager");
					go.tag = "GameManager";
				}
				
				go.AddComponent<GameManager>();
			}
			return instance;
        }
    }

	#endregion

	public GameObject mainMenu;
	public GameObject level;

	public event LevelWillBeLoaded OnLevelWillBeLoaded;
	public event LevelWasLoaded OnLevelWasLoaded;

	private SceneFader fader;

	private bool changeLevel;
	private bool loadingLevel;
	private int currentLevelIndex = 0;
	
	private GameObject levelManager;
	
	public SceneFader Fader {
		get { return fader; }
	}
	
	public bool IsPaused {
		get {
			return false;
		}
	}
	
	void Awake () {
		if (instance != null && instance != this) {
			DestroyImmediate(gameObject);
			return;
		} else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		fader = new SceneFader();
		OnLevelWasLoaded += new LevelWasLoaded(this.LevelWasLoaded);
	}
	
	// Use this for initialization
	void Start () {
		mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
		level = GameObject.FindGameObjectWithTag("Level");
	}
	
	// Update is called once per frame
	void Update () {
		if (changeLevel && !fader.IsFadingOut) {
			changeLevel = false;
			mainMenu.SetActive(false);
			Application.LoadLevelAdditive(currentLevelIndex);
			loadingLevel = false;
			level = GameObject.FindGameObjectWithTag("Level");

			Debug.Log(level == null);

			/*
			foreach (Transform child in level.transform) {
				Debug.Log(child.gameObject.name + " aktiviert");
				child.gameObject.SetActive(true);
			}
			*/

			//fader.FadeIn();
			//StartCoroutine(LoadLevelCoroutine(currentLevelIndex));
			//loadingLevel = true;
		}

		if (level == null && currentLevelIndex != 0) {
			level = GameObject.FindGameObjectWithTag("Level");
			if (level != null) {
				foreach (Transform child in level.transform) {
					Debug.Log(child.gameObject.name + " aktiviert");
					child.gameObject.SetActive(true);
				}

				fader.FadeIn();
			}
		}

		fader.Update();
	}
	
	void OnGUI () {
		fader.OnGUI();
	}

	public bool IsLevelLoaded {
		get { return (level != null); }
	}

	public void LoadLevel(int levelIndex) {
		changeLevel = true;
		currentLevelIndex = levelIndex;
		fader.FadeOutAndStay();
	}

	public void LoadLevelAsync(int levelIndex) {
		changeLevel = true;
		currentLevelIndex = levelIndex;
		fader.FadeOutAndStay();
	}

	public void UnloadLevel() {
		if (IsLevelLoaded) {
			Destroy(level);
		}
	}

	public void ReloadLevelAsync() {
		if (currentLevelIndex != 0) {
			UnloadLevel();
			LoadLevelAsync(currentLevelIndex);
		}
	}

	private IEnumerator LoadLevelCoroutine(int levelIndex) {
		if (OnLevelWillBeLoaded != null)
			OnLevelWillBeLoaded();
		
		yield return Application.LoadLevelAdditiveAsync(levelIndex);

		if (OnLevelWasLoaded != null)
			OnLevelWasLoaded();
	}

	private void LevelWasLoaded() {
		loadingLevel = false;
		level = GameObject.FindGameObjectWithTag("Level");
		
		foreach (Transform child in level.transform) {
			Debug.Log(child.gameObject.name + " aktiviert");
			child.gameObject.SetActive(true);
		}
		
		fader.FadeIn();
	}
}

public enum GameState { Menu, Gameplay };