﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Irony.Parsing {
  //First sketch of Symbol object and Symbol table
  public class Symbol {
    public readonly string Text;
    //used for symbol comparison in case-insensitive environments
    public readonly Symbol LowerSymbol; 
    private int _hashCode;

    internal Symbol(string text, Symbol lowerSymbol) {
      Text = text; 
      LowerSymbol = lowerSymbol?? this; //lowerSymbol == null means "text is all lowercase, so use 'this' as LowerSymbol" 
      _hashCode = Text.GetHashCode(); 
    }

    public override int GetHashCode() {
      return _hashCode;
    }
    public override string ToString() {
      return Text;
    }

    public static bool AreEqual(Symbol first, Symbol second, bool caseSensitive) {
      return (caseSensitive ? first == second : first.LowerSymbol == second.LowerSymbol);
    }

  }//Symbol class

  public class CaseSensitiveSymbolComparer : IComparer<Symbol> {
    public int Compare(Symbol x, Symbol y) {
      return x == y ? 0 : 1;  
    }
  }

  public class SymbolTable {
    #region nested classes
    internal class SymbolDictionary : Dictionary<string, Symbol> { 
      internal SymbolDictionary() : base(1000) { }
    }
    internal class SymbolList : List<Symbol> { }
    #endregion

    internal readonly SymbolDictionary Dictionary = new SymbolDictionary();

    public void CopyFrom(SymbolTable other) {
      foreach(var entry in other.Dictionary)
        Dictionary.Add(entry.Key, entry.Value); 
    }

    public Symbol FindSymbol(string text) {
      Symbol symbol;
      Dictionary.TryGetValue(text, out symbol);
      return symbol;
    }

    public Symbol TextToSymbol(string text) {
      Symbol symbol, lowerSymbol;
      if (Dictionary.TryGetValue(text, out symbol))
        return symbol;
      //Create symbol; first find/create lower symbol
      var lowerText = text.ToLower(CultureInfo.InvariantCulture); //ToLowerInvariant looks better but it's not in Silverlight, so using ToLower 
      if (!Dictionary.TryGetValue(lowerText, out lowerSymbol))
        lowerSymbol = NewSymbol(lowerText, null);
      //if the text is all lower, return lowerSymbol as result
      if (lowerText == text) 
        return lowerSymbol;
      //otherwise create new symbol
      return NewSymbol(text, lowerSymbol);
    }//method

    private Symbol NewSymbol(string text, Symbol lowerSymbol) {
      var result = new Symbol(text, lowerSymbol);
      Dictionary.Add(text, result);
      return result; 
    }

  }//class
}
