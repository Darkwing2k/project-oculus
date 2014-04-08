using System.Collections;

public interface ICVarListener<T> {
	void Modified(CVar<T> cvar);
}
