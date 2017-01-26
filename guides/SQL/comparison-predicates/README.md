# Comparison predicates

The comparison predicates "equal" (<code><var>x</var> = <var>y</var></code>) and "not equal" (<code><var>x</var> &lt;&gt; <var>y</var></code>) are supported for all data types. See for example query below.

```sql
SELECT e FROM Employee e WHERE e.FirstName = 'Bob'
```
Note that all string comparisons are case insensitive. The query (1) is therefore equivalent to the query.

```sql
SELECT e FROM Employee e WHERE e.FirstName = 'bob'
```

The comparison predicates "less than" (<code><var>x</var> < <var>y</var></code>), "greater than" (<code><var>x</var> > <var>y</var></code>), "less than or equal" (<code><var>x</var> <= <var>y</var></code>) and "greater than or equal" (<code><var>x</var> >= <var>y</var></code>) are implemented for the data types <code>String</code>, <code>DateTime</code> and all numerical types. See example below.

```sql
SELECT e FROM Employee e WHERE e.LastName >= 'Smith'
```

Since a <code>DateTime</code> value represents a timestamp it is often necessary to compare it with a <code>DateTime</code> range. The query below returns all employees with a <code>HireDate</code> between <code>'2006-11-01 00:00:00.000'</code> and <code>'2006-11-01 23:59:59.999'</code>.

```sql
SELECT e.FirstName, e.HireDate FROM Employee e 
  WHERE e.HireDate >= DATE '2006-11-01' AND e.HireDate < DATE '2006-11-02'
```

The comparison predicates "is null" (<code><var>x</var> IS NULL</code>) and "is not null" (<code><var>x</var> IS NOT NULL</code>) are implemented for all data types. See for example query below.

```sql
SELECT e FROM Employee e WHERE e.Manager IS NULL
```

The comparison predicate "like" (<code><var>x</var> LIKE <var>y</var> [ESCAPE <var>z</var>]</code>) is implemented for the data type <code>String</code>. In the specified pattern (<code><var>y</var></code>) the underscore character (<code>'_'</code>) match any single character in the string, and the percent character (<code>'%'</code>) match any sequence (possibly empty) of characters in the string. See for example query below.

```sql
SELECT e FROM Employee e WHERE e.FirstName LIKE 'B_b%'
```

The optional third argument to the <code>LIKE</code> predicate is an "escape character", for use when a percent or underscore character is required in the pattern without its special meaning. This is exemplified in query below.

```sql
SELECT s FROM Share s WHERE s.Unit LIKE '\%' ESCAPE '\\'
```

The comparison predicate `IS` can be used to check the type of argument. See for example query below.

```sql
SELECT p FROM Employee p WHERE p IS Employee
```

<!--
<p><strong>The current implementation of <code>LIKE</code> is not
optimized and in general the optimizer cannot make use of indexes on
<code>LIKE</code> conditions. Therefore they should be used very restrictively.</strong></p>
-->