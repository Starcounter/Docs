# Sorting

## Introduction

The result of a query can be ordered with the `ORDER BY` clause. The `ASC` and `DESC` keywords can be added after the clause to specify the order of the sort.

## Example

```sql
SELECT e.LastName, e.Salary 
    FROM Employee e 
    ORDER BY e.Salary DESC, e.LastName ASC
```

## Sorting and Indexes

When there is an index matching the sort specification in the `ORDER BY` clause then the result can be obtained without executing any sorting. We therefore for performance reasons strongly recommend that necessary indexes are declared when using `ORDER BY` clauses as shown in statement below.

```sql
CREATE INDEX EmployeeIndex ON Employee (Salary DESC, LastName ASC)
```

{% page-ref page="indexes.md" %}



