using UnityEngine;
using System.Collections;
using System;

public delegate void CCmdExecuteDelegate(string[] args);

public class CCmdByDelegate : ConsoleCmd {
	private CCmdExecuteDelegate executeDelegate;

	public CCmdByDelegate(string name, CCmdFlags flags, string description, int minParams, string paramUsage, CCmdExecuteDelegate func)
		: base (name, flags, description, minParams, paramUsage) {
		if (func == null) {
			throw new ArgumentNullException("func");
		}
		executeDelegate = func;
	}

	public CCmdByDelegate(string name, CCmdFlags flags, string description, CCmdExecuteDelegate func) :
		this(name, flags, description, 0, "", func) {
	}

	public override void Execute(string[] args) {
		if (executeDelegate != null) {
			executeDelegate(args);
		}
	}
}
