# Joins

## Introduction

Starcounter supports inner joins, cross joins, left outer joins and right outer joins.

The default join type is inner join, so if you use the reserved word `JOIN`, the parser will interpret it as an inner join.

## Cross Join

A cross join is an inner join without join condition, and thus can be regarded as a special case rather than a separate type of join.

This query is interpreted as a cross join:

```sql
SELECT e, d FROM Employee e JOIN Department d
```

It can also be written as an explicit cross join:

```sql
SELECT e, d FROM Employee e CROSS JOIN Department d
```

## Inner Join

This query is interpreted as an inner join:

```sql
SELECT e, d FROM Employee e JOIN Department d ON e.Department = d
```

It can also be written as an explicit inner join:

```sql
SELECT e, d FROM Employee e INNER JOIN Department d ON e.Department = d
```

## Outer Joins

For left outer join and right outer join you may omit the reserved word `OUTER`.

This query returns all employees and their managers, including the employees that have no manager:

```sql
SELECT e1, e2 FROM Employee e1 LEFT JOIN Employee e2 ON e1.Manager = e2
```

This left outer join can also be written as a right outer join:

```sql
SELECT e1, e2 FROM Employee e2 RIGHT JOIN Employee e1 ON e1.Manager = e2
```

