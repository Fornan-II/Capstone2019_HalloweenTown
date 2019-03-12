﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConsoleCommandParser : MonoBehaviour
{
    UnityEngine.UI.InputField _field;
    private void Awake()
    {
        _field = GetComponent<UnityEngine.UI.InputField>();
    }

    private void Update()
    {
        if (gameObject.activeSelf && _field)
        {
            if (!_field.IsActive())
            {
                SetFocus();
            }
        }
    }

    //Main Parser
    public void ParseCommand(string cmd)
    {
        if (cmd.EndsWith("`"))
        {
            _field.text = "";
            return;
        }
        cmd = cmd.ToLower();
        if (waitForResponseDelegate != null)
        {
            OnResponse(cmd);
        }
        else
        {
            string[] splitCmd = cmd.Split(' ');

            switch (splitCmd[0])
            {
                case "round":
                    {
                        Round(splitCmd);
                        break;
                    }
                case "console":
                    {
                        Console(splitCmd);
                        break;
                    }
                case "player":
                    {
                        Player(splitCmd);
                        break;
                    }
                case "quit":
                    {
                        BuildConsole.WriteLine("Are you sure you want to quit? Y / N");
                        waitForResponseMessage = "y";
                        waitForResponseDelegate = Application.Quit;
                        break;
                    }
                default:
                    {
                        BuildConsole.WriteLine("Invalid command");
                        break;
                    }
            }
        }

        _field.text = "";
        SetFocus();
    }

    #region Command specific methods
    [Header("For command \"round\"")]
    public GameObject RoundManagerPrefab;
    protected void Round(string[] keywords)
    {
        RoundManager activeRM = FindObjectOfType<RoundManager>();
        if(keywords.Length < 2)
        {
            BuildConsole.WriteLine("Insufficient parameters");
            return;
        }

        switch(keywords[1])
        {
            case "time":
                {
                    if (keywords[2] == "get")
                    {
                        if(activeRM)
                        {
                            BuildConsole.WriteLine("Elapsed time: " + activeRM.roundElapsedTime);
                        }
                        else
                        {
                            BuildConsole.WriteLine("Round not active");
                        }
                        break;
                    }
                    else
                    {
                        float value;
                        if(float.TryParse(keywords[2], out value))
                        {
                            if (activeRM)
                            {
                                activeRM.roundRunningEndTime = value;
                                BuildConsole.WriteLine("Round time set to " + value);
                            }
                            else if(RoundManagerPrefab)
                            {
                                RoundManager rm = RoundManagerPrefab.GetComponent<RoundManager>();
                                if(rm)
                                {
                                    rm.roundRunningEndTime = value;
                                    BuildConsole.WriteLine("Round time set to " + value);
                                }
                            }
                        }
                        else
                        {
                            BuildConsole.WriteLine("Expecting \'get\' or number as parameters, found neither");
                        }
                    }
                    break;
                }
            case "phase":
                {
                    if(!activeRM)
                    {
                        BuildConsole.WriteLine("Round not active");
                    }

                    switch (keywords[2])
                    {
                        case "pre_game":
                            {
                                activeRM.currentPhase = RoundManager.RoundPhase.PRE_GAME;
                                BuildConsole.WriteLine("Set phase to pre_game");
                                break;
                            }
                        case "round_starting":
                            {
                                activeRM.currentPhase = RoundManager.RoundPhase.ROUND_STARTING;
                                BuildConsole.WriteLine("Set phase to round_starting");
                                break;
                            }
                        case "round_running":
                            {
                                activeRM.currentPhase = RoundManager.RoundPhase.ROUND_RUNNING;
                                BuildConsole.WriteLine("Set phase to round_running");
                                break;
                            }
                        case "round_ending":
                            {
                                activeRM.currentPhase = RoundManager.RoundPhase.ROUND_ENDING;
                                BuildConsole.WriteLine("Set phase to round_ending");
                                break;
                            }
                        case "round_over":
                            {
                                activeRM.currentPhase = RoundManager.RoundPhase.ROUND_OVER;
                                BuildConsole.WriteLine("Set phase to round_over");
                                break;
                            }
                        default:
                            {
                                BuildConsole.WriteLine("Invalid parameter. Expecting \'pre_game\', \'round_starting\', \'round_running\', \'round_ending\', or \'round_over\'");
                                break;
                            }
                    }
                    break;
                }
            default:
                {
                    BuildConsole.WriteLine("Invalid parameters for round");
                    break;
                }
        }
    }

    protected void Console(string[] keywords)
    {
        if(keywords.Length < 2)
        {
            BuildConsole.WriteLine("Insufficient parameters");
            return;
        }

        switch (keywords[1])
        {
            case "clear":
                {
                    BuildConsole.ClearLog();
                    break;
                }
        }
    }

    protected void Player(string[] keywords)
    {
        if (keywords.Length < 3)
        {
            BuildConsole.WriteLine("Insufficient parameters");
            return;
        }

        RoundManager activeRM = FindObjectOfType<RoundManager>();
        if (!activeRM)
        {
            BuildConsole.WriteLine("Round not active");
            return;
        }

        PlayerController[] activePlayers = activeRM.GetPlayers();

        if (keywords[1] == "all")
        {
            foreach(PlayerController pc in activePlayers)
            {
                switch (keywords[2])
                {
                    case "respawn":
                        {
                            int candy = 0;
                            if (pc.IsControllingPawn)
                            {
                                candy = pc.ControlledPawn.MyCandy.candy;
                                Destroy(pc.ControlledPawn.gameObject);
                            }
                            GameObject pawn = pc.ControlledPawn.gameObject;
                            SpawnPoint.GetRandomValidSpawn().SpawnPlayer(pc, activeRM.playerPrefab);
                            pc.ControlledPawn.MyCandy.candy = candy;
                            break;
                        }
                    case "candy":
                        {
                            if (keywords.Length < 5)
                            {
                                BuildConsole.WriteLine("Insufficient parameters");
                                return;
                            }
                            int amount;
                            if (!int.TryParse(keywords[4], out amount))
                            {
                                BuildConsole.WriteLine("Looking for integer, found " + keywords[4]);
                                return;
                            }
                            if (keywords[3] == "set")
                            {
                                pc.ControlledPawn.MyCandy.candy = amount;
                            }
                            else if (keywords[3] == "add")
                            {
                                pc.ControlledPawn.MyCandy.candy += amount;
                            }
                            else
                            {
                                BuildConsole.WriteLine("Invalid command. Looking for \'set\' or \'add\'");
                            }
                            break;
                        }
                }
            }
        }
        else
        {
            int playerNum;
            if(int.TryParse(keywords[1], out playerNum))
            {
                playerNum--;
                if(0 < playerNum || playerNum >= activePlayers.Length)
                {
                    BuildConsole.WriteLine("There is not a player " + (playerNum + 1));
                    return;
                }

                switch(keywords[2])
                {
                    case "respawn":
                        {
                            int candy = 0;
                            if(activePlayers[playerNum].IsControllingPawn)
                            {
                                candy = activePlayers[playerNum].ControlledPawn.MyCandy.candy;
                                Destroy(activePlayers[playerNum].ControlledPawn.gameObject);
                            }
                            GameObject pawn = activePlayers[playerNum].ControlledPawn.gameObject;
                            SpawnPoint.GetRandomValidSpawn().SpawnPlayer(activePlayers[playerNum], activeRM.playerPrefab);
                            activePlayers[playerNum].ControlledPawn.MyCandy.candy = candy;
                            break;
                        }
                    case "candy":
                        {
                            if(keywords.Length < 5)
                            {
                                BuildConsole.WriteLine("Insufficient parameters");
                                return;
                            }
                            int amount;
                            if(!int.TryParse(keywords[4], out amount))
                            {
                                BuildConsole.WriteLine("Looking for integer, found " + keywords[4]);
                                return;
                            }
                            if(keywords[3] == "set")
                            {
                                activePlayers[playerNum].ControlledPawn.MyCandy.candy = amount;
                            }
                            else if(keywords[3] == "add")
                            {
                                activePlayers[playerNum].ControlledPawn.MyCandy.candy += amount;
                            }
                            else
                            {
                                BuildConsole.WriteLine("Invalid command. Looking for \'set\' or \'add\'");
                            }
                            break;
                        }
                }
            }
            else
            {
                BuildConsole.WriteLine("Looking for player number");
                return;
            }
        }
    }
    #endregion

    #region Util
    protected string waitForResponseMessage;
    protected delegate void OnResponseMatch();
    protected OnResponseMatch waitForResponseDelegate;

    protected void OnResponse(string cmd)
    {
        if(cmd == waitForResponseMessage)
        {
            waitForResponseDelegate.Invoke();
        }
        waitForResponseDelegate = null;
        waitForResponseMessage = "";
    }

    protected void SetFocus()
    {
        _field.Select();
        _field.ActivateInputField();
        //EventSystem.current.SetSelectedGameObject(gameObject, null);
        //_field.OnPointerClick(new PointerEventData(EventSystem.current));
    }
    #endregion
}