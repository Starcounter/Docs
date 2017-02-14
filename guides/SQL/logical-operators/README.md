# Logical operators

The logical operators "not" (<code>NOT <var>p</var></code>), "and" (<code><var>p</var> AND <var>q</var></code>) and "or" (<code><var>p</var> OR <var>q</var></code>) are supported. See examples below.

```sql
SELECT e FROM Employee e WHERE e.FirstName = 'Bob' AND e.LastName = 'Smith'
SELECT e FROM Employee e WHERE e.FirstName = 'Bob' OR e.FirstName = 'John'
SELECT e FROM Employee e WHERE NOT e.FirstName = 'Bob'
```

<strong>Performance note:</strong> The logical operators <code>NOT</code> and <code>OR</code> usually imply that indexes cannot be used in the execution of the query, and therefore these operators should be used very restrictively.