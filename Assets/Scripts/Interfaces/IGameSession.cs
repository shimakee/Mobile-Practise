using UnityEngine;

public interface IGameSession
{
    WordManager WordManager { get;}
    //GameOptions GameOptions { get; }

    void SessionStart();
    void SessionStart(string textFileName);
    void SessionPause(GameObject pauseCanvas);
    void SessionResume(GameObject pauseCanvas);
    void SessionEnd();
}
