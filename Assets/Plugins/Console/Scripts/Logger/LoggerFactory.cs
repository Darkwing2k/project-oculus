using System;
using System.Collections;

public static class LoggerFactory {
	private static Logger rootLogger;

	private static Logger GetLogger() {
		if (rootLogger == null) {
			rootLogger = new Logger("root");
		}
		return rootLogger;
	}

	public static Logger GetLogger(string loggerName) {
		return GetLogger();
	}

	public static Logger GetLogger(Type type) {
		return GetLogger();
	}
}
