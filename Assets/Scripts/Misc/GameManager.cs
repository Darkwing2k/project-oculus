using UnityEngine;
using System.Collections;

public delegate void LevelWillBeLoaded();
public delegate void LevelWasLoaded();

public class GameManager : MonoBehaviour {
	
	#region Statics
	
	private static GameManager instance;
	
	public static GameManager Instance
    {
        get { return instance; }
    }

	#endregion

	public GameObject mainMenu;
	public GameObject level;

	//public event LevelWillBeLoaded OnLevelWillBeLoaded;
	//public event LevelWasLoaded OnLevelWasLoaded;

	private SceneFader fader;

	private bool unloadLevel = false;
	private bool loadingLevel;
	private int currentLevelIndex = -1;

	public GameState currentState = GameState.Menu;

	private GameState nextState = GameState.Menu;

	public SceneFader Fader {
		get { return fader; }
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
		//OnLevelWasLoaded += new LevelWasLoaded(this.LevelWasLoaded);
	}
	
	// Use this for initialization
	void Start () {
		mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
		level = GameObject.FindGameObjectWithTag("Level");
	}
	
	// Update is called once per frame
	void Update () {
		if (currentState == GameState.Change && !fader.IsFadingOut) {
			switch (nextState) {
				case GameState.Menu:
					mainMenu.SetActive(true);
					levelActivation(false);

					if (unloadLevel) {
						UnloadLevel();
					}

					fader.FadeIn();
					break;
				case GameState.Gameplay:
					mainMenu.SetActive(false);
					if (IsLevelLoaded) {
						levelActivation(true);
						fader.FadeIn();
					} else {
						Application.LoadLevelAdditive(currentLevelIndex);
					}
					break;
			}
			currentState = nextState;
		}

		// muss hier gemacht werden, da das LevelObjekt nicht direkt nach "LoadLevelAdditive" gefunden werden kann
		if (level == null && currentLevelIndex > -1) {
			level = GameObject.FindGameObjectWithTag("Level");
			if (level != null) {
				levelActivation(true);
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

	public bool IsMenuLoaded {
		get { return (mainMenu != null); }
	}

	public void ReloadLevel() {
		if (currentLevelIndex != 0) {
			UnloadLevel();
			LoadLevel(currentLevelIndex);
		}
	}

	public void UnloadLevel() {
		if (IsLevelLoaded) {
			Destroy(level);
			level = null;
		}
		unloadLevel = false;
	}

	public void LoadLevel(int levelIndex) {
		if (currentState == GameState.Change) {
			return;
		}
		currentState = GameState.Change;
		nextState = GameState.Gameplay;
		currentLevelIndex = levelIndex;
		fader.FadeOutAndStay();
	}

	public void LoadMenu() {
		LoadMenu(false);
	}

	public void LoadMenu(bool unloadLevel) {
		if (currentState == GameState.Change) {
			return;
		}
		this.unloadLevel = unloadLevel;
		currentState = GameState.Change;
		nextState = GameState.Menu;
		fader.FadeOutAndStay();
	}

	private void levelActivation(bool activate) {
		foreach (Transform child in level.transform) {
			child.gameObject.SetActive(activate);
		}
	}

	/*
	private void LevelWasLoaded() {
		loadingLevel = false;
		level = GameObject.FindGameObjectWithTag("Level");

		foreach (Transform child in level.transform) {
			Debug.Log(child.gameObject.name + " aktiviert");
			child.gameObject.SetActive(true);
		}

		fader.FadeIn();
	}
	*/


	/*
	// Async Level loading test 
	public void LoadLevelAsync(int levelIndex) {
		changeLevel = true;
		currentLevelIndex = levelIndex;
		fader.FadeOutAndStay();
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
	*/
}

public enum GameState { Menu, Gameplay, Change };