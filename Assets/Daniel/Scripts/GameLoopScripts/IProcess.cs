public interface IProcess
{
    bool IsCompleted { get; }
    void ExecuteProcess(System.Action onComplete);
}
