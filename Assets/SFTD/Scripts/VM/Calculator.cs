using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator {
    public delegate VariableData UnaryOperator(VariableData v);
    public delegate VariableData BinaryOperator(VariableData lhs, VariableData rhs);

    public static VariableData CalculateAdd(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        if (lhs.type == VariableType.STRING && rhs.type == VariableType.STRING) {
            res = lhs; res.s += rhs.s;
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res = lhs; res.i += rhs.i;
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.FLOAT) {
            res = lhs; res.f += rhs.f;
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.INT) {
            res = lhs; res.f += rhs.i;
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.FLOAT) {
            res = rhs; res.f += lhs.i;
        }
        else {
            Debug.LogError("Add Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateSub(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res = lhs; res.i -= rhs.i;
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.FLOAT) {
            res = lhs; res.f -= rhs.f;
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.INT) {
            res = lhs; res.f -= rhs.i;
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.FLOAT) {
            res = rhs; res.f -= lhs.i;
        }
        else {
            Debug.LogError("Sub Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateMul(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res = lhs; res.i *= rhs.i;
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.FLOAT) {
            res = lhs; res.f *= rhs.f;
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.INT) {
            res = lhs; res.f *= rhs.i;
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.FLOAT) {
            res = rhs; res.f *= lhs.i;
        }
        else {
            Debug.LogError("Mul Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateDiv(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        res.type = VariableType.FLOAT;
        if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res.f = lhs.f; res.f /= rhs.i;
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.FLOAT) {
            res = lhs; res.f /= rhs.f;
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.INT) {
            res = lhs; res.f /= rhs.i;
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.FLOAT) {
            res = rhs; res.f /= lhs.i;
        }
        else {
            Debug.LogError("Div Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateMod(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res = lhs; res.i %= rhs.i;
        }
        else {
            Debug.LogError("Add Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateEqual(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        res.type = VariableType.BOOL;
        if (lhs.type == VariableType.STRING && rhs.type == VariableType.STRING) {
            res.b = (lhs.s == rhs.s);
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res.b = (lhs.i == rhs.i);
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.FLOAT) {
            res.b = (Mathf.Abs(lhs.f - rhs.f) < 0.0001f);
        }
        else if (lhs.type == VariableType.BOOL && rhs.type == VariableType.BOOL) {
            res.b = (lhs.b == rhs.b);
        }
        else {
            res.b = false;
        }

        return res;
    }

    public static VariableData CalculateAnd(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        if (lhs.type == VariableType.BOOL && rhs.type == VariableType.BOOL) {
            res = lhs; res.b = res.b && rhs.b;
        }
        else {
            Debug.LogError("And Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateOr(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        if (lhs.type == VariableType.BOOL && rhs.type == VariableType.BOOL) {
            res = lhs; res.b = res.b || rhs.b;
        }
        else {
            Debug.LogError("Or Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateLess(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        res.type = VariableType.BOOL;
        if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res.b = (lhs.i < rhs.i);
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.FLOAT) {
            res.b = (lhs.f < rhs.f);
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.INT) {
            res.b = (lhs.f < rhs.i);
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.FLOAT) {
            res.b = (lhs.i < rhs.f);
        }
        else {
            Debug.LogError("Less Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateGreater(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        res.type = VariableType.BOOL;
        if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res.b = (lhs.i > rhs.i);
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.FLOAT) {
            res.b = (lhs.f > rhs.f);
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.INT) {
            res.b = (lhs.f > rhs.i);
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.FLOAT) {
            res.b = (lhs.i > rhs.f);
        }
        else {
            Debug.LogError("Greater Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateLE(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        res.type = VariableType.BOOL;
        if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res.b = (lhs.i <= rhs.i);
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.FLOAT) {
            res.b = (lhs.f <= rhs.f);
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.INT) {
            res.b = (lhs.f <= rhs.i);
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.FLOAT) {
            res.b = (lhs.i <= rhs.f);
        }
        else {
            Debug.LogError("Less Equal Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateGE(VariableData lhs, VariableData rhs) {
        VariableData res = new VariableData();
        res.type = VariableType.BOOL;
        if (lhs.type == VariableType.INT && rhs.type == VariableType.INT) {
            res.b = (lhs.i >= rhs.i);
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.FLOAT) {
            res.b = (lhs.f >= rhs.f);
        }
        else if (lhs.type == VariableType.FLOAT && rhs.type == VariableType.INT) {
            res.b = (lhs.f >= rhs.i);
        }
        else if (lhs.type == VariableType.INT && rhs.type == VariableType.FLOAT) {
            res.b = (lhs.i >= rhs.f);
        }
        else {
            Debug.LogError("Greater Equal Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateNot(VariableData v) {
        VariableData res = new VariableData();
        if (v.type == VariableType.BOOL) {
            res = v; res.b = !res.b;
        }
        else {
            Debug.LogError("Not Runtime Error");
        }

        return res;
    }

    public static VariableData CalculateNeg(VariableData v) {
        VariableData res = new VariableData();
        if (v.type == VariableType.INT) {
            res = v; res.i = -res.i;
        }
        else if (v.type == VariableType.FLOAT) {
            res = v; res.f = -res.f;
        }
        else {
            Debug.LogError("Neg Runtime Error");
        }

        return res;
    }
}
