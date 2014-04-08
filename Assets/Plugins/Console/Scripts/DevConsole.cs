using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class DevConsole {
	private static readonly Logger logger = LoggerFactory.GetLogger(typeof(DevConsole));
	private static DevConsole instance;

	public static DevConsole Instance {
		get {
			if (instance == null) {
				instance = new DevConsole();
				instance.initStandardCmds();
				instance.initStandardCVars();
			}
			return instance; 
		}
	}


	private Dictionary<string, ConsoleCmd> cmdList;
	private Dictionary<string, ICVar> cvarList;
	//private Queue<string> cmdQueue;
	private LinkedList<string> inputHistory;

	protected DevConsole() {
		this.cmdList = new Dictionary<string, ConsoleCmd>();
		this.cvarList = new Dictionary<string, ICVar>();
		//this.cmdQueue = new Queue<string>();
		this.inputHistory = new LinkedList<string>();
	}

	protected void initStandardCmds() {
		// register Standard-Commands here
		this.Register(new CCmdHelp());
		this.Register(new CCmdListCmds());
		this.Register(new CCmdListCvars());
	}

	protected void initStandardCVars() {
		ICVar cvar;
		cvar = new CVar<bool>("sys_allow_cheats", false, CVarFlag.SYSTEM, "Enable or disable cheats"); 
		this.Register(cvar);
		cvar = new CVar<int>("max_hp", 3000, CVarFlag.DEVELOPER, "Adjust max health points"); 
		this.Register(cvar);
	}

	/**
     * Register a console command
     *
     * @param cmd The command
     */
	public void Register(ConsoleCmd cmd) {
		if (cmdList.ContainsKey(cmd.Name)) {
			logger.Warn("Trying to add command '{}', which already exists!", cmd.Name);
			return;
		}
		cmdList.Add(cmd.Name, cmd);
	}

	public void Register(ICVar cvar) {
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
	public void Unregister(ConsoleCmd cmd) {
		Unregister(cmd.Name);
	}

	public void Unregister(string cmdName) {
		cmdList.Remove(cmdName);
	}

	public void UnregisterByFlags(CCmdFlags flags) {
		Dictionary<string, ConsoleCmd> tmpList = new Dictionary<string, ConsoleCmd>(this.cmdList);

		foreach (ConsoleCmd cmd in tmpList.Values) {
			if ((cmd.Flags & flags) != 0) {
				Unregister(cmd.Name);
			}
		}
	}

	public bool SubmitInput(string command) {
		command = command.Trim();
		if (command.Length > 0) {
			this.inputHistory.AddLast(command);
			logger.Info("] {}", command);
			return this.ExecuteCmd(command);
		}
		return false;
	}

	public bool ExecuteCmd(string command) {
		string[] args = StringUtil.Tokenize(command);

		if (args.Length == 0) {
			return false;
		}

		ConsoleCmd cmd;
		ICVar cvar;

		string arg0 = args[0];
		if (this.TryGetConsoleCmd(arg0, out cmd)) {
			if (cmd.isCallable(true)) {
				if (cmd.MinParams > (args.Length - 1)) {
					this.ShowUsage(cmd);
					return false;
				} else {
					cmd.Execute(args.Skip(1).ToArray());
				}
			}
		} else if (this.TryGetCVar(arg0, out cvar)) {
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

	public bool TryGetConsoleCmd(string cmdName, out ConsoleCmd cmd) {
		return cmdList.TryGetValue(cmdName, out cmd);
	}

	public bool TryGetCVar(string cvarName, out ICVar cvar) {
		return cvarList.TryGetValue(cvarName, out cvar);
	}

	public void ShowUsage(ConsoleCmd cmd) {
		logger.Info("HowToUse: {} {}", cmd.Name, cmd.ParamUsage);
	}

	public void ShowUsage(ICVar cvar) {
		logger.Info("{}\n{}: {} ({}) - Default: {}", cvar.Description, cvar.Name, cvar.Value, cvar.Value.GetType().Name, cvar.DefaultValue);
	}

	public void ShowUsage(string command) {
		ConsoleCmd cmd;
		ICVar cvar;
		if (this.TryGetConsoleCmd(command, out cmd)) {
			this.ShowUsage(cmd);
		} else if (this.TryGetCVar(command, out cvar)) {
			this.ShowUsage(cvar);
		} else {
			logger.Warn("Command/CVar '{}' does not exists", command);
		}
	}

	public void ShowAllCommands() {
		StringBuilder sb = new StringBuilder();
		foreach (ConsoleCmd cmd in this.cmdList.Values) {
			if (sb.Length > 0) {
				sb.Append('\n');
			}
			sb.AppendFormat("{0} - - {1}", cmd.Name, cmd.Description);

		}
		logger.Info(sb.ToString());
	}

	public void ShowAllCVars() {
		StringBuilder sb = new StringBuilder();
		foreach (ICVar cvar in this.cvarList.Values) {
			if (sb.Length > 0) {
				sb.Append('\n');
			}
			sb.AppendFormat("{0}: {1}", cvar.Name, cvar.Value);
		}
		logger.Info(sb.ToString());
	}

	// ================ Private Classes =====================

	private class CCmdHelp : ConsoleCmd {
		public CCmdHelp() : 
			base("help", CCmdFlags.SYSTEM, "Shows how to use a command or cvar", 1, "<command/cvar>") {
		}
		
		public override void Execute(string[] args) {
			DevConsole.Instance.ShowUsage(args[0]);
		}
	}

	private class CCmdListCmds : ConsoleCmd {
		public CCmdListCmds() :
			base("listcmds", CCmdFlags.SYSTEM, "Lists all available console commands") {
		}
		
		public override void Execute(string[] args) {
			DevConsole.Instance.ShowAllCommands();
		}
	}

	private class CCmdListCvars : ConsoleCmd {
		public CCmdListCvars() :
		base("listcvars", CCmdFlags.SYSTEM, "Lists all available CVars") {
		}
		
		public override void Execute(string[] args) {
			DevConsole.Instance.ShowAllCVars();
		}
	}
}


