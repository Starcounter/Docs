Starcounter does not support SQL92's &lt;code&gt;INSERT&lt;/code&gt; statement. Instead, objects are created directly in the programming code. Marking a class in the code as a database class is done by setting the &lt;code&gt;\[Database\]&lt;/code&gt; attribute. This class becomes a part of the database schema.



New records are created with the native program code operator &lt;code&gt;new&lt;/code&gt;. All instances of a database class are database objects and are stored in the database.



Public fields \(e.g., \`Person.FirstName\` and \`Quote.Person\`\), public auto-created properties \(e.g., \`Person.LastName\`\) and public properties getting and setting private fields \(e.g., \`Quote.Text\`\) become database columns. More complex public properties become code properties, which are not stored as columns, but can be accessed in SQL queries \(e.g., \`Person.FullName\`\).



\`\`\`cs

using Starcounter;



\[Database\]

public class Person {

   public string FirstName;

   public string LastName { get; set; }

   public string FullName { get { return FirstName + " " + LastName; } }

}



\[Database\]

public class Quote {

   public Person Person;

   private string \_Text;

   public string Text { get { return \_Text; } set { \_Text = value; } }

}

\`\`\`



\#\# Preventing fields from becoming database columns



Using the \`Transient\` custom attribute, it is possible to exclude fields and auto-implemented properties of a database class from becoming database columns. A field or auto-implemented property with this attribute applied to it will remain a regular .NET field/property and its value will be stored on the CLR heap and hence garbage collected along with the object it belongs to. Such fields and properties are not available using SQL; instead, Starcounter ignores them as a whole.



\`\`\`cs

using Starcounter;



\[Database\]

public class Person {

   public string FirstName;

   public string LastName { get; set; }

   public string FullName { get { return FirstName + " " + LastName; } }

   \[Transient\]

   public int ProcessSessionID;

   \[Transient\]

   public int ProcessSessionNumber { get; set; }

}

\`\`\`



