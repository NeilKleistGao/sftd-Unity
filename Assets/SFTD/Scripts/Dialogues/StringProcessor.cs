using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class StringProcessor {
    public static string ReplaceWithVariables(string pStr) {
        string pattern = @"\{\{\$(?<value>[_\w]+[\w\d]*)\}\}";
        string result = pStr;
        MatchCollection matches = Regex.Matches(pStr, pattern);

        VariableDatabase variableDatabase = VariableDatabase.Instance;
        foreach (Match match in matches) {
            result = result.Replace(match.Value, variableDatabase.GetAny(match.Groups[1].Value));
        }

        return result;
    }
}
