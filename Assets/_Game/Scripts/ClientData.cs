using System;
using System.Collections.Generic;

[Serializable]
public class ClientData
{
    public Biped biped;
    public List<int> EquippedMeshes = new List<int>();
    public List<string> EquippedItems = new List<string>();
}

public enum Biped
{
    Female,
    Male
}