using SimpleJSON;

namespace WMExtendedMutations
{
    public static class History
    {
        // Adds an string to the specified history spice node array. For example, if we want to add
        // a new item to the winter eremite profession array, we might call this method as follows:
        //     
        //     AddToHistorySpice("spice.professions.winter eremite.items", "snowy ideal");
        public static bool AddToHistorySpice(string path, string itemToAdd, bool allowDuplicates = false)
        {
            JSONNode textNode = itemToAdd;
            return AddToHistorySpice(path, textNode, allowDuplicates);
        }

        // Adds a new child node to the specified history spice node. This version of the method allows
        // adding entirely new structures to history spice. For example, to add a new element that represents
        // the favorite types of scrap in the tinker profession, we might call this method as follows:
        //     
        //     string scrapInfoSource = "{ \"favorite scrap\": [\"burnt capacitator\", \"cracked lens\"] }";
        //     SimpleJSON.JSONNode scrapInfo = SimpleJSON.JSON.Parse(scrapInfoSource);
        //     AddToHistorySpice("spice.professions.tinker", scrapInfo["favorite scrap"]);
        public static bool AddToHistorySpice(string path, JSONNode nodeToAdd, bool allowDuplicates = false)
        {
            if (path.StartsWith("spice."))
            {
                path = path.Substring(6);
            }
            JSONNode currentNode = HistoryKit.HistoricSpice.root;
            foreach (string targetNode in path.Split('.'))
            {
                currentNode = GetJSONNodeChildByKey(currentNode, targetNode);
                if (currentNode == null)
                {
                    XRL.Core.XRLCore.Log($"YourModName: (Error) Failed to find HistorySpice.json node \"{targetNode}\" in path "
                        + $"\"{path}\". Item \"{GetJSONNodeName(nodeToAdd)}\" will not be added to the specified path node.");
                    return false;
                }
            }
            if (allowDuplicates || GetJSONNodeChildByKey(currentNode, GetJSONNodeName(nodeToAdd)) == null)
            {
                if (!string.IsNullOrEmpty(nodeToAdd.Key))
                {
                    currentNode.Add(nodeToAdd.Key, nodeToAdd);
                }
                else
                {
                    currentNode.Add(nodeToAdd);
                }
            }
            return true;
        }

        public static JSONNode GetJSONNodeChildByKey(JSONNode node, string key)
        {
            foreach (JSONNode childNode in node.Childs)
            {
                if (GetJSONNodeName(childNode) == key)
                {
                    return childNode;
                }
            }
            return null;
        }

        public static string GetJSONNodeName(JSONNode node)
        {
            return !string.IsNullOrEmpty(node.Key) ? node.Key : node.ToString();
        }
    }
}