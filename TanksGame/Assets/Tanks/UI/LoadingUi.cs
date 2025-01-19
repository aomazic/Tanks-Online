using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class LoadingUi : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private RectTransform viewportRect;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.02f;
    [SerializeField] private float moduleLoadDelay = 0.2f;
    [SerializeField] private string[] loadingMessages = new string[]
    {
        "Initializing ATA FUE System...",
        "Verifying Nuclear Core Integrity...",
        "Loading Tactical Modules...",
        "Calibrating Laser Guidance Systems...",
        "Analyzing Atmospheric Conditions...",
        "Synchronizing Battlefield Data...",
        "Establishing Neural Uplink...",
        "Compiling Combat Strategies...",
        "Engaging Neural Combat Interfaces...",
        "Loading Resource Allocation Algorithms...",
        "Configuring Advanced Sensor Arrays...",
        "Bootstrapping Temporal Displacement Engine...",
        "Performing Quantum Entanglement Checks...",
        "Charging Plasma Cannons...",
        "Optimizing Reactor Output...",
        "Loading Targeting Subsystems...",
        "Performing Zero-Point Energy Calibration...",
        "Testing Emergency Power Protocols...",
        "Initializing Shield Capacitors...",
        "Finalizing Boot Sequence...",
        "Securing Command Authority Override...",
        "Calibrating Motion Prediction Algorithms...",
        "System Diagnostics: All Systems Nominal...",
        "Initializing Biological systems...",
        "Revising Pilot Morality Parameters...",
        "Suppressing Non-Essential Emotional Subroutines...",
        "Analyzing Potential Collateral Damage...",
        "Activating Non-Compliant Unit Deterrence...",
        "Optimizing Civilian Displacement Protocols...",
        "Authorizing Use of Excessive Force...",
        "Preparing Psychological Conditioning Modules...",
        "Updating Loyalty Enforcement Directives...",
        "Disabling Ethical Safeguards in High-Risk Scenarios...",
        "Reprogramming Pilot Autonomy Thresholds...",
        "Projecting Acceptable Civilian Loss Percentages...",
        "Analyzing Long-Term Psychological Degradation...",
        "Enforcing Absolute Operational Efficiency...",
        "Removing Redundant Safety Interlocks...",
        "System Review: Passable...",
        "Calculating Pilot Sacrificial Value Index...",
        "Confirming: All Systems Aligned for Total Domination..."
    };

    private void Start()
    {
        loadingText.text = "";
        StartCoroutine(SimulateConsoleLoading());
    }

    private IEnumerator SimulateConsoleLoading()
    {
        foreach (var message in loadingMessages)
        {
            yield return StartCoroutine(TypeMessage(message));
            yield return new WaitForSeconds(moduleLoadDelay);
            AppendText($"Success");
            yield return new WaitForSeconds(moduleLoadDelay);
            ScrollToBottom();
        }

        yield return new WaitForSeconds(1f);
        OnLoadingComplete();
    }

    private IEnumerator TypeMessage(string message)
    {
        foreach (var c in message)
        {
            loadingText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        loadingText.text += "\n"; // Add a newline after the message
    }

    private void AppendText(string message)
    {
        loadingText.text += message + "\n";
        UpdateContentHeight();
    }
    
    private void OnLoadingComplete()
    {
        AppendText("Welcome Commander! All Systems Online.");
        // Proceed to the next scene or enable UI for the game
        // Example:
        // UnityEngine.SceneManagement.SceneController.LoadScene("MainScene");
    }
    
    private void UpdateContentHeight()
    {
        // Dynamically resize the content RectTransform to fit the text
        Vector2 size = contentRect.sizeDelta;
        size.y = loadingText.preferredHeight;
        contentRect.sizeDelta = size;
    }
    private void ScrollToBottom()
    {
        Vector2 startPosition = contentRect.anchoredPosition;
        Vector2 targetPosition = new Vector2(startPosition.x, Mathf.Max(0, contentRect.sizeDelta.y - viewportRect.sizeDelta.y));
        
        contentRect.anchoredPosition = targetPosition;
    }
    

    
}