using System;
using System.Collections;
using UnityEngine;

public class MockWebClient : MonoBehaviour, IWebClient
{
    public IEnumerator Login(string username, string password, Action<bool, string> callback)
    {
        yield return null;
        callback(true, "Mock login response");
    }

    public IEnumerator Register(string username, string password, string email, Action<bool, string> callback)
    {
        yield return null;
        callback(true, "Mock register response");
    }

    public IEnumerator CheckUsername(string username, Action<bool, string> callback)
    {
        yield return null;
        // Simulate that username is available
        callback(true, "Username is available");
    }

    public IEnumerator GuestRegister(Action<bool, string> callback)
    {
        yield return null;
        callback(true, "Mock guest register response");
    }

    public IEnumerator UpdateUserStatus(string username, UserStatus status, Action<bool, string> callback)
    {
        yield return null;
        callback(true, "Mock update status response");
    }

    public IEnumerator GetWaitingGameSessions(int page, int size, Action<bool, string> callback)
    { 
        yield return null;
        callback(true, MockResponses.GameSessionResponse);
    }
}