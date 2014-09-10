using UnityEngine;
using System.Collections;


public class GameManager : MonoBehaviour {
	
	#region Statics
	
	private static GameManager instance;
	
	public static GameManager Instance
    {
        get { return instance; }
    }

	#endregion

	public GameObject mainMenu;

    private static GameObject m_level;
    public static GameObject level
    {
        get
        {
            return m_level;
        }
        set
        {
            m_level = value;
            DontDestroyOnLoad(level);
        }
    }

	//public event LevelWillBeLoaded OnLevelWillBeLoaded;
	//public event LevelWasLoaded OnLevelWasLoaded;

	private SceneFader fader;

	private bool unloadLevel = false;
	private bool loadingLevel;
	private int currentLevelIndex = -1;

	public GameState currentState = GameState.MainMenu;

	private GameState nextState = GameState.MainMenu;

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
                //case GameState.MainMenu:
                //    mainMenu.SetActive(true);
                //    levelActivation(false);
                //    //Application.LoadLevel(0);

                //    if (unloadLevel) {
                //        UnloadLevel();
                //    }

                //    fader.FadeIn();
                //    break;
                case GameState.GameMenu:
                    levelActivation(false);
                    Application.LoadLevel(0);
                    fader.FadeIn();
                    break;
				case GameState.Gameplay:
                    
					if (IsLevelLoaded) 
                    {
                        mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
						levelActivation(true);
                        DestroyImmediate(mainMenu);
						fader.FadeIn();
					}
                    else
                    {
                        Application.LoadLevel(currentLevelIndex);
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

        if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Gameplay)
        {
            LoadMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.GameMenu)
        {
            LoadLevel(currentLevelIndex);
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
		nextState = GameState.GameMenu;
		fader.FadeOutAndStay();
	}

	private void levelActivation(bool activate) {
            foreach (Transform child in level.transform)
            {
                child.gameObject.SetActive(activate);
            }
	}

}

public enum GameState { MainMenu, GameMenu, Gameplay, Change };