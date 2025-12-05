using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("GameManager Enums")]
    // Make GameManager a Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // If GameManager exists, destory it
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple GameManagers in scene, destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Enums
    public enum TurnOwner
    {
        Player,
        Enemy
    }

    public enum GamePhase
    {
        ChoosingBattery,
        ChoosingSlot,
        EnemyTurn,
        GameOver
    }

    [Header("Water Levels (0-100)")]
    public int playerWater = 0;
    public int enemyWater = 0;

    [Header("Batteries & Items")]
    public List<BatteryObject.BatteryType> playerBatteries = new();
    public List<string> playerItems = new();

    [Header("Turn State")]
    public TurnOwner currentTurn = TurnOwner.Player;
    public GamePhase currentPhase = GamePhase.ChoosingBattery;
    public bool gameEnded = false;

    // What the player has chosen this turn
    private BatteryObject.BatteryType? selectedBatteryThisTurn = null;

    private void Start()
    {
        SetupNewGame();
    }

    private void SetupNewGame()
    {
        Debug.Log("// New Game Started //");

        playerWater = 0;
        enemyWater = 0;
        gameEnded = false;

        playerBatteries.Clear();
        playerItems.Clear();

        // Player starting hand
        playerBatteries.Add(BatteryObject.BatteryType.Default);
        playerBatteries.Add(BatteryObject.BatteryType.Chance);
        playerBatteries.Add(BatteryObject.BatteryType.Chance);

        currentTurn = TurnOwner.Player;
        currentPhase = GamePhase.ChoosingBattery;
        selectedBatteryThisTurn = null;

        LogState();
        Debug.Log("Player turn begins. Choose a battery.");
    }

    // When player clicks a battery
    public void OnBatteryClicked(BatteryObject.BatteryType batteryType)
    {
        // Clickable checks
        if (gameEnded)
        {
            Debug.Log("Game is over, ignoring battery click");
            return;
        }
        if (currentTurn != TurnOwner.Player)
        {
            Debug.Log("Not players turn, ignoring battery click");
            return;
        }
        if (currentPhase != GamePhase.ChoosingBattery)
        {
            Debug.Log("Not currently choosing a battery, ignoring battery click");
            return;
        }
        // If doesnt player own the battery
        if (!playerBatteries.Contains(batteryType))
        {
            Debug.Log($"Player doesn't have a {batteryType} battery available");
            return;
        }

        selectedBatteryThisTurn = batteryType;
        Debug.Log($"[GameManager] Player selected battery: {batteryType}. Now choose a slot.");
        currentPhase = GamePhase.ChoosingSlot;
    }

    // When player clicks a slot in the world
    public void OnSlotClicked(BatterySlot.SlotType slotType)
    {
        if (gameEnded)
        {
            Debug.Log("Game is over, ignoring slot click");
            return;
        }
        if (currentTurn != TurnOwner.Player)
        {
            Debug.Log("Not players turn, ignoring slot click");
            return;
        }
        if (currentPhase != GamePhase.ChoosingSlot)
        {
            Debug.Log("Not currently choosing a battery, slot battery click");
            return;
        }
        if (selectedBatteryThisTurn == null)
        {
            Debug.Log("No battery selected yet. Must choose a battery first");
            return;
        }

        Debug.Log($"[GameManager] Player selected slot: {slotType} with battery: {selectedBatteryThisTurn.Value}");

        // Delete battery from player inventory
        playerBatteries.Remove(selectedBatteryThisTurn.Value);
        ResolvePlayerAction(selectedBatteryThisTurn.Value, slotType);
        selectedBatteryThisTurn = null;

        CheckWinLose();

        if (!gameEnded)
        {
            StartEnemyTurn();
        }
    }

    public void OnItemClicked(string itemId)
    {
        if (gameEnded)
        {
            Debug.Log("Game is over. Ignoring item click");
            return;
        }

        Debug.Log($"[GameManager] Player clicked item: {itemId} (MVPv0 – not implemented yet).");
    }

    private void LogState()
    {
        string batteryList = string.Join(", ", playerBatteries);
        string itemList = playerItems.Count > 0 ? string.Join(", ", playerItems) : "None";

        Debug.Log($"[GameManager ]STATE | PlayerWater: {playerWater} | EnemyWater: {enemyWater} |" + 
        $"Batteries: [{batteryList}] | Items: [{itemList}] | Turn: {currentTurn} | Phase {currentPhase}");
    }

    private void StartPlayerTurn()
    {
        currentTurn = TurnOwner.Player;
        currentPhase = GamePhase.ChoosingBattery;
        Debug.Log("// Players turn, choose a battery //");

        playerBatteries.Add(BatteryObject.BatteryType.Default);
        LogState();
    }

    // Handle player actions
    private void ResolvePlayerAction(BatteryObject.BatteryType batteryType, BatterySlot.SlotType slotType)
    {
        int outcome = 0;

        switch (slotType)
        {
            case BatterySlot.SlotType.Low:
                outcome = Random.Range(-10, 6);
                break;
            case BatterySlot.SlotType.Medium:
                outcome = Random.Range(-20, 11);
                break;
            case BatterySlot.SlotType.High:
                outcome = Random.Range(-35, 21);
                break;
        }

        playerWater += outcome;
        playerWater = Mathf.Clamp(playerWater, 0, 100);

        if (outcome < 0)
        {
            Debug.Log($"[Player] GOOD outcome ({outcome}) water. New water: {playerWater}");
        }
        else if (outcome > 0)
        {
            Debug.Log($"[Player] BAD outcome (+{outcome}) water. New water: {playerWater}");
        }
        else
        {
            Debug.Log($"[Player] NEUTRAL outcome (0) water. New water: {playerWater}");
        }

        LogState();
    }

    private void StartEnemyTurn()
    {
        currentTurn = TurnOwner.Enemy;
        currentPhase = GamePhase.EnemyTurn;
        Debug.Log("// Enemy Turn //");

        BatterySlot.SlotType randomSlot = (BatterySlot.SlotType)Random.Range(0, 3);
        ResolveEnemyAction(randomSlot);

        CheckWinLose();

        if (!gameEnded)
        {
            StartPlayerTurn();
        }
    }

    private void ResolveEnemyAction(BatterySlot.SlotType slotType)
    {
        int outcome = 0;

        switch (slotType)
        {
            case BatterySlot.SlotType.Low:
                outcome = Random.Range(-10, 6);
                break;
            case BatterySlot.SlotType.Medium:
                outcome = Random.Range(-20, 11);
                break;
            case BatterySlot.SlotType.High:
                outcome = Random.Range(-35, 21);
                break;
        }

        enemyWater += outcome;
        enemyWater = Mathf.Clamp(enemyWater, 0, 100);

        if (outcome < 0)
        {
            Debug.Log($"[Enemy] GOOD outcome ({outcome}) water. New water: {enemyWater}");
        }
        else if (outcome > 0)
        {
            Debug.Log($"[Enemy] BAD outcome (+{outcome}) water. New water: {enemyWater}");
        }
        else
        {
            Debug.Log($"[Enemy] NEUTRAL outcome (0) water. New water: {enemyWater}");
        }

        LogState();
    }

    private void CheckWinLose()
    {
        if (playerWater >= 100 && enemyWater >= 100)
        {
            gameEnded = true;
            currentPhase = GamePhase.GameOver;
            Debug.Log("[GameManager] Both lose. Draw");
        }
        else if (playerWater >= 100)
        {
            gameEnded = true;
            currentPhase = GamePhase.GameOver;
            Debug.Log("[GameManager] Player lost. Game Over");
        }
        else if (enemyWater >= 100) {
            gameEnded = true;
            currentPhase = GamePhase.GameOver;
            Debug.Log("[GameManager] Enemy lost. Game Over");
        }
    }
}
