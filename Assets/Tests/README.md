# Setting Up

To make a new test, follow this standard:

```csharp
[Test]
public void TestTESTNAMEHERE()
{
   // your test here
}
```

which should also follow the general convention of having `Assign`, `Act`, and `Assert`.

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
