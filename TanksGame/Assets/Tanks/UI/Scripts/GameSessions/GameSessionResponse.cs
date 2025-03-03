using System.Collections.Generic;

[System.Serializable]
public class GameSessionResponse
{
    public List<GameSession> content;
    public bool last;
    public int totalPages;
}