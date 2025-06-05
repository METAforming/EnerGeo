using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TableData
{
    public List<string> headers;
    public List<RowData> rows;
}

[System.Serializable]
public class RowData
{
    public List<string> cells;
}

public class ComparisonTableUI : MonoBehaviour
{
    public Transform tableRoot;
    public GameObject headerPrefab;
    public GameObject cellPrefab;

    public void BuildTable(string json)
    {
        TableData table = JsonUtility.FromJson<TableData>(json);

        foreach (string header in table.headers)
        {
            var h = Instantiate(headerPrefab, tableRoot);
            h.GetComponentInChildren<Text>().text = header;
        }

        foreach (var row in table.rows)
        {
            foreach (var cell in row.cells)
            {
                var c = Instantiate(cellPrefab, tableRoot);
                c.GetComponentInChildren<Text>().text = cell;
            }
        }
    }
}
