using System;
using System.Collections.Generic;

[Serializable]
class StillMaster {
    public uint stageCode = 0;
    public string imagePath = "";
    public string title = "";
    public List<string> texts = new List<string> { };
}