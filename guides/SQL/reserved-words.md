# Reserved words

The following words are reserved words in Starcounter SQL.

<code>ALL</code>, <code>AND</code>, <code>AS</code>, <code>ASC</code>, <code>AVG</code>,
<code>BY</code>, <code>BINARY</code>,
<code>CAST</code>, <code>COUNT</code>, <code>CREATE</code>, <code>CROSS</code>,
<code>DATE</code>, <code>DATETIME</code>, <code>DELETE</code>, <code>DESC</code>, <code>DISTINCT</code>,
<code>ESCAPE</code>, <code>EXISTS</code>,
<code>FALSE</code>, <code>FETCH</code>, <code>FIRST</code>, <code>FIXED</code>, <code>FORALL</code>, <code>FROM</code>, <code>FULL</code>,
<code>GROUP</code>,
<code>HAVING</code>,
<code>IN</code>, <code>INDEX</code>, <code>INNER</code>, <code>INSERT</code>, <code>IS</code>,
<code>JOIN</code>,
<code>LEFT</code>, <code>LIKE</code>, <code>LIMIT</code>,
<code>MAX</code>, <code>MIN</code>,
<code>NOT</code>, <code>NULL</code>,
<code>OBJ</code>, <code>OBJECT</code>, <code>OFFSET</code>, <code>OFFSETKEY</code>, <code>ON</code>, <code>ONLY</code>,
<code>OPTION</code>, <code>OR</code>, <code>ORDER</code>, <code>OUT</code>, <code>OUTER</code>, <code>OUTPUT</code>,
<code>PROC</code>, <code>PROCEDURE</code>,
<code>RANDOM</code>, <code>RIGHT</code>, <code>ROWS</code>,
<code>SELECT</code>, <code>STARTS</code>, <code>SUM</code>,
<code>TIME</code>, <code>TIMESTAMP</code>, <code>TRUE</code>,
<code>UNIQUE</code>, <code>UNKNOWN</code>, <code>UPDATE</code>,
<code>VALUES</code>, <code>VAR</code>, <code>VARIABLE</code>,
<code>WHEN</code>, <code>WHERE</code>, <code>WITH</code>.

Be aware that the list of reserved words might be extended in later versions of Starcounter SQL. In particular some keywords in SQL92 might become reserved words in Starcounter SQL.

Reserved words cannot be used in queries directly. They have to be surrounded with double quotes as in example:

```sql
SELECT d FROM "DATE" d
SELECT o FROM "ORDER" o
```

Double quoting can be applied to any identifier, but only necessary for reserved keywords. It is important to double quote each identifier in identifier change, e.g.:

```sql

SELECT t FROM "Order"."Date" t

```