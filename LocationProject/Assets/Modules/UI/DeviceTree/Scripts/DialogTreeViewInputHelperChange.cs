using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

public class DialogTreeViewInputHelperChange : MonoBehaviour {

    [SerializeField]
    public ChangeTreeView Folders;

    ObservableList<TreeNode<TreeViewItem>> nodes;
    bool _InitDone;

    public void Refresh()
    {
        if (!_InitDone)
        {
            var config = new List<int>() { 5, 5, 2 };
            nodes = ChangeTestTreeView.GenerateTreeNodes(config, isExpanded: true);

            // Set nodes
            Folders.Start();
            Folders.Nodes = nodes;

            _InitDone = true;
        }
    }
}

