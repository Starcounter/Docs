# Logical operators

The logical operators "not" (`NOT p`), "and" (`p AND q`) and "or" (`p OR q`) are supported. See examples below.

```sql
SELECT e FROM Employee e WHERE e.FirstName = 'Bob' AND e.LastName = 'Smith'
SELECT e FROM Employee e WHERE e.FirstName = 'Bob' OR e.FirstName = 'John'
SELECT e FROM Employee e WHERE NOT e.FirstName = 'Bob'
```

**Performance note:** The logical operators `NOT` and `OR` usually imply that indexes cannot be used in the execution of the query, and therefore these operators should be used very restrictively.