using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class DevConsole
{
	private static readonly Logger logger = LoggerFactory.GetLogger(typeof(DevConsole));

	private static Dictionary<string, ConsoleCmd> cmdList;
	private static Dictionary<string, ICVar> cvarList;
	//private static Queue<string> cmdQueue;
	private static LinkedList<string> inputHistory;

	private static bool isInitialized = false;

	static DevConsole() {
		if (!isInitialized) {
			Initialize();
		}
	}

	public static void Initialize() {
		if (!isInitialized) {
			cmdList = new Dictionary<string, ConsoleCmd>();
			cvarList = new Dictionary<string, ICVar>();
			//this.cmdQueue = new Queue<string>();
			inputHistory = new LinkedList<string>();
			initStandardCmds();
			initStandardCVars();
			isInitialized = true;
		}
	}

	protected static void initStandardCmds() {
		// register Standard-Commands here
		Register(new CCmdHelp());
		Register(new CCmdListCmds());
		Register(new CCmdListCvars());
	}

	protected static void initStandardCVars() {
		Register(new CVar<bool>("sys_allow_cheats", false, CVarFlag.SYSTEM, "Enable or disable cheats"));
		Register(new CVar<int>("max_hp", 3000, CVarFlag.DEVELOPER, "Adjust max health points"));
	}

	/**
	 * Register a console command
	 *
	 * @param cmd The command
	 */
	public static void Register(ConsoleCmd cmd) {
		if (cmdList.ContainsKey(cmd.Name)) {
			logger.Warn("Trying to add command '{}', which already exists!", cmd.Name);
			return;
		}
		cmdList.Add(cmd.Name, cmd);
	}

	public static void Register(ICVar cvar) {
		if (cvarList.ContainsKey(cvar.Name)) {
			logger.Warn("Trying to add cvar '{}', which already exists!", cvar.Name);
			return;
		}
		cvarList.Add(cvar.Name, cvar);
	}

	/**
	 * Unregister a command previously registered by register
	 *
	 * @param cmd The command
	 */
	public static void Unregister(ConsoleCmd cmd) {
		Unregister(cmd.Name);
	}

	public static void Unregister(string name) {
		ConsoleCmd cmd;
		ICVar cvar;

		if (TryGetConsoleCmd(name, out cmd)) {
			cmdList.Remove(name);
		} else if (TryGetCVar(name, out cvar)) {
			cvarList.Remove(name);
		}
	}

	public static void UnregisterCmdsByFlags(CCmdFlags flags) {
		Dictionary<string, ConsoleCmd> tmpList = new Dictionary<string, ConsoleCmd>(cmdList);

		foreach (ConsoleCmd cmd in tmpList.Values) {
			if ((cmd.Flags & flags) != 0) {
				Unregister(cmd.Name);
			}
		}
	}

	public static bool ExecuteCmd(string command) {
		command = command.Trim();

		string[] args = command.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

		if (args.Length == 0) {
			return false;
		}

		inputHistory.AddLast(command);
		logger.Log("> {}", command);

		ConsoleCmd cmd;
		ICVar cvar;

		string arg0 = args[0];
		if (TryGetConsoleCmd(arg0, out cmd)) {
			if (cmd.isCallable(true)) {
				if (cmd.MinParams > (args.Length - 1)) {
					ShowUsage(cmd);
					return false;
				} else {
					cmd.Execute(args.Skip(1).ToArray());
				}
			}
		} else if (TryGetCVar(arg0, out cvar)) {
			if (args.Length == 2) {
				try {
					cvar.Value = args[1];
					logger.Info("Set '{}' to '{}'", cvar.Name, cvar.Value);
				} catch (FormatException) {
					logger.Warn("Wrong format!");
				} catch (Exception ex) {
					logger.Warn("Unknown Exception!\n{}", ex.StackTrace);
				}
			} else {
				logger.Info("{}: {}", cvar.Name, cvar.Value, cvar.DefaultValue);
			}
		} else {
			logger.Warn("Unknown Command '{}'", arg0);
			return false;
		}

		return true;
	}

	public static bool TryGetConsoleCmd(string cmdName, out ConsoleCmd cmd) {
		return cmdList.TryGetValue(cmdName, out cmd);
	}

	public static bool TryGetCVar(string cvarName, out ICVar cvar) {
		return cvarList.TryGetValue(cvarName, out cvar);
	}

	public static void ShowUsage(ConsoleCmd cmd) {
		logger.Info("HowToUse: {} {}", cmd.Name, cmd.ParamUsage);
	}

	public static void ShowUsage(ICVar cvar) {
		logger.Info("{}\n{}: {} ({}) - Default: {}", cvar.Description, cvar.Name, cvar.Value, cvar.Value.GetType().Name, cvar.DefaultValue);
	}

	public static void ShowUsage(string command) {
		ConsoleCmd cmd;
		ICVar cvar;
		if (TryGetConsoleCmd(command, out cmd)) {
			ShowUsage(cmd);
		} else if (TryGetCVar(command, out cvar)) {
			ShowUsage(cvar);
		} else {
			logger.Warn("Command/CVar '{}' does not exists", command);
		}
	}

	public static void ShowAllCommands() {
		StringBuilder sb = new StringBuilder();
		foreach (ConsoleCmd cmd in cmdList.Values) {
			if (sb.Length > 0) {
				sb.Append('\n');
			}
			sb.AppendFormat("{0} - - {1}", cmd.Name, cmd.Description);

		}
		logger.Info(sb.ToString());
	}

	public static void ShowAllCVars() {
		StringBuilder sb = new StringBuilder();
		foreach (ICVar cvar in cvarList.Values) {
			if (sb.Length > 0) {
				sb.Append('\n');
			}
			sb.AppendFormat("{0}: {1}", cvar.Name, cvar.Value);
		}
		logger.Info(sb.ToString());
	}


	// Catch Unity-Logs and show them on console
	private static void HandleLog(string logString, string stackTrace, LogType type) {
		switch (type) {
			case LogType.Error:
			case LogType.Exception:
				logger.Error(logString);
				break;
			case LogType.Warning:
				logger.Warn(logString);
				break;
			case LogType.Log:
				logger.Log(logString);
				break;
		}
	}

	#region === Logger-Methods ===

	public static void Log(string msg) {
		logger.Log(msg);
	}

	public static void Log(string msg, params object[] args) {
		logger.Log(msg, args);
	}

	public static void Info(string msg) {
		logger.Info(msg);
	}

	public static void Info(string msg, params object[] args) {
		logger.Info(msg, args);
	}

	public static void Trace(string msg) {
		logger.Trace(msg);
	}

	public static void Trace(string msg, params object[] args) {
		logger.Trace(msg, args);
	}

	public static void Debug(string msg) {
		logger.Debug(msg);
	}

	public static void Debug(string msg, params object[] args) {
		logger.Debug(msg, args);
	}

	public static void Warn(string msg) {
		logger.Warn(msg);
	}

	public static void Warn(string msg, params object[] args) {
		logger.Warn(msg, args);
	}

	public static void Error(string msg) {
		logger.Error(msg);
	}

	public static void Error(string msg, params object[] args) {
		logger.Error(msg, args);
	}

	#endregion

	// ================ Private Classes =====================

	private class CCmdHelp : ConsoleCmd
	{
		public CCmdHelp() :
			base("help", CCmdFlags.SYSTEM, "Shows how to use a command or cvar", 1, "<command/cvar>") {
		}

		public override void Execute(string[] args) {
			DevConsole.ShowUsage(args[0]);
		}
	}

	private class CCmdListCmds : ConsoleCmd
	{
		public CCmdListCmds() :
			base("listcmds", CCmdFlags.SYSTEM, "Lists all available console commands") {
		}

		public override void Execute(string[] args) {
			DevConsole.ShowAllCommands();
		}
	}

	private class CCmdListCvars : ConsoleCmd
	{
		public CCmdListCvars() :
			base("listcvars", CCmdFlags.SYSTEM, "Lists all available CVars") {
		}

		public override void Execute(string[] args) {
			DevConsole.ShowAllCVars();
		}
	}
}


