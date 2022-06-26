using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;

public class Compiler {

    [DllImport("sftd")]
    private static extern bool Compile(byte[] pInput, byte[] pOutput, byte[] pI18NPrefix, [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pErr);

    public static void CompileDialogue(string pFilename) {
        Debug.LogFormat("Compiling dialogue file {0}...", pFilename);

        string prefix = pFilename.Substring(0, pFilename.LastIndexOf('.'));
        string outputName = prefix + ".dd.txt";
        byte[] err = new byte[2048];
        bool res = Compile(Encoding.Default.GetBytes(pFilename + '\0'), Encoding.Default.GetBytes(outputName + '\0'), Encoding.Default.GetBytes(prefix + '\0'), err);

        if (res) {
            Debug.LogErrorFormat("Error: {0}", Encoding.Default.GetString(err));
        }
    }
}
