using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class Game : MonoBehaviour
{
    [SerializeField]
    private UserDesign playerDesign;

    [SerializeField]
    private UserDesign enemyDesign;

    [SerializeField]
    private UserDisplayView playerView;

    [SerializeField]
    private UserInteractionView playerInteractionView;

    [SerializeField]
    private UserDisplayView enemyView;

    protected void Awake()
    {
        // Create Users
        IUser playerUser = new UserModel(playerDesign);
        IUser enemyUser = new UserModel(enemyDesign);

        // Player
        UserController playerController = new UserController(playerUser, playerView);
        UserInteractionController playerInteractionController = new UserInteractionController(playerUser, enemyUser, playerInteractionView);

        // Enemy
        UserController enemyController = new UserController(enemyUser, enemyView);
    }
}
