using UnityEngine;
using System.Collections.Generic;

public abstract class ConsoleCmd /* IConsoleCompleter */ {
	
	protected static readonly Logger logger = LoggerFactory.GetLogger(typeof(ConsoleCmd));
	
	/** The name of the command (as used in the console) */
	protected readonly string name;
	/** The flags of the command */
	protected readonly CCmdFlags flags;
	/** The minimum number of arguments to call */
	protected readonly int minParams;
	/** The description of the parameter usage */
	protected readonly string paramUsage; 
	/** The description printed by the help command */
	protected readonly string description;
	
	/**
     * @param name The name use in the console
     * @param flags Flags (@see CCmdFlags)
     * @param description The description printed by the help command
     * @param minArguments The minimum number of arguments to call
    */
	public ConsoleCmd(string name, CCmdFlags flags, string description, int minParams, string paramUsage) {
		this.name = name;
		this.flags = flags;
		this.description = description;
		this.minParams = minParams;
		this.paramUsage = paramUsage;
	}
	
	/**
     * @param name The name use in the console
     * @param flags Flags (@see CCmdFlags)
     * @param description The description printed by the help command
    */
	public ConsoleCmd(string name, CCmdFlags flags, string description) :
		this(name, flags, description, 0, "") {
	}

	public string Name {
		get { return name; }
	}
	
	public CCmdFlags Flags {
		get { return flags; }
	}
	
	public int MinParams {
		get { return minParams; }
	}
	
	public string Description {
		get { return description; }
	}

	public string ParamUsage {
		get { return paramUsage; }
	}

	/*
	@Override
	public void complete(String arg, List<String> results) {
	}
	*/

	public bool isCallable(bool log) {
		if ((flags & CCmdFlags.CHEAT) != 0 /*&& !DevConsole.com_allowCheats.get()*/) {
			if (log) {
				logger.Warn("{} is cheat protected", name);
			}
			return false;
		}
		if ((flags & CCmdFlags.DEVELOPER) != 0 /*&& !DevConsole.com_developer.get()*/) {
			if (log) {
				logger.Warn("{} is for developers only", name);
			}
			return false;
		}
		return true;
	}

	public abstract void Execute(string[] args);
}
