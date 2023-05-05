using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerControllerTests
{
    PlayerController PLAYER     = new PlayerController();

    [Test]
    public void TestSomemthing()
    {
        int x = 4;
        Debug.Log(x);
        Assert.AreEqual(1,1);
    }

    [Test]
    public void TestHurt()
    {
        GameObject player = new GameObject("Player");
        PlayerController playerController = player.AddComponent<PlayerController>();

        // Arrange
        player.health = 100;

        // Act
        player.hurt(2);

        // Assert
        Debug.Log(player.health);
        Assert.AreEqual(player.health, 98);


        
    }


}
