using System.Collections.Generic;

/// <summary>
/// Stores terminal text strings mapped to different console states.
/// </summary>
public static class TerminalTexts
{
    private static readonly Dictionary<ConsoleState, string> terminalTextMap = new Dictionary<ConsoleState, string>
    {
        { ConsoleState.MainMenu, @"
===================================
   Choose Option to Enter System
===================================
   1. Login
   2. Register
   3. Enter as a Guest
" },

        { ConsoleState.LoginPassword, @"
===================================
     Enter Password to Login
===================================" },

        { ConsoleState.LoginUsername, @"
===================================
     Enter Username to Login
===================================" },

        { ConsoleState.RegisterEmail, @"
===================================
    Enter Email to Register
===================================" },

        { ConsoleState.RegisterPassword, @"
===================================
    Enter Password to Register
===================================" },

        { ConsoleState.RegisterUserName, @"
===================================
    Enter Username to Register
===================================" },

        { ConsoleState.Guest, @"
=======================================
 Entering as Guest - Limitations Apply
=======================================" },

        { ConsoleState.Entering, @"
===========================================
 FUE Combat System Presented by ATA Systems
===========================================" }
    };

    /// <summary>
    /// Retrieves terminal text based on the given ConsoleState.
    /// </summary>
    /// <param name="key">The ConsoleState key.</param>
    /// <returns>Corresponding terminal text or an empty string if not found.</returns>
    public static string GetTerminalText(ConsoleState key)
    {
        return terminalTextMap.TryGetValue(key, out var value) ? value : string.Empty;
    }
}