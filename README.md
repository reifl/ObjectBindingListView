# ObjectBindingListView

Tokenizer and Parser is based on [Vanlightly DslParser Project](https://github.com/Vanlightly/DslParser)

I've created some new Tokens and modified the MatchConditions so it can work with Properties of the Objects.

The Linq-Where Extension Method accepts a String as Parameter. This String will be tokenized by the Tokenizer and parsed by the DSL Parser.
After parsing the Method BuildExpression builds the Expressions for the Original Linq-Where Method.

Since I don't have enough time there will be no or not much support for this.

Usage:
```csharp
IList<MyDataElement> liste = new List<MyDataElement>();

ObjectListView<MyDataElement> objList = new ObjectListView<MyDataElement>();
objList.DataSource = liste;

bindingSource1.DataSource = objList;
bindingSource1.Filter = "x > 3";
```
