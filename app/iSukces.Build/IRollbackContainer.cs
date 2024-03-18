using System;

namespace iSukces.Build;

public interface IRollbackContainer
{
    void AddRollbackAction(Action action);
    void RollbackModifications();
}
