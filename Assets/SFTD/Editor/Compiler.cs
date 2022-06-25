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
        string outputName = prefix + ".dd";
        byte[] err = new byte[2048];

        var byteFilename = Encoding.Default.GetBytes(pFilename);
        var byteOutputName = Encoding.Default.GetBytes(outputName);
        var bytePrefix = Encoding.Default.GetBytes(prefix);

        Debug.LogFormat("filename: {0}", Encoding.Default.GetString(byteFilename));
        Debug.LogFormat("output: {0}", Encoding.Default.GetString(byteOutputName));
        Debug.LogFormat("prefix: {0}", Encoding.Default.GetString(bytePrefix));

        bool res = Compile(Encoding.Default.GetBytes(pFilename + '\0'), Encoding.Default.GetBytes(outputName + '\0'), Encoding.Default.GetBytes(prefix + '\0'), err);

        if (res) {
            Debug.LogErrorFormat("Error: {0}", Encoding.Default.GetString(err));
        }
    }
}
