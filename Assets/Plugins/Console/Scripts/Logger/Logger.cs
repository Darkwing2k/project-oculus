using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;

public class Logger {

	protected string name;
	protected List<LogEntry> entryList;

	public Logger(string name) {
		this.name = name;
		this.entryList = new List<LogEntry>();
	}

	public void Info(string msg) {
		this.addToList(msg, LogLevel.INFO);
	}

	public void Info(string msg, params object[] args) {
		this.Info(resolveParameters(msg, args));
	}

	public void Trace(string msg) {
		this.addToList(msg, LogLevel.TRACE);
	}

	public void Trace(string msg, params object[] args) {
		this.Trace(resolveParameters(msg, args));
	}

	public void Debug(string msg) {
		this.addToList(msg, LogLevel.DEBUG);
	}

	public void Debug(string msg, params object[] args) {
		this.Debug(resolveParameters(msg, args));
	}

	public void Warn(string msg) {
		this.addToList(msg, LogLevel.WARN);
	}

	public void Warn(string msg, params object[] args) {
		this.Warn(resolveParameters(msg, args));
	}

	public void Error(string msg) {
		this.addToList(msg, LogLevel.ERROR);
	}

	public void Error(string msg, params object[] args) {
		this.Error(resolveParameters(msg, args));
	}

	/*
	public List<string> GetInfoMessages() {
		return new List<string>(this.info);
	}

	public List<string> GetTraceMessages() {
		return new List<string>(this.trace);
	}

	public List<string> GetDebugMessages() {
		return new List<string>(this.debug);
	}

	public List<string> GetWarnMessages() {
		return new List<string>(this.warn);
	}

	public List<string> GetErrorMessages() {
		return new List<string>(this.error);
	}
	*/

	public IList<LogEntry> LogEntries {
		get { return this.entryList.AsReadOnly(); }
	}

	protected void addToList(string msg, LogLevel level) {
		LogEntry le = new LogEntry(msg, level, DateTime.Now);
		this.entryList.Add(le);
	}

	protected string resolveParameters(string msg, object[] args) {
		string tmp = msg;
		int counter = 0;
		while(tmp.IndexOf("{}") != -1) {
			tmp = ReplaceFirst(tmp, "{}", args[counter].ToString());
			if (++counter >= args.Length) {
				break;
			}
		}
		return tmp;
	}

	protected string ReplaceFirst(string text, string search, string replace) {
		int pos = text.IndexOf(search);
		if (pos < 0) {
			return text;
		}
		return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
	}


	// ================ Private Classes =====================

	public class LogEntry {
		private DateTime timestamp;
		private LogLevel level;
		private string msg;

		public LogEntry(string msg, LogLevel level, DateTime timestamp) {
			this.msg = msg;
			this.level = level;
			this.timestamp = timestamp;
		}

		public DateTime Timestamp {
			get { return this.timestamp; }
		}

		public LogLevel Level {
			get { return this.level; }
		}

		public string Message {
			get { return this.msg; }
		}
	}
}
