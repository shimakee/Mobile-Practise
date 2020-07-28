using System.Runtime.InteropServices;
using UnityEngine;

public interface IGameSession
{
    WordManager WordManager { get;}
    //GameOptions GameOptions { get; }

    void SessionPause(GameObject pauseCanvas);
    void SessionResume(GameObject pauseCanvas);
    void SessionStart();
    void SessionStart(string textFileName);
    void SessionEnd();
}
