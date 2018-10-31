namespace TPSShooter
{

    /// <summary>
    /// This methods will be called when GameManager class calls StopGame/ResumeGame/EndGame methods. 
    /// </summary>
    /// 
    /// <remarks>
    /// To extend class as IGameNotifier you have to:
    /// 1) implement this interface;
    /// 2) add this notifier to GameManager (call AddNotifier method).
    /// </remarks>

    public interface IGameNotifier
    {

        void GameResumed();
        void GameStopped();
        void GameEnded();

    }
}