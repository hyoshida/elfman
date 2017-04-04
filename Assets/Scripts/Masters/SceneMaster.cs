using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
class SceneMaster {
    public uint code = 0;
    public uint stageCode = 0;
    public bool opening = false;
    public string imagePath = "";
    public List<string> texts = new List<string> { };
}

