using System;
using System.Collections;

public interface IWebClient
{
    IEnumerator Login(string username, string password, Action<bool, string> callback);
    IEnumerator Register(string username, string password, string email, Action<bool, User> callback);
    IEnumerator CheckUsername(string username, Action<bool, string> callback);
    IEnumerator GuestRegister(Action<bool, User> callback);
    IEnumerator UpdateUserStatus(string username, UserStatus status, Action<bool, string> callback);
    IEnumerator GetWaitingGameSessions(int page, int size, Action<bool, string> callback);
    IEnumerator CreateGameSession(string name, string password, System.Action<bool, GameSession> callback);
}