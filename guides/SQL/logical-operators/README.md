# Logical operators

There are three logical operators in Starcounter SQL, `AND`, `OR`, and `NOT`. For instructions on how to use these, take a look at these examples:

```sql
SELECT e FROM Employee e WHERE e.FirstName = 'Bob' AND e.LastName = 'Smith'
SELECT e FROM Employee e WHERE e.FirstName = 'Bob' OR e.FirstName = 'John'
SELECT e FROM Employee e WHERE NOT e.FirstName = 'Bob'
```

**Performance note:** The logical operators `NOT` and `OR` usually imply that indexes cannot be used in the execution of the query, and therefore these operators should be used restrictively.