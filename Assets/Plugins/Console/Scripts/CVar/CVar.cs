using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

public class CVar<T> : ICVar {

	private string name;
	private T value;
	private T defaultValue;
	private CVarFlag flags;
	private string description;

	private List<ICVarListener<T>> listener;

	public CVar(string name, T defaultValue, CVarFlag flags, string description) {
		this.name = name;
		this.value = defaultValue;
		this.defaultValue = defaultValue;
		this.flags = flags;
		this.description = description;

		this.listener = new List<ICVarListener<T>>();
	}

	public T Value {
		get { return this.value; }
		set { this.value = value; }
	}

	public T DefaultValue {
		get { return this.defaultValue; }
	}

	public string Name {
		get { return this.name; }
	}

	public CVarFlag Flags {
		get { return this.flags; }
	}

	public string Description {
		get { return this.description; }
	}

	public void Reset() {
		this.value = this.defaultValue;
	}

	public void AddListener(ICVarListener<T> l) {
		this.listener.Add(l);
	}

	public void RemoveListener(ICVarListener<T> l) {
		this.listener.Remove(l);
	}

	protected void notifyListener() {
		foreach (ICVarListener<T> l in this.listener) {
			l.Modified(this);
		}
	}

	// ICVar implementation
	object ICVar.Value { 
		get { return (object)this.value; } 
		set {
			if (typeof(T).Equals(typeof(bool))) {
				this.value = (T) Convert.ChangeType(StringUtil.ToBoolean(Convert.ToString(value)), typeof(T));
			} else {
				this.value = (T) Convert.ChangeType(value, typeof(T));
			}
		}
	}

	object ICVar.DefaultValue {
		get { return (object)this.defaultValue; }
	}
}
