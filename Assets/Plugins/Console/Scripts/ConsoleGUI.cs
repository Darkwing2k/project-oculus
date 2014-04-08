using UnityEngine;
using System.Collections;

public class ConsoleGUI : MonoBehaviour {
	private static ConsoleGUI instance;

	public static ConsoleGUI Instance {
		get { return instance; }
	}

	private string input = "";
	private Rect consoleRect;
	private bool focusWindow = false;
    private const int WINDOW_ID = 50;
	private Vector2 scrollPos = Vector2.zero;
	private Logger rootLogger;

	public bool Toggle() {
		return (this.enabled = !this.enabled);
	}

	void Awake() {
		if (instance != null) {
			Debug.LogError ("There is more than one instance of the " + this.GetType().Name);
		}
		instance = this;
	}

    void Start() {
        consoleRect = new Rect(0, 0, Screen.width, Mathf.Min(300, Screen.height));
		rootLogger = LoggerFactory.GetLogger("root");
    }

    void OnEnable() {
		focusWindow = true;
    }

    void OnDisable() {
		this.input = "";
    }

	void Update () {

	}

    void OnGUI() {
		GUILayout.Window(WINDOW_ID, consoleRect, RenderWindow, "Console");
    }

    private void RenderWindow(int id) {
        HandleSubmit();
        HandleEscape();
		//GUI.SetNextControlName("log");
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        
		// Do not add new GUI elements during other events than "Repaint" and "Layout"!
		// Otherwise you get an exception
		if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint) {
			foreach (Logger.LogEntry le in this.rootLogger.LogEntries) {
				GUIStyle localStyle = new GUIStyle(GUI.skin.label);
				switch (le.Level) {
				case LogLevel.INFO:
					localStyle.normal.textColor = Color.yellow;
					break;
				case LogLevel.TRACE:
					localStyle.normal.textColor = Color.magenta;
					break;
				case LogLevel.DEBUG:
					localStyle.normal.textColor = Color.cyan;
					break;
				case LogLevel.WARN:
					localStyle.normal.textColor = new Color(1.0f, 0.5f, 0.0f);
					break;
				case LogLevel.ERROR:
					localStyle.normal.textColor = Color.red;
					break;
				default:
					localStyle.normal.textColor = Color.white;
					break;
				}
				GUILayout.Label(le.Message, localStyle);
			}
		}

        GUILayout.EndScrollView();
        GUI.SetNextControlName("input");
        input = GUILayout.TextField(input);
		HandleFocus();
    }

	private void HandleFocus() {
		if (focusWindow) {
			GUI.FocusWindow(WINDOW_ID);
			//GUI.FocusControl("log");
			focusWindow = false;
		} else {
			if (IsKeyDownEvent() && (int)Event.current.keyCode >= 97 && (int)Event.current.keyCode <= 122) {
				GUI.FocusControl("input");
			}
		}
	}

    private void HandleSubmit() {
        if (GUI.GetNameOfFocusedControl().Equals("input")) {
			if (KeyDown(KeyCode.KeypadEnter) || KeyDown(KeyCode.Return)) {
	            if (input.Length > 0) {
					DevConsole.Instance.SubmitInput(input);
					scrollPos = new Vector2(float.MaxValue, float.MaxValue);
					input = "";
	            }
	        }
		}
    }

    private void HandleEscape() {
		if (GUI.GetNameOfFocusedControl().Equals("input")) {
	        if (KeyDown(KeyCode.Escape) || KeyDown(KeyCode.F12)) {
				Toggle();
	        }
		}
    }

    private bool KeyDown(KeyCode key) {
		return (IsKeyDownEvent() && Event.current.keyCode == key);
		// alternative (key muss dann aber ein string sein):
		// return Event.current.Equals(Event.KeyboardEvent(key));
    }

	private bool IsKeyDownEvent() {
		return (Event.current.type == EventType.KeyDown);
	}
}

/* =================================================
 *  TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
    editor.selectPos = s.Length + 1;
    editor.pos = s.Length + 1;
 * 
 * 
 * 
 * 
 */
