# Setting Up

To make a new test, follow this standard:

```csharp
[Test]
public void TestTESTNAMEHERE()
{
   // your test here
}
```

which should also follow the general convention of having `Arrange`, `Act`, and `Assert`.

It was annoying to set the unit testing because the `projectPath` cannot be found however the easy fix was to set the path to the root directory, not the project name.
So, the project path is now `./`.

# Files

Checkout `./../Scripts/Main.asmdef` for the main assembly file. This is used as a reference for the `EditModeTests`.

# Useful Stuffs

## Printing a variable in Unity Console

```csharp
int var = 69;
Debug.Log(var);
```

# References

[How to Setup a Project for Testing (Unity Tutorial)](https://www.youtube.com/watch?v=Dox5aZjuy3M)
