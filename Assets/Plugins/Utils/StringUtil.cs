using UnityEngine;
using System.Collections.Generic;
using System;

public static class StringUtil {
	public static string ReplaceFirstOccur(string text, string search, string replace) {
		int pos = text.IndexOf(search);
		if (pos < 0) {
			return text;
		}
		return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
	}

	public static string[] Tokenize(string text) {
		text = text.Trim();
		
		if (text.Length == 0) {
			return new string[0];
		}
		
		return text.Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries);
	}

	public static bool ToBoolean(string s) {
		s = s.Trim();
		try {
			return Convert.ToBoolean(s);
		} catch (FormatException ex) {
			if (s.Equals("0")) {
				return false;
			} else if (s.Equals("1")) {
				return true;
			}
			throw ex;
		}
	}
}
